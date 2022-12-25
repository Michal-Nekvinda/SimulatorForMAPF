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
        VertexState GetVertexState(Vertex vertex, int time);
        MapfSolutionState MapfSolutionState { get; }
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

        public MapfSolutionState MapfSolutionState => AgentsPositionProvider.GetMapfSolutionState();

        public VertexState GetVertexState(Vertex vertex, int time)
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