using System;
using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    public enum AgentState
    {
        CanMove = 0,
        MustStay = 1,
    }
    
    class SmartAgent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }

        private readonly ICommunicator _communicator;
        private readonly ICollisionPolicy _collisionPolicy;
        private AgentState _currentState;
        private const int MaxTimeInterval = 100;

        public SmartAgent(Vertex start, Vertex target, int id)
        {
            this.id = id;
            this.start = start;
            this.target = target;
            _communicator = new Communicator(id);
            _currentState = AgentState.CanMove;
            _collisionPolicy = new NoPolicy();
        }
        
        public SmartAgent(Vertex start, Vertex target, int id, ICollisionPolicy collisionPolicy)
        {
            this.id = id;
            this.start = start;
            this.target = target;
            _communicator = new Communicator(id);
            _currentState = AgentState.CanMove;
            _collisionPolicy = collisionPolicy;
        }

        public override string ToString()
        {
            return "SmartAgent " + id + ": " + start + " --> " + target;
        }

        public int NextVertex(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            var filteredOptionsByPolicy =
                _collisionPolicy.FilterOptions(plan, time, plan.GetPossibleOptionsFromVertex(vertexNumber));
            
            if (!plan.HasNextVertex(vertexNumber) || filteredOptionsByPolicy.Count == 0)
            {
                _currentState = AgentState.MustStay;
                return vertexNumber;   
            }
            
            _currentState = AgentState.CanMove;
            if (filteredOptionsByPolicy.Count == 1)
            {
                //v tomto kroku neni mozne vybrat jiny vrchol
                return filteredOptionsByPolicy[0];
            }

            var allAgentsPositions = _communicator.GetAgentsPredictedPositions();
            var bestOption = -1;
            var minRisk = -1;
            foreach (var option in filteredOptionsByPolicy)
            {
                var risk = ComputeCollisionRisk(option, plan, time, allAgentsPositions);
                if (minRisk == -1 || risk < minRisk)
                {
                    minRisk = risk;
                    bestOption = option;
                }
            }

            return bestOption;
        }
        public virtual void UpdatePosition(int vertexNumber, int time, Plan plan)
        {
            var planToTarget = new Dictionary<Vertex, List<int>>();
            var currentVertexNumber = vertexNumber;

            var currentTime = time;
            AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), currentTime);
            while (plan.HasNextVertex(currentVertexNumber))
            {
                currentTime++;
                var nextVertex = plan.GetNext(currentVertexNumber, out int nextNumber);
                currentVertexNumber = nextNumber;
                AddToDictionary(planToTarget, nextVertex, currentTime);
            }

            var possibleVerticesToMove =
                plan.GetPossibleOptionsFromVertex(vertexNumber).Select(plan.GetNth).ToList();
            
            _communicator.UpdatePosition(planToTarget);
            _collisionPolicy.RequestVerticesBlocking(possibleVerticesToMove, time + 1);
            _collisionPolicy.SendAgentState(_currentState);
        }

        private int ComputeCollisionRisk(int option, Plan plan, int time,
            Dictionary<int, Dictionary<Vertex, List<int>>> allAgentsPositions)
        {
            var path = ComputePathToNextBranching(option, plan);
            var timeToNextVisit = MaxTimeInterval;

            for (int i = 0; i < path.Count; i++)
            {
                foreach (var agentId in allAgentsPositions.Keys)
                {
                    if (agentId == id)
                    {
                        continue;
                    }

                    if (!allAgentsPositions[agentId].ContainsKey(path[i]))
                    {
                        continue;
                    }

                    //spocitam, jaky je nejmensi casovy rozdil kdy na ceste kterou pocitam prijede dalsi agent
                    // -> je-li rozdil k, potom pri zpozdeni k dojde ke kolizi
                    var lastVertexVisit = ComputeNextVertexVisit(time + i + 1, allAgentsPositions[agentId][path[i]]);
                    if (lastVertexVisit < timeToNextVisit)
                    {
                        timeToNextVisit = lastVertexVisit;
                    }
                }
            }

            //risk v hodnote od 0 do 100 -> 0 = (temer) zadny, 100 = nejvyssi
            return Math.Max(MaxTimeInterval - timeToNextVisit, 0);
        }

        private int ComputeNextVertexVisit(int timeInVertex, IList<int> timesOfOtherAgent)
        {
            var diff = timesOfOtherAgent.Select(t => Math.Abs(t - timeInVertex));

            return diff.Min();
        }

        private List<Vertex> ComputePathToNextBranching(int vertexNumber, Plan plan)
        {
            var vertices = new List<Vertex>();
            var currentNumber = vertexNumber;
            vertices.Add(plan.GetNth(vertexNumber));

            while (plan.HasNextVertex(currentNumber) && plan.GetPossibleOptionsFromVertex(currentNumber).Count <= 1)
            {
                vertices.Add(plan.GetNext(currentNumber, out var next));
                currentNumber = next;
            }

            return vertices;
        }
        
        private void AddToDictionary(IDictionary<Vertex, List<int>> dict, Vertex key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(value);
            }
            else
            {
                dict[key] = new List<int> { value };
            }
        }
    }
}