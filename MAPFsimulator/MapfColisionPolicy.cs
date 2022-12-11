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
    
    public interface ICollisionPolicy
    {
        VertexState GetVertexState(Vertex vertex);
        AgentState GetAgentState();
        MapfSolutionState GetMapfSolutionState();
        void SendState(AgentState signal);
        void SendRequest(int time, Vertex vertex);
    }
    
    public class CollisionPolicy: ICollisionPolicy
    {
        public VertexState GetVertexState(Vertex vertex)
        {
            throw new System.NotImplementedException();
        }

        public MapfSolutionState GetMapfSolutionState()
        {
            throw new System.NotImplementedException();
        }

        public void SendState(AgentState signal)
        {
            throw new System.NotImplementedException();
        }

        public void SendRequest(int time, Vertex vertex)
        {
            throw new System.NotImplementedException();
        }

        public AgentState GetAgentState()
        {
            throw new System.NotImplementedException();
        }
    }
}