namespace MAPFsimulator
{
    /// <summary>
    /// Trida reprezentujici podminku, kterou si vynucuji vrcholove (a k-delay) konflikty.
    /// </summary>
    class Constraint
    {
        public int agentID { get; }
        public Vertex vertex { get; }
        public int time { get; }
        public int duration { get; }

        /// <summary>
        /// Vytvori nove omezeni.
        /// Agent agentID nesmi byt pritomen ve vrcholu vertex v casovem intervalu [time, time+duration].
        /// </summary>
        public Constraint(int agentID, Vertex vertex, int time, int duration = 0)
        {
            this.agentID = agentID;
            this.vertex = vertex;
            this.time = time;
            this.duration = duration;
        }

        /// <summary>
        /// Dve omezeni se rovnaji, pokud se tykaji stejnych agentu, ve stejnem vrcholu, ve stejny cas a trvaji stejnou dobu.
        /// </summary>
        public static bool operator ==(Constraint a, Constraint b)
        {
            if ((object)b == null)
            {
                return (object)a == null;
            }
            return a.agentID == b.agentID && a.vertex == b.vertex && a.time == b.time && a.duration == b.duration;

        }
        public static bool operator !=(Constraint a, Constraint b) { return !(a == b); }
        public override bool Equals(object obj)
        {
            //TestConstraint je specialni pripad omezeni pouzivany v prohledavacim algoritmu A*.
            if (obj is TestConstraint)
            {
                TestConstraint sc = (TestConstraint)obj;
                return agentID == sc.agentID && vertex == sc.vertex && time <= sc.time && sc.time <= time + duration;

            }
            return this == (Constraint)obj;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + agentID + 1;
            hash = hash * 23 + vertex.GetHashCode();
            return hash;
        }
    }
    /// <summary>
    /// Trida reprezentujici podminku, kterou si vynucuji konflikty vymeny vrcholu.
    /// </summary>
    class SwapConstraint : Constraint
    {
        public Vertex moveFrom { get; }
        /// <summary>
        /// Vytvori nove omezeni.
        /// Agent agentID nesmi prejizdet v case time z vrcholu moveFrom do vrcholu vertex.
        /// </summary>
        public SwapConstraint(int agentID, Vertex vertex, int time, Vertex moveFrom) : base(agentID, vertex, time)
        {
            this.moveFrom = moveFrom;
        }

        /// <summary>
        /// Dve omezeni se rovnaji, pokud se tykaji stejnych agentu, ve stejnych vrcholech vrcholu a ve stejny cas.
        /// </summary>
        public static bool operator ==(SwapConstraint a, SwapConstraint b)
        {
            if ((object)b == null)
            {
                return (object)a == null;
            }
            return a.agentID == b.agentID && a.vertex == b.vertex && a.time == b.time && a.moveFrom == b.moveFrom;
        }
        public static bool operator !=(SwapConstraint a, SwapConstraint b) { return !(a == b); }
        public override bool Equals(object obj)
        {
            return this == (SwapConstraint)obj;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + agentID + 1;
            hash = hash * 23 + vertex.GetHashCode();
            hash = hash * 23 + moveFrom.GetHashCode();
            hash = hash * 23 + time;
            return hash;
        }

    }
    /// <summary>
    /// Omezeni pouzivane pouze v ramci algoritmu A* - umoznuje rychle eliminovat kandidaty na nasledniky vrcholu.
    /// </summary>
    class TestConstraint : Constraint
    {
        public TestConstraint(int agentID, Vertex vertex, int time) : base(agentID, vertex, time) { }
    }
}
