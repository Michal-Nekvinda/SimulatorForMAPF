using System.Collections.Generic;

namespace MAPFsimulator
{
    public interface IAgent
    {
        Vertex start { get; }
        Vertex target { get; }
        int id { get; set; }
        int NextVertexToMove(int vertexNumber, Plan plan, int delay = 0);
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

        public virtual int NextVertexToMove(int vertexNumber, Plan plan, int delay = 0)
        {
            plan.GetNext(vertexNumber, out var nextVertexNumber, delay);
            return nextVertexNumber;
        }

        public void SetCurrentPosition(int vertexNumber, int time, Plan plan) { }
    }

    class SmartAgent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }

        private Communicator _communicator;
        public SmartAgent(Vertex start, Vertex target, int id)
        {
            this.id = id;
            this.start = start;
            this.target = target;
            _communicator = new Communicator(id);
        }
        
        public override string ToString()
        {
            return "SmartAgent " + id + ": " + start + " --> " + target;
        }
        
        public virtual int NextVertexToMove(int vertexNumber, Plan plan, int delay = 0)
        {
            var possibleMoves = plan.GetAvailableVerticesFromPosition(vertexNumber);
            if (possibleMoves.Count == 1)
            {
                //v tomto kroku neni mozne vybrat jiny vrchol
                return possibleMoves[0];
            }

            var allAgentsPositions = _communicator.GetAllAgentsPredictedPositions();
            var bestOption = -1;
            var minRisk = int.MaxValue;
            foreach (var option in possibleMoves)
            {
                var risk = ComputeCollisionRisk(option, plan, allAgentsPositions);
                if (risk < minRisk)
                {
                    minRisk = risk;
                    bestOption = option;
                }
            }
            return bestOption;
        }

        private int ComputeCollisionRisk(int option, Plan plan, Dictionary<int, Dictionary<Vertex, List<int>>> allAgentsPositions)
        {
            //risk computation
            return 0;
        }

        public void SetCurrentPosition(int vertexNumber, int time, Plan plan)
        {
            var planToTarget = new Dictionary<Vertex, List<int>>();
            var currentVertexNumber = vertexNumber;
            
            AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), currentVertexNumber);
            while (plan.HasNextVertex(currentVertexNumber))
            {
                var nextVertex = plan.GetNext(currentVertexNumber, out int nextNumber);
                currentVertexNumber = nextNumber;
                AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), currentVertexNumber);
            }
            
            _communicator.UpdatePosition(vertexNumber, time, planToTarget);   
        }

        private void AddToDictionary(Dictionary<Vertex, List<int>> dict, Vertex key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(value);
            }
            else
            {
                dict[key] = new List<int> {value};
            }
        }
    }
    
    class SmartAgentWithPolicy : SmartAgent
    {
        private ICollisionPolicy _collisionPolicy;
        public SmartAgentWithPolicy(Vertex start, Vertex target, int id, ICollisionPolicy collisionPolicy) : base(start, target, id)
        {
            _collisionPolicy = collisionPolicy;
        }
        
        public override string ToString()
        {
            return "SmartAgent with " + nameof(_collisionPolicy) + " policy" + id + ": " + start + " --> " + target;
        }
        
        public override int NextVertexToMove(int vertexNumber, Plan plan, int delay = 0)
        {
            var possibleMoves = plan.GetAvailableVerticesFromPosition(vertexNumber);
            return possibleMoves[0];
        }
    }
}
