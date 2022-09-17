using System.Drawing;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida obsahujici mnoziny barev pro agenty.
    /// </summary>
    class SetOfColors
    {
        /// <summary>
        /// Agentovi s id index vrati jeho barvu.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="max">maximalni pocet agentu, pro ktery chceme barvu - na zaklade poctu vybereme prislusnou paletu</param>
        public static Color GetColor(int index, int max)
        {
            if (max <= smallCount)
            {
                return ColorTranslator.FromHtml(small[index]);
            }
            if (max <= mediumCount)
            {
                return ColorTranslator.FromHtml(medium[index]);
            }
            return ColorTranslator.FromHtml(big[index % bigCount]);
        }
        //basic html color codes
        static string[] small = {"#FF0000", "#0000FF", "#00FF00", "#FFFF00", "#FF00FF", "#800080", "#FF937E", "#808080",
                                 "#800000", "#FF8C00", "#00FFFF", "#008000", "#808000", "#000080", "#000000", "#008080"};

        //prevzato z stackoverflow.com/questions/1168260/algorithm-for-generating-unique-colors
        static string[] medium = {"	#00FF00	","	#0000FF	","	#FF0000	","	#01FFFE	","	#FFA6FE	","	#FFDB66	","	#006401	","	#010067	",
                                  "	#95003A	","	#007DB5	","	#FF00F6	","	#F6FFA8	","	#774D00	","	#90FB92	","	#0076FF	","	#D5FF00	",
                                  "	#FF937E	","	#6A826C	","	#FF029D	","	#FE8900	","	#7A4782	","	#7E2DD2	","	#85A900	","	#FF0056	",
                                  "	#683D3B	","	#BDC6FF	","	#263400	","	#BDD393	","	#00B917	","	#9E008E	","	#001544	","	#C28C9F	",
                                  "	#FF74A3	","	#A42400	","	#00AE7E	","	#01D0FF	","	#004754	","	#E56FFE	","	#788231	","	#0E4CA1	",
                                  "	#91D0CB	","	#BE9970	","	#968AE8	","	#BB8800	","	#43002C	","	#DEFF74	","	#00FFC6	","	#FFE502	",
                                  "	#620E00	","	#008F9C	","	#98FF52	","	#7544B1	","	#B500FF	","	#00FF78	","	#FF6E41	","	#005F39	",
                                  "	#6B6882	","	#5FAD4E	","	#A75740	","	#A5FFD2	","	#FFB167	","	#009BFF	","	#E85EBE	","	#000000	", };

        //prevzato z godsnotwheregodsnot.blogspot.com/2012/09/color-distribution-methodology.html
        static string[] big = new string[]{
            "#000000", "#FFFF00", "#1CE6FF", "#FF34FF", "#FF4A46", "#008941", "#006FA6", "#A30059",
            "#FFDBE5", "#7A4900", "#0000A6", "#63FFAC", "#B79762", "#004D43", "#8FB0FF", "#997D87",
            "#5A0007", "#809693", "#FEFFE6", "#1B4400", "#4FC601", "#3B5DFF", "#4A3B53", "#FF2F80",
            "#61615A", "#BA0900", "#6B7900", "#00C2A0", "#FFAA92", "#FF90C9", "#B903AA", "#D16100",
            "#DDEFFF", "#000035", "#7B4F4B", "#A1C299", "#300018", "#0AA6D8", "#013349", "#00846F",
            "#372101", "#FFB500", "#C2FFED", "#A079BF", "#CC0744", "#C0B9B2", "#C2FF99", "#001E09",
            "#00489C", "#6F0062", "#0CBD66", "#EEC3FF", "#456D75", "#B77B68", "#7A87A1", "#788D66",
            "#885578", "#FAD09F", "#FF8A9A", "#D157A0", "#BEC459", "#456648", "#0086ED", "#886F4C",
            "#34362D", "#B4A8BD", "#00A6AA", "#452C2C", "#636375", "#A3C8C9", "#FF913F", "#938A81",
            "#575329", "#00FECF", "#B05B6F", "#8CD0FF", "#3B9700", "#04F757", "#C8A1A1", "#1E6E00",
            "#7900D7", "#A77500", "#6367A9", "#A05837", "#6B002C", "#772600", "#D790FF", "#9B9700",
            "#549E79", "#FFF69F", "#201625", "#72418F", "#BC23FF", "#99ADC0", "#3A2465", "#922329",
            "#5B4534", "#FDE8DC", "#404E55", "#0089A3", "#CB7E98", "#A4E804", "#324E72", "#6A3A4C" };

        static int smallCount = 16;
        static int mediumCount = 64;
        static int bigCount = 104;
    }
}
