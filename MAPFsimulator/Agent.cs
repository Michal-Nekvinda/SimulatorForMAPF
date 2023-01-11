namespace MAPFsimulator
{
    /// <summary>
    /// Rozhrani popisujici agenta v MAPF instanci
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Startovni vrchol agenta
        /// </summary>
        Vertex start { get; }

        /// <summary>
        /// Cilovy vrchol agenta
        /// </summary>
        Vertex target { get; }

        /// <summary>
        /// Unikatni identifikator
        /// </summary>
        int id { get; set; }

        /// <summary>
        /// Vrati poradove cislo vrcholu z planu, do ktereho se chce agent premistit z vrcholu vertexNumber
        /// </summary>
        /// <param name="time">cas od zacatku exekuce</param>
        /// <param name="vertexNumber">cislo vrcholu, ve kterem se agent aktualne nachazi</param>
        /// <param name="plan">plan agenta</param>
        /// <param name="delay">aktualni zpozdeni</param>
        /// <returns></returns>
        int NextVertex(int time, int vertexNumber, Plan plan, int delay = 0);

        /// <summary>
        /// Updatuje aktualni pozici a sestavi svoji predpokladanou cestu do cile
        /// </summary>
        /// <param name="vertexNumber">aktualni pozice</param>
        /// <param name="time">cas od zacatku exekuce</param>
        /// <param name="plan">plan agenta</param>
        void UpdatePosition(int vertexNumber, int time, Plan plan);
    }

    /// <summary>
    /// Trida popisujici agenta. Kazdy agent ma svuj pocatecni a cilovy vrchol a svuj identifikator.
    /// </summary>
    class Agent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }

        public Agent(Vertex start, Vertex target, int id)
        {
            this.id = id;
            this.start = start;
            this.target = target;
        }

        public override string ToString()
        {
            return "Agent " + id + ": " + start + " --> " + target;
        }

        public int NextVertex(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            plan.GetNext(vertexNumber, out var nextVertexNumber, delay);
            return nextVertexNumber;
        }

        public void UpdatePosition(int vertexNumber, int time, Plan plan)
        {
        }
    }
}