using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    interface CostFunction
    {
        int GetCost(List<Plan> sol);
    }
    /// <summary>
    /// Ucelova funkce makespan - maximum z delek planu jednotlivych agentu.
    /// </summary>
    class Makespan : CostFunction
    {
        public int GetCost(List<Plan> sol)
        {
            if (sol == null)
            {
                return -1;
            }
            return sol.Max(p => p.GetCost());
        }
    }
    /// <summary>
    /// Ucelova funkce SoC - soucet delek planu jednotlivych agentu.
    /// </summary>
    class SumOfCosts : CostFunction
    {
        public int GetCost(List<Plan> sol)
        {
            if (sol == null)
            {
                return -1;
            }
            return sol.Sum(p => p.GetCost());
        }
    }
    /// <summary>
    /// Makespan pro modifikovany MAPF problem - cas dokonceni nejpozdejsiho planu.
    /// </summary>
    class ModifiedMakespan : CostFunction
    {
        public static List<int> startTimes;
        public int GetCost(List<Plan> sol)
        {
            int makespan = -1;
            if (sol==null)
            {
                return -1;
            }
            for (int i = 0; i < sol.Count; i++)
            {
                if (sol[i].GetCost() + startTimes[i] > makespan)
                {
                    makespan = sol[i].GetCost() + startTimes[i];
                }
            }
            return makespan;
        }
    }
}
