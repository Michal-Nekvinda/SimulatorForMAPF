using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida s implementaci (rozsireneho) prohledavaciho algortimu A*.
    /// </summary>
    class AStar
    {
        //navstivene vrcholy
        Dictionary<LowLevelNode, int> visited;

        //halda s otevrenymi uzly k prohledani
        IHeap<int, LowLevelNode> open;

        /// <summary>
        /// Hleda nejkratsi cestu pro agenta a v grafu g s minimalni delkou minL a startovnim casem startTime. 
        /// Vysledna cesta musi splnovat vsechna vsechny podminky z mnoziny omezeni c a musi skoncit do casu maxDepth.
        /// Jako heuristicka funkce je pouzita vzdalenost vrcholu v metrice Manhattan.
        /// </summary>
        /// <returns>Plan obsahujici nejkratsi cestu.</returns>
        public Plan SearchPath(IAgent a, Graph g, int minL, HashSet<Constraint> c, int startTime, int maxDepth)
        {
            //pokud jsou vrcholy startu ci cile nepristupne v grafu g, cesta neexistuje
            if (!g.IsVertex(a.start.x, a.start.y) || !g.IsVertex(a.target.x, a.target.y))
            {
                return null;
            }
            Init();
            LowLevelNode root = new LowLevelNode(a.start, startTime);
            open.insert(HeuristicManhattan(a.start, a.target), root);
            while (open.size() > 0)
            {
                double fVal = open.getMinKey();
                LowLevelNode minNode = open.removeMin();
                //pokud cas uzlu presahl maximalni pozadnou hloubku, skoncim neuspechem
                if (minNode.time > maxDepth)
                {
                    return null;
                }
                //pokud mam cestu a splnuje pozadavek na minimalni delku, vratim tuto cestu
                if (minNode.vertex == a.target && minNode.time >= minL - 1)
                {
                    List<Vertex> path = new List<Vertex>();
                    while (minNode != null)
                    {
                        path.Add(minNode.vertex);
                        minNode = minNode.parent;
                    }
                    path.Reverse();
                    return new Plan(path);
                }
                //jinak hledam dal - pokracuji se vsemi sousedy vrcholu
                var successors = g.GetNeighbours(minNode.vertex);
                foreach (var v in successors)
                {
                    int gVal = minNode.gVal + 1;
                    int time = minNode.time + 1;
                    //pokud vrchol splnuje vsechny podminky, pridam ho
                    if (!IsInConstraints(v, c, a.id, time, minNode.vertex))
                    {
                        LowLevelNode node = new LowLevelNode(v, time);
                        if (visited.ContainsKey(node))
                        {
                            if (visited[node] <= gVal)
                                continue;
                            else
                            {
                                node.gVal = gVal;
                                node.parent = minNode;
                                node.time = minNode.time + 1;
                            }
                        }
                        else
                        {
                            node.gVal = gVal;
                            node.parent = minNode;
                            node.time = minNode.time + 1;
                            visited.Add(node, gVal);
                        }
                        int hVal = HeuristicManhattan(v, a.target);
                        open.insert(hVal + gVal, node);
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// Inicializace datovych struktur.
        /// </summary>
        private void Init()
        {
            open = new RegularBinaryHeap<LowLevelNode>();
            visited = new Dictionary<LowLevelNode, int>();
        }

        /// <summary>
        /// Zkousi, zda je obsazeni vrcholu v agentem agentID v case time v rozporu s podminkami v mnozine constraints.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="constraints"></param>
        /// <param name="agentID"></param>
        /// <param name="time"></param>
        /// <param name="parent">predchudce vrcholu v</param>
        /// <returns>True, pokud je vrchol v v rozporu s podminkami v constraints</returns>
        protected virtual bool IsInConstraints(Vertex v, HashSet<Constraint> constraints, int agentID, int time, Vertex parent)
        {
            if (constraints == null)
            {
                return false;
            }
            return constraints.Contains(new TestConstraint(agentID, v, time)) || constraints.Contains(new SwapConstraint(agentID, v, time, parent));

        }

        /// <summary>
        /// Vypocet heuristiky Manhattan mezi vrcholy a,b.
        /// </summary>
        private int HeuristicManhattan(Vertex a, Vertex b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Trida s reprezentaci uzlu, ktery pouzivame v ramci algoritmu A*.
        /// Uzel obsahuje informaci o vrcholu, case, predchudci a hodnote sve funkce g.
        /// </summary>
        class LowLevelNode
        {
            public Vertex vertex;
            public int time;
            public int gVal;
            public LowLevelNode parent;
            public LowLevelNode(Vertex coord, int time)
            {
                this.vertex = coord;
                this.time = time;
                gVal = 0;
                parent = null;
            }
            /// <summary>
            /// Dva uzly se rovnaji, pokud reprezentuji totozny vrchol ve stejnem case.
            /// Potrebujeme ve chvili, kdy kontrolujeme, zda se kandidatni vrchol na pridani jiz vyskytuje v mnozine navstivenych vrcholu.
            /// </summary>
            public static bool operator ==(LowLevelNode a, LowLevelNode b)
            {
                if ((object)b == null)
                {
                    return (object)a == null;
                }
                return a.vertex == b.vertex && a.time == b.time;
            }
            public static bool operator !=(LowLevelNode a, LowLevelNode b) { return !(a == b); }
            public override bool Equals(object obj)
            {
                return this == (LowLevelNode)obj;
            }
            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + vertex.GetHashCode();
                hash = hash * 23 + time;
                return hash;
            }
        }

    }
    /// <summary>
    /// Upravena verze algoritmu, kterou pouzivame pri hledani alternativnich planu.
    /// </summary>
    class AStarForContingencyPlan : AStar
    {
        protected Dictionary<Vertex, Reservation> safeIntervals;
        protected int secondaryMain;
        protected int mainSecondary;
        /// <summary>
        /// Vytvori novou instanci tridy pro hledani alternativnich cest
        /// </summary>
        /// <param name="safeIntervals">mnozina obsahujici vrcholy a jejich bezpecne intervaly</param>
        /// <param name="secondaryMain">minimalni delka bezpecneho intervalu alternativnich cest vuci hlavnim cestam</param>
        /// <param name="mainSecondary">minimalni delka bezpecneho intervalu hlavnich cest vuci alternativnim cestam</param>
        public AStarForContingencyPlan(Dictionary<Vertex, Reservation> safeIntervals, int secondaryMain, int mainSecondary)
        {
            this.safeIntervals = safeIntervals;
            this.secondaryMain = secondaryMain;
            this.mainSecondary = mainSecondary;
        }

        /// <summary>
        /// Zkousi, zda agent agentID v case time muze v ramci alternativni cesty pouzit vrchol v, aniz by byl porusen bezpecny interval
        /// jineho agenta v tomto vrcholu.
        /// </summary>
        /// <returns>True, pokud pridani vrcholu v do alternativni cesty neni mozne z duvodu poruseni bezpecneho intervalu jineho z agentu.</returns>
        protected override bool IsInConstraints(Vertex v, HashSet<Constraint> constraints, int agentID, int time, Vertex parent)
        {
            if (!safeIntervals.ContainsKey(v))
            {
                return false;
            }
            var r = safeIntervals[v];
            return !r.CanReserved(agentID, time, secondaryMain, mainSecondary);
        }
    }

    /// <summary>
    /// Upraveny algoritmus A*, kterym hledame cesty pro modifikovany MAPF problem v ramci strikni alternativnu k-robustnosti.
    /// </summary>
    class AStarForModifiedMapf : AStarForContingencyPlan
    {
        //pole novych identifikatoru pro agenty - stejni agenti se mohou objevit vicekrat, ale my chceme zachovat unikatni id.
        int[] pseudoID;
        public AStarForModifiedMapf(Dictionary<Vertex, Reservation> safeIntervals, int[] pseudoID, int secondaryMain, int mainSecondary) :
            base(safeIntervals, secondaryMain, mainSecondary)
        {
            this.pseudoID = pseudoID;
        }

        /// <summary>
        /// Zkousi, zda agent agentID v case time muze v ramci alternativni cesty pouzit vrchol v, aniz by byl porusen bezpecny interval
        /// jineho agenta v tomto vrcholu. Zaroven nesmi byt umisteni v rozporu s podminkami v mnozine constraints.
        /// Jedna se vlastne o kombinaci omezeni na tride AStar a AstarForContingencyPlan
        /// </summary>
        /// <returns>True, pokud je vrchol v v rozporu s omezenimi, ci porusuje bezpecny interval.</returns>
        protected override bool IsInConstraints(Vertex v, HashSet<Constraint> constraints, int agentID, int time, Vertex parent)
        {
            if (!safeIntervals.ContainsKey(v))
            {
                return false;
            }
            var r = safeIntervals[v];
            if (!r.CanReserved(pseudoID[agentID], time, secondaryMain, mainSecondary))
            {
                return true;
            }
            if (constraints == null)
            {
                return false;
            }
            return constraints.Contains(new TestConstraint(agentID, v, time)) || constraints.Contains(new SwapConstraint(agentID, v, time, parent));
        }
    }

}
