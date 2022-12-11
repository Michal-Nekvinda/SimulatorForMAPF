using System.Collections.Generic;

namespace MAPFsimulator
{
    public static class AgentsPositionProvider
    {
        private static Dictionary<int, Dictionary<Vertex, List<int>>> _predictedPositions;
        private static Dictionary<int, Vertex> _currentPositions;
        private static Dictionary<int, Vertex> _intendedPositions;

        public static void Reset()
        {
            _predictedPositions = new Dictionary<int, Dictionary<Vertex, List<int>>>();
            _intendedPositions = new Dictionary<int, Vertex>();
            _currentPositions = new Dictionary<int, Vertex>();
        }
        
        public static void UpdatePosition(int agentId, Vertex current, Vertex next, Dictionary<Vertex, List<int>> planToTarget)
        {
            _predictedPositions[agentId] = planToTarget;
            _currentPositions[agentId] = current;
            _intendedPositions[agentId] = next;
        }
        
        public static Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return _predictedPositions;
        }
    }
}