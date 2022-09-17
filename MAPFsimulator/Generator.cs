using System;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida s jedinou instanci generatoru prirozenych cisel.
    /// Pouzivame k nahodnemu vyberu agentu do instanci.
    /// </summary>
    public class IntGenerator
    {
        private static Random rnd = null;
        private static IntGenerator instance = null;
        private IntGenerator()
        {
            rnd = new Random();
        }
        /// <summary>
        /// Vraci jedinou instanci IntGeneratoru.
        /// </summary>
        public static IntGenerator GetInstance()
        {
            if (instance == null)
            {
                instance = new IntGenerator();
            }
            return instance;
        }
        /// <summary>
        /// Vraci pseudonahodne cislo z intervalu [lowerBound, upperBound).
        /// </summary>
        public int Next(int lowerBound, int upperBound)
        {
            return rnd.Next(lowerBound, upperBound);
        }
        /// <summary>
        /// Resetuje generator s novou hodnotou seed.
        /// </summary>
        public void Reseed(int seed)
        {
            rnd = new Random(seed);
        }
    }
    /// <summary>
    /// Trida s jedinou instanci generatoru desetinnych cisel.
    /// Pouzivame v exekuci pri rozhodovani, zda nastalo zpozdeni.
    /// </summary>
    class DoubleGenerator
    {
        private static Random rnd = null;
        private static DoubleGenerator instance = null;
        private DoubleGenerator()
        {
            rnd = new Random();
        }
        public static DoubleGenerator GetInstance()
        {
            if (instance == null)
            {
                instance = new DoubleGenerator();
            }
            return instance;
        }
        public double NextDouble()
        {
            return rnd.NextDouble();
        }
        public void Reseed(int seed)
        {
            rnd = new Random(seed);
        }
    }
}
