using System.Collections.Generic;

namespace MAPFsimulator
{
    public class ExecutionWithSmartAgents : IPlansExecutor
    {
        int _colTime = 0;
        string _colType = "";
        double _delay;
        List<double>[] _positionsInTime;
        Queue<Vertex>[] _swapConfStruct;
        int _agentsCount;
        HashSet<Vertex> _currentVertices;
        bool _vertexConflict;
        bool _swappingConflict;
        int _maxTime;
        Conflict _confV;

        public ExecutionWithSmartAgents(int agentsCount, double delay)
        {
            _agentsCount = agentsCount;
            _positionsInTime = new List<double>[agentsCount];
            _swapConfStruct = new Queue<Vertex>[agentsCount];
            _delay = delay;
            for (var i = 0; i < agentsCount; i++)
            {
                _positionsInTime[i] = new List<double> {0};
                _swapConfStruct[i] = new Queue<Vertex>();
                //vypln pomoci fiktivniho vrcholu
                _swapConfStruct[i].Enqueue(new Vertex(-1, -1));
            }
            _maxTime = 1;
        }
        
        public MapfExecutionResult ExecuteSolution(List<Plan> plans, List<IAgent> agents)
        {
            var result = new MapfExecutionResult();
            bool finished = false;
            _vertexConflict = false;
            _swappingConflict = false;
            int time = 1;
            for (int i = 0; i < plans.Count; i++)
            {
                _swapConfStruct[i].Enqueue(plans[i].GetNth(0));
            }
            //dokud nejsou vsichni agenti v cili
            while (!finished)
            {
                finished = true;
                _currentVertices = new HashSet<Vertex>();
                //pro plan kazdeho z agentu zacnu pocitat od casu 1 (cas 0 = startovni vrchol)
                //positionsInTime[i][j] = poradove cislo vrcholu agenta i v case j (0 = start a tedy 1. vrchol cesty, 1 = 2.vrchol cesty,...)
                for (int i = 0; i < plans.Count; i++)
                {
                    //pokud ma zpozdeni, zustava tam, kde byl predtim
                    if (WillDelay(i, plans[i], time))
                    {
                        _positionsInTime[i].Add(_positionsInTime[i][time - 1]);
                    }
                    //jinak se posouva do dalsiho vrcholu
                    else
                    {
                        _positionsInTime[i].Add(_positionsInTime[i][time - 1]);
                    }
                    //pokud nektery z agentu jeste neni v cili (ma pred sebou jeste nejake vrcholy), bude nasledovat dalsi iterace cyklu
                    if (finished && plans[i].HasNextVertex(DoubleToInt.ToInt(_positionsInTime[i][time])))
                    {
                        finished = false;
                    }
                    //zda byl objeven hranovy konflikt
                    if (!_swappingConflict && !_vertexConflict && IsVertexConflict(i, plans[i], time, plans))
                    {
                        _vertexConflict = true;
                    }
                }

                //kontrola swapping konfliktu
                if (!_vertexConflict && !_swappingConflict && IsSwappingConflict(time))
                {
                    _swappingConflict = true;
                }
                time++;
            }

            result.ExecutionLength = time - 1;
            if (new Makespan().GetCost(plans) == 0)
            {
                result.ExecutionLength = 0;
            }
            //pri provadeni doslo ke kolizi
            if (_vertexConflict || _swappingConflict)
            {
                result.Message = "ended with conflict";
                _colTime = _confV.time;
                TypOfConf(plans, _confV.agentID1, _confV.agentID2, DoubleToInt.ToInt(_positionsInTime[_confV.agentID1][_confV.time]), DoubleToInt.ToInt(_positionsInTime[_confV.agentID2][_confV.time]));

            }
            else
            {
                result.Message = "ended successfully";
            }

            result.Conflict = _confV;
            result.AbstractPositions = _positionsInTime;
            return result;
        }
        
        /// <summary>
        /// Vraci true, pokud ma agentID s planem p v case t vrcholovy konflikt s jinym agentem.
        /// </summary>
        protected virtual bool IsVertexConflict(int agentId, Plan p, int t, List<Plan> plans)
        {
            var v = p.GetNth(_positionsInTime[agentId][t]);
            _swapConfStruct[agentId].Dequeue();
            _swapConfStruct[agentId].Enqueue(v);
            //vrchol uz v hash setu je  - tedy mame konflikt - zbyva detekovat druheho z agentu
            if (!_currentVertices.Add(v))
            {
                int theSecond = 0;
                for (int i = 0; i < agentId; i++)
                {
                    if (v == plans[i].GetNth(_positionsInTime[i][t]))
                    {
                        theSecond = i;
                        break;
                    }
                }
                //konflikt ulozime a vratime po provedeni planu
                _confV = new Conflict(agentId, theSecond, v, t);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Vraci true, pokud byl v planech agentu v case time detekovan konflikt vymeny vrcholu.
        /// </summary>
        private bool IsSwappingConflict(int time)
        {
            for (var i = 0; i < _agentsCount; i++)
            {
                for (var j = i + 1; j < _agentsCount; j++)
                {
                    var v1 = _swapConfStruct[i].Peek();
                    var v2 = _swapConfStruct[j].Peek();
                    if (v1 == v2)
                    {
                        //maji vrcholovy konflikt
                        return false;
                    }
                    //kazda fronta ma prave 2 prvky
                    //agenti nemaji vrcholovy konflikt - to jsme zkontrolovali drive
                    //pokud tedy fronty navzajem obsahuji posledni pridane vrcholy, maji swapping conflict
                    if (_swapConfStruct[i].Contains(v2) && _swapConfStruct[j].Contains(v1))
                    {
                        //konflikt ulozime a vratime po provedeni planu
                        _confV = new SwapConflict(i, j, v1, time, v2);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Vraci true, pokud se agentID v planu p v case t opozdi.
        /// </summary>
        private bool WillDelay(int agentId, Plan p, int t)
        {
            //pokud agent mel provest akci wait, nebo uz je na konci planu, tak vratime false, protoze se nezpozdi
            if (p.GetNth(_positionsInTime[agentId][t - 1]) == p.GetNth(_positionsInTime[agentId][t - 1] + 1))
            {
                return false;
            }
            var dd = DoubleGenerator.GetInstance().NextDouble();
            return dd < _delay;
        }
        /// <summary>
        /// Rozlisuje, jaky typ konfliktu mezi agenty nastal (pro contigency robustnost).
        /// </summary>
        private void TypOfConf(List<Plan> plans, int ag1, int ag2, int vertex1, int vertex2)
        {
            if (plans[ag1].IsInMainPlan(vertex1))
            {
                _colType = plans[ag2].IsInMainPlan(vertex2) ? "Hlavni" : "Hlavni+Vedlejsi";
            }
            else
            {
                _colType = plans[ag2].IsInMainPlan(vertex2) ? "Hlavni+Vedlejsi" : "Vedlejsi";
            }
        }
    }
}