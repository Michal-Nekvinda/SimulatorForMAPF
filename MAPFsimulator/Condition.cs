namespace MAPFsimulator
{
    /// <summary>
    /// Podminka na vybrani alternativniho planu.
    /// Pouziti: z vrcholu x lze jit do vrcholu s poradovym cislem successor, pokud je zpozdeni v intervalu [min, max].
    /// </summary>
    struct Condition
    {
        public int delayMin;
        public int delayMax;
        public int successor;
        public Condition(int min, int max)
        {
            this.delayMin = min;
            this.delayMax = max;
            successor = -1;           
        }
    }
}