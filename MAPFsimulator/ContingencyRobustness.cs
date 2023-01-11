using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida s implementaci algoritmu na hledani robustnich planu zalozenych na technice contingency planning.
    /// Cast je spolecna pro alternativni k-robustnost a semi k-robustnost.
    /// </summary>
    abstract class ContigencyRobustness
    {
        //pocet alternativ, ktere byly pridany do hlavniho planu
        public static int realAlt = 0;
        //minimalni casova mezera, kterou musi mit alternativni cesta vuci hlavni
        public int secondaryMain;
        //minimalni casova mezera, kterou musi mit hlavni cesta garantovanou vuci vsem alternativnim cestam
        public int mainSecondary;
        //true, pokud pro kazdou hodnotu zpozdeni z garantovaneho intervalu najdu cestu zvlast
        public bool separateAlternatives;
        protected List<Plan> mainPlan;
        //true pro striktni pristup
        bool strict;
        public ContigencyRobustness(int secondaryMain, int mainSecondary, bool separateAlternatives, bool strict = false)
        {
            this.secondaryMain = secondaryMain;
            this.mainSecondary = mainSecondary;
            this.separateAlternatives = separateAlternatives;
            this.strict = strict;
        }

        /// <summary>
        /// Vygeneruje alternativni cesty k hlavnimu tak, aby splnoval podminky dane robustnosti.
        /// </summary>
        /// <param name="mainPlan">hlavni plan</param>
        /// <param name="g">graf</param>
        /// <param name="agents">agenti</param>
        /// <param name="minSafeMain"> bezpecny interval mezi dvema hlavnimi plany </param>
        /// <returns>Plan splnujici podminky robustnosti, nebo null, pokud takovy neexistuje.</returns>
        public virtual List<ContingencyPlan> GetRobustPlan(List<Plan> mainPlan, Graph g, List<IAgent> agents, int minSafeMain)
        {
            if (mainPlan==null)
            {
                return null;
            }
            realAlt = 0;
            List<ContingencyPlan> alternatives = new List<ContingencyPlan>();
            this.mainPlan = mainPlan;
            //nahrajeme hlavni plany a nastavime bezpecne intervaly
            for (int i = 0; i < mainPlan.Count; i++)
            {
                alternatives.Add(new ContingencyPlan(mainPlan[i].GetPath()));
            }
            PlanProcessing pp = new PlanProcessing(mainPlan);
            var safeIntervals = pp.GetSafeIntervals(minSafeMain);
            var Bset = pp.GetBset(minSafeMain);
            var conflictSet = pp.GetConflictSet(minSafeMain);

            //pro hledani pouzivame upraveny A* algoritmus
            AStarForContingencyPlan astar = new AStarForContingencyPlan(safeIntervals, secondaryMain, mainSecondary);
            var newAgents = new List<IAgent>();
            var startTimes = new List<int>();
            var vertexOrder = new List<int>();
            foreach (var safeVio in Bset)
            {
                int order;
                //najdeme odkud vest alternativni plan
                Vertex v = FindVertexForAlternatePlan(safeVio, conflictSet, out order);
                //pokud existuje
                if (v.x != -1)
                {
                    //odkud zacne cyklus - cesta bude bud 1 (alt k-rob), nebo pro kazde jedno zpozdeni (semi k-rob)
                    int indexFrom = separateAlternatives ? minSafeMain : safeVio.length + 1;
                    for (int i = safeVio.length+1; i <= indexFrom; i++)
                    {
                        //mame fiktivniho agenta, ktery shani cestu z vrcholu, ktery jsme zvolili (cil ma stejny)
                        //cesta se zacne hledat od casu: 
                            //order [to je naplanovany cas prijezdu do vetviciho vrcholu] + duration [to je delka bezpecneho intervalu, tedy takove zpozdeni mi u agenta jeste nevadi a proto nepotrebujeme jit alternativou] + 1
                        var fakeAgent = AgentFactory.CreateAgent(v, agents[safeVio.agentID].target, safeVio.agentID, false);
                        newAgents.Add(fakeAgent);
                        startTimes.Add(order + i);
                        vertexOrder.Add(order);
                    }
                    //neexistence vrcholu ovsem neovlivnuje existenci reseni prislusneho robustniho planu
                }
            }
            //ve striknim pristupu zkonstruujeme modifikovany MAPF problem, ktery nasledne vyresime CBS algoritmem
            if (strict)
            {
                //musime precislovat agenty, abychom dodrzeli unikatni id
                int[] pseudoID = new int[newAgents.Count];
                newAgents = ReduceAgents(newAgents, startTimes, vertexOrder);
                startTimes = startTimes.Where(t => t >= 0).ToList();
                vertexOrder = vertexOrder.Where(v => v >= 0).ToList();
                for (int i = 0; i < newAgents.Count; i++)
                {
                    pseudoID[i] = newAgents[i].id;
                    newAgents[i].id = i;
                }
                ModifiedMakespan.startTimes = startTimes;
                var solver = new CBSsolverForModifiedMapf<ModifiedMakespan>(safeIntervals, startTimes, pseudoID,secondaryMain,mainSecondary);
                var alter = solver.GetPlan(g, newAgents);
                if (alter==null)
                {
                    return null;
                }
                for (int i = 0; i < alter.Count; i++)
                {
                    var path = alter[i].GetPath();
                    //pokud je cesta delky 1, znamena to, ze agent muze zustat v cili, aniz by hrozil konflikt - tedy cesta
                    //jineho agenta, kvuli ktere musel v CBS cil opustit se behem algoritmu zmenila
                    if (path.Count > 1)
                    {
                        path.RemoveAt(0);
                    }
                    alternatives[pseudoID[i]].AddAlternatePlan(
                                path, vertexOrder[i], new Condition(startTimes[i] - vertexOrder[i], startTimes[i] - vertexOrder[i] + secondaryMain));
                    realAlt++;
                }

            }
            //volny pristup
            else
            {
                for (int i = 0; i < newAgents.Count; i++)
                {
                    //omezeni na delku alternativni cesty - po makespan krocich uz budou vsichni agenti na svych cilech, 
                    //takze delka cesty pokud existuje musi byt mensi nez makespan + pocet vrcholu grafu
                    var plan = astar.SearchPath(newAgents[i], g, 0, null, startTimes[i],g.activeVertices + new Makespan().GetCost(mainPlan));
                    if (plan != null)
                    {
                        var conPlan = plan.GetPath();
                        //plan zacal z nami znameho vrcholu, ktery je spolecny s hlavnim planem
                        //takze prvni vrchol z alternativniho planu je spolecny (uz ho zname)
                        if (conPlan.Count > 1)
                        {
                            conPlan.RemoveAt(0);
                            int maxDelay = 
                                separateAlternatives ? startTimes[i] - vertexOrder[i] : startTimes[i] - vertexOrder[i] + secondaryMain;
                            //agentovi newAgents[i] pridame do jeho planu conPlan alternativni cestu z vrcholu s cislem vertex[i]
                            //tato cesta se vybere, pokud je aktualni zpozdeni pri prujezdu vrcholem s cislem vertexOrder[i] v intervalu urcenem Condition
                            alternatives[newAgents[i].id].AddAlternatePlan(
                                conPlan, vertexOrder[i], new Condition(startTimes[i] - vertexOrder[i], maxDelay));
                            realAlt++;
                        }
                    }
                    //neexistuje-li cesta, pak neexistuje ani plan splnujici danou formu robustnosti
                    else
                    {
                        return null;
                    }
                }

            }
            return alternatives;
        }
        /// <summary>
        /// Odstrani duplicity v hledani alternativnich cest - vzhledem ke hledani pocatecniho vrcholu alternativniho planu u
        /// alternativni k-robustnosti se muze stat, ze dve alternativy splynou do jedne - tim si usetrime duplicitni hledani te same cesty.
        /// </summary>
        private List<IAgent> ReduceAgents(IList<IAgent> agents, IList<int> initialTimes, IList<int> vertexOrder)
        {
            var reduced = new List<IAgent>();
            for (int i = 0; i < agents.Count; i++)
            {
                bool add = true;
                for (int j = i+1; j < agents.Count; j++)
                {
                    //jedna se o tu samou cestu, pokud zacina a konci ve stejnem vrcholu a hledani by zacalo ve stejny cas
                    if (agents[i].start==agents[j].start && agents[i].target==agents[j].target && initialTimes[i]==initialTimes[j])
                    {
                        add = false;
                        initialTimes[i] = -1;
                        vertexOrder[i] = -1;
                        break;
                    }
                }
                if (add)
                {
                    reduced.Add(agents[i]);
                }
            }
            return reduced;
        }
        protected abstract Vertex FindVertexForAlternatePlan(SafeSpace s, Dictionary<Vertex, int> conflictSet, out int order);
    }

    /// <summary>
    /// Konkretni implementace robustnosti pomoci contingency planning - semi k-robustnost.
    /// </summary>
    class SemiK_rob : ContigencyRobustness
    {
        public SemiK_rob() : base(0, 1, true) { }

        /// <summary>
        /// Vygeneruje alternativni cesty k hlavnimu tak, aby splnoval podminky dane robustnosti.
        /// </summary>
        /// <param name="mainPlan">hlavni plan</param>
        /// <param name="g">graf</param>
        /// <param name="agents">agenti</param>
        /// <param name="minSafeMain"> bezpecny interval mezi dvema hlavnimi plany </param>
        /// <returns>Plan splnujici podminky robustnosti, nebo null, pokud takovy neexistuje.</returns>
        public override List<ContingencyPlan> GetRobustPlan(List<Plan> mainPlan, Graph g, List<IAgent> agents, int minSafeMain)
        {
            return base.GetRobustPlan(mainPlan, g, agents, minSafeMain);
        }
        /// <summary>
        /// Vraci vrchol, ze ktereho povede alternativni cestu. Jedna o vrchol, ktery bezprostredne predchazi mistu potencialni kolize.
        /// </summary>
        protected override Vertex FindVertexForAlternatePlan(SafeSpace s, Dictionary<Vertex, int> conflictSet, out int order)
        {
            order = s.time - 1;
            if (order < 0)
            {
                return new Vertex(-1, -1);
            }
            return mainPlan[s.agentID].GetNth(s.time-1);
        }
    }

    /// <summary>
    /// Konkretni implementace robustnosti pomoci contingency planning - alternativni k-robustnost.
    /// </summary>
    class AlternativeK_rob : ContigencyRobustness
    {
        public AlternativeK_rob(int secondaryMain, bool strict) : base(secondaryMain, secondaryMain, false, strict) { }

        /// <summary>
        /// Vygeneruje alternativni cesty k hlavnimu tak, aby splnoval podminky dane robustnosti.
        /// </summary>
        /// <param name="mainPlan">hlavni plan</param>
        /// <param name="g">graf</param>
        /// <param name="agents">agenti</param>
        /// <param name="minSafeMain"> bezpecny interval mezi dvema hlavnimi plany </param>
        /// <returns>Plan splnujici podminky robustnosti, nebo null, pokud takovy neexistuje.</returns>
        public override List<ContingencyPlan> GetRobustPlan(List<Plan> mainPlan, Graph g, List<IAgent> agents, int minSafeMain)
        {
            return base.GetRobustPlan(mainPlan, g, agents, minSafeMain);
        }

        /// <summary>
        /// Vraci vrchol, ze ktereho povede alternativni cestu. Jedna o nejblizsi mozny vrchol, ktery nepatri do konfliktni mnoziny.
        /// </summary>
        protected override Vertex FindVertexForAlternatePlan(SafeSpace s, Dictionary<Vertex, int> conflictSet, out int order)
        {
            order = -1;
            for (int i = s.time - 1; i >= 0; i--)
            {
                if (!conflictSet.ContainsKey(mainPlan[s.agentID].GetNth(i)))
                {
                    order = i;
                    return mainPlan[s.agentID].GetNth(i);
                }
            }
            return new Vertex(-1, -1);
        }
    }
}
