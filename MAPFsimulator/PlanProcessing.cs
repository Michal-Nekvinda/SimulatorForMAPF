using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida pro pocitani datovych struktur, ktera nas zajimaji pro tvorbe robustnich planu zalozanych na planovani s alternativami.
    /// </summary>
    class PlanProcessing
    {
        List<Plan> plans;
        //casy navstev agentu v jednotlivych vrcholech
        Dictionary<Vertex, Reservation> safeIntervals;
        //konfliktni mnozina
        Dictionary<Vertex, int> conflictSet;
        int lastComputedInterval;

        /// <summary>
        /// Nahrani planu, pro ktery budeme pocitat data.
        /// </summary>
        public PlanProcessing(List<Plan> plans)
        {
            this.plans = plans;
            lastComputedInterval = -1;
        }

        /// <summary>
        /// Vrati konfliktni mnozinu pro ulozeny plan. Delka bezpecneho intervalu je cislo intervalLength.
        /// </summary>
        public Dictionary<Vertex, int> GetConflictSet(int intervalLength)
        {
            if (intervalLength != lastComputedInterval)
            {
                ComputeSafeIntervals(intervalLength);
                lastComputedInterval = intervalLength;
            }
            return conflictSet;
        }

        /// <summary>
        /// Vrati mnozinu vrcholu s bezpecnymi intervaly pro nahrany plan.
        /// </summary>
        public Dictionary<Vertex, Reservation> GetSafeIntervals(int intervalLength)
        {
            if (intervalLength != lastComputedInterval)
            {
                ComputeSafeIntervals(intervalLength);
                lastComputedInterval = intervalLength;
            }
            return safeIntervals;
        }

        /// <summary>
        /// Vraci mnozinu B - tedy mnozinu poruseni bezpecnych intervalu pro ulozeny plan.
        /// </summary>
        public List<SafeSpace> GetBset(int intervalLength)
        {
            List<SafeSpace> Bset = new List<SafeSpace>();
            //pokud nejsou spocitane, aktualizuji pomocne datove struktury
            if (intervalLength != lastComputedInterval)
            {
                ComputeSafeIntervals(intervalLength);
                lastComputedInterval = intervalLength;
            }
            foreach (var item in conflictSet)
            {
                var v = safeIntervals[item.Key].GetSafeIntervalViolations();
                if (v.Count > 0)
                {
                    foreach (var violation in v)
                    {
                        Bset.Add(new SafeSpace(violation.Item1, item.Key, violation.Item2, violation.Item3));
                    }
                }
            }
            return Bset;
        } 

        /// <summary>
        /// Pocita mnozinu bezpecnych intervalu pro delku intervalLength.
        /// </summary>
        private void ComputeSafeIntervals(int intervalLength)
        {
            safeIntervals = new Dictionary<Vertex, Reservation>();
            conflictSet = new Dictionary<Vertex, int>();
            for (int i = 0; i < plans.Count; i++)
            {
                HashSet<Vertex> hs = new HashSet<Vertex>();
                for (int j = 0; j < plans[i].GetLenght(); j++)
                {
                    var v = plans[i].GetNth(j);
                    if (!safeIntervals.ContainsKey(v))
                    {
                        safeIntervals.Add(v, new Reservation(plans.Count, intervalLength));
                    }
                    safeIntervals[v].AddReservation(j, i, j == plans[i].GetLenght() - 1);

                    if (!conflictSet.ContainsKey(v))
                    {
                        conflictSet.Add(v, 0);
                    }
                    //hlida, aby se opakujici se vrchol jednoho agenta objevil v dictionary jen jednou
                    if (hs.Add(v))
                    {
                        conflictSet[v]++;
                    }
                }
            }
            //v mnozine zustanou jen ty vrcholy, kde se objevuji alespon 2 ruzni agenti
            conflictSet = conflictSet.Where(x => x.Value > 1).ToDictionary(key => key.Key, val => val.Value);
        }
    }
}
