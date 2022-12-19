using System;

namespace MAPFsimulator
{
    public class AgentIdNotFoundException : Exception
    { 
        public AgentIdNotFoundException(int agentId):base($"Agent id {agentId} not found."){}
    }
}