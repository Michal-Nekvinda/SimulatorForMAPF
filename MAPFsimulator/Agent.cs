namespace MAPFsimulator
{
    interface IAgent
    {
        Vertex start { get; }
        Vertex target { get; }
        int id { get; set; }
        Vertex Move(Vertex currentPosition, Plan plan);
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
        
        public Vertex Move(Vertex currentPosition, Plan plan)
        {
            throw new System.NotImplementedException();
        }
    }
}
