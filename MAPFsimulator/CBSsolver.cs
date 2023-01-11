using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MAPFsimulator
{
    /// <summary>
    /// Implementace algoritmu CBS pro MAPF s ucelovou funkci T.
    /// </summary>
    class CBSsolver<T> where T : CostFunction, new()
    {
        IHeap<int, CBSNode> open;
        protected AStar lowLevelSearch;
        protected Stopwatch sw;
        protected int maxSolLength = int.MaxValue;

        //cislo k definujici k-robustnost
        protected int k;

        /// <summary>
        /// Inicializuje novy CBS solver, ktery pri prohledavani na nizsi urovni pouziva algoritmus A*.
        /// </summary>
        public CBSsolver()
        {
            sw = new Stopwatch();
            lowLevelSearch = new AStar();
            k = 0;
        }

        /// <summary>
        /// Hleda v planu vrcholove konflikty.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>vrcholovy konflikt, ktery nastal nejdrive, nebo null, pokud je plan bez konfliktu</returns>
        protected virtual Conflict Validate(List<Plan> paths)
        {
            //mene nez 2 agenti v MAPF nemaji konflikty
            if (paths.Count < 2)
            {
                return null;
            }
            int maxSteps = paths.Max(p => p.GetLength());
            //pro vsechny timestepy
            for (int i = 0; i < maxSteps; i++)
            {
                //dictionary [pozice,id_agent]
                Dictionary<Vertex, int> positions = new Dictionary<Vertex, int>();
                //pro vsechny cesty
                for (int j = 0; j < paths.Count; j++)
                {
                    //pokud uz tam je, tak mam konflikt
                    if (positions.ContainsKey(paths[j].GetNth(i)))
                    {
                        int agent1 = positions[paths[j].GetNth(i)];
                        return new Conflict(agent1, j, paths[j].GetNth(i), i);
                    }
                    else
                    {
                        positions.Add(paths[j].GetNth(i), j);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Vraci minimalni delku noveho planu, ktery muze vzniknout opravenim konfliktu c agenta agentID v planu p.
        /// </summary>
        protected virtual int MinimalPathLength(Plan p, Conflict c, int id)
        {
            if (p.GetLength() <= c.time + c.duration)
            {
                //jeho plan se tedy musi prodlouzit tak, aby skoncil nejdrive v case konfliktu + 1
                return c.time + c.duration + 1;
            }
            else
                return p.GetLength();
        }

        /// <summary>
        /// Vraci hodnotu pocatecniho casu u agenta id.
        /// </summary>
        protected virtual int StartTime(int id) { return 0; }

        /// <summary>
        /// Vraci validni plan pro skupinu agentu agents v grafu g.
        /// Pokud plan neexistuje, nebo dojde-li k vyprseni casoveho limitu, vraci null.
        /// </summary>
        public List<Plan> GetPlan(Graph g, List<IAgent> agents)
        {
            //cas
            sw.Restart();
            //horni odhad delky - delsi cesta byt nemuze (plati pro cesty v koreni, v prubehu algoritmu dale menime)
            int maxDepth = g.activeVertices + 1;
            open = new RegularBinaryHeap<CBSNode>();
            //vytvorime koren CBS prohledavani a najdeme jednotlive nejkratsi cesty
            var root = new CBSNode();
            for (int i = 0; i < agents.Count; i++)
            {
                Plan p = lowLevelSearch.SearchPath(agents[i], g, 0, root.constraints, StartTime(i),maxDepth);
                //pokud neexistuje cesta, nema MAPF reseni
                if (p == null)
                {
                    return null;
                }
                root.AddSolution(agents[i].id, p);
            }
            open.insert(root.cost, root);

            //Hlavni cyklus algoritmu - vybereme uzel s nejmensi cenou, zkontrolujeme konflikt.
            //Pokud je konflikt, pridame podminku, rozdelime na dva nove uzly, ktere pridame do haldy.
            while (open.size() > 0)
            {
                //dojde-li cas, vracime null
                if (Properties.Settings.Default.CBSLimit*1000 > 0 && sw.ElapsedMilliseconds > Properties.Settings.Default.CBSLimit*1000)
                {
                    sw.Stop();
                    return null;
                }
                CBSNode p = open.removeMin();
                var conflict = Validate(p.solution);
                //reseni je validni
                if (conflict == null)
                {
                    return p.solution;
                }
                else
                {
                    //konflikt je vzdy mezi dvema agenty
                    foreach (var aID in conflict.AgentInConflict())
                    {
                        var newNode = new CBSNode(new HashSet<Constraint>(p.constraints.ToArray()), new List<Plan>(p.solution.ToArray()));
                        //pokud jsme nasli konflikt vymeny vrcholu
                        if (conflict is SwapConflict)
                        {
                            //moveFrom je vrchol, ze ktereho jel prvni agent uvedeny v konfliktu
                            Vertex moveFrom = ((SwapConflict)conflict).moveFrom;
                            if (aID == conflict.AgentInConflict()[0])
                            {
                                newNode.constraints.Add(new SwapConstraint(aID, conflict.vertex, conflict.time, moveFrom));

                            }
                            else
                                newNode.constraints.Add(new SwapConstraint(aID, moveFrom, conflict.time, conflict.vertex));
                        }
                        //jinak se jedna o vrcholovy konflikt
                        else
                        {
                            //v pripade k-robustnosti pro k > 0 muze nastat konflikt i v case 0
                            //pokud se ale jedna o startovni vrchol tohoto agenta, neni mozne zaridit, aby tam v case 0 nebyl
                            if (agents[aID].start == conflict.vertex && conflict.time == 0)
                            {
                                continue;
                            }
                            newNode.constraints.Add(new Constraint(aID, conflict.vertex, conflict.time, conflict.duration));

                        }
                        //minimalni delka nove cesty se nikdy nezkrati
                        //uvazujeme-li model stay at target musime delku hlidat -> agent zustava v cili az do casu nekonecno
                        //zkraceni reseni muze zapricinit ignorovani podminky a vznikne novy konflikt
                        //pokud chceme zmenit cestu agentovi, u nehoz nastal koflikt ve chvili, kdy uz stal v cili, minimum se zvetsi
                        int minL = MinimalPathLength(p.solution[aID],conflict, aID);
                        //mame reseni, kde vsichni agenti dojedou do cile do casu cost a pak uz stoji - cesta delsi nez cost + pocet vrcholu existovat nemuze
                        maxDepth = g.activeVertices + p.cost;
                        Plan path1 = lowLevelSearch.SearchPath(agents[aID], g, minL, newNode.constraints, StartTime(aID),maxDepth);
                        if (path1 == null)
                        {
                            continue;
                        }
                        newNode.ChangeSolution(path1, aID);

                        //pokud je to kandidat na nejlepsi reseni, pridame ho do haldy
                        if (newNode.cost < maxSolLength)
                        {
                            open.insert(newNode.cost, newNode);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Trida s implementaci uzlu, ktery pouzivame v algoritmu CBS.
        /// Kazdy uzel obsahuje mnozinu podminek, (castecne) reseni, a cenu vypocitanou dle ucelove funkce.
        /// </summary>
        class CBSNode
        {
            public HashSet<Constraint> constraints;
            public List<Plan> solution { get; }
            public int cost;
            T costFunction;

            /// <summary>
            /// Vytvori novy uzel bez podminek s praznym planem a cenou 0.
            /// </summary>
            public CBSNode()
            {
                solution = new List<Plan>();
                constraints = new HashSet<Constraint>();
                cost = 0;
                costFunction = new T();

            }
            /// <summary>
            /// Vytvori uzel s podminkami constraints, resenim solution a spocita jeho cenu.
            /// </summary>
            public CBSNode(HashSet<Constraint> constraints, List<Plan> solution)
            {
                costFunction = new T();
                if (constraints == null)
                    this.constraints = new HashSet<Constraint>();
                else
                    this.constraints = constraints;
                this.solution = solution;
                cost = costFunction.GetCost(solution);
            }
            /// <summary>
            /// V reseni zmeni cestu agentovi agentID.
            /// </summary>
            public void ChangeSolution(Plan newSolution, int agentID)
            {
                solution[agentID] = newSolution;
                cost = costFunction.GetCost(solution);
            }
            /// <summary>
            /// Do reseni prida cestu pro agenta agentID.
            /// </summary>
            /// <param name="agentID"></param>
            /// <param name="p"></param>
            public void AddSolution(int agentID, Plan p)
            {
                solution.Insert(agentID, p);
                cost = costFunction.GetCost(solution);
            }
        }
    }

    /// <summary>
    /// Trida rozsirujici tridu CBSsolver o detekci konfliktu vymeny vrcholu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CBSwithSwapping<T> : CBSsolver<T> where T :CostFunction,new()
    {
        /// <summary>
        /// Pri validaci hlidame vrcholovy konflikt i konflikt vymeny vrcholu.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>Vrcholovy konflikt, nebo konflikt vymeny vrcholu, ktery nastal nejdrive. Pokud je plan validni, vraci null.</returns>
        protected override Conflict Validate(List<Plan> paths)
        {
            //najdeme nejdelsi cestu
            int steps = paths.Max(p => p.GetLength());
            //postupujeme od zacatku smerem do konce --> najdeme nejblizsi konflikt od pocatku
            for (int k = 0; k < steps; k++)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    for (int j = i + 1; j < paths.Count; j++)
                    {
                        //zkontrolujeme konflikt u dane dvojice i,j v case k
                        int maxL = Math.Max(paths[i].GetLength(), paths[j].GetLength());
                        int minL = Math.Min(paths[i].GetLength(), paths[j].GetLength());
                        //pokud zkoumame cas, kdy uz jsou oba v cili, pak konflikt mit nebudou --> kontrolujeme jen pro k < maxL
                        if (k < maxL)
                        {
                            //kontrola vrcholoveho konfliktu
                            if (paths[i].GetNth(k) == paths[j].GetNth(k))
                            {
                                //nalezen vrcholovy konflikt - agenti i,j v case k
                                return new Conflict(i, j, paths[i].GetNth(k), k);
                            }
                            //kontrola konfliktu vymeny vrcholu - pokud k > minL pro buno i, pak  paths[i].GetNth(k) = paths[i].GetNth(k+1) a konflikt uz by se odhalil o krok driv
                            if (k < minL)
                            {
                                if (paths[i].GetNth(k) == paths[j].GetNth(k + 1) && paths[i].GetNth(k + 1) == paths[j].GetNth(k))
                                {
                                    //nalezen konflikt vymeny vrcholu - agenti i,j cas je mezi k a k+1
                                    return new SwapConflict(i, j, paths[i].GetNth(k + 1), k + 1, paths[i].GetNth(k));
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Trida rozsirujici CBSsolver o detekci k-delay konfliktu.
    /// Timto algoritmem hledame k-robustni plany.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class I_k_CBSsolver<T> : CBSsolver<T> where T : CostFunction,new()
    {
        /// Inicializuje novy I_k_CBS solver vracejici k-robustni plany, ktery pri prohledavani na nizsi urovni pouziva algoritmus A*.
        public I_k_CBSsolver(int k) : base()
        {
            this.k = k;
        }
        /// <summary>
        /// Hleda konflikt pro dvojici planu v case time.
        /// </summary>
        private Conflict CheckConflict2(Plan p1, Plan p2, int id1, int id2, int time)
        {
            int maxL = Math.Max(p1.GetLength(), p2.GetLength());
            int minL = Math.Min(p1.GetLength(), p2.GetLength());
            //kontrolujeme pouze pokud agenti jeste nejsou v case time v cili
            if (time < maxL)
            {
                if (p1.GetNth(time) == p2.GetNth(time))
                {
                    //vrcholovy konflikt
                    return new Conflict(id1, id2, p1.GetNth(time), time, k);
                }
                //k-delay konflikt musime zkontrolovat pro k po sobe jdoucich casovych useku
                if (time < minL)
                {
                    //urcime, do jakeho casu musime pocitat (nejdele k-kroku)
                    int stop = Math.Min(time + k, maxL - 1);
                    for (int j = time + 1; j <= stop; j++)
                    {
                        if (p1.GetNth(time) == p2.GetNth(j))
                        {
                            return new Conflict(id1, id2, p1.GetNth(time), time, k);
                        }
                        if (p2.GetNth(time) == p1.GetNth(j))
                        {
                            return new Conflict(id1, id2, p2.GetNth(time), time, k);
                        }
                    }
                }
                //Pokud je time >= minL, pak jeden z agentu uz je v case time v cili.
                //Tento cilovy vrchol byl zkontrolovan s vrcholem druheho agenta v case time.
                //V casech time+1 az maxL bude zkontrolovan pri dalsich volanich teto funkce.
            }
            return null;
        }
        /// <summary>
        /// Hleda k-delay konflikt.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>Konflikt, ktery nastal nejdrive, nebo null, pokud je plan validni.</returns>
        protected override Conflict Validate(List<Plan> paths)
        {
            int steps = paths.Max(p => p.GetLength());
            for (int l = 0; l < steps; l++)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    for (int j = i + 1; j < paths.Count; j++)
                    {
                        //kazdou dvojici cest zkontrolujeme
                        Conflict c = CheckConflict2(paths[i], paths[j], i, j, l);
                        if (c != null)
                        {
                            return c;
                        }
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Trida s implementaci CBS algoritmu pro modifikovany MAPF problem.
    /// Detekuje vrcholovy konflikt a konflikt vymeny vrcholu.
    /// </summary>
    class CBSsolverForModifiedMapf<T>: CBSsolver<T> where T : CostFunction, new()
    {
        Dictionary<Vertex, Reservation> safeIntervals;
        List<int> initialTimes;
        int[] pseudoID;
        public CBSsolverForModifiedMapf(Dictionary<Vertex, Reservation> safeIntervals, List<int> initialTimes, int[] pseudoID, int secondaryMain, int mainSecondary): base()
        {
            this.safeIntervals = safeIntervals;
            this.initialTimes = initialTimes;
            this.pseudoID = pseudoID;
            lowLevelSearch = new AStarForModifiedMapf(safeIntervals,pseudoID, secondaryMain, mainSecondary);
        }

        /// <summary>
        /// Vraci minimalni delku noveho planu, ktery muze vzniknout opravenim konfliktu c agenta agentID v planu p.
        /// </summary>
        protected override int MinimalPathLength(Plan p, Conflict c, int id)
        {
            if (initialTimes[id] + p.GetLength() <= c.time)
            {
                return c.time + 1;
            }
            else return initialTimes[id] + p.GetLength();
        }

        /// <summary>
        /// Pri validaci hlidame vrcholovy konflikt i konflikt vymeny vrcholu.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>Vrcholovy konflikt, nebo konflikt vymeny vrcholu, ktery nastal nejdrive. Pokud je plan validni, vraci null.</returns>
        protected override Conflict Validate(List<Plan> paths)
        {
            ModifiedMakespan.startTimes = initialTimes;
            var maximalTime = new ModifiedMakespan().GetCost(paths);
            for (int t = 0; t < maximalTime; t++)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    for (int j = i + 1; j < paths.Count; j++)
                    {
                        //agent nema konflikt sam se sebou
                        if (pseudoID[i] == pseudoID[j])
                        {
                            continue;
                        }
                        int from = Math.Max(initialTimes[i], initialTimes[j]);
                        int to = Math.Max(initialTimes[i] + paths[i].GetLength(), initialTimes[j] + paths[j].GetLength());
                        if (t >= from && t < to)
                        {
                            if (paths[i].GetNth(t - initialTimes[i]) == paths[j].GetNth(t - initialTimes[j]))
                            {
                                //nalezen vrcholovy konflikt - agenti i,j v case k
                                return new Conflict(i, j, paths[i].GetNth(t - initialTimes[i]), t);
                            }
                            if (paths[i].GetNth(t - initialTimes[i]) == paths[j].GetNth(t + 1 - initialTimes[j]) &&
                                paths[i].GetNth(t + 1 - initialTimes[i]) == paths[j].GetNth(t - initialTimes[j]))
                            {
                                //nalezen konflikt vymeny vrcholu - agenti i,j cas je mezi k a k+1
                                return new SwapConflict(i, j, paths[i].GetNth(t + 1 - initialTimes[i]),
                                    t + 1, paths[i].GetNth(t - initialTimes[i]));
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Vraci startovni cas agenta s cislem id.
        /// </summary>
        protected override int StartTime(int id)
        {
            return initialTimes[id];
        }
    }
}

