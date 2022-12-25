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
                var risk = ComputeCollisionRisk(option, plan, allAgentsPositions);
                if (minRisk == -1 || risk < minRisk)
                {
                    minRisk = risk;
                    bestOption = option;
                }
            }

            return bestOption;
        }

        protected int ComputeCollisionRisk(int option, Plan plan,
            Dictionary<int, Dictionary<Vertex, List<int>>> allAgentsPositions)
        {
            var path = ComputePathToNextBranching(option, plan);
            var timeToNextVisit = int.MaxValue;
            foreach (var vertex in path)
            {
                foreach (var agentId in allAgentsPositions.Keys)
                {
                    if (agentId == id)
                    {
                        continue;
                    }

                    if (!allAgentsPositions[agentId].ContainsKey(vertex))
                    {
                        continue;
                    }

                    var lastVertexVisit = ComputeNextVertexVisit(option, allAgentsPositions[agentId][vertex]);
                    if (lastVertexVisit < timeToNextVisit)
                    {
                        timeToNextVisit = lastVertexVisit;
                    }
                }
            }

            return timeToNextVisit;
        }

        private int ComputeNextVertexVisit(int timeInVertex, IList<int> timesOfOtherAgents)
        {
            if (timesOfOtherAgents.Max() < timeInVertex)
            {
                return int.MaxValue;
            }

            var diff = timesOfOtherAgents.Select(t => t - timeInVertex);

            return diff.Where(x => x >= 0).Min();
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

        public virtual void SetCurrentPosition(int vertexNumber, int time, Plan plan)
        {
            var planToTarget = new Dictionary<Vertex, List<int>>();
            var currentVertexNumber = vertexNumber;

            AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), currentVertexNumber);
            while (plan.HasNextVertex(currentVertexNumber))
            {
                plan.GetNext(currentVertexNumber, out int nextNumber);
                currentVertexNumber = nextNumber;
                AddToDictionary(planToTarget, plan.GetNth(currentVertexNumber), currentVertexNumber);
            }

            communicator.UpdatePosition(planToTarget);
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

        List<int> FilterOptionsByPolicy(int time, IList<int> possibleOptions, Plan plan)
        {
            var filteredOptions = new List<int>();
            foreach (var option in possibleOptions)
            {
                var v = plan.GetNth(option);
                if (_collisionPolicy.GetVertexState(v, time) == VertexState.FREE)
                {
                    filteredOptions.Add(option);
                }
            }

            return filteredOptions;
        }

        public override int NextVertexToMove(int time, int vertexNumber, Plan plan, int delay = 0)
        {
            var possibleOptions = plan.GetPossibleOptionsFromVertex(vertexNumber);
            if (_collisionPolicy.MapfSolutionState != MapfSolutionState.DEADLOCK)
            {
                possibleOptions = FilterOptionsByPolicy(time, possibleOptions, plan);
            }

            //kvuli policy se nemohu posunout do zadneho z vrcholu dle planu
            if (possibleOptions.Count == 0)
            {
                _currentState = AgentState.MUST_STAY;
                return vertexNumber;
            }

            _currentState = AgentState.CAN_MOVE;
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
                var risk = ComputeCollisionRisk(option, plan, allAgentsPositions);
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
            var possibleNextMoves = plan.GetPossibleOptionsFromVertex(vertexNumber);
            foreach (var move in possibleNextMoves)
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