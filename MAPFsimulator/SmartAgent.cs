using System;
using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    class SmartAgent : IAgent
    {
        public Vertex start { get; }
        public Vertex target { get; }
        public int id { get; set; }

        protected readonly Communicator communicator;
        const int MaxTimeInterval = 100;

        public SmartAgent(Vertex start, Vertex target, int id)
        {
            this.id = id;
            this.start = start;
            this.target = target;
            communicator = new Communicator(id);
        }

        public override string ToString()
        {
            return "SmartAgent " + id + ": " + start + " --> " + target;
        }

        public virtual int NextVertexToMove(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            var possibleOptions = plan.GetPossibleOptionsFromVertex(vertexNumber);
            if (possibleOptions.Count == 1)
            {
                //v tomto kroku neni mozne vybrat jiny vrchol
                return possibleOptions[0];
            }

            var allAgentsPositions = communicator.GetAllAgentsPredictedPositions();
            var bestOption = -1;
            var minRisk = -1;
            foreach (var option in possibleOptions)
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
        public virtual void SetCurrentPosition(int vertexNumber, int time, Plan plan)
        {
            var planToTarget = new Dictionary<Vertex, List<int>>();
            var currentVertexNumber = vertexNumber;

            AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), time);
            while (plan.HasNextVertex(currentVertexNumber))
            {
                time++;
                var nextVertex = plan.GetNext(currentVertexNumber, out int nextNumber);
                currentVertexNumber = nextNumber;
                AddToDictionary(planToTarget, nextVertex, time);
            }

            communicator.UpdatePosition(planToTarget);
        }

        protected int ComputeCollisionRisk(int option, Plan plan, int time,
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

    class SmartAgentWithPolicy : SmartAgent
    {
        private AgentState _currentState;
        private ICollisionPolicy _collisionPolicy;

        public SmartAgentWithPolicy(Vertex start, Vertex target, int id, ICollisionPolicy collisionPolicy) : base(start,
            target, id)
        {
            _collisionPolicy = collisionPolicy;
            _currentState = AgentState.CAN_MOVE;
        }
        
        public override int NextVertexToMove(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            if (!plan.HasNextVertex(vertexNumber))
            {
                _currentState = AgentState.MUST_STAY;
                return vertexNumber;   
            }

            var filteredOptionsByPolicy =
                _collisionPolicy.FilterOptions(plan, time, plan.GetPossibleOptionsFromVertex(vertexNumber));

            //kvuli policy se nemohu posunout do zadneho z vrcholu dle planu
            if (filteredOptionsByPolicy.Count == 0)
            {
                _currentState = AgentState.MUST_STAY;
                return vertexNumber;
            }

            _currentState = AgentState.CAN_MOVE;
            if (filteredOptionsByPolicy.Count == 1)
            {
                //v tomto kroku neni mozne vybrat jiny vrchol
                return filteredOptionsByPolicy[0];
            }
            
            var allAgentsPositions = communicator.GetAllAgentsPredictedPositions();
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

        public override void SetCurrentPosition(int vertexNumber, int time, Plan plan)
        {
            base.SetCurrentPosition(vertexNumber, time, plan);
            foreach (var move in plan.GetPossibleOptionsFromVertex(vertexNumber))
            {
                _collisionPolicy.SendRequest(plan.GetNth(move), time + 1);
            }

            _collisionPolicy.SendState(_currentState);
        }

        public override string ToString()
        {
            return "SmartAgent with " + nameof(_collisionPolicy) + " policy" + id + ": " + start + " --> " + target;
        }
    }
}