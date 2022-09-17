using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida s implementaci algoritmu, ktery zajistuje robustnost pomoci pridani wait akci.
    /// </summary>
    class WaitAddingRobustness
    {
        List<Plan> plans;
        /// <summary>
        /// Nahrani planu k oprave.
        /// </summary>
        public WaitAddingRobustness(List<Plan> plans)
        {
            this.plans = plans;
        }

        /// <summary>
        /// Vrati novy robustni plan, do ktereho jsou pridany wait akce tak, aby se co nejvice poruseni bezpecnych intervalu delky nejvyse robustness.
        /// </summary>
        public List<Plan> GetRobustPlan(int robustness)
        {
            if (plans == null)
            {
                return null;
            }
            //priprava datovych struktur
            PlanProcessing pp = new PlanProcessing(plans);
            var safeIntervals = pp.GetSafeIntervals(robustness);
            var conflictSet = pp.GetConflictSet(robustness);
            var violations = new List<Tuple<int, int, int, int>>();
            foreach (var item in conflictSet)
            {
                violations.AddRange(safeIntervals[item.Key].GetSafeIntervalViolations());
            }

            int makespan = new Makespan().GetCost(plans);
            //[i,j] = nejdrivejsi cas, kdy agent i muze prijet do j-teho vrcholu (minimalni prujezd vrcholem)
            int[,] min = new int[plans.Count, makespan + 1];
            //[i,j] = nejzazsi cas, kdy agent i muze prijet do j-teho vrcholu (maximalni prujezd vrcholem)
            int[,] max = new int[plans.Count, makespan + 1];
            //[i,j] = maximalni pocet wait akci, ktere muze navic udelat agent i ve vrcholu s poradovym cislem j (maximalni cekaci doba)
            int[,] maxWait = new int[plans.Count, makespan + 1];
            //[i,j] = kolik casu agent i ceka v j-tem vrcholu
            int[,] waitInCurrentPlan = new int[plans.Count, makespan + 1];

            //zapis hodnot intervalu
            for (int i = 0; i < plans.Count; i++)
            {
                max[i, plans[i].GetLenght() - 1] = makespan;
                //kolik muze kazdy agent cekat v cili 
                maxWait[i, plans[i].GetLenght() - 1] = makespan - plans[i].GetCost();
                for (int j = 0; j < plans[i].GetLenght(); j++)
                {
                    if (j > 0)
                    {
                        if (plans[i].GetNth(j)==plans[i].GetNth(j-1))
                        {
                            min[i, j] = min[i, j - 1];
                        }
                        else
                        {
                            min[i, j] = min[i, j - 1] + 1;
                        }

                        int newJ = plans[i].GetLenght() - 1 - j;
                        if (plans[i].GetNth(newJ) == plans[i].GetNth(newJ + 1))
                        {
                            max[i, newJ] = max[i, newJ + 1];
                        }
                        else
                        {
                            max[i, newJ] = max[i, newJ + 1] - 1;
                        }
                    }
                }
            }

            for (int i = 0; i < plans.Count; i++)
            {
                for (int j = 0; j < plans[i].GetLenght(); j++)
                {
                    //doba o kterou je mozne se zpozdit ve vrcholu - bezpecny interval
                    int safeInt = safeIntervals[plans[i].GetNth(j)].GetSafeInt(i, j);
                    //nova hodnota je min z predchozi hodnoty a minima z toho, co si muze dovolit vuci ostatnim a vuci dodrzeni makespanu
                    maxWait[i, j] = Math.Min(safeInt - robustness, max[i, j] - j);
                    //pokud je vrchol j stejny jako predchozi, poznamenam esi, ze v planu je wait akce
                    if (j > 0 && plans[i].GetNth(j) == plans[i].GetNth(j - 1))
                    {
                        waitInCurrentPlan[i, j - 1] = 1;
                    }
                }

                //probublani spravnych hodnot
                for (int j = plans[i].GetLenght() - 2; j >= 0; j--)
                {
                    if (waitInCurrentPlan[i, j] > 0)
                    {
                        waitInCurrentPlan[i, j] += waitInCurrentPlan[i, j + 1];
                    }
                }
            }
            //uprava wait intervalu
            for (int i = 0; i < plans.Count; i++)
            {
                for (int j = plans[i].GetLenght()-2; j >= 0; j--)
                {
                    //lze cekat maximalne tolik, kolik sam muzu, nebo kolik muze muj nasledovnik, protoze to zpozdeni se prenese i na nej
                    maxWait[i, j] = Math.Min(maxWait[i, j], maxWait[i, j + 1]);
                    //to neplati pokud sam absorbuji zpozdeni - tedy ve vrcholu se ceka po nejakou dobu, takze pozdejsi vrcholy se o zpozdeni nedozvi
                    maxWait[i, j] = Math.Max(maxWait[i, j], waitInCurrentPlan[i, j]);
                }
            }

            int[,] repairs = new int[plans.Count, makespan + 1];
            violations.Sort((x, y) => y.Item3.CompareTo(x.Item3));
            violations.Reverse();
            //projdeme jednotliva poruseni intervalu a kazde se pokusime opravit
            foreach (var item in violations)
            {
                int agentForMove = item.Item4;
                int timeThisAgent = item.Item3 + item.Item2 + 1;
                if (maxWait[agentForMove, timeThisAgent-1] > 0)
                {
                    //wait akce lze pridat
                    int howMany = Math.Min(maxWait[agentForMove, timeThisAgent-1], robustness);
                    howMany = Math.Min(howMany, maxWait[agentForMove, timeThisAgent + 1]);
                    //navyseni si lze poznamenat pouze u nasledujiciho vrcholu a cyklus provest az po zpracovani vsech prvku (z hlediska slozitosti)
                    for (int i = timeThisAgent-1; i < plans[agentForMove].GetLenght(); i++)
                    {
                        if (repairs[agentForMove, i] < howMany)
                        {
                            repairs[agentForMove, i] = howMany;

                        }
                    }
                }
            }
            //nove plany na zaklade opravy
            List<Plan> newPlans = new List<Plan>(plans.Count);
            for (int i = 0; i < plans.Count; i++)
            {
                Plan p = new Plan();
                int addExtra = 0;
                for (int j = 0; j < plans[i].GetLenght(); j++)
                {
                    int rep = j > 0 ? repairs[i, j - 1] : 0;
                    if (repairs[i, j] - rep > 0)
                    {
                        Vertex v = plans[i].GetNth(j);
                        for (int k = 0; k < repairs[i, j] - rep; k++)
                        {
                            p.AddVertex(new Vertex(v.x,v.y));
                            addExtra++;
                        }
                    }
                    //pokud nasledujeme dva stejne vrcholy za sebou a byly jiz drive vycpany nejakymi wait akcemi,
                    //tak pridame tento vrchol jen jednou a usetrime jednu jednotku casu 
                    if (j < plans[i].GetLenght()-1 && plans[i].GetNth(j) == plans[i].GetNth(j + 1) && addExtra > 0)
                    {
                        addExtra--;
                    }
                    else
                    {
                        p.AddVertex(plans[i].GetNth(j));
                    }
                }
                newPlans.Add(p);
            }
            return newPlans;
        }
    }
}
