using System.Collections.Generic;

namespace MAPFsimulator
{
    public class Communicator
    {
        public Communicator(int agentId)
        {
            this.agentId = agentId;
        }
        private int agentId { get;}
        
        public void UpdatePosition(int vertexNumber, int time, Dictionary<Vertex, List<int>> planToTarget)
        {
            AgentsPositionProvider.UpdatePosition(agentId, vertexNumber, time, planToTarget); 
        }
        
        public Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return AgentsPositionProvider.GetAllAgentsPredictedPositions();
        }
        
    }
}