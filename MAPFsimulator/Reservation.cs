using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida spravujici casove intervaly jednotlivych agentu - pouziti pro jednotlive vrcholy grafu.
    /// </summary>
    class Reservation
    {
        List<int>[] times;
        bool[] hasRecord;
        int[] stayAtTarget;
        int gap;

        /// <summary>
        /// Rezervace pro celkem maxAgents agentu, kazdy z nich bude na 1 zaznam blokovat interval delky gap.
        /// </summary>
        public Reservation(int maxAgents, int gap)
        {
            times = new List<int>[maxAgents];
            hasRecord = new bool[maxAgents];
            stayAtTarget = new int[maxAgents];
            this.gap = gap;
            for (int i = 0; i < maxAgents; i++)
            {
                times[i] = new List<int>();
                stayAtTarget[i] = int.MaxValue;
            }
        }
        
        /// <summary>
        /// Vraci list poruseni bezpecnych intervalu. 
        /// Violation je ctverice - id agenta, cas, delka jeho bezpecneho intervalu (ktery je mensi, nez by mel byt) a id druheho agenta, ktery prijizdi bezprostredne po id.
        /// </summary>
        public List<Tuple<int, int, int, int>> GetSafeIntervalViolations()
        {
            var violations = new List<Tuple<int, int, int, int>>();
            for (int i = 0; i < hasRecord.Length; i++)
            {
                for (int j = i + 1; j < hasRecord.Length; j++)
                {
                    if (hasRecord[i] && hasRecord[j])
                    {
                        var v = GetViolations(i, j);
                        if (v.Count > 0)
                        {
                            violations.AddRange(v);
                        }
                    }
                }
            }
            return violations;
        }
        /// <summary>
        /// Zaeviduje zaznam pro agenta agentID v case time. Pri nastaveni permanent = true se jedna o rezervaci ciloveho vrcholu,
        /// tedy agent zde zustane naporad.
        /// </summary>
        public void AddReservation(int time, int agentID, bool permanent)
        {
            times[agentID].Add(time);
            hasRecord[agentID] = true;
            if (permanent)
            {
                stayAtTarget[agentID] = time;
            }
        }
        /// <summary>
        /// Vraci true, pokud je mozne rezervovat vrchol pro agenta agentID v case time na dobu space.
        /// </summary>
        public bool CanReserved(int agentID, int time, int secondaryMain, int mainSecondary)
        {
            for (int i = 0; i < times.Length; i++)
            {
                if (hasRecord[i] && i != agentID && IsOccupied(times[i], time, secondaryMain, mainSecondary, i))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Vraci odpoved na otazku, zda agent id s rozvrhem agentTimeTable dovoluje rezervaci v case time na dobu space.
        /// </summary>
        private bool IsOccupied(List<int> agentTimeTable, int time, int secondaryMain, int mainSecondary, int id)
        {
            foreach (var i in agentTimeTable)
            {
                if (time>=stayAtTarget[id])
                {
                    return true;   
                }
            
                if (i <= time + secondaryMain && time <= i + mainSecondary)
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Vraci delku bezpecneho intervalu agenta agentID v case - tedy kolik ma casu, nez do vrcholu prijede nekdo dalsi.
        /// </summary>
        public int GetSafeInt(int agentID, int time)
        {
            int minInterval = int.MaxValue;
            for (int i = 0; i < times.Length; i++)
            {
                if (hasRecord[i] && i != agentID)
                {
                    foreach (var t in times[i])
                    {
                        if (t > time)
                        {
                            if (minInterval > t - time -1)
                            {
                                //protoze ty casy jdou postupne, takze dalsi v listu times[i] uz budou pozdejsi casy
                                minInterval = t - time - 1;
                                break;
                            }
                        }
                    }
                }
            }
            return minInterval;
        }
        private List<Tuple<int, int, int, int>> GetViolations(int i, int j)
        {
            var c = new List<Tuple<int, int, int, int>>();
            foreach (var time1 in times[i])
            {
                foreach (var time2 in times[j])
                {
                    if (time1 < time2)
                    {
                        if (time1 + gap >= time2)
                        {
                            c.Add(new Tuple<int, int, int, int>(i, time1, time2 - time1 - 1, j));
                        }
                    }
                    //time2 < time1
                    else
                    {
                        if (time2 + gap >= time1)
                        {
                            c.Add(new Tuple<int, int, int, int>(j, time2, time1 - time2 - 1, i));
                        }
                    }
                    //time1 == time2 nenastane. protoze puvodni plan je validni
                }
            }
            return c;
        }
    }
}
