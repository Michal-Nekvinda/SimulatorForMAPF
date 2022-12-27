namespace MAPFsimulator
{
    public class AgentFactory
    {
        public static IAgent CreateAgent(Vertex start, Vertex target, int id, bool isSmart)
        {
            if (isSmart)
            {
                return new SmartAgent(start, target, id, new CollisionPolicy(id));
            }
            
            return new Agent(start, target, id);
        }
    }
}