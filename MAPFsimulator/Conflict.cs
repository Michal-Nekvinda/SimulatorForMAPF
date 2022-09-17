namespace MAPFsimulator
{
    /// <summary>
    /// Trida reprezentujici vrcholovy konflikt.
    /// Konflikt je vzdy mezi dvema agenty (agentID1 a agentID2) v urcitem case (time), vrcholu (vertex) s dobou trvani (duration).
    /// </summary>
    class Conflict
    {
        public int agentID1 { get; }
        public int agentID2 { get; }
        public int time { get; }
        public Vertex vertex { get; }
        public int duration { get; }

        /// <summary>
        /// Vytvoreni noveho konfliktu.
        /// Konflikt rika, ze agent agentID1 a agent agentID2 maji ve vrcholu vertex v case time konflikt s delkou trvani duration.
        /// </summary>
        /// <param name="agentID1"></param>
        /// <param name="agentID2"></param>
        /// <param name="vertex"></param>
        /// <param name="time"></param>
        /// <param name="duration">urcuje, jak dlouho by mel byt zamezen pobyt agenta v inkriminovanem vrcholu -> je 0 pro vrcholovy konflikt a k pro k-delay konflikt</param>
        public Conflict(int agentID1, int agentID2, Vertex vertex, int time, int duration = 0)
        {
            this.agentID1 = agentID1;
            this.agentID2 = agentID2;
            this.vertex = vertex;
            this.time = time;
            this.duration = duration;
        }
        /// <summary>
        /// Vraci pole s dvojici id agentu, kteri jsou v konfliktu.
        /// </summary>
        public int[] AgentInConflict()
        {
            return new int[] { agentID1, agentID2 };
        }
        public override string ToString()
        {
            string s = "Konflikt ve vrcholu " + vertex + " v čase " + time;
            return s;
        }

    }
    /// <summary>
    /// Trida reprezentujici konflikt vymeny vrcholu
    /// Tento konflikt je odvozen od vrcholoveho konfliktu (trida Conflict), navic pridavame informaci o predchudci vrcholu vertex.
    /// </summary>
    class SwapConflict : Conflict
    {
        public Vertex moveFrom { get; }
        public SwapConflict(int agentID1, int agentID2, Vertex vertex, int time, Vertex moveFrom) : base(agentID1, agentID2, vertex, time)
        {
            this.moveFrom = moveFrom;
        }
        public override string ToString()
        {
            string s = "Konflikt mezi vrcholy " + moveFrom + " a " + vertex + " v časech " + (time - 1).ToString() + "-" + time;
            return s;
        }

    }
}
