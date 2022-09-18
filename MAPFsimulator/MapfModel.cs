using System.Collections.Generic;
using System.Linq;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida sdruzujici data MAPF problemu.
    /// Obsahuje graf, agenty s pocatecnimi a cilovymi vrcholy a reseni (pokud bylo nalezeno).
    /// </summary>
    class MapfModel
    {
        public List<IAgent> agents { get; }
        public Graph graph { get; set; }
        public List<Plan> solution { get; set; }
        
        //parametry pro hledani robustnich planu
        public RobustnessType typeOfSol { get; internal set; }
        public int solParameter1 { get; internal set; }
        public int solParameter2 { get; internal set; }

        /// <summary>
        /// Vytvori novou instanci MAPF problemu.
        /// </summary>
        public MapfModel()
        {
            agents = new List<IAgent>();
        }
        /// <summary>
        /// Prida graf map do instance MAPF problemu.
        /// </summary>
        /// <param name="map"></param>
        public bool LoadGraph(string[] map)
        {
            solution = null;
            if (CheckGraph(map))
            {
                graph = new Graph(map);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Kontroluje, zda pole rezetezcu je validni graf.
        /// </summary>
        private bool CheckGraph(string[] map)
        {
            if (map == null)
            {
                return false;
            }
            int length = map[0].Length;
            if (length == 0)
            {
                return false;
            }
            foreach (var row in map)
            {
                if (row.Length != length)
                {
                    return false;
                }
                foreach (var c in row)
                {
                    if (!Graph.allowedChars.Contains(c))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Zkusi pridat agenta agent do instance MAPF problemu. Zaroven zkontroluje, zda jeho pozice startovniho ci ciloveho vrcholu
        /// nekoliduje s nekterym jiz pridanym agentem. Pokud ano, agenta do grafu neprida (jinak by instance nemela reseni).
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>true, pokud doslo k pridani agenta</returns>
        public bool LoadAndCheck(IAgent agent)
        {
            foreach (var a in agents)
            {
                if (a.start == agent.start || a.target == agent.target)
                {
                    return false;
                }
            }
            LoadAgent(agent);
            return true;
        }
        /// <summary>
        /// Prida agenta agent do instance MAPF problemu.
        /// </summary>
        /// <param name="agent"></param>
        public void LoadAgent(IAgent agent)
        {
            agents.Add(agent);
            solution = null;
        }
        /// <summary>
        /// Smaze vsechny agenty.
        /// </summary>
        public void DeleteAgents()
        {
            agents.Clear();
            solution = null;
        }
        /// <summary>
        /// Vraci makespan aktualniho reseni. Pokud reseni zatim nebylo nalezeno, vraci -1.
        /// </summary>
        public int GetMakespan()
        {
            if (HasSolution())
            {
                return new Makespan().GetCost(solution);
            }
            return -1;
        }
        /// <summary>
        /// True, pokud obsahuje validni reseni.
        /// </summary>
        public bool HasSolution()
        {
            if (solution==null || solution.Count==0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Provede exekuci pritomneho reseni s pravdepodobnosti kolize delay. 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="length">skutecna delka provedeneho reseni (vcetne zpozdeni)</param>
        /// <param name="exMessage">zprava o prubehu exekuce</param>
        /// <returns>List abstraktnich pozic agentu v jednotlivych casovych usecich. 
        /// Abstraktni pozice je poradove cislo vrcholu v puvodnim nezpozdenem planu. </returns>
        public List<double>[] ExecuteSolution(double delay, out int length, out string exMessage)
        {
            exMessage = "No solution for execution";
            length = -1;
            if (!HasSolution())
            {
                return null;
            }
            
            IPlansExecutor plansExecutor;
            //vyber exekuce dle typu robustnosti
            if (typeOfSol == RobustnessType.min_max)
            {
                plansExecutor = new Min_MaxRobustExecution(agents.Count, solParameter1, solParameter2, delay);
            }
            else
            {
                if (typeOfSol == RobustnessType.alternative_k || typeOfSol == RobustnessType.semi_k)
                {
                    plansExecutor = new ContigencyExecution(agents.Count, delay);
                }
                else
                {
                    plansExecutor = new SimpleExecution(agents.Count, delay);
                }

            }
            
            Conflict c;
            var abstractPositions = plansExecutor.ExecuteSolution(solution, out exMessage, out c, out length);
            
            if (typeOfSol == RobustnessType.min_max)
            {
                double d = length / (double)solParameter1;
                length = length / solParameter1;
                length = DoubleToInt.DecimalPart(d) < 0.5 ? length : length + 1;
            }
            
            if (exMessage == "ended successfully")
            {
                exMessage = "Exekuce proběhla bez kolizí.";
            }
            else
            {
                exMessage = "Exekuce skončila kolizí: " + c.ToString();
            }
            return abstractPositions;

        }
        /// <summary>
        /// Najde plan pro zadany MAPF problem s pozadovanym typem robustnosti
        /// </summary>
        /// <param name="robustnessType">typ robustnosti, ktery v planu pozadujeme</param>
        /// <param name="solver">typ resice, kterym budeme plan hledat</param>
        /// <param name="rob1">parametr robustnosti</param>
        /// <param name="rob2">druhy parametr robustnosti - validni pouze jako parametr max pro min/max robustnost</param>
        /// <param name="strict">true, pokud chceme striktni alternativni k-robustnost</param>
        /// <returns>Makespan nalezeno planu</returns>
        public int FindSolution(RobustnessType robustnessType, Solver solver, int rob1, int rob2 = 0, bool strict = false)
        {
            solution = null;
            
            typeOfSol = robustnessType;
            solParameter1 = rob1;
            solParameter2 = rob2;
            
            if (solver==Solver.CBS)
            {
                CBSsolver<Makespan> cbs = null;
                switch (robustnessType)
                {
                    case RobustnessType.k:
                        if (rob1 > 0)
                        {
                            cbs = new I_k_CBSsolver<Makespan>(rob1);
                        }
                        else
                        {
                            cbs = new CBSwithSwapping<Makespan>();
                        }
                        break;
                    //vse ostatni
                    default:
                        cbs = new CBSwithSwapping<Makespan>();
                        break;
                }
                solution = cbs.GetPlan(graph, agents);
            }
            if (solver==Solver.Picat)
            {
                int picatRob = (robustnessType == RobustnessType.k) ? rob1 : 0;
                //spusti resic Picat v jinem vlaknu
                PicatSolving ps = new PicatSolving();
                solution = ps.SolveByPicat(picatRob, graph.ConvertForPicat(), agents);
            }
            if (robustnessType==RobustnessType.semi_k || robustnessType==RobustnessType.alternative_k)
            {
                WaitAddingRobustness war = new WaitAddingRobustness(solution);
                //solution = war.GetRobustPlan(rob1);
            }
            //pridani alternativnich planu
            if (robustnessType==RobustnessType.semi_k)
            {
                SemiK_rob rob = new SemiK_rob();
                var c_sol = rob.GetRobustPlan(solution, graph, agents, rob1);
                if (c_sol==null)
                {
                    solution = null;
                }
                else
                    solution = c_sol.Cast<Plan>().ToList();
            }
            if (robustnessType==RobustnessType.alternative_k)
            {
                AlternativeK_rob rob = new AlternativeK_rob(rob1, strict);
                var c_sol = rob.GetRobustPlan(solution, graph, agents, rob1);
                if (c_sol == null)
                {
                    solution = null;
                }
                else
                    solution = c_sol.Cast<Plan>().ToList();
            }
            return GetMakespan();
        }
    }
}
