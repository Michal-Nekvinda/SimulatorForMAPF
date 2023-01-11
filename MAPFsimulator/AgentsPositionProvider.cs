using System;
using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    /// <summary>
    /// Vycet hodnot pro stavy, ve kterych se muze nachazet MAPF instance
    /// </summary>
    public enum MapfSolutionState
    {
        /// <summary>
        /// Alespon jeden z agentu je ve stavu CanMove
        /// </summary>
        Ok = 0,
        
        /// <summary>
        /// Vsichni agenti jsou ve stavu MustStay
        /// </summary>
        Deadlock = 1,
    }
    
    /// <summary>
    /// Vycet stavu, ve kterych se muze nachazet vrchol grafu
    /// </summary>
    public enum VertexState
    {
        /// <summary>
        /// Vrchol neni zablokovan zadnym agentem
        /// </summary>
        Free = 0,
        
        /// <summary>
        /// Vrchol je blokovany pro agenta
        /// </summary>
        Blocked = 1,
    }
    
    /// <summary>
    /// Trida pro poskytovani informaci z prubehu exekuce vsech agentu
    /// </summary>
    public static class AgentsPositionProvider
    {
        private static Dictionary<int, Dictionary<Vertex, List<int>>> _predictedPositions;
        private static Dictionary<Vertex, List<Tuple<int, int>>> _blockedVertex; //tuple <time, agentId>
        private static Dictionary<int, AgentState> _agentStates;

        /// <summary>
        /// Vymaze data ze vsech datovych struktur
        /// </summary>
        public static void Reset()
        {
            _predictedPositions = new Dictionary<int, Dictionary<Vertex, List<int>>>();
            _blockedVertex = new Dictionary<Vertex, List<Tuple<int, int>>>();
            _agentStates = new Dictionary<int, AgentState>();
        }

        /// <summary>
        /// Vraci predpokladane plany vsech agentu
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<Vertex, List<int>>> GetAgentsPredictedPositions()
        {
            return _predictedPositions;
        }

        /// <summary>
        /// Vraci aktualni stav MAPF instance
        /// </summary>
        /// <returns></returns>
        public static MapfSolutionState GetMapfSolutionState()
        {
            if (_agentStates.Count == 0)
                return MapfSolutionState.Ok;

            return _agentStates.Any(x => x.Value == AgentState.CanMove)
                ? MapfSolutionState.Ok
                : MapfSolutionState.Deadlock;
        }

        /// <summary>
        /// Vraci stav vrcholu grafu vertex v case time pro agenta agentId
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="time"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public static VertexState GetVertexState(Vertex vertex, int time, int agentId)
        {
            if (!_blockedVertex.ContainsKey(vertex))
            {
                return VertexState.Free;
            }

            var blockedVerticesInTime = _blockedVertex[vertex].FindAll(tuple => tuple.Item1 == time);
            if (blockedVerticesInTime.Count == 0)
            {
                return VertexState.Free;
            }

            //vrchol je pro agenta volny, pokud si ho zablokoval a zaroven ho nema zablokovany jiny agent s nizsim id
            return blockedVerticesInTime.Contains(new Tuple<int, int>(time, agentId))
                   && !blockedVerticesInTime.Any(t => t.Item2 < agentId)
                ? VertexState.Free
                : VertexState.Blocked;
        }

        /// <summary>
        /// Zapise aktualni pozici agenta agentId a cestu do cile
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="planToTarget"></param>
        public static void UpdateAgentPosition(int agentId, Dictionary<Vertex, List<int>> planToTarget)
        {
            _predictedPositions[agentId] = planToTarget;
        }

        /// <summary>
        /// Aktualizuje stav agenta agentId
        /// </summary>
        /// <param name="state"></param>
        /// <param name="agentId"></param>
        public static void UpdateAgentState(AgentState state, int agentId)
        {
            _agentStates[agentId] = state;
        }

        /// <summary>
        /// Zkusi zablokovat vrchol vertex v case time pro agenta agentId
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="time"></param>
        /// <param name="agentId"></param>
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