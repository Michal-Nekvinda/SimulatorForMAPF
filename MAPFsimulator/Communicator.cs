using System.Collections.Generic;

namespace MAPFsimulator
{
    interface ICommunicator
    {
        void UpdatePosition(Dictionary<Vertex, List<int>> planToTarget);
        Dictionary<int, Dictionary<Vertex, List<int>>> GetAgentsPredictedPositions();
    }

    public class Communicator : ICommunicator
    {
        public Communicator(int agentId)
        {
            this.agentId = agentId;
        }

        private int agentId { get; }

        public void UpdatePosition(Dictionary<Vertex, List<int>> planToTarget)
        {
            AgentsPositionProvider.UpdateAgentPosition(agentId, planToTarget);
        }

        public Dictionary<int, Dictionary<Vertex, List<int>>> GetAgentsPredictedPositions()
        {
            return AgentsPositionProvider.GetAgentsPredictedPositions();
        }
    }
}