namespace MAPFsimulator
{
    /// <summary>
    /// Struktura uchovavajici udaj o delce bezpecneho intervalu agenta v danem case a vrcholu.
    /// </summary>
    struct SafeSpace
    {
        public int agentID { get; }
        public Vertex vertex { get; }
        public int time { get; }
        public int length { get; }

        /// <summary>
        /// Vytvori udaj o bezpecnem miste agenta agentID ve vrcholu vertex v intervalu [time, time+length].
        /// </summary>
        public SafeSpace(int agentID, Vertex vertex, int time, int length)
        {
            this.agentID = agentID;
            this.vertex = vertex;
            this.time = time;
            this.length = length;
        }
    }
}
