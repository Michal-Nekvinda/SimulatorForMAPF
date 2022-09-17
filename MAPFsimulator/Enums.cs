namespace MAPFsimulator
{
    /// <summary>
    /// Vycet stavu, kterymi prochazi program (pri reseni MAPF v hlavnim okne aplikace).
    /// </summary>
    enum State
    {
        Start,
        MapLoaded,
        AgentLoaded,
        ComputedSolution,
        NoSolution,
    }
    /// <summary>
    /// Vycet s dostupnymi resici.
    /// </summary>
    enum Solver
    {
        CBS = 0,
        Picat = 1
    }
    /// <summary>
    /// Vycet dostupnych typu robustnich planu.
    /// </summary>
    enum RobustnessType
    {
        k = 0,
        min_max = 1,
        alternative_k = 2,
        semi_k = 3,
    }
}
