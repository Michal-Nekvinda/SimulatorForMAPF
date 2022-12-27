using System.Collections.Generic;

namespace MAPFsimulator
{
    public enum MapfSolutionState
    {
        OK = 0,
        DEADLOCK = 1,
    }

    public enum AgentState
    {
        CAN_MOVE = 0,
        MUST_STAY = 1,
    }

    public enum VertexState
    {
        FREE = 0,
        BLOCKED = 1,
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICollisionPolicy
    {
        IList<int> FilterOptions(Plan plan, int time, IList<int> possibleOptions);
        void SendState(AgentState state);
        void SendRequest(Vertex vertex, int time);
    }

    public class CollisionPolicy : ICollisionPolicy
    {
        private readonly int _agentId;

        public CollisionPolicy(int agentId)
        {
            _agentId = agentId;
        }
        
        public IList<int> FilterOptions(Plan plan, int time, IList<int> possibleOptions)
        {
            if (AgentsPositionProvider.GetMapfSolutionState() == MapfSolutionState.DEADLOCK)
            {
                return possibleOptions;
            }
            
            var filteredOptions = new List<int>();
            foreach (var option in possibleOptions)
            {
                var v = plan.GetNth(option);
                if (GetVertexState(v, time) == VertexState.FREE)
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

        public void SendState(AgentState state)
        {
            AgentsPositionProvider.UpdateAgentState(state, _agentId);
        }

        public void SendRequest(Vertex vertex, int time)
        {
            AgentsPositionProvider.BlockVertex(vertex, time, _agentId);
        }
    }
}