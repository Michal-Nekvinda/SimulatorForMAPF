using System.Collections.Generic;

namespace MAPFsimulator
{
    public class ExecutionWithSmartAgents : IPlansExecutor
    {
        private string _colType = "";

        protected double delay;
        //datova struktura s poradovymi cisly vrcholu, ve kterych se agenti nachazeji v danem case
        protected List<double>[] positionsInTime;
        //fronta pro detekci konfliktu vymeny vrcholu
        Queue<Vertex>[] swapConfStruct;
        protected int agentsCount;
        HashSet<Vertex> currentVertices;
        protected bool vertexConflict;
        protected bool swappingConflict;
        protected int maxTime;
        protected Conflict confV;

        public ExecutionWithSmartAgents(int agentsCount, double delay)
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
        
        public List<double>[] ExecuteSolution(List<Plan> plans, List<IAgent> agents, out string message, out Conflict conf, out int length)
        {
            //pocatecni nastaveni promennych
            _colType = "normal";
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
                        //TODO tady se musi rozhodovat agent
                        positionsInTime[i].Add(positionsInTime[i][time - 1]);
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
                TypOfConf(plans, confV.agentID1, confV.agentID2, DoubleToInt.ToInt(positionsInTime[confV.agentID1][confV.time]), DoubleToInt.ToInt(positionsInTime[confV.agentID2][confV.time]));

            }
            else
                message = "ended successfully";

            conf = confV;
            return positionsInTime;
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
        /// Vraci true, pokud se agentID v planu p v case t opozdi.
        /// </summary>
        protected virtual bool WillDelay(int agentID, Plan p, int t)
        {
            bool inVertex = DoubleToInt.DecimalPart(positionsInTime[agentID][t - 1] / maxTime) == 0;
            //pokud agent mel provest akci wait, nebo uz je na konci planu, tak vratime false, protoze se nezpozdi
            if (inVertex && (p.GetNth(positionsInTime[agentID][t - 1]) ==
                p.GetNth(positionsInTime[agentID][t - 1] + 1)))
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
        
        protected bool IsVertexConflict(int agentID, Plan p, int t,List<Plan> plans)
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
        /// Rozlisuje, jaky typ konfliktu mezi agenty nastal (pro contigency robustnost).
        /// </summary>
        private void TypOfConf(List<Plan> plans, int ag1, int ag2, int vertex1, int vertex2)
        {
            if (plans[ag1].IsInMainPlan(vertex1))
            {
                if (plans[ag2].IsInMainPlan(vertex2))
                {
                    _colType = "Hlavni";
                }
                else
                {
                    _colType = "Hlavni+Vedlejsi";
                }
            }
            else
            {
                if (plans[ag2].IsInMainPlan(vertex2))
                {
                    _colType = "Hlavni+Vedlejsi";
                }
                else
                {
                    _colType = "Vedlejsi";
                }
            }
        }
    }
}