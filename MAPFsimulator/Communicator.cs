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
        
        public void UpdatePosition(Dictionary<Vertex, List<int>> planToTarget)
        {
            AgentsPositionProvider.UpdateAgentPosition(agentId, planToTarget); 
        }
        
        public Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return AgentsPositionProvider.GetAllAgentsPredictedPositions();
        }
        
    }
}