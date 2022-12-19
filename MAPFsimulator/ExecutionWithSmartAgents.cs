namespace MAPFsimulator
{
    class ExecutionWithSmartAgents : SimpleExecution
    {
        int[] lastVertexNumbers;
        public ExecutionWithSmartAgents(int agentsCount, double delay) : base(agentsCount, delay)
        {
            lastVertexNumbers = new int[agentsCount];
        }
        
        // public override List<double>[] ExecuteSolution(List<Plan> plans, List<IAgent> agents, out string message, out Conflict conf, out int length)
        // {
        //     //pocatecni nastaveni promennych
        //     colTime = -1;
        //     colType = "normal";
        //     bool finished = false;
        //     vertexConflict = false;
        //     swappingConflict = false;
        //     int time = 1;
        //     for (int i = 0; i < plans.Count; i++)
        //     {
        //         swapConfStruct[i].Enqueue(plans[i].GetNth(0));
        //     }
        //     //dokud nejsou vsichni agenti v cili
        //     while (!finished)
        //     {
        //         finished = true;
        //         currentVertices = new HashSet<Vertex>();
        //         //pro plan kazdeho z agentu zacnu pocitat od casu 1 (cas 0 = startovni vrchol)
        //         //positionsInTime[i][j] = poradove cislo vrcholu agenta i v case j (0 = start a tedy 1. vrchol cesty, 1 = 2.vrchol cesty,...)
        //         for (int i = 0; i < plans.Count; i++)
        //         {
        //             //pokud ma zpozdeni, zustava tam, kde byl predtim
        //             if (WillDelay(i, plans[i], time))
        //             {
        //                 positionsInTime[i].Add(positionsInTime[i][time - 1]);
        //             }
        //             //jinak se posouva do dalsiho vrcholu
        //             else
        //             {
        //                 positionsInTime[i].Add(positionsInTime[i][time - 1] + CurrentSpeed(agents[i], i, plans[i]));
        //             }
        //             //pokud nektery z agentu jeste neni v cili (ma pred sebou jeste nejake vrcholy), bude nasledovat dalsi iterace cyklu
        //             if (finished && plans[i].HasNextVertex(DoubleToInt.ToInt(positionsInTime[i][time])))
        //             {
        //                 finished = false;
        //             }
        //             //zda byl objeven hranovy konflikt
        //             if (!swappingConflict && !vertexConflict && IsVertexConflict(i, plans[i], time, plans))
        //             {
        //                 vertexConflict = true;
        //             }
        //         }
        //
        //         //kontrola swapping konfliktu
        //         if (!vertexConflict && !swappingConflict && IsSwappingConflict(time))
        //         {
        //             swappingConflict = true;
        //         }
        //         time++;
        //     }
        //
        //     length = time - 1;
        //     if (new Makespan().GetCost(plans) == 0)
        //     {
        //         length = 0;
        //     }
        //     //pri provadeni doslo ke kolizi
        //     if (vertexConflict || swappingConflict)
        //     {
        //         message = "ended with conflict";
        //         colTime = confV.time;
        //         TypOfConf(plans, confV.agentID1, confV.agentID2, DoubleToInt.ToInt(positionsInTime[confV.agentID1][confV.time]), DoubleToInt.ToInt(positionsInTime[confV.agentID2][confV.time]));
        //
        //     }
        //     else
        //         message = "ended successfully";
        //
        //     conf = confV;
        //     return positionsInTime;
        // }
        
        //Pro smart agenty neni potreba resit wait akce nebo konec planu, protoze si to agenti resi sami
        protected override bool WillDelay(int i, Plan p, int t)
        {
            double dd = DoubleGenerator.GetInstance().NextDouble();
            return dd < delay;
        }
        
        protected override double CurrentSpeed(IAgent agent, int i, Plan p, int time)
        {
            var newVertexNumber = agent.NextVertexToMove(time, lastVertexNumbers[i], p);
            int tmp = lastVertexNumbers[i];
            lastVertexNumbers[i] = newVertexNumber;
            return newVertexNumber - tmp;
        }
    }
}