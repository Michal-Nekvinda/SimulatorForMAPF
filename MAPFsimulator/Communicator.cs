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
        
        public void UpdatePosition(Vertex current, Vertex next, Dictionary<Vertex, List<int>> planToTarget)
        {
            AgentsPositionProvider.UpdateAgentPosition(agentId, current, next, planToTarget); 
        }
        
        public Dictionary<int, Dictionary<Vertex, List<int>>> GetAllAgentsPredictedPositions()
        {
            return AgentsPositionProvider.GetAllAgentsPredictedPositions();
        }
        
    }
}