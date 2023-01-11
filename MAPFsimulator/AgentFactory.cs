namespace MAPFsimulator
{
    /// <summary>
    /// Trida pro vytvareni instanci konkretnich typuu agentu
    /// </summary>
    public static class AgentFactory
    {
        /// <summary>
        /// Vytvori noveho agenta
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="id"></param>
        /// <param name="isSmart"></param>
        /// <returns></returns>
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