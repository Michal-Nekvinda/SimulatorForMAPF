using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;


namespace MAPFsimulator
{
    public partial class Form1 : Form
    {
        Label info;
        int timeStep;
        MapfModel model;
        MapfView view;
        //okno s grafem
        PictureBox graphPicture;
        State currentState;
        int makespanOfSolution;
        int makespanOfExecution;
        int scrollBarMax;
        List<double>[] abstractPositions;

        /// <summary>
        /// nazev vybraneho grafu
        /// </summary>
        internal string graphName = "";

        /// <summary>
        /// Vytvori hlavni okno programu.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            IntGenerator.GetInstance().Reseed(0);
            DoubleGenerator.GetInstance().Reseed(1);
            ChangeState(State.Start);
            comboBoxRobustType.SelectedIndex = 0;
            info = new Label();
            info.BackColor = Color.LightYellow;
            info.Size = new Size(60, 15);
        }
        /// <summary>
        /// Provede prechod programu do stavu newState.
        /// V kazdem stavu nastavujeme ruznou viditelnost GUI komponent a dovolujeme provadet ruzne akce.
        /// </summary>
        private void ChangeState(State newState)
        {
            switch (newState)
            {
                //po spusteni programu
                case State.Start:
                    timeStep = 0;
                    model = new MapfModel();
                    view = new MapfView(model);
                    groupBoxAgentAdding.Visible = false;
                    groupBoxSolverPicker.Visible = false;
                    agentsPositionsToolStripMenuItem.Enabled = false;
                    groupBoxSimulation.Visible = false;
                    buttonNextExec.Visible = false;
                    checkBoxStartAgents.Visible = false;
                    break;
                //po nacteni validni mapy
                case State.MapLoaded:
                    listBox1.Items.Clear();
                    listBoxAgenti.Items.Clear();
                    model.DeleteAgents();
                    graphPicture = view.VisualizeGraph(panel1);
                    graphPicture.Paint += new PaintEventHandler(DrawAgentsFromView);
                    graphPicture.MouseClick += new MouseEventHandler(ViewInfo);
                    graphPicture.Controls.Add(info);
                    info.Visible = false;
                    groupBoxAgentAdding.Visible = true;
                    agentsPositionsToolStripMenuItem.Enabled = true;
                    groupBoxSolverPicker.Visible = false;
                    groupBoxSimulation.Visible = false;
                    buttonNextExec.Visible = false;
                    checkBoxStartAgents.Visible = false;
                    listBox1.Items.Clear();
                    break;
                //po nacteni pozice alespon 1 agenta
                case State.AgentLoaded:
                    groupBoxSolverPicker.Visible = true;
                    groupBoxSimulation.Visible = false;
                    buttonNextExec.Visible = false;
                    checkBoxStartAgents.Visible = true;
                    graphPicture.Invalidate();
                    listBox1.Items.Clear();
                    break;
                //po zjisteni, ze MAPF problem nema reseni
                case State.NoSolution:
                    listBox1.Items.Clear();
                    listBox1.Items.Add("Řešení nenalezeno!");
                    MessageBox.Show("Řešení nenalezeno.","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    groupBoxSimulation.Visible = false;
                    buttonNextExec.Visible = false;
                    checkBoxStartAgents.Visible = true;
                    graphPicture.Invalidate();
                    break;
                //po nalezeni validniho reseni
                case State.ComputedSolution:
                    groupBoxSimulation.Visible = true;
                    PlanToListbox();
                    labelMakespan.Text = makespanOfSolution.ToString();
                    hScrollBar1.Maximum = scrollBarMax;
                    labelRealMakespan.Text = makespanOfExecution.ToString();
                    view.ComputeAgentPositions(0, GetTimeStepFromAbstractPositions(0));
                    graphPicture.Invalidate();
                    hScrollBar1.Value = 0;
                    buttonNextExec.Visible = true;
                    checkBoxStartAgents.Visible = false;
                    break;
                default:
                    break;
            }
            currentState = newState;
        }

        /// <summary>
        /// Pro dany cas timestep vraci abstraktni pozice vsech agentu v tomto case.
        /// </summary>
        private double[] GetTimeStepFromAbstractPositions(int timestep)
        {
            double[] array = new double[abstractPositions.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = abstractPositions[i][timestep];
            }
            return array;
        }
        /// <summary>
        /// Otevre dialogove okno s nabidkou vyberu souboru obsahujiciho graf.
        /// </summary>
        internal string[] LoadGraphFromFileDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Map files (*.map)|*.map|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog1.FileName;
                    var a = filePath.Split(Path.DirectorySeparatorChar);
                    graphName = a[a.Length - 1];
                    //nacteme podobu grafu - podporuje formaty grafu z benchmarku dostupnych na https://movingai.com/benchmarks/mapf/index.html
                    using (StreamReader str = new StreamReader(filePath))
                    {
                        //hlavicka
                        string type = str.ReadLine();
                        int height = int.Parse(str.ReadLine().Split(null)[1]);
                        int width = int.Parse(str.ReadLine().Split(null)[1]);
                        string[] map = new string[height];
                        if (str.ReadLine() == "map")
                        {
                            for (int i = 0; i < height; i++)
                            {
                                map[i] = str.ReadLine().Substring(0, width);
                            }
                            return map;
                        }
                        else
                        {
                            MessageBox.Show("Špatný formát zadaného grafu.","Chyba",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Soubor s grafem se nepodařilo otevřít a nahrát." + ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            graphName = "";
            return null;
        }

        /// <summary>
        /// Metoda vyvolana po stisku tlacitka s nactenim grafu z Menu.
        /// </summary>
        private void LoadGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var map = LoadGraphFromFileDialog();
            if (map != null)
            {
                if (model.LoadGraph(map))
                {
                    ChangeState(State.MapLoaded);
                }
                else
                {
                    MessageBox.Show("Špatný formát zadaného grafu.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Zakresli do grafu aktualni pozice vsech agentu obsazenych v MAPF modelu.
        /// </summary>
        private void DrawAgentsFromView(object sender, PaintEventArgs e)
        {
            //musime mit spocitane reseni
            if (currentState == State.ComputedSolution)
            {
                Graphics g = e.Graphics;
                for (int i = 0; i < view.currentAgentsPositions.Count; i++)
                {
                    Brush b = new SolidBrush(view.GetColor(i));
                    g.FillEllipse(b, view.currentAgentsPositions[i]);
                    
                }
                DrawTargets(g);
                info.Visible = false;
            }
            //nebo zobrazujeme pocatecni postaveni
            if (currentState == State.AgentLoaded || currentState == State.NoSolution)
            {
                if (checkBoxStartAgents.Checked)
                {
                    view.ComputeAgentPositions(0, null);
                    Graphics g = e.Graphics;
                    for (int i = 0; i < view.currentAgentsPositions.Count; i++)
                    {
                        Brush b = new SolidBrush(view.GetColor(i));
                        g.FillEllipse(b, view.currentAgentsPositions[i]);

                    }
                    DrawTargets(g);
                }
                else
                {
                    info.Visible = false;
                }
            }

        }
        /// <summary>
        /// Vykresleni pozic cilovych vrcholu na obrazovku.
        /// </summary>
        private void DrawTargets(Graphics g)
        {
            var tar = view.GetTargets();
            for (int i = 0; i < tar.Length; i++)
            {
                Pen p = new Pen(view.GetColor(i),2);
                g.DrawRectangle(p, tar[i]);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Vykresli graf rucne zadany uzivatelem v okne aplikace.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            string[] map = textBoxGraph.Text.Trim().Split(',').Select(x => x.Trim()).ToArray();
            if (model.LoadGraph(map))
            {
                ChangeState(State.MapLoaded);
            }
            else
            {
                MessageBox.Show("Špatný formát zadaného grafu. Zadejte graf po řádcích, oddělený čárkami a použijte jen povolené znaky. Všechny řádky musí být stejně dlouhé.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EnableButtons(bool enabled)
        {
            souborToolStripMenuItem.Enabled = enabled;
            button3.Enabled = enabled;
            vlozAgenta.Enabled = enabled;
            vymazAgenty.Enabled = enabled;
            buttonNextExec.Enabled = enabled;
            buttonFindSol.Enabled = enabled;
        }

        /// <summary>
        /// Akce provadene po zapnuti timeru.
        /// Po kazdem tiku vzdy spocitame aktualni pozice agentu a vykreslime je.
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            view.ComputeAgentPositions(timeStep, GetTimeStepFromAbstractPositions(timeStep));
            graphPicture.Invalidate();
            hScrollBar1.Value = timeStep;
            timeStep++;
            if (hScrollBar1.Value == hScrollBar1.Maximum)
            {
                timer1.Enabled = false;
                timeStep = 0;
                EnableButtons(true);
            }
        }

        /// <summary>
        /// Vykresli pozice agentu v case, jehoz hodnota je uvedena na posuvniku.
        /// </summary>
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            view.ComputeAgentPositions(hScrollBar1.Value, GetTimeStepFromAbstractPositions(hScrollBar1.Value));
            graphPicture.Invalidate();
        }

        /// <summary>
        /// Stisknuti tlacitka, ktere provede simulaci planu.
        /// Jeho funkci je zapnuti timeru a zablokovani tlacitek v prubehu simulace.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                EnableButtons(false);
                timer1.Interval = Properties.Settings.Default.TimerInterval;
                timer1.Enabled = true;
            }
        }

        /// <summary>
        /// Odstrani vsechny agenty z modelu a listboxu z obrazovky.
        /// </summary>
        private void vymazAgenty_Click(object sender, EventArgs e)
        {
            listBoxAgenti.Items.Clear();
            model.DeleteAgents();
            if (currentState != State.Start && currentState != State.MapLoaded)
            {
                ChangeState(State.MapLoaded);
            }
        }

        /// <summary>
        /// Vlozeni noveho agenta po stisku tlacitka Vloz agenta.
        /// Kontrolujeme, zda je mozne agenta vlozit, aniz by automaticky doslo k neresitelnosti MAPF problemu.
        /// </summary>
        private void vlozAgenta_Click(object sender, EventArgs e)
        {
            string[] pos = textBoxVlozAg.Text.Trim().Split(',');
            int[] position = new int[4];
            if (pos.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!int.TryParse(pos[i], out position[i]))
                    {
                        MessageBox.Show("Zadejte pozici agenta ve formatu startX, startY, cilX, cilY", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                AddAgentToModel(position, checkBoxSmart.Checked);
                ChangeState(State.AgentLoaded);

            }
            else
                MessageBox.Show("Zadejte pozici agenta ve formatu startX, startY, cilX, cilY", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddAgentToModel(int[] position, bool isSmart)
        {
            var a = AgentFactory.CreateAgent(new Vertex(position[0], position[1]), new Vertex(position[2], position[3]),
                listBoxAgenti.Items.Count, isSmart);
            if (model.LoadAndCheck(a))
            {
                listBoxAgenti.Items.Add(a.ToString());
                textBoxVlozAg.Text = "";
            }
            else
            {
                MessageBox.Show(a +" nelze vložit, protože jeho startovní nebo cílový vrchol je již obsazen jiným agentem.","Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prevede pole cisel do textoveho retezce, ktery zobrazujeme v listboxu s pozicemi agentu.
        /// </summary>
        private string ToListBox(int[] ar)
        {
            string s = "";
            for (int i = 0; i < ar.Length; i++)
            {
                s += ar[i] + " ";
            }
            return s;
        }

        /// <summary>
        /// Otevre nove dialogove okno pro nahrani souboru s pozicemi agentu.
        /// </summary>
        private void agentsPositionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Agents files (*.scen)|*.scen|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<int[]> agents = new List<int[]>();
                    var filePath = openFileDialog1.FileName;
                    using (StreamReader str = new StreamReader(filePath))
                    {
                        //hlavicka
                        string version = str.ReadLine();
                        String line;
                        int count = 0;
                        while ((line = str.ReadLine()) != null)
                        {
                            string[] data = line.Split(null);
                            if (data.Length > 8)
                            {
                                agents.Add(new int[] { int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6]), int.Parse(data[7]) });
                                count++;
                            }


                        }
                        //otevre nove okno, ve kterem si vybereme pocet agentu, ktere chceme do grafu nahrat
                        AgentsLoading al = new AgentsLoading();
                        al.SetText(count);
                        al.FormClosed += (a, ea) =>
                        {
                            if (al.confirmed)
                            {
                                if (model.agents.Count > 0)
                                {
                                    DialogResult result;
                                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                                    result = MessageBox.Show("Smazat všechny doposud přidané agenty?", "Graf již pozice agentů obsahuje", buttons, MessageBoxIcon.Information);
                                    if (result == DialogResult.Yes)
                                    {
                                        listBoxAgenti.Items.Clear();
                                        model.DeleteAgents();
                                    }
                                }
                                //pokud jsme vybrali nahodny vyber, pozice agentu promichame
                                var shuffleIndexes = GetShuffleList(agents.Count, IntGenerator.GetInstance(), al.randomChoice);
                                for (int i = 0; i < al.n; i++)
                                {
                                    AddAgentToModel(agents[shuffleIndexes[i]], al.smartAgents);
                                }
                                if (model.agents.Count > 0)
                                {
                                    ChangeState(State.AgentLoaded);
                                }
                                else
                                {
                                    ChangeState(State.MapLoaded);
                                }
                            }
                            
                        };
                        al.Show();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načtení souboru. " + ex.Message,"Chyba",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Automaticka zmena textu na labelu pri posunuti posuvniku.
        /// </summary>
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            labelTimeStep.Text = hScrollBar1.Value.ToString();
        }

        /// <summary>
        /// Otevre nove okno s benchmarky
        /// </summary>
        private void spustitBenchmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BenchmarksRuns br = new BenchmarksRuns();
            br.Show();
            br.SetForm(this);
        }

        /// <summary>
        /// Metoda vyvolana po zmene typu robustniho planu pomoci polozky v comboBoxu.
        /// Na zaklade vybrane robustni metody zobrazi dalsi parametry k nastaveni.
        /// </summary>
        private void comboBoxRobustType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RobustnessType rt = (RobustnessType)comboBoxRobustType.SelectedIndex;
            if (rt == RobustnessType.min_max)
            {
                labelRobustness.Text = "min";
                numericUpDown1.Value = 1;
                numericUpDown1.Minimum = 1;
                labelMax.Visible = true;
                numericUpDown2.Visible = true;
            }
            else
            {
                labelMax.Visible = false;
                numericUpDown2.Visible = false;
                numericUpDown1.Minimum = 0;
                labelRobustness.Text = "k";
            }
            if (rt == RobustnessType.semi_k || rt == RobustnessType.alternative_k)
            {
                groupBoxSolver.Text = "Řešič pro nalezení hlavního plánu";
                checkBoxStrict.Visible = rt == RobustnessType.alternative_k;
            }
            else
            {
                groupBoxSolver.Text = "Řešič pro nalezení plánu";
                checkBoxStrict.Visible = false;
            }
        }

        /// <summary>
        /// Metoda vyvolana po stisknuti tlactika Najdi reseni.
        /// Na zaklade zadanych parametru najde prislusne reseni.
        /// </summary>
        private void buttonFindSol_Click(object sender, EventArgs e)
        {
            Solver s = Solver.CBS;
            if (radioButtonPicat.Checked)
            {
                s = Solver.Picat;
            }
            RobustnessType p1 = (RobustnessType)comboBoxRobustType.SelectedIndex;
            int p3 = (int)numericUpDown1.Value;
            int p4 = (int)numericUpDown2.Value;
            bool strict = checkBoxStrict.Checked;
            Cursor.Current = Cursors.WaitCursor;
            makespanOfSolution = model.FindSolution(p1, s, p3, p4, strict);
            Cursor.Current = Cursors.Default;
            if (makespanOfSolution == -1)
            {
                ChangeState(State.NoSolution);
            }
            else
            {
                ExecuteSolutionWithDelay();
            }
        }
        /// <summary>
        /// Provede exekuci nalezeneho reseni.
        /// </summary>
        private void ExecuteSolutionWithDelay()
        {
            var delay = (double)numericUpDown3.Value;
            abstractPositions = model.ExecuteSolution(delay, out makespanOfExecution, out var result);
            scrollBarMax = abstractPositions.Max(p => p.Count - 1);
            ChangeState(State.ComputedSolution);
            MessageBox.Show(result, "Exekuce ukončena", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonNextExec_Click(object sender, EventArgs e)
        {
            ExecuteSolutionWithDelay();
        }

        /// <summary>
        /// Vyrovani hodnot na posuvnicich v pripade min/max robustnosti - zmena na prvnim z nich.
        /// </summary>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > numericUpDown2.Value)
            {
                numericUpDown2.Value = numericUpDown1.Value;
            }
        }
        /// <summary>
        /// Vyrovani hodnot na posuvnicich v pripade min/max robustnosti - zmena na druhem z nich.
        /// </summary>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > numericUpDown2.Value)
            {
                numericUpDown1.Value = numericUpDown2.Value;
            }
        }
        /// <summary>
        /// Prekresleni okna pri volbe zobrazit/nezobrazit pozice agentu
        /// </summary>
        private void checkBoxStartAgents_CheckedChanged(object sender, EventArgs e)
        {
            graphPicture.Invalidate();
        }

        private void checkBoxStartAgents_VisibleChanged(object sender, EventArgs e)
        {
            if (checkBoxStartAgents.Visible == false)
            {
                checkBoxStartAgents.Checked = false;
            }
        }

        /// <summary>
        /// Po kliknuti na agenta v grafu zobrazujeme informacni popisek s jeho id.
        /// </summary>
        private void ViewInfo(object sender, MouseEventArgs e)
        {
            if (currentState==State.NoSolution || currentState == State.AgentLoaded || currentState == State.MapLoaded)
            {
                if (!checkBoxStartAgents.Checked)
                {
                    return;
                }
            }
            bool showInfo = false;
            string id = "";
            for (int i = 0; i < view.currentAgentsPositions.Count; i++)
            {
                if (IsInRectangle(view.currentAgentsPositions[i], e.Location))
                {
                    showInfo = true;
                    id = i.ToString();
                    break;
                }
            }
            if (showInfo)
            {
                info.Location = e.Location;
                info.Text = "Agent "+id;
                info.Visible = true;
            }
            else
            {
                info.Visible = false;
            }

        }
        /// <summary>
        /// True je-li bod p uvnitr obdelniku r.
        /// </summary>
        private bool IsInRectangle(Rectangle r, Point p)
        {
            if (p.X > r.X && p.X < r.X + r.Width && p.Y > r.Y && p.Y < r.Y + r.Height)
            {
                return true;
            }
            return false;
        }

        private void radioButtonPlanMake_CheckedChanged(object sender, EventArgs e)
        {
            PlanToListbox();
        }
        /// <summary>
        /// Zobrazi textovou podobu planu v listboxu.
        /// </summary>
        private void PlanToListbox()
        {
            if (radioButtonPlanMake.Checked)
            {
                view.ViewPlans(listBox1);
            }
            else
            {
                view.ViewExecution(listBox1, abstractPositions);
            }
        }
        /// <summary>
        /// Ukonci aplikaci.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Vraci list s indexy [0, capacity - 1] v nahodnem poradi, pokud je swapping = true a ve vzestupnem poradi jinak.
        /// Pouzivame pro nahodny vyber n agentu do instance.
        /// </summary>
        internal List<int> GetShuffleList(int capacity, IntGenerator intGen, bool swapping)
        {
            List<int> numbers = new List<int>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                numbers.Add(i);
            }
            if (swapping)
            {
                for (int i = 0; i < 10 * capacity; i++)
                {
                    int x = intGen.Next(0, capacity);
                    int y = intGen.Next(0, capacity);
                    int tmp = numbers[x];
                    numbers[x] = numbers[y];
                    numbers[y] = tmp;
                }
            }
            return numbers;
        }

        /// <summary>
        /// Zobrazi okno s nastavenim.
        /// </summary>
        private void nastaveníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings set = new Settings();
            set.ShowDialog();
        }

        internal void VisualizeFromBenchmark(List<Plan> plans, List<double>[] abstractPositions, Graph graph,RobustnessType rt, int solPar1, int solPar2)
        {
            //pridam graf
            model.graph = graph;
            ChangeState(State.MapLoaded);
            //nacteni agentu
            for (int i = 0; i < plans.Count; i++)
            {
                var a = AgentFactory.CreateAgent(plans[i].GetNth(0), plans[i].GetNth(plans[i].GetLength() - 1), i,
                    checkBoxSmart.Checked);
                model.LoadAgent(a);
                listBoxAgenti.Items.Add(a.ToString());
            }
            //pridani reseni
            //parametr nutny ke spravnemu vykresleni min/max reseni
            model.solParameter2 = solPar2;
            model.solution = plans;
            this.abstractPositions = abstractPositions;
            makespanOfSolution = model.GetMakespan();
            scrollBarMax = abstractPositions.Max(p => p.Count - 1);
            int length = scrollBarMax;
            if (rt == RobustnessType.min_max)
            {
                double d = length / (double)solPar1;
                length = length / solPar1;
                length = DoubleToInt.DecimalPart(d) < 0.5 ? length : length + 1;
            }
            makespanOfExecution = length;
            ChangeState(State.ComputedSolution);



        }
    }
}
