using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
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
        public Min_MaxRobustExecution(int agentsCount, int minTime, int maxTime, double delay) : base(agentsCount,
            delay)
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
        protected override double CurrentSpeed(IAgent agent, int i, Plan p, int time)
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
        protected override bool IsVertexConflict(int agentID, Plan p, int t, List<Plan> plans)
        {
            if (last.Count == agentsCount)
            {
                last.Clear();
                next.Clear();
            }

            double v = positionsInTime[agentID][t];
            last.Add(p.GetNth(v));
            next.Add(p.GetNth(v + 1));
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
                for (int j = i + 1; j < agentsCount; j++)
                {
                    //pokud 2 agenti vyjeli ze stejneho vrcholu
                    if (last[i] == last[j])
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
                            confV = new SwapConflict(i, j, next[i], time, last[i]);
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