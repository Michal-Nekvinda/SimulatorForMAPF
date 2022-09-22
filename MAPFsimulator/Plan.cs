using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Datova struktura uchovavajici plan agenta v MAPF problemu.
    /// Plan je List vrcholu (struktura Vertex).
    /// </summary>
    public class Plan
    {
        //List vrcholu reprezentujici plan
        protected List<Vertex> path;
        public static string delimiter = "-->";
        /// <summary>
        /// Vytvori prazdny plan bez vrcholu.
        /// </summary>
        public Plan()
        {
            path = new List<Vertex>();
        }
        /// <summary>
        /// Vytvori novy plan z listu vrcholu uvedenych v path.
        /// </summary>
        /// <param name="path"></param>
        public Plan(List<Vertex> path)
        {
            this.path = path;
        }

        /// <summary>
        /// True, pokud po vrcholu v planu s poradovym cislem i nasleduje v budoucnou dalsi vrchol, ruzny od i-teho.
        /// </summary>
        public virtual bool HasNextVertex(int i)
        {
            //pro sekvencni plany jednoduche
            return i < path.Count - 1;
        }
        /// <summary>
        /// Nacte plan z textoveho retezce.
        /// Jednotlive vrcholy musi byt zapsany ve forme souradnic x,y, oddelene carkami.
        /// Mezi vrcholy se musi nachazet oddelovac "-->"
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True, pokud nacteni probehlo uspesne.</returns>
        public bool LoadFromString(string s)
        {
            bool success = true;
            string[] vertices = s.Split(new string[] { delimiter }, StringSplitOptions.None);
            List<Vertex> newPath = new List<Vertex>();
            for (int i = 0; i < vertices.Length; i++)
            {
                string[] coords = vertices[i].Trim().Split(',');
                int x;
                int y;
                if (coords.Length == 2 && int.TryParse(coords[0], out x) && int.TryParse(coords[1], out y))
                {
                    newPath.Add(new Vertex(x, y));
                }
                else
                {
                    return false;
                }
            }
            this.path = newPath;
            return success;
        }
        /// <summary>
        /// Vraci pocet hran cesty v planu - to je pocet vrcholu - 1.
        /// </summary>
        public virtual int GetCost()
        {
            if (path == null || path.Count == 0)
                return int.MaxValue;
            return path.Count - 1;
        }

        /// <summary>
        /// Vraci pocet vrcholu planu.
        /// </summary>
        public virtual int GetLenght()
        {
            return path.Count;
        }

        /// <summary>
        /// Vraci posloupnost vrcholu, ktere tvori plan.
        /// </summary>
        /// <returns></returns>
        public List<Vertex> GetPath()
        {
            return path;
        }
        /// <summary>
        /// Vraci vrchol, ktery je naslednikem vrcholu s poradovym cislem vertexNumber pri zpozdeni delay.
        /// </summary>
        /// <param name="vertexNumber"></param>
        /// <param name="newNumber">poradove cislo naslednika</param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public virtual Vertex GetNext(int vertexNumber, out int newNumber, int delay = 0)
        {
            newNumber = vertexNumber;
            if (vertexNumber < path.Count - 1)
            {
                newNumber++;
            }
            //posledni vrchol planu je sam sobe naslednikem
            return GetNth(vertexNumber + 1);
        }

        /// <summary>
        /// Prida vrchol na konec planu.
        /// </summary>
        /// <param name="v"></param>
        public void AddVertex(Vertex v)
        {
            path.Add(v);
        }

        /// <summary>
        /// Vrati n-ty vrchol planu. Pro n > pocet vrcholu v planu vraci posledni (cilovy) vrchol.
        /// </summary>
        public Vertex GetNth(int n)
        {
            if (n >= path.Count)
            {
                return path[path.Count - 1];
            }
            else
                return path[n];
        }
        /// <summary>
        /// Vrati n-ty vrchol planu. Pro n > pocet vrcholu v planu vraci posledni (cilovy) vrchol.
        /// Desetinne cislo d, udavajici poradi vrcholu je zaokrouhleno zavolanim metody DoubleToInt.ToInt.
        /// </summary>
        public Vertex GetNth(double d)
        {
            return this.GetNth(DoubleToInt.ToInt(d));
        }

        public virtual IList<int> GetAvailableVerticesFromPosition(int vertexNumber)
        {
            var vertexNumbers = new List<int> {vertexNumber};
            if (vertexNumber < path.Count - 1)
            {
                vertexNumbers.Add(vertexNumber + 1);
            }
            if (vertexNumber > 0)
            {
                vertexNumbers.Add(vertexNumber - 1);
            }
            
            return vertexNumbers;
        }

        /// <summary>
        /// Vraci true, pokud je vrchol, ktery je vracen po zavolani metody GetNth(i), soucasti hlavniho planu.
        /// </summary>
        public virtual bool IsInMainPlan(int i)
        {
            return true;
        }
        public override string ToString()
        {
            if (path == null || path.Count == 0)
                return "no solution";
            else
            {
                string p = "";
                for (int i = 0; i < path.Count; i++)
                {
                    p += path[i].ToString();
                    if (i != path.Count - 1)
                    {
                        p += " --> ";
                    }
                }
                return p;
            }
        }
    }


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

    /// <summary>
    /// Plan s alternativami - trida rozsirujici puvodni sekvenci plan na plan se stromovou strukturou.
    /// Vyuziva ho alternatvni a semi k-robustnost.
    /// </summary>
    class ContingencyPlan : Plan
    {
        //prechodova tabulka - z vrcholu s poradovym cislem x se da jit do vrcholu na zaklade urcitych podminek
        Dictionary<int, List<Condition>> transitionTable;
        private int maxBranches = 1;
        private int mainLength;
        //pomocna tabulka pro korektni vypis planu v metode ToString()
        private Dictionary<int, int> successors;

        /// <summary>
        /// Vytvori novy plan s alternativami a jako hlavni plan vezme list vrcholu mainPlan.
        /// </summary>
        public ContingencyPlan(List<Vertex> mainPlan)
        {
            path = new List<Vertex>();
            transitionTable = new Dictionary<int, List<Condition>>();
            AddMainPlan(mainPlan);
            successors = new Dictionary<int, int>();
        }
        /// <summary>
        /// Vraci true, pokud je cislo i poradove cislo vrcholu, ktery patri do hlavniho planu.
        /// </summary>
        public override bool IsInMainPlan(int i)
        {
            return i < mainLength;
        }

        /// <summary>
        /// True, pokud po vrcholu v planu s poradovym cislem i nasleduje v budoucnou dalsi vrchol, ruzny od i-teho.
        /// </summary>
        public override bool HasNextVertex(int i)
        {
            if (i >= path.Count)
            {
                return false;
            }
            return transitionTable[i][0].successor != i;
        }
        /// <summary>
        /// Vraci pocet hran hlavni cesty - tedy pocet vrcholu - 1.
        /// </summary>
        public override int GetCost()
        {
            return mainLength - 1;
        }
        /// <summary>
        /// Vraci pocet vrcholu hlavniho planu.
        /// </summary>
        public override int GetLenght()
        {
            return mainLength;
        }

        /// <summary>
        /// Vraci vrchol, ktery je naslednikem vrcholu s poradovym cislem vertexNumber pri zpozdeni delay.
        /// </summary>
        /// <param name="vertexNumber"></param>
        /// <param name="newNumber">poradove cislo naslednika</param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public override Vertex GetNext(int vertexNumber, out int newNumber, int delay = 0)
        {
            //podivame se, kam muzeme z vrcholu cislo vertexNumber prejit
            var conditions = transitionTable[vertexNumber];
            newNumber = conditions[0].successor;
            //pokud je tam jen default option, tak ji vratime
            if (conditions.Count == 1)
            {
                return path[conditions[0].successor];
            }
            else
            {
                for (int i = 1; i < conditions.Count; i++)
                {
                    if (conditions[i].delayMin <= delay && conditions[i].delayMax >= delay)
                    {
                        newNumber = conditions[i].successor;
                        return path[conditions[i].successor];
                    }
                }
                return path[conditions[0].successor];
            }

        }

        public override IList<int> GetAvailableVerticesFromPosition(int vertexNumber)
        {
            //TODO chybi predchudce
            var vertexNumbers = new List<int> {vertexNumber};
            var conditions = transitionTable[vertexNumber];
            foreach (var condition in conditions)
            {
                vertexNumbers.Add(condition.successor);
            }

            return vertexNumbers;
        }

        /// <summary>
        /// Nahraje novy hlavni plan - List vrcholu plan.
        /// </summary>
        public void AddMainPlan(List<Vertex> plan)
        {
            mainLength = plan.Count;
            path.AddRange(plan);
            for (int i = 0; i < plan.Count; i++)
            {
                transitionTable.Add(i, new List<Condition>());
                var con = new Condition(-1, -1);
                if (i==plan.Count-1)
                {
                    con.successor = i;
                }
                else
                {
                    con.successor = i + 1;
                }
                transitionTable[i].Add(con);

            }
        }
        /// <summary>
        /// Pridani planu, ktery se napojuje na vrchol s poradovym cislem from za podminek urcenych v c.
        /// </summary>
        public void AddAlternatePlan(List<Vertex> plan, int from, Condition c)
        {
            //novou alternativu, coz je take List vrcholu, pridame na konec Listu, ve kterem uchovavame nas contingency plan
            int vertices = path.Count;
            //spocitame poradove cislo, od ktereho se zacne pocitat tato alternativni cesta
            c.successor = vertices;
            path.AddRange(plan);
            //do prechodove tabulky predame informaci, ze se jedna o alternativu
            transitionTable[from].Add(c);
            successors.Add(vertices, from);
            if (transitionTable[from].Count > maxBranches)
            {
                maxBranches = transitionTable[from].Count;
            }
            for (int i = 0; i < plan.Count; i++)
            {
                transitionTable.Add(vertices + i, new List<Condition>());
                var con = new Condition(-1, -1);
                if (i == plan.Count - 1)
                {
                    con.successor = vertices + i;
                }
                else
                {
                    con.successor = vertices + i + 1;
                }
                transitionTable[vertices + i].Add(con);
            }

        }
        public override string ToString()
        {
            if (path == null || path.Count == 0)
                return "no solution";
            else
            {
                string p = "";
                for (int i = 0; i < path.Count; i++)
                {
                    p += path[i].ToString();
                    if (i != path.Count - 1)
                    {
                        if (transitionTable[i][0].successor == i)
                        {
                            //pristi vrchol bude 1. vrchol alternativni cesty
                            p += "  | ["+successors[i+1]+"]  ";
                        }
                        else
                            p += " --> ";
                    }
                }
                return p;
            }
        }
    }   
}
