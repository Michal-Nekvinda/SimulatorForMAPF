namespace MAPFsimulator
{
    public interface IAgent
    {
        Vertex start { get; }
        Vertex target { get; }
        int id { get; set; }
        int NextVertexToMove(int time, int vertexNumber, Plan plan, int delay = 0);
        void SetCurrentPosition(int vertexNumber, int time, Plan plan);
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

        public virtual int NextVertexToMove(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            plan.GetNext(vertexNumber, out var nextVertexNumber, delay);
            return nextVertexNumber;
        }

        public void SetCurrentPosition(int vertexNumber, int time, Plan plan) { }
    }
}
