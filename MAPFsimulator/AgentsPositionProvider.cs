using System;
using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    public static class AgentsPositionProvider
    {
        private static Dictionary<int, Dictionary<Vertex, List<int>>> _predictedPositions;
        private static Dictionary<Vertex, List<Tuple<int, int>>> _blockedVertex; //tuple <time, agentId>
        private static Dictionary<int, AgentState> _agentStates;

        public static void Reset()
        {
            _predictedPositions = new Dictionary<int, Dictionary<Vertex, List<int>>>();
            _blockedVertex = new Dictionary<Vertex, List<Tuple<int, int>>>();
            _agentStates = new Dictionary<int, AgentState>();
        }

        public static Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return _predictedPositions;
        }

        public static MapfSolutionState GetMapfSolutionState()
        {
            if (_agentStates.Count == 0)
                return MapfSolutionState.OK;

            return _agentStates.Any(x => x.Value == AgentState.CAN_MOVE)
                ? MapfSolutionState.OK
                : MapfSolutionState.DEADLOCK;
        }

        public static VertexState GetVertexState(Vertex vertex, int time, int agentId)
        {
            if (!_blockedVertex.ContainsKey(vertex))
            {
                return VertexState.FREE;
            }

            var blockedVerticesInTime = _blockedVertex[vertex].FindAll(tuple => tuple.Item1 == time);
            if (blockedVerticesInTime.Count == 0)
            {
                return VertexState.FREE;
            }

            //vrchol je pro agenta volny, pokud si ho zablokoval a zaroven ho nema zablokovany jiny agent s nizsim id
            return blockedVerticesInTime.Contains(new Tuple<int, int>(time, agentId))
                   && !blockedVerticesInTime.Any(t => t.Item2 < agentId)
                ? VertexState.FREE
                : VertexState.BLOCKED;
        }

        public static void UpdateAgentPosition(int agentId, Dictionary<Vertex, List<int>> planToTarget)
        {
            _predictedPositions[agentId] = planToTarget;
        }

        public static void UpdateAgentState(AgentState state, int agentId)
        {
            _agentStates[agentId] = state;
        }

        public static void BlockVertex(Vertex vertex, int time, int agentId)
        {
            if (!_blockedVertex.ContainsKey(vertex))
            {
                _blockedVertex.Add(vertex, new List<Tuple<int, int>> { new Tuple<int, int>(time, agentId) });
                return;
            }

            var blockedVertices = _blockedVertex[vertex];
            var newVertices = new List<Tuple<int, int>> { new Tuple<int, int>(time, agentId) };
            foreach (var tuple in blockedVertices)
            {
                if (tuple.Item1 < time - 1)
                {
                    continue; //blokace v case t-2 a starsi uz nepotrebujeme
                }

                newVertices.Add(tuple);
            }

            _blockedVertex[vertex] = newVertices;
        }
    }
}