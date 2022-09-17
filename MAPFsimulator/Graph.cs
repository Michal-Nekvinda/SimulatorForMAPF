using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    /// <summary>
    /// Reprezentace grafu, ve kterem se agenti pohybuji.
    /// </summary>
    class Graph
    {
        //vychazime z notace na https://movingai.com/benchmarks/formats.html
        public static readonly char[] allowedChars = { '.', 'G','S', '@', 'O', 'T', 'W' };
        static readonly char[] obstacles = { '@', 'O', 'T', 'W' };
        private string[] map;
        public int width { get; }
        public int height { get; }
        public int activeVertices { get; private set; }

        /// <summary>
        /// Vytvori novy graf z pole retezcu.
        /// Delka vsech retezcu musi byt stejne dlouha (tedy graf musi byt obdelnikovy).
        /// Predpokladame, ze g obsahuje jen povolene znaky.
        /// </summary>
        /// <param name="g"></param>
        public Graph(string[] g)
        {
            map = g;
            width = map[0].Length;
            height = map.Length;
            ComputeActiveVertices();
        }
        /// <summary>
        /// Spocita vrcholy, na ktere agenti mohou vstoupit (zbyvajici cast jsou prekazky).
        /// </summary>
        private void ComputeActiveVertices()
        {
            activeVertices = width * height;
            foreach (var str in map)
            {
                foreach (var c in str)
                {
                    if (Graph.obstacles.Contains(c))
                    {
                        activeVertices--;
                    }
                }
            }
        }
        /// <summary>
        /// Vraci vsechny vrcholy, ze kterych muzeme dojit z vrcholu v na jeden krok (vcetne vrcholu v samotneho).
        /// </summary>
        public List<Vertex> GetNeighbours(Vertex v)
        {
            List<Vertex> neigbours = new List<Vertex>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        if (IsVertex(v.x + i, v.y + j))
                        {
                            neigbours.Add(new Vertex(v.x + i, v.y + j));
                        }
                    }
                }
            }
            return neigbours;
        }

        /// <summary>
        /// Pretransformuje tento graf do formatu, ktery vyzaduje resic v deklarativnim modelu Picat.
        /// </summary>
        /// <returns>Graf ve formatu pouzivanem v Picatu.</returns>
        public int[,] ConvertForPicat()
        {
            int[,] array = new int[width, height];
            int k = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (IsObstacle(j, i))
                    {
                        array[j, i] = 0;
                    }
                    else
                    {
                        array[j, i] = k;
                        k++;
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// Vraci true, pokud se na souradnicich [x,y] nachazi platny vrchol, na ktery muze agent vstoupit.
        /// Cislujeme zleva doprava a shora dolu, tedy [0,0] se nachazi v levem hornim rohu.
        /// </summary>
        public bool IsVertex(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }
            if (x >= width || y >= height)
            {
                return false;
            }
            if (Graph.obstacles.Contains(map[y][x]))
            {
                return false;
            }
            return true;

        }
        /// <summary>
        /// True, pokud je na pozici [x,y] prekazka.
        /// </summary>
        public bool IsObstacle(int x, int y)
        {
            //protoze graf je interne ulozen jako pole retezcu, maji zde x, y opacny vyznam.
            return  Graph.obstacles.Contains(map[y][x]);
        }
    }

    /// <summary>
    /// Dvojice - pouziti pro souradnice grafu
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// x-ova souradnice vrcholu
        /// </summary>
        public int x { get; }
        /// <summary>
        /// y-ova souradnice vrcholu
        /// </summary>
        public int y { get; }

        /// <summary>
        /// Vytvori novy vrchol se souradnicemi x,y.
        /// </summary>
        public Vertex(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 2 vrcholy jsou stejne, maji-li stejne souradnice.
        /// </summary>
        public static bool operator ==(Vertex a, Vertex b)
        {
            if ((object)b == null)
            {
                return (object)a == null;
            }
            return a.x == b.x && a.y == b.y;
        }
        /// <summary>
        /// 2 vrcholy jsou ruzne, nemaji-li stejne souradnice.
        /// </summary>
        public static bool operator !=(Vertex a, Vertex b) { return !(a == b); }
        /// <summary>
        /// 2 vrcholy jsou stejne, maji-li stejne souradnice.
        /// </summary>
        public override bool Equals(object obj)
        {
            return this == (Vertex)obj;
        }
        /// <summary>
        ///pouzivame grafy vyrazne mensich rozmeru nez 1000x1000 (max je 161x63)
        /// </summary>
        public override int GetHashCode()
        {
            return this.x * 1000 + this.y;
        }
        /// <summary>
        /// Vypise vrchol ve forme x,y.
        /// </summary>
        public override string ToString()
        {
            return x + "," + y;
        }
    }
}
