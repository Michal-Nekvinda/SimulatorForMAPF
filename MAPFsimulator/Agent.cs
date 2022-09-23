namespace MAPFsimulator
{
    public interface IAgent
    {
        Vertex start { get; }
        Vertex target { get; }
        int id { get; set; }
        int NextVertexToMove(int vertexNumber, Plan plan);
    }
    
    /// <summary>
    /// Trida popisujici agenta. Kazdy agent ma svuj pocatecni a cilovy vrchol a svuj identifikator.
    /// </summary>
    class Agent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }
        public int NextVertexToMove(int vertexNumber, Plan plan)
        {
            plan.GetNext(vertexNumber, out var nextVertexNumber);
            return nextVertexNumber;
        }

        public Agent(Vertex start, Vertex target, int id)
        {
            this.id = id;
            this.start = start;
            this.target = target;
        }
        public Agent(int sx, int sy, int tx, int ty, int id)
        {           
            this.id = id;			
            this.start = new Vertex(sx, sy);
            this.target = new Vertex(tx, ty);
        }
        public override string ToString()
        {
            return "Agent " + id + ": " + start + " --> " + target;
        }
    }

    class SmartAgent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }
        public int NextVertexToMove(int vertexNumber, Plan plan)
        {
            var possibleMoves = plan.GetAvailableVerticesFromPosition(vertexNumber);
            return possibleMoves[0];
        }
    }
}
