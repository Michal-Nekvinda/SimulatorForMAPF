using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    interface IPlansExecutor
    { 
        List<double>[] ExecuteSolution(List<Plan> plans, List<IAgent> agents, out string message, out Conflict conf, out int length);
    }
    
    /// <summary>
    /// Trida zajistujici provadeni planu.
    /// Simuluje exekuci planu s urcitou pravdepodobnosti zpozdeni a detekuje vrcholove konflikty a konflikty vymeny vrcholu.
    /// </summary>
    class SimpleExecution : IPlansExecutor
    {
        public int colTime = 0;
        public string colType = "";

        protected double delay;
        //datova struktura s poradovymi cisly vrcholu, ve kterych se agenti nachazeji v danem case
        protected List<double>[] positionsInTime;
        //fronta pro detekci konfliktu vymeny vrcholu
        protected Queue<Vertex>[] swapConfStruct;
        protected int agentsCount;
        protected HashSet<Vertex> currentVertices;
        protected bool vertexConflict;
        protected bool swappingConflict;
        protected int maxTime;
        protected Conflict confV;

        /// <summary>
        /// Exekuce planu pro pocet agentu agents se zpozdenim delay.
        /// </summary>
        public SimpleExecution(int agentsCount, double delay)
        {
            this.agentsCount = agentsCount;
            positionsInTime = new List<double>[agentsCount];
            swapConfStruct = new Queue<Vertex>[agentsCount];
            this.delay = delay;
            for (int i = 0; i < agentsCount; i++)
            {
                positionsInTime[i] = new List<double>();
                positionsInTime[i].Add(0);
                swapConfStruct[i] = new Queue<Vertex>();
                //vypln pomoci fiktivniho vrcholu
                swapConfStruct[i].Enqueue(new Vertex(-1, -1));
            }
            maxTime = 1;
        }

        /// <summary>
        /// Provede exekuci planu ulozenych v listu plans. 
        /// </summary>
        /// <param name="plans"></param>
        /// <param name="agents"></param>
        /// <param name="message">zprava o prubehu exekuce - zda skoncila uspesne, ci nikoliv</param>
        /// <param name="conf">pokud nastal konflikt, je vracena jeho instance, jinak je null</param>
        /// <param name="length">delka provedeneho planu (tedy i vcetne zpozdeni)</param>
        /// <returns>List abstraktnich pozic agentu v jednotlivych casovych usecich. 
        /// Abstraktni pozice je poradove cislo vrcholu v puvodnim nezpozdenem planu. Jedna se o desetinna cisla kvuli min/max robustnosti.</returns>
        public virtual List<double>[] ExecuteSolution(List<Plan> plans, List<IAgent> agents, out string message, out Conflict conf, out int length)
        {
            //pocatecni nastaveni promennych
            colTime = -1;
            colType = "normal";
            bool finished = false;
            vertexConflict = false;
            swappingConflict = false;
            int time = 1;
            for (int i = 0; i < plans.Count; i++)
            {
                swapConfStruct[i].Enqueue(plans[i].GetNth(0));
            }
            //dokud nejsou vsichni agenti v cili
            while (!finished)
            {
                finished = true;
                currentVertices = new HashSet<Vertex>();
                //pro plan kazdeho z agentu zacnu pocitat od casu 1 (cas 0 = startovni vrchol)
                //positionsInTime[i][j] = poradove cislo vrcholu agenta i v case j (0 = start a tedy 1. vrchol cesty, 1 = 2.vrchol cesty,...)
                for (int i = 0; i < plans.Count; i++)
                {
                    //pokud ma zpozdeni, zustava tam, kde byl predtim
                    if (WillDelay(i, plans[i], time))
                    {
                        positionsInTime[i].Add(positionsInTime[i][time - 1]);
                    }
                    //jinak se posouva do dalsiho vrcholu
                    else
                    {
                        positionsInTime[i].Add(positionsInTime[i][time - 1] + CurrentSpeed(agents[i], i, plans[i]));
                    }
                    //pokud nektery z agentu jeste neni v cili (ma pred sebou jeste nejake vrcholy), bude nasledovat dalsi iterace cyklu
                    if (finished && plans[i].HasNextVertex(DoubleToInt.ToInt(positionsInTime[i][time])))
                    {
                        finished = false;
                    }
                    //zda byl objeven hranovy konflikt
                    if (!swappingConflict && !vertexConflict && IsVertexConflict(i, plans[i], time, plans))
                    {
                        vertexConflict = true;
                    }
                }

                //kontrola swapping konfliktu
                if (!vertexConflict && !swappingConflict && IsSwappingConflict(time))
                {
                    swappingConflict = true;
                }
                time++;
            }

            length = time - 1;
            if (new Makespan().GetCost(plans) == 0)
            {
                length = 0;
            }
            //pri provadeni doslo ke kolizi
            if (vertexConflict || swappingConflict)
            {
                message = "ended with conflict";
                colTime = confV.time;
                TypOfConf(plans, confV.agentID1, confV.agentID2, DoubleToInt.ToInt(positionsInTime[confV.agentID1][confV.time]), DoubleToInt.ToInt(positionsInTime[confV.agentID2][confV.time]));

            }
            else
                message = "ended successfully";

            conf = confV;
            return positionsInTime;

        }
        /// <summary>
        /// Vraci aktualni rychlost, se kterou se agent i pohybuje ve svem planu p.
        /// Pro min/max robusnost se meni, pro ostatni je 1.
        /// </summary>
        protected virtual double CurrentSpeed(IAgent agent, int i, Plan p)
        {
            return 1;
        }
        /// <summary>
        /// Vraci true, pokud ma agentID s planem p v case t vrcholovy konflikt s jinym agentem.
        /// </summary>
        protected virtual bool IsVertexConflict(int agentID, Plan p, int t, List<Plan> plans)
        {
            Vertex v = p.GetNth(positionsInTime[agentID][t]);
            swapConfStruct[agentID].Dequeue();
            swapConfStruct[agentID].Enqueue(v);
            //vrchol uz v hash setu je  - tedy mame konflikt - zbyva detekovat druheho z agentu
            if (!currentVertices.Add(v))
            {
                int theSecond = 0;
                for (int i = 0; i < agentID; i++)
                {
                    if (v == plans[i].GetNth(positionsInTime[i][t]))
                    {
                        theSecond = i;
                        break;
                    }
                }
                //konflikt ulozime a vratime po provedeni planu
                confV = new Conflict(agentID, theSecond, v, t);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Vraci true, pokud byl v planech agentu v case time detekovan konflikt vymeny vrcholu.
        /// </summary>
        protected virtual bool IsSwappingConflict(int time)
        {
            for (int i = 0; i < agentsCount; i++)
            {
                for (int j = i + 1; j < agentsCount; j++)
                {
                    Vertex v1 = swapConfStruct[i].Peek();
                    Vertex v2 = swapConfStruct[j].Peek();
                    if (v1 == v2)
                    {
                        //maji vrcholovy konflikt
                        return false;
                    }
                    //kazda fronta ma prave 2 prvky
                    //agenti nemaji vrcholovy konflikt - to jsme zkontrolovali drive
                    //pokud tedy fronty navzajem obsahuji posledni pridane vrcholy, maji swapping conflict
                    if (swapConfStruct[i].Contains(v2) && swapConfStruct[j].Contains(v1))
                    {
                        //konflikt ulozime a vratime po provedeni planu
                        confV = new SwapConflict(i, j, v1, time, v2);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Vraci true, pokud se agent v planu p v case t opozdi.
        /// </summary>
        protected virtual bool WillDelay(int i, Plan p, int t)
        {
            bool inVertex = DoubleToInt.DecimalPart(positionsInTime[i][t - 1] / maxTime) == 0;
            //pokud agent mel provest akci wait, nebo uz je na konci planu, tak vratime false, protoze se nezpozdi
            if (inVertex && p.GetNth(positionsInTime[i][t - 1]) ==
                p.GetNth(positionsInTime[i][t - 1] + 1))
            {
                return false;
            }
            double dd = DoubleGenerator.GetInstance().NextDouble();
            if (dd < delay)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Rozlisuje, jaky typ konfliktu mezi agenty nastal (pro contigency robustnost).
        /// </summary>
        protected void TypOfConf(List<Plan> plans, int ag1, int ag2, int vertex1, int vertex2)
        {
            if (plans[ag1].IsInMainPlan(vertex1))
            {
                if (plans[ag2].IsInMainPlan(vertex2))
                {
                    colType = "Hlavni";
                }
                else
                {
                    colType = "Hlavni+Vedlejsi";
                }
            }
            else
            {
                if (plans[ag2].IsInMainPlan(vertex2))
                {
                    colType = "Hlavni+Vedlejsi";
                }
                else
                {
                    colType = "Vedlejsi";
                }
            }
        }

    }

    /// <summary>
    /// Exekuce planu, s upravami pro stromove plany.
    /// Oproti klasicke exekuci musime hlidat vypocet abstraktnich pozic nasledniku - v normalnim pripade se jedna vzdy o posun o 1,
    /// zde to muze byt vic (pri presunu z hlavni na alternativni cestu)
    /// </summary>
    class ContigencyExecution : SimpleExecution
    {
        int[] lastVertexNumbers;
        int[] currentDelays;
        public ContigencyExecution(int agentsCount, double delay) : base(agentsCount, delay)
        {
            lastVertexNumbers = new int[agentsCount];
            currentDelays = new int[agentsCount];
        }
        /// <summary>
        /// Na zaklade aktualniho zpozdeni agenta i v planu p vraci poradove cislo vrcholu, do ktereho agent pojede.
        /// Tedy za normalnich okolnosti se pohybuje po hlavnim planu (vzdy o 1), v pripade, ze prejizdi na alternativni plan,
        /// tak se pomoci teto metody vypocita rozdil poradovych cisel, o ktere se musi agent posunout, aby dale pokracoval v alternativnim planu.
        /// </summary>
        protected override double CurrentSpeed(IAgent agent, int i, Plan p)
        {
            int newVertex;
            p.GetNext(lastVertexNumbers[i], out newVertex, currentDelays[i]);
            int tmp = lastVertexNumbers[i];
            lastVertexNumbers[i] = newVertex;
            return newVertex - tmp;
        }
        /// <summary>
        /// Vraci true, pokud se i-ty agent v planu p v case t opozdi.
        /// </summary>
        protected override bool WillDelay(int i, Plan p, int t)
        {
            //pokud agent mel provest akci wait, nebo uz je na konci planu, tak vratim false, protoze se nezpozdi
            if (p.GetNth(positionsInTime[i][t - 1]) ==
                p.GetNext(DoubleToInt.ToInt(positionsInTime[i][t - 1]), out _, currentDelays[i]))
            {
                return false;
            }
            double dd = DoubleGenerator.GetInstance().NextDouble();
            if (dd < delay)
            {
                currentDelays[i]++;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Exekuce planu, s upravami min/max robustnost.
    /// Oproti klasicke exekuci pouzivame notaci poradove cislo vrcholu + cast hrany.
    /// Dale take umoznujeme menit rychlost, se kterou se agenti v grafu pohybuji.
    /// </summary>
    class Min_MaxRobustExecution : SimpleExecution
    {
        List<Vertex> last;
        List<Vertex> next;
        double[] edgePart;
        double[] currentSpeed;
        int minTime;
        /// <summary>
        /// Min/max robustni exekuce pro pocet agentu agentu agents, s pravdepodobnosti zpozdeni delay a parametry robustnosti minTime, maxTime. 
        /// </summary>
        public Min_MaxRobustExecution(int agentsCount, int minTime, int maxTime, double delay) : base(agentsCount, delay)
        {
            last = new List<Vertex>();
            next = new List<Vertex>();
            edgePart = new double[agentsCount];
            currentSpeed = new double[agentsCount];
            for (int i = 0; i < agentsCount; i++)
            {
                currentSpeed[i] = 1.0 / maxTime;
            }
            this.maxTime = maxTime;
            this.minTime = minTime;
        }
        /// <summary>
        /// Aktualni rychlost agenta i v planu p.
        /// </summary>
        protected override double CurrentSpeed(IAgent agent, int i, Plan p)
        {
            return currentSpeed[i];
        }
        /// <summary>
        /// Na zaklade zpozdeni agenta agentID pocita novou rychlost.
        /// </summary>
        protected void ChangeSpeed(double vertex, int time, int agentID)
        {
            double expectedTime = vertex * maxTime;
            double delay = time - expectedTime;
            double edgeTime = Math.Max(maxTime - delay, minTime);
            currentSpeed[agentID] = 1.0 / edgeTime;
        }
        /// <summary>
        /// Spocita a ulozi u daneho agenta agentID vrchol, ve kterem byl naposled, vrchol, kam se chysta jet a cast hrany, 
        /// kterou uz projel.
        /// V pripade, ze agent stoji primo ve vrcholu, spocita na zaklade zpozdeni jeho novou rychlost.
        /// </summary>
        protected override bool IsVertexConflict(int agentID, Plan p, int t,List<Plan> plans)
        {
            if (last.Count==agentsCount)
            {
                last.Clear();
                next.Clear();
            }
            double v = positionsInTime[agentID][t];
            last.Add(p.GetNth(v));
            next.Add(p.GetNth(v+1));
            edgePart[agentID] = DoubleToInt.DecimalPart(v);
            if (edgePart[agentID] == 0)
            {
                //je ve vrcholu
                ChangeSpeed(v, t, agentID);
            }
            return false;
        }
        /// <summary>
        /// Projede zaznamy vsech agentu a urci, zda v casovem useku time nastal konflikt (vrcholovy i swapping).
        /// </summary>
        protected override bool IsSwappingConflict(int time)
        {
            for (int i = 0; i < agentsCount; i++)
            {
                for (int j = i+1; j < agentsCount; j++)
                {
                    //pokud 2 agenti vyjeli ze stejneho vrcholu
                    if (last[i]==last[j])
                    {
                        //pokud tam oba jeste stoji, pak vrcholovy konflikt
                        if (edgePart[i] == 0 && edgePart[j] == 0)
                        {
                            vertexConflict = true;
                            confV = new Conflict(i, j, last[i], time);
                            return true;
                        }
                        //pokud je mezi nimi mezera mensi nez cast hrany, kterou ujedou za 1 casovy usek, pak take nastala kolize
                        if (Math.Abs(edgePart[i] - edgePart[j]) + DoubleToInt.epsilon < (1.0 / maxTime))
                        {
                            vertexConflict = true;
                            confV = new SwapConflict(i, j, next[i], time,last[i]);
                            return true;
                        }
                    }
                    if (last[i] == next[j])
                    {
                        //swapping conflict
                        if (last[j] == next[i])
                        {
                            //pokud se chystaji prejet po stejne hrane, tak nekdy ten konflikt na hrane proste nastane a uz neni uplne nutne vedet kdy
                            swappingConflict = true;
                            confV = new SwapConflict(i, j, last[i], time, last[j]);
                            return true;
                        }
                        //Pozn.
                        //pokud jeden z agentu do vrcholu v dojizdi a druhy z nej odjizdi, pak predpokladame, ze jsme zvolili zjemneni
                        //takove, ze mezi zustane bezpecna vzdalenost a nesrazili se
                    }
                }
            }
            return false;
        }
    }
}
