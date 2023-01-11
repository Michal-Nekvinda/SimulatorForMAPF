using System.Collections.Generic;

namespace MAPFsimulator
{
    interface ICommunicator
    {
        void UpdatePosition(Dictionary<Vertex, List<int>> planToTarget);
        Dictionary<int, Dictionary<Vertex, List<int>>> GetAgentsPredictedPositions();
    }

    /// <summary>
    /// Rozhrani pro komunikaci mezi agenty
    /// </summary>
    public class Communicator : ICommunicator
    {
        /// <summary>
        /// Vytvori novy komunikator pro agenta s id agentId
        /// </summary>
        /// <param name="agentId"></param>
        public Communicator(int agentId)
        {
            this.agentId = agentId;
        }

        private int agentId { get; }

        /// <summary>
        /// Zapise plan agenta do AgentsPositionProvider
        /// </summary>
        /// <param name="planToTarget"></param>
        public void UpdatePosition(Dictionary<Vertex, List<int>> planToTarget)
        {
            AgentsPositionProvider.UpdateAgentPosition(agentId, planToTarget);
        }

        /// <summary>
        /// Vraci predpokladane plany vsech agentu 
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Dictionary<Vertex, List<int>>> GetAgentsPredictedPositions()
        {
            return AgentsPositionProvider.GetAgentsPredictedPositions();
        }
    }
}