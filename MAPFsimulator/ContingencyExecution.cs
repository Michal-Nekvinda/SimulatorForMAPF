namespace MAPFsimulator
{
    /// <summary>
    /// Exekuce planu, s upravami pro stromove plany.
    /// Oproti klasicke exekuci musime hlidat vypocet abstraktnich pozic nasledniku - v normalnim pripade se jedna vzdy o posun o 1,
    /// zde to muze byt vic (pri presunu z hlavni na alternativni cestu)
    /// </summary>
    class ContingencyExecution : SimpleExecution
    {
        int[] lastVertexNumbers;
        int[] currentDelays;
        public ContingencyExecution(int agentsCount, double delay) : base(agentsCount, delay)
        {
            lastVertexNumbers = new int[agentsCount];
            currentDelays = new int[agentsCount];
        }
        /// <summary>
        /// Na zaklade aktualniho zpozdeni agenta i v planu p vraci poradove cislo vrcholu, do ktereho agent pojede.
        /// Tedy za normalnich okolnosti se pohybuje po hlavnim planu (vzdy o 1), v pripade, ze prejizdi na alternativni plan,
        /// tak se pomoci teto metody vypocita rozdil poradovych cisel, o ktere se musi agent posunout, aby dale pokracoval v alternativnim planu.
        /// </summary>
        protected override double CurrentSpeed(IAgent agent, int i, Plan p, int time)
        {
            var newVertex = agent.NextVertexToMove(time, lastVertexNumbers[i], p, currentDelays[i]);
            var tmp = lastVertexNumbers[i];
            lastVertexNumbers[i] = newVertex;
            return newVertex - tmp;
        }
        /// <summary>
        /// Vraci true, pokud se i-ty agent v planu p v case t opozdi.
        /// </summary>
        protected override bool WillDelay(IAgent agent, int i, Plan p, int t)
        {
            //pokud agent mel provest akci wait, nebo uz je na konci planu, tak vratim false, protoze se nezpozdi
            if (p.GetNth(positionsInTime[i][t - 1]) == p.GetNth(agent.NextVertexToMove(t,
                    DoubleToInt.ToInt(positionsInTime[i][t - 1]), p, currentDelays[i])))
            {
                return false;
            }
            double dd = DoubleGenerator.GetInstance().NextDouble();
            if (dd < delay)
            {
                currentDelays[i]++;
                return true;
            }
            return false;
        }
    }
}