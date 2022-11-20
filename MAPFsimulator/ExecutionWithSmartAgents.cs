using System.Collections.Generic;

namespace MAPFsimulator
{
    public class ExecutionWithSmartAgents : IPlansExecutor
    {
        protected double delay;
        //datova struktura s poradovymi cisly vrcholu, ve kterych se agenti nachazeji v danem case
        List<double>[] positionsInTime;
        //fronta pro detekci konfliktu vymeny vrcholu
        Queue<Vertex>[] swapConfStruct;
        int agentsCount;
        HashSet<Vertex> currentVertices;
        bool vertexConflict;
        bool swappingConflict;
        Conflict confV;

        public ExecutionWithSmartAgents(int agentsCount, double delay)
        {
            this.agentsCount = agentsCount;
            positionsInTime = new List<double>[agentsCount];
            swapConfStruct = new Queue<Vertex>[agentsCount];
            this.delay = delay;
            for (var i = 0; i < agentsCount; i++)
            {
                positionsInTime[i] = new List<double>();
                positionsInTime[i].Add(0);
                swapConfStruct[i] = new Queue<Vertex>();
                //vypln pomoci fiktivniho vrcholu
                swapConfStruct[i].Enqueue(new Vertex(-1, -1));
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plans"></param>
        /// <param name="agents"></param>
        /// <param name="message"></param>
        /// <param name="conf"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public List<double>[] ExecuteSolution(List<Plan> plans, List<IAgent> agents, out string message, out Conflict conf, out int length)
        {
            //pocatecni nastaveni promennych
            var finished = false;
            vertexConflict = false;
            swappingConflict = false;
            var time = 1;
            
            for (var i = 0; i < plans.Count; i++)
            {
                swapConfStruct[i].Enqueue(plans[i].GetNth(0));
            }
            
            //TODO opravit a zprovoznit pro smart agenty
            //dokud nejsou vsichni agenti v cili
            while (!finished)
            {
                finished = true;
                currentVertices = new HashSet<Vertex>();
                //pro plan kazdeho z agentu zacnu pocitat od casu 1 (cas 0 = startovni vrchol)
                //positionsInTime[i][j] = poradove cislo vrcholu agenta i v case j (0 = start a tedy 1. vrchol cesty, 1 = 2.vrchol cesty,...)
                for (var i = 0; i < plans.Count; i++)
                {
                    //pokud ma zpozdeni, zustava tam, kde byl predtim
                    positionsInTime[i].Add(WillDelay()
                        ? positionsInTime[i][time - 1]
                        : agents[i].NextVertexToMove((int)positionsInTime[i][time - 1], plans[i]));
                    
                    //pokud nektery z agentu jeste neni v cili (ma pred sebou jeste nejake vrcholy), bude nasledovat dalsi iterace cyklu
                    if (finished && plans[i].HasNextVertex((int)positionsInTime[i][time]))
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
            }
            else
            {
                message = "ended successfully";
            }

            conf = confV;
            return positionsInTime;
        }
        
        /// <summary>
        /// Vraci true, pokud byl v planech agentu v case time detekovan konflikt vymeny vrcholu.
        /// </summary>
        protected virtual bool IsSwappingConflict(int time)
        {
            for (var i = 0; i < agentsCount; i++)
            {
                for (var j = i + 1; j < agentsCount; j++)
                {
                    var v1 = swapConfStruct[i].Peek();
                    var v2 = swapConfStruct[j].Peek();
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
        private bool WillDelay()
        {
            return DoubleGenerator.GetInstance().NextDouble() < delay;
        }

        private bool IsVertexConflict(int agentId, Plan p, int t, List<Plan> plans)
        {
            var v = p.GetNth(positionsInTime[agentId][t]);
            swapConfStruct[agentId].Dequeue();
            swapConfStruct[agentId].Enqueue(v);
            //vrchol uz v hash setu je  - tedy mame konflikt - zbyva detekovat druheho z agentu
            if (currentVertices.Add(v))
            {
                return false;
            }
            var theSecond = 0;
            for (var i = 0; i < agentId; i++)
            {
                if (v == plans[i].GetNth(positionsInTime[i][t]))
                {
                    theSecond = i;
                    break;
                }
            }
            //konflikt ulozime a vratime po provedeni planu
            confV = new Conflict(agentId, theSecond, v, t);
            return true;
        }
    }
}