using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida zajistujici provadeni testu.
    /// </summary>
    public partial class BenchmarksRuns : Form
    {
        Form1 mainForm;
        MapfModel model;
        List<int[]> agentsPositions;
        int rowsInResults = 0;
        int currentInstance = 0;
        int currentPlan = 0;
        int numberOfExecutions = 0;
        int markedExecution = 0;
        int repetitions;
        int wholeTestRepetitions;
        int experiments;
        List<Plan>[,] realPlans;
        private SynchronizationContext originalContext;
        private bool stopRunning = false;
        List<List<List<List<double>[]>>> abstractPlans;
        RobustnessType rtGlobal;
        string logString;

        /// <summary>
        /// Vytvori okno s benchmarky.
        /// </summary>
        public BenchmarksRuns()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            comboBoxRobustType.SelectedIndex = 0;
            model = new MapfModel();
            agentsPositions = new List<int[]>();
            originalContext = SynchronizationContext.Current;

        }

        /// <summary>
        /// Nastavuje odkaz na hlavni okno programu.
        /// </summary>
        /// <param name="f"></param>
        public void SetForm(Form1 f)
        {
            mainForm = f;
        }
        private void ClearListBoxes()
        {
            listBoxResults.Items.Clear();
            listBoxSimulations.Items.Clear();
            listBoxPlans.Items.Clear();
            listBoxExecutions.Items.Clear();
        }

        /// <summary>
        /// Metoda vyvolana po stisku tlacitka Vybrat u polozky Graf. 
        /// Otevre dialogove okno pro vyber souboru s grafem k testovani.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGraphChoice_Click(object sender, EventArgs e)
        {
            var map = mainForm.LoadGraphFromFileDialog();
            if (map != null)
            {
                if (model.LoadGraph(map))
                {
                    labelPath.Text = mainForm.graphName;
                }
                else
                {
                    MessageBox.Show("Špatný formát zadaného grafu.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Metoda vyvolana po stisku tlacitka Vybrat u polozky Pozice agentu. 
        /// Otevre dialogove okno pro vyber souboru s pozicemi agentu k testovani.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAgentsChoice_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Agents files (*.scen)|*.scen|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //list pro ulozeni pozic vsech dostupnych agentu
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
                            //kontrola delky a nacteni relevantnich udaju 
                            if (data.Length > 8)
                            {
                                agents.Add(new int[] { int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6]), int.Parse(data[7]) });
                                count++;
                            }
                        }
                        agentsPositions = agents;
                        var a = filePath.Split(Path.DirectorySeparatorChar);
                        agentiCesta.Text = a[a.Length - 1];
                        agentiPocet.Text = " Nalezeno " + agentsPositions.Count.ToString() + " agentů";
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načtení souboru." + ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Metoda vyvolana po stisku tlacitka Spustit test.
        /// Na zaklade vyplnenych parametru provede pozadovany benchmark.
        /// </summary>
        private async void buttonBenchmarkRun_Click(object sender, EventArgs e)
        {
            //nastaveni generatoru nahodnych cisel pro opakovatelnost testu
            IntGenerator.GetInstance().Reseed(0);
            DoubleGenerator.GetInstance().Reseed(1);

            //kontrola parametru testu a pripadny vypis chybovych hlasek
            int min = 0;
            int.TryParse(agentsMin.Text, out min);
            agentsMin.Text = min.ToString();
            int max = 1;
            int.TryParse(agentsMax.Text, out max);
            agentsMax.Text = max.ToString();
            buttonVisualize.Visible = false;

            if (max < min)
            {
                MessageBox.Show("Maximální počet agentů musí být větší než minimální počet agentů.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            repetitions = 0;
            int.TryParse(textBoxRepetition.Text, out repetitions);
            int repetitionsExec = 0;
            int.TryParse(textBoxRepetitionExec.Text, out repetitionsExec);
            if (repetitions <= 0 || repetitionsExec <= 0)
            {
                MessageBox.Show("Počet opakování musí být větší než 0.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (agentsPositions.Count < min)
            {
                MessageBox.Show("Minimální počet agentů je větší než počet nalezených agentů.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (agentsPositions.Count < max)
            {
                max = agentsPositions.Count;
                MessageBox.Show("Počet nalezených agentů je pouze " + max + ".", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (min == 0)
            {
                MessageBox.Show("Minimální počet agentů musí být alespoň 1.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            experiments = ((max - min) / (int)numericUpDownStep.Value) + 1;
            double numberOfAlternatives = 0;
            stopRunning = false;
            ClearListBoxes();
            listBoxResults.Items.Clear();
            model.DeleteAgents();
            numberOfExecutions = repetitionsExec;

            //nacteni zbytku dat
            Solver s = Solver.CBS;
            if (radioButtonPicat.Checked)
            {
                s = Solver.Picat;
            }
            RobustnessType param1 = (RobustnessType)comboBoxRobustType.SelectedIndex;
            rtGlobal = param1;
            int param2 = (int)numericUpDown1.Value;
            int param3 = (int)numericUpDown2.Value;
            double delay = (double)numericUpDownDelay.Value;
            wholeTestRepetitions = (int)numericUpDownTestRep.Value;
            string strict = checkBoxStrict.Checked ? "true" : "";
            logString = agentiCesta.Text + "; " + comboBoxRobustType.SelectedItem + "; " + numericUpDown1.Value + "; " + strict + "; " + numericUpDownDelay.Value * 100 + "; ";

            //inicializace datovych struktur pro ukladani vysledku
            rowsInResults = wholeTestRepetitions * ((max - min) / (int)numericUpDownStep.Value + 1);
            double avgMakespan = 0;
            double colisionRate = 0;
            double successInstances = 0;
            realPlans = new List<Plan>[rowsInResults, repetitions];
            abstractPlans = new List<List<List<List<double>[]>>>();
            int[] solvedInstances = new int[wholeTestRepetitions];
            ContigencyRobustness.realAlt = 0;

            //nastaveni casu
            Stopwatch sw = new Stopwatch();
            int limit = 0;
            int.TryParse(textBoxMaxTime.Text, out limit);
            textBoxMaxTime.Text = limit.ToString();

            //nastaveni panelu s daty o dokoncenych vypoctech
            groupBoxDone.Visible = true;
            labelInsDone.Text = "0/" + repetitions.ToString();
            labelExpDone.Text = "0/" + experiments.ToString();
            labelTestsDone.Text = "0/" + wholeTestRepetitions.ToString();
            progressBarInstances.Maximum = repetitions;
            progressBarExperiments.Maximum = experiments;
            progressBarTests.Maximum = wholeTestRepetitions;

            //zacina vypocet
            buttonBenchmarkRun.Enabled = false;
            buttonStopRunning.Visible = true;
            //pro vsechna opakovani testu    
            for (int t = 0; t < wholeTestRepetitions; t++)
            {
                //restart mereni casu, je-li vyzadovano
                if (limit > 0)
                {
                    sw.Restart();
                }

                //pro vsechny pocty agentu
                for (int i = 0; i < max - min + 1; i += (int)numericUpDownStep.Value)
                {
                    //kazdy test ma jiny seed, v ramci testu pridavame agenty vzdy do stejnych instanci
                    IntGenerator.GetInstance().Reseed(t * 10);
                    int solvedRepetitions = 0;
                    avgMakespan = 0;
                    colisionRate = 0;
                    successInstances = 0;
                    abstractPlans.Add(new List<List<List<double>[]>>());
                    //pro vsechny instance pri danem poctu agentu
                    for (int k = 0; k < repetitions; k++)
                    {
                        //pokud bylo pomoci tlacitka Zastavit pozadovano ukonceni testu, tak skonci
                        if (stopRunning)
                        {
                            Cursor = Cursors.Default;
                            buttonBenchmarkRun.Enabled = true;
                            buttonStopRunning.Visible = false;
                            return;
                        }
                        model.DeleteAgents();
                        //nahodny vyber agentu
                        var numbers = mainForm.GetShuffleList(agentsPositions.Count, IntGenerator.GetInstance(), (repetitions > 1 || wholeTestRepetitions > 1));
                        for (int l = 0; l < min + i; l++)
                        {
                            var a = new Agent(agentsPositions[numbers[l]][0], agentsPositions[numbers[l]][1], agentsPositions[numbers[l]][2], agentsPositions[numbers[l]][3], l);
                            model.LoadAgent(a);
                        }
                        int solLen = 0;
                        //update poctu vyresenych instanci
                        UIupdate(t, i / (int)numericUpDownStep.Value, k);
                        //najdeme plan v separatnim vlakne
                        await Task.Run(() =>
                        {
                            solLen = model.FindSolution(param1, s, param2, param3, checkBoxStrict.Checked);
                        });
                    
                        //kontrola casu - pokud byl prekrocen limit, tento plan jiz do nej nezapocitam
                        if (limit > 0 && sw.ElapsedMilliseconds > limit)
                        {
                            sw.Stop();
                            break;
                        }
                        solvedRepetitions++;
                        int ii = (rowsInResults / wholeTestRepetitions) * t + i / (int)numericUpDownStep.Value;
                        //ulozeni nalezeneho planu
                        realPlans[ii, k] = model.solution;
                        abstractPlans[ii].Add(new List<List<double>[]>());
                        //pokud plan nebyl nalezen, exekuce neprovadime
                        if (solLen == -1)
                        {
                            successInstances += 1;
                            continue;
                        }
                        numberOfAlternatives += ContigencyRobustness.realAlt;

                        //pro vsechny exekuce konkretniho planu
                        for (int j = 0; j < repetitionsExec; j++)
                        {
                            int length;
                            string message;
                            var x = model.ExecuteSolution(delay, out length, out message);

                            abstractPlans[ii][k].Add(x);
                            avgMakespan += length;
                            if (message == "Exekuce proběhla bez kolizí.") { }
                            else
                            {
                                colisionRate += 1;
                            }
                        }

                    }//konec opakovani pro stejny pocet agentu

                    solvedInstances[t] += solvedRepetitions;

                    //kontrola casu - pokud vyprsel, vypiseme, ze zbyvajici pocty agentu uz se nestihnou
                    if (limit > 0 && sw.ElapsedMilliseconds > limit)
                    {
                        //pokud ma nasledovat dalsi test, vypiseme, ze se nestihly vsechny ostatni pocty agentu v ramci tohoto testu
                        if (t < wholeTestRepetitions - 1)
                        {
                            for (int j = i; j < max - min + 1; j += (int)numericUpDownStep.Value)
                            {
                                abstractPlans.Add(new List<List<List<double>[]>>());
                                listBoxResults.Items.Add("-!-Počet agentů: " + (j + min) + "; nedokončeno - vypršel časový limit"+ ";  vyřešených instancí: " + solvedRepetitions + "/" + (repetitions));
                            }
                        }
                        //pokud je test posledni, vypiseme aktualni pocet agentu a skoncime
                        else
                            listBoxResults.Items.Add("-!-Počet agentů: " + (i + min) + "; nedokončeno - vypršel časový limit"+ ";  vyřešených instancí: " + solvedRepetitions + "/" + (repetitions));

                        break;
                    }
                    //vypocet dulezitych udaju, vypis vysledku do programu
                    successInstances = repetitions - successInstances;
                    //textBoxLog.AppendText("Alternativ: " + (numberOfAlternatives / (successInstances * repetitionsExec)).ToString() + "\n");
                    avgMakespan = avgMakespan / (successInstances * repetitionsExec);
                    colisionRate = 1 - colisionRate / (successInstances * repetitionsExec);
                    string str = "";
                    if (successInstances > 0)
                    {
                        str = "Počet agentů: " + (i + min) + ";  makespan: " + Math.Round(avgMakespan, 2) + ";  bezkolizní: " +
                            Math.Round(colisionRate, 2) + ";  vyřešených instancí: " + successInstances + "/" + (repetitions);

                    }
                    else
                    {
                        str = "Počet agentů: " + (i + min) + "; řešení nenalezeno.";
                    }
                    listBoxResults.Items.Add(str);
                }
                UIupdate(t, (max - min) / (int)numericUpDownStep.Value + 1, repetitions);
                //konec jednoho kompletniho testu

            }
            UIupdate(wholeTestRepetitions, (max - min) / (int)numericUpDownStep.Value + 1, repetitions);
            //konec vsech testu

            Array.Sort(solvedInstances);
            textBoxLog.AppendText("Medián vyřešených instancí v časovém limitu testu: "+ solvedInstances[(wholeTestRepetitions-1)/2].ToString()+"\n");
            //vysledek pripravim do schranky pro kopirovani a vypisu do logu
            ResultsToClipboard();
            buttonBenchmarkRun.Enabled = true;
            buttonStopRunning.Visible = false;
        }

        /// <summary>
        /// Aktualizuje udaje o prubehu testu v GUI.
        /// </summary>
        private void UIupdate(int t, int i, int k)
        {
            originalContext.Post(new SendOrPostCallback(o =>
            {
                progressBarInstances.Value = k;
                progressBarExperiments.Value = i;
                progressBarTests.Value = t;
                labelInsDone.Text = (k)+"/" + repetitions.ToString();
                labelExpDone.Text = (i)+"/" + experiments.ToString();
                labelTestsDone.Text = (t)+"/" + wholeTestRepetitions.ToString();
            }), null);
        }

        /// <summary>
        /// Kliknuti na dilci vysledek testu.
        /// Zobrazi se seznam vsech instanci v ramci testu a prislusneho poctu agentu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxResults_MouseClick(object sender, MouseEventArgs e)
        {
            int n = this.listBoxResults.IndexFromPoint(e.Location);
            if (n == ListBox.NoMatches)
            {
                return;
            }
            currentInstance = n;
            listBoxSimulations.Items.Clear();
            listBoxExecutions.Items.Clear();
            listBoxPlans.Items.Clear();
            for (int i = 0; i < repetitions; i++)
            {
                listBoxSimulations.Items.Add("Instance " + i);
            }
            buttonVisualize.Visible = false;
        }

        /// <summary>
        /// Poklepani na instanci v ramci drive vybraneho dilciho testu.
        /// Zobrazi se nalezeny plan pro tuto instanci.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxSimulations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int trial = this.listBoxSimulations.IndexFromPoint(e.Location);
            if (trial == ListBox.NoMatches)
            {
                return;
            }
            listBoxPlans.Items.Clear();
            int i = 0;
            if (realPlans[currentInstance, trial] == null)
            {
                listBoxPlans.Items.Add("Řešení nenalezeno.");
                return;
            }
            foreach (var path in realPlans[currentInstance, trial])
            {
              listBoxPlans.Items.Add("Agent " + i + ": " + path.ToString());
              i++;
            }

        }

        /// <summary>
        /// Kliknuti na instanci v ramci drive vybraneho dilciho testu. 
        /// Provede se nacteni vsech provedenych exekuci v ramci teto instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxSimulations_MouseClick(object sender, MouseEventArgs e)
        {
            int n = this.listBoxSimulations.IndexFromPoint(e.Location);
            if (n == ListBox.NoMatches)
            {
                return;
            }
            currentPlan = n;
            listBoxExecutions.Items.Clear();
            if (realPlans[currentInstance, n] == null)
            {
                listBoxExecutions.Items.Add("Exekuce neproběhly, protože plán nebyl nalezen.");
                return;
            }
            for (int i = 0; i < numberOfExecutions; i++)
            {
                listBoxExecutions.Items.Add("Exekuce " + i);
            }
            buttonVisualize.Visible = false;
        }

        private void listBoxExecutions_MouseClick(object sender, MouseEventArgs e)
        {
            int execution = this.listBoxExecutions.IndexFromPoint(e.Location);
            if (execution == ListBox.NoMatches)
            {
                return;
            }
            if (listBoxExecutions.Items[execution].ToString() == "Exekuce neproběhly, protože plán nebyl nalezen.")
            {
                return;
            }
            buttonVisualize.Visible = true;
            markedExecution = execution;
        }
        /// <summary>
        /// Poklepani na konkretni exekuce v ramci instance a dilciho testu.
        /// Vygeneruje okno s grafickou podobou exekuce planu s vyznacenymi kolizemi.
        /// </summary>
        private void listBoxExecutions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int execution = this.listBoxExecutions.IndexFromPoint(e.Location);
            if (execution == ListBox.NoMatches)
            {
                return;
            }
            if (listBoxExecutions.Items[execution].ToString() == "Exekuce neproběhly, protože plán nebyl nalezen.")
            {
                return;
            }
            //zobrazeni exekuce graficky
            PlanTable pt = new PlanTable();
            pt.Show();
            pt.ViewTable(abstractPlans[currentInstance][currentPlan][execution], realPlans[currentInstance, currentPlan], rtGlobal==RobustnessType.min_max);
        }

        /// <summary>
        /// Vyrovani hodnot na posuvnicich v pripade min/max robustnosti - zmena na prvnim z nich.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > numericUpDown2.Value)
            {
                numericUpDown1.Value = numericUpDown2.Value;
            }
        }

        /// <summary>
        /// Metoda vyvolana po zmene typu robustniho planu pomoci polozky v comboBoxu.
        /// Na zaklade vybrane robustni metody zobrazi dalsi parametry k nastaveni.
        /// </summary>
        private void comboBoxRobustType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RobustnessType rt = (RobustnessType)comboBoxRobustType.SelectedIndex;
            //pro min/max robustnost potrebujeme 2 parametry, pro ostatni staci 1
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
            //pro contingency robustnost mame hlavni a alternativni cesty, pro ostatni je plan jen jeden
            if (rt == RobustnessType.semi_k || rt == RobustnessType.alternative_k)
            {
                labelSolver.Text = "Řešič pro nalezení hlavního plánu";
                checkBoxStrict.Visible = rt == RobustnessType.alternative_k;
            }
            else
            {
                labelSolver.Text = "Řešič pro nalezení plánu";
                checkBoxStrict.Visible = false;
            }
        }
        /// <summary>
        /// Zkopirovani vysledku do schranky a zobrazeni v logu.
        /// </summary>
        private void ResultsToClipboard()
        {
            StringBuilder logBuffer = new StringBuilder();
            StringBuilder clipboardBuffer = new StringBuilder();
            foreach (object item in listBoxResults.Items)
            {
                //do logu a do schranky pridame jen ty radky, ktere reprezentuji dokoncene instance
                if (!item.ToString().StartsWith("-!-"))
                {
                    logBuffer.AppendLine(logString + item.ToString());
                    clipboardBuffer.AppendLine(item.ToString());
                }

            }

            if (logBuffer.Length > 0)
            {
                Clipboard.SetText(clipboardBuffer.ToString());
                textBoxLog.AppendText(logBuffer.ToString());
            }
        }

        /// <summary>
        /// Pri zavreni okna zavreme celou aplikaci.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Po stisknuti tlacitka zastavit dojde ke zmene vzhledu kurzoru a pokynu k ukonceni provadeneho benchmarku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStopRunning_Click(object sender, EventArgs e)
        {
            stopRunning = true;
            Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Skryti textoveho pole s vypisem hodnot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLog.Visible = !checkBox1.Checked;
            labelLog.Visible = !checkBox1.Checked;
        }

        private void buttonVisualize_Click(object sender, EventArgs e)
        {
            mainForm.VisualizeFromBenchmark(realPlans[currentInstance, currentPlan],
                abstractPlans[currentInstance][currentPlan][markedExecution], model.graph, model.typeOfSol, model.solParameter1, model.solParameter2);
            MessageBox.Show("Vybraná MAPF instance byla nahrána do hlavního okna programu.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
