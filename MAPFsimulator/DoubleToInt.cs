using System;

namespace MAPFsimulator
{
    class DoubleToInt
    {
        public static double epsilon = 0.0001;
        /// <summary>
        /// Vraci nejblizsi nizsi cele cislo mimo epsilonoveho okoli. 
        /// V epsilonovem okoli celeho cisla vraci dane cislo.
        /// </summary>
        public static int ToInt(double d)
        {
            return (int)Math.Floor(d + epsilon);
        }
        /// <summary>
        /// Vraci desetinou cast cisla. V pripade, ze je cislo v epsilonovem okoli celeho cisla, tak vraci 0.
        /// </summary>
        public static double DecimalPart(double d)
        {
            int i = ToInt(d);
            if (Math.Abs(d - i) < 2*epsilon)
            {
                return 0;
            }
            return d - i;
        }
    }
}
