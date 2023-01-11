using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Rozhrani pro pravidlo pro predchazeni kolizi
    /// </summary>
    public interface ICollisionPolicy
    {
        /// <summary>
        /// Vrati seznam dostunych moznosti prechodu v planu v case time
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="time"></param>
        /// <param name="possibleOptions"></param>
        /// <returns></returns>
        IList<int> FilterOptions(Plan plan, int time, IList<int> possibleOptions);

        /// <summary>
        /// Odesle stav agenta
        /// </summary>
        /// <param name="state"></param>
        void SendAgentState(AgentState state);

        /// <summary>
        /// Zkusi zablokovat vrcholy v case time
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="time"></param>
        void RequestVerticesBlocking(IList<Vertex> vertices, int time);
    }

    /// <summary>
    /// Implementace pro Smart agenta bez collision policy
    /// </summary>
    class NoPolicy : ICollisionPolicy
    {
        public IList<int> FilterOptions(Plan plan, int time, IList<int> possibleOptions)
            => possibleOptions;

        public void SendAgentState(AgentState state)
        {
        }

        public void RequestVerticesBlocking(IList<Vertex> vertices, int time)
        {
        }
    }

    class CollisionPolicy : ICollisionPolicy
    {
        private readonly int _agentId;

        public CollisionPolicy(int agentId)
        {
            _agentId = agentId;
        }

        public IList<int> FilterOptions(Plan plan, int time, IList<int> possibleOptions)
        {
            if (AgentsPositionProvider.GetMapfSolutionState() == MapfSolutionState.Deadlock)
            {
                return possibleOptions;
            }

            var filteredOptions = new List<int>();
            foreach (var option in possibleOptions)
            {
                var v = plan.GetNth(option);
                if (GetVertexState(v, time) == VertexState.Free)
                {
                    filteredOptions.Add(option);
                }
            }

            return filteredOptions;
        }

        private VertexState GetVertexState(Vertex vertex, int time)
        {
            return AgentsPositionProvider.GetVertexState(vertex, time, _agentId);
        }

        public void SendAgentState(AgentState state)
        {
            AgentsPositionProvider.UpdateAgentState(state, _agentId);
        }

        public void RequestVerticesBlocking(IList<Vertex> vertices, int time)
        {
            foreach (var v in vertices)
            {
                AgentsPositionProvider.BlockVertex(v, time, _agentId);
            }
        }
    }
}