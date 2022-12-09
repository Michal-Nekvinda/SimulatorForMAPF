using System.Collections.Generic;

namespace MAPFsimulator
{
    public static class AgentsPositionProvider
    {
        private static Dictionary<int, Dictionary<Vertex, List<int>>> _predictedPositions;
        
        public static void Reset()
        {
            _predictedPositions = new Dictionary<int, Dictionary<Vertex, List<int>>>();
        }
        
        public static void UpdatePosition(int agentId, int vertexNumber, int time, Dictionary<Vertex, List<int>> planToTarget)
        {
            _predictedPositions[agentId] = planToTarget;
        }
        
        public static Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return _predictedPositions;
        }
    }
}