namespace MAPFsimulator
{
    /// <summary>
    /// Main class designer.
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.souborToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.agentsPositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spustitBenchmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nastaveníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxGraph = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.labelTimeStep = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.vymazAgenty = new System.Windows.Forms.Button();
            this.listBoxAgenti = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxVlozAg = new System.Windows.Forms.TextBox();
            this.vlozAgenta = new System.Windows.Forms.Button();
            this.labelMakespan = new System.Windows.Forms.Label();
            this.groupBoxAgentAdding = new System.Windows.Forms.GroupBox();
            this.groupBoxGraphDraw = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxStartAgents = new System.Windows.Forms.CheckBox();
            this.groupBoxSolverPicker = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxRobustType = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelMax = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.labelRobustness = new System.Windows.Forms.Label();
            this.groupBoxSolver = new System.Windows.Forms.GroupBox();
            this.radioButtonPicat = new System.Windows.Forms.RadioButton();
            this.radioButtonCBS = new System.Windows.Forms.RadioButton();
            this.GroupBoxDelay = new System.Windows.Forms.GroupBox();
            this.buttonNextExec = new System.Windows.Forms.Button();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.buttonFindSol = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxSmart = new System.Windows.Forms.CheckBox();
            this.checkBoxStrict = new System.Windows.Forms.CheckBox();
            this.groupBoxSimulation = new System.Windows.Forms.GroupBox();
            this.radioButtonRealMake = new System.Windows.Forms.RadioButton();
            this.radioButtonPlanMake = new System.Windows.Forms.RadioButton();
            this.labelRealMakespan = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxAgentAdding.SuspendLayout();
            this.groupBoxGraphDraw.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBoxSolverPicker.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBoxSolver.SuspendLayout();
            this.GroupBoxDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBoxSimulation.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(8);
            this.panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panel1.Size = new System.Drawing.Size(1226, 766);
            this.panel1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.souborToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1904, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // souborToolStripMenuItem
            // 
            this.souborToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGraphToolStripMenuItem,
            this.agentsPositionsToolStripMenuItem,
            this.spustitBenchmarkToolStripMenuItem,
            this.nastaveníToolStripMenuItem});
            this.souborToolStripMenuItem.Name = "souborToolStripMenuItem";
            this.souborToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.souborToolStripMenuItem.Text = "Soubor";
            // 
            // loadGraphToolStripMenuItem
            // 
            this.loadGraphToolStripMenuItem.Name = "loadGraphToolStripMenuItem";
            this.loadGraphToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadGraphToolStripMenuItem.Text = "Načíst graf";
            this.loadGraphToolStripMenuItem.Click += new System.EventHandler(this.LoadGraphToolStripMenuItem_Click);
            // 
            // agentsPositionsToolStripMenuItem
            // 
            this.agentsPositionsToolStripMenuItem.Name = "agentsPositionsToolStripMenuItem";
            this.agentsPositionsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.agentsPositionsToolStripMenuItem.Text = "Načíst pozice agentů";
            this.agentsPositionsToolStripMenuItem.Click += new System.EventHandler(this.agentsPositionsToolStripMenuItem_Click);
            // 
            // spustitBenchmarkToolStripMenuItem
            // 
            this.spustitBenchmarkToolStripMenuItem.Name = "spustitBenchmarkToolStripMenuItem";
            this.spustitBenchmarkToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.spustitBenchmarkToolStripMenuItem.Text = "Spustit benchmark";
            this.spustitBenchmarkToolStripMenuItem.Click += new System.EventHandler(this.spustitBenchmarkToolStripMenuItem_Click);
            // 
            // nastaveníToolStripMenuItem
            // 
            this.nastaveníToolStripMenuItem.Name = "nastaveníToolStripMenuItem";
            this.nastaveníToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.nastaveníToolStripMenuItem.Text = "Nastavení";
            this.nastaveníToolStripMenuItem.Click += new System.EventHandler(this.nastaveníToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(9, 22);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(1230, 783);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Graf";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(239, 76);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 32);
            this.button3.TabIndex = 2;
            this.button3.Text = "Zobrazit graf";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBoxGraph
            // 
            this.textBoxGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxGraph.Location = new System.Drawing.Point(4, 39);
            this.textBoxGraph.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxGraph.Multiline = true;
            this.textBoxGraph.Name = "textBoxGraph";
            this.textBoxGraph.Size = new System.Drawing.Size(526, 30);
            this.textBoxGraph.TabIndex = 1;
            this.textBoxGraph.Text = "GGGG,GGGG,GGGG,GGGG";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Graf";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.LargeChange = 1;
            this.hScrollBar1.Location = new System.Drawing.Point(7, 14);
            this.hScrollBar1.Maximum = 10;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(346, 21);
            this.hScrollBar1.TabIndex = 16;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // labelTimeStep
            // 
            this.labelTimeStep.AutoSize = true;
            this.labelTimeStep.Location = new System.Drawing.Point(355, 18);
            this.labelTimeStep.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTimeStep.Name = "labelTimeStep";
            this.labelTimeStep.Size = new System.Drawing.Size(13, 13);
            this.labelTimeStep.TabIndex = 20;
            this.labelTimeStep.Text = "0";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(433, 10);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 29);
            this.button2.TabIndex = 1;
            this.button2.Text = "Simulovat plán";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // vymazAgenty
            // 
            this.vymazAgenty.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.vymazAgenty.Location = new System.Drawing.Point(68, 367);
            this.vymazAgenty.Margin = new System.Windows.Forms.Padding(2);
            this.vymazAgenty.Name = "vymazAgenty";
            this.vymazAgenty.Size = new System.Drawing.Size(101, 27);
            this.vymazAgenty.TabIndex = 1;
            this.vymazAgenty.Text = "Vymazat agenty";
            this.vymazAgenty.UseVisualStyleBackColor = true;
            this.vymazAgenty.Click += new System.EventHandler(this.vymazAgenty_Click);
            // 
            // listBoxAgenti
            // 
            this.listBoxAgenti.FormattingEnabled = true;
            this.listBoxAgenti.Location = new System.Drawing.Point(4, 19);
            this.listBoxAgenti.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxAgenti.Name = "listBoxAgenti";
            this.listBoxAgenti.Size = new System.Drawing.Size(228, 342);
            this.listBoxAgenti.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 23);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Pozice nového agenta";
            // 
            // textBoxVlozAg
            // 
            this.textBoxVlozAg.Location = new System.Drawing.Point(7, 40);
            this.textBoxVlozAg.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxVlozAg.Name = "textBoxVlozAg";
            this.textBoxVlozAg.Size = new System.Drawing.Size(110, 20);
            this.textBoxVlozAg.TabIndex = 1;
            // 
            // vlozAgenta
            // 
            this.vlozAgenta.Location = new System.Drawing.Point(31, 83);
            this.vlozAgenta.Margin = new System.Windows.Forms.Padding(2);
            this.vlozAgenta.Name = "vlozAgenta";
            this.vlozAgenta.Size = new System.Drawing.Size(58, 24);
            this.vlozAgenta.TabIndex = 2;
            this.vlozAgenta.Text = "Vložit";
            this.vlozAgenta.UseVisualStyleBackColor = true;
            this.vlozAgenta.Click += new System.EventHandler(this.vlozAgenta_Click);
            // 
            // labelMakespan
            // 
            this.labelMakespan.AutoSize = true;
            this.labelMakespan.Location = new System.Drawing.Point(832, 18);
            this.labelMakespan.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMakespan.Name = "labelMakespan";
            this.labelMakespan.Size = new System.Drawing.Size(13, 13);
            this.labelMakespan.TabIndex = 29;
            this.labelMakespan.Text = "0";
            // 
            // groupBoxAgentAdding
            // 
            this.groupBoxAgentAdding.Controls.Add(this.checkBoxSmart);
            this.groupBoxAgentAdding.Controls.Add(this.label5);
            this.groupBoxAgentAdding.Controls.Add(this.textBoxVlozAg);
            this.groupBoxAgentAdding.Controls.Add(this.vlozAgenta);
            this.groupBoxAgentAdding.Location = new System.Drawing.Point(2, 2);
            this.groupBoxAgentAdding.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxAgentAdding.Name = "groupBoxAgentAdding";
            this.groupBoxAgentAdding.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxAgentAdding.Size = new System.Drawing.Size(128, 112);
            this.groupBoxAgentAdding.TabIndex = 0;
            this.groupBoxAgentAdding.TabStop = false;
            this.groupBoxAgentAdding.Text = "Přidání agenta";
            // 
            // groupBoxGraphDraw
            // 
            this.groupBoxGraphDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGraphDraw.Controls.Add(this.label2);
            this.groupBoxGraphDraw.Controls.Add(this.textBoxGraph);
            this.groupBoxGraphDraw.Controls.Add(this.button3);
            this.groupBoxGraphDraw.Location = new System.Drawing.Point(2, 2);
            this.groupBoxGraphDraw.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxGraphDraw.Name = "groupBoxGraphDraw";
            this.groupBoxGraphDraw.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxGraphDraw.Size = new System.Drawing.Size(535, 115);
            this.groupBoxGraphDraw.TabIndex = 0;
            this.groupBoxGraphDraw.TabStop = false;
            this.groupBoxGraphDraw.Text = "Ruční vložení grafu";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.listBoxAgenti);
            this.groupBox5.Controls.Add(this.checkBoxStartAgents);
            this.groupBox5.Controls.Add(this.vymazAgenty);
            this.groupBox5.Location = new System.Drawing.Point(298, 2);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(235, 430);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Pozice agentů v grafu";
            // 
            // checkBoxStartAgents
            // 
            this.checkBoxStartAgents.AutoSize = true;
            this.checkBoxStartAgents.Location = new System.Drawing.Point(3, 406);
            this.checkBoxStartAgents.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStartAgents.Name = "checkBoxStartAgents";
            this.checkBoxStartAgents.Size = new System.Drawing.Size(135, 17);
            this.checkBoxStartAgents.TabIndex = 2;
            this.checkBoxStartAgents.Text = "Zobrazit agenty v grafu";
            this.checkBoxStartAgents.UseVisualStyleBackColor = true;
            this.checkBoxStartAgents.CheckedChanged += new System.EventHandler(this.checkBoxStartAgents_CheckedChanged);
            this.checkBoxStartAgents.VisibleChanged += new System.EventHandler(this.checkBoxStartAgents_VisibleChanged);
            // 
            // groupBoxSolverPicker
            // 
            this.groupBoxSolverPicker.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxSolverPicker.Location = new System.Drawing.Point(2, 124);
            this.groupBoxSolverPicker.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxSolverPicker.Name = "groupBoxSolverPicker";
            this.groupBoxSolverPicker.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxSolverPicker.Size = new System.Drawing.Size(239, 300);
            this.groupBoxSolverPicker.TabIndex = 1;
            this.groupBoxSolverPicker.TabStop = false;
            this.groupBoxSolverPicker.Text = "Typ robustnosti";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 228F));
            this.tableLayoutPanel1.Controls.Add(this.comboBoxRobustType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxSolver, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.GroupBoxDelay, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindSol, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 17);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(228, 278);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // comboBoxRobustType
            // 
            this.comboBoxRobustType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRobustType.Items.AddRange(new object[] {
            "k-robustnost",
            "min/max robustnost",
            "alternativní k-robustnost",
            "semi k-robustnost"});
            this.comboBoxRobustType.Location = new System.Drawing.Point(2, 8);
            this.comboBoxRobustType.Margin = new System.Windows.Forms.Padding(2, 8, 2, 2);
            this.comboBoxRobustType.Name = "comboBoxRobustType";
            this.comboBoxRobustType.Size = new System.Drawing.Size(166, 21);
            this.comboBoxRobustType.TabIndex = 0;
            this.comboBoxRobustType.SelectedIndexChanged += new System.EventHandler(this.comboBoxRobustType_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.labelMax);
            this.groupBox3.Controls.Add(this.numericUpDown2);
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Controls.Add(this.labelRobustness);
            this.groupBox3.Location = new System.Drawing.Point(2, 39);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(224, 59);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Parametry";
            // 
            // labelMax
            // 
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(4, 37);
            this.labelMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(26, 13);
            this.labelMax.TabIndex = 2;
            this.labelMax.Text = "max";
            this.labelMax.Visible = false;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(114, 36);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(51, 20);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Visible = false;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(114, 13);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(51, 20);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // labelRobustness
            // 
            this.labelRobustness.AutoSize = true;
            this.labelRobustness.Location = new System.Drawing.Point(4, 15);
            this.labelRobustness.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRobustness.Name = "labelRobustness";
            this.labelRobustness.Size = new System.Drawing.Size(13, 13);
            this.labelRobustness.TabIndex = 0;
            this.labelRobustness.Text = "k";
            // 
            // groupBoxSolver
            // 
            this.groupBoxSolver.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSolver.Controls.Add(this.radioButtonPicat);
            this.groupBoxSolver.Controls.Add(this.radioButtonCBS);
            this.groupBoxSolver.Location = new System.Drawing.Point(2, 102);
            this.groupBoxSolver.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxSolver.Name = "groupBoxSolver";
            this.groupBoxSolver.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxSolver.Size = new System.Drawing.Size(224, 54);
            this.groupBoxSolver.TabIndex = 2;
            this.groupBoxSolver.TabStop = false;
            this.groupBoxSolver.Text = "Řešič pro nalezení plánu";
            // 
            // radioButtonPicat
            // 
            this.radioButtonPicat.AutoSize = true;
            this.radioButtonPicat.Location = new System.Drawing.Point(126, 30);
            this.radioButtonPicat.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonPicat.Name = "radioButtonPicat";
            this.radioButtonPicat.Size = new System.Drawing.Size(49, 17);
            this.radioButtonPicat.TabIndex = 1;
            this.radioButtonPicat.Text = "Picat";
            this.radioButtonPicat.UseVisualStyleBackColor = true;
            // 
            // radioButtonCBS
            // 
            this.radioButtonCBS.AutoSize = true;
            this.radioButtonCBS.Checked = true;
            this.radioButtonCBS.Location = new System.Drawing.Point(4, 30);
            this.radioButtonCBS.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonCBS.Name = "radioButtonCBS";
            this.radioButtonCBS.Size = new System.Drawing.Size(46, 17);
            this.radioButtonCBS.TabIndex = 0;
            this.radioButtonCBS.TabStop = true;
            this.radioButtonCBS.Text = "CBS";
            this.radioButtonCBS.UseVisualStyleBackColor = true;
            // 
            // GroupBoxDelay
            // 
            this.GroupBoxDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxDelay.Controls.Add(this.buttonNextExec);
            this.GroupBoxDelay.Controls.Add(this.numericUpDown3);
            this.GroupBoxDelay.Location = new System.Drawing.Point(2, 184);
            this.GroupBoxDelay.Margin = new System.Windows.Forms.Padding(2);
            this.GroupBoxDelay.Name = "GroupBoxDelay";
            this.GroupBoxDelay.Padding = new System.Windows.Forms.Padding(2);
            this.GroupBoxDelay.Size = new System.Drawing.Size(224, 54);
            this.GroupBoxDelay.TabIndex = 4;
            this.GroupBoxDelay.TabStop = false;
            this.GroupBoxDelay.Text = "Zpoždění agentů";
            // 
            // buttonNextExec
            // 
            this.buttonNextExec.Location = new System.Drawing.Point(114, 11);
            this.buttonNextExec.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNextExec.Name = "buttonNextExec";
            this.buttonNextExec.Size = new System.Drawing.Size(94, 30);
            this.buttonNextExec.TabIndex = 1;
            this.buttonNextExec.Text = "Provést znovu";
            this.buttonNextExec.UseVisualStyleBackColor = true;
            this.buttonNextExec.Click += new System.EventHandler(this.buttonNextExec_Click);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DecimalPlaces = 2;
            this.numericUpDown3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown3.Location = new System.Drawing.Point(4, 17);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown3.TabIndex = 0;
            // 
            // buttonFindSol
            // 
            this.buttonFindSol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buttonFindSol.Location = new System.Drawing.Point(74, 242);
            this.buttonFindSol.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFindSol.Name = "buttonFindSol";
            this.buttonFindSol.Size = new System.Drawing.Size(80, 34);
            this.buttonFindSol.TabIndex = 5;
            this.buttonFindSol.Text = "Najít řešení";
            this.buttonFindSol.UseVisualStyleBackColor = true;
            this.buttonFindSol.Click += new System.EventHandler(this.buttonFindSol_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBoxStrict);
            this.panel2.Location = new System.Drawing.Point(0, 158);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(226, 24);
            this.panel2.TabIndex = 6;
            // 
            // checkBoxSmart
            // 
            this.checkBoxSmart.AutoSize = true;
            this.checkBoxSmart.Location = new System.Drawing.Point(7, 64);
            this.checkBoxSmart.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSmart.Name = "checkBoxSmart";
            this.checkBoxSmart.Size = new System.Drawing.Size(94, 17);
            this.checkBoxSmart.TabIndex = 4;
            this.checkBoxSmart.Text = "SMART agent";
            this.checkBoxSmart.UseVisualStyleBackColor = true;
            this.checkBoxSmart.Visible = true;
            // 
            // checkBoxStrict
            // 
            this.checkBoxStrict.AutoSize = true;
            this.checkBoxStrict.Location = new System.Drawing.Point(2, 2);
            this.checkBoxStrict.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStrict.Name = "checkBoxStrict";
            this.checkBoxStrict.Size = new System.Drawing.Size(95, 17);
            this.checkBoxStrict.TabIndex = 3;
            this.checkBoxStrict.Text = "striktní přístup";
            this.checkBoxStrict.UseVisualStyleBackColor = true;
            this.checkBoxStrict.Visible = false;
            // 
            // groupBoxSimulation
            // 
            this.groupBoxSimulation.Controls.Add(this.radioButtonRealMake);
            this.groupBoxSimulation.Controls.Add(this.radioButtonPlanMake);
            this.groupBoxSimulation.Controls.Add(this.labelRealMakespan);
            this.groupBoxSimulation.Controls.Add(this.button2);
            this.groupBoxSimulation.Controls.Add(this.hScrollBar1);
            this.groupBoxSimulation.Controls.Add(this.labelTimeStep);
            this.groupBoxSimulation.Controls.Add(this.labelMakespan);
            this.groupBoxSimulation.Location = new System.Drawing.Point(2, 2);
            this.groupBoxSimulation.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxSimulation.Name = "groupBoxSimulation";
            this.groupBoxSimulation.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxSimulation.Size = new System.Drawing.Size(1224, 40);
            this.groupBoxSimulation.TabIndex = 36;
            this.groupBoxSimulation.TabStop = false;
            this.groupBoxSimulation.Text = "Zobrazit v čase";
            // 
            // radioButtonRealMake
            // 
            this.radioButtonRealMake.AutoSize = true;
            this.radioButtonRealMake.Location = new System.Drawing.Point(936, 16);
            this.radioButtonRealMake.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonRealMake.Name = "radioButtonRealMake";
            this.radioButtonRealMake.Size = new System.Drawing.Size(113, 17);
            this.radioButtonRealMake.TabIndex = 33;
            this.radioButtonRealMake.Text = "Reálný makespan:";
            this.radioButtonRealMake.UseVisualStyleBackColor = true;
            // 
            // radioButtonPlanMake
            // 
            this.radioButtonPlanMake.AutoSize = true;
            this.radioButtonPlanMake.Checked = true;
            this.radioButtonPlanMake.Location = new System.Drawing.Point(625, 16);
            this.radioButtonPlanMake.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonPlanMake.Name = "radioButtonPlanMake";
            this.radioButtonPlanMake.Size = new System.Drawing.Size(203, 17);
            this.radioButtonPlanMake.TabIndex = 32;
            this.radioButtonPlanMake.TabStop = true;
            this.radioButtonPlanMake.Text = "Plánovaný makespan (bez zpoždění):";
            this.radioButtonPlanMake.UseVisualStyleBackColor = true;
            this.radioButtonPlanMake.CheckedChanged += new System.EventHandler(this.radioButtonPlanMake_CheckedChanged);
            // 
            // labelRealMakespan
            // 
            this.labelRealMakespan.AutoSize = true;
            this.labelRealMakespan.Location = new System.Drawing.Point(1053, 18);
            this.labelRealMakespan.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRealMakespan.Name = "labelRealMakespan";
            this.labelRealMakespan.Size = new System.Drawing.Size(13, 13);
            this.labelRealMakespan.TabIndex = 31;
            this.labelRealMakespan.Text = "0";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(2, 59);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1764, 173);
            this.listBox1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 44);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Plány agentů\r\n";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 539F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBoxGraphDraw, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1243, 24);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 119F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 306F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(539, 557);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 239F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 121);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(535, 434);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.groupBoxSolverPicker, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.groupBoxAgentAdding, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.37838F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.62162F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(292, 430);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1771F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.listBox1, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.groupBoxSimulation, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(9, 803);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1771, 235);
            this.tableLayoutPanel5.TabIndex = 37;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1904, 1042);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "MAPFsimulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBoxAgentAdding.ResumeLayout(false);
            this.groupBoxAgentAdding.PerformLayout();
            this.groupBoxGraphDraw.ResumeLayout(false);
            this.groupBoxGraphDraw.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBoxSolverPicker.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBoxSolver.ResumeLayout(false);
            this.groupBoxSolver.PerformLayout();
            this.GroupBoxDelay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBoxSimulation.ResumeLayout(false);
            this.groupBoxSimulation.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem souborToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxGraph;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label labelTimeStep;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button vymazAgenty;
        private System.Windows.Forms.ListBox listBoxAgenti;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxVlozAg;
        private System.Windows.Forms.Button vlozAgenta;
        private System.Windows.Forms.ToolStripMenuItem agentsPositionsToolStripMenuItem;
        private System.Windows.Forms.Label labelMakespan;
        private System.Windows.Forms.ToolStripMenuItem spustitBenchmarkToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxAgentAdding;
        private System.Windows.Forms.GroupBox groupBoxGraphDraw;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBoxSolverPicker;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBoxRobustType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelRobustness;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBoxSolver;
        private System.Windows.Forms.RadioButton radioButtonPicat;
        private System.Windows.Forms.RadioButton radioButtonCBS;
        private System.Windows.Forms.Button buttonFindSol;
        private System.Windows.Forms.CheckBox checkBoxStrict;
        private System.Windows.Forms.GroupBox GroupBoxDelay;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.GroupBox groupBoxSimulation;
        private System.Windows.Forms.Label labelRealMakespan;
        private System.Windows.Forms.ToolStripMenuItem loadGraphToolStripMenuItem;
        private System.Windows.Forms.Button buttonNextExec;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxStartAgents;
        private System.Windows.Forms.RadioButton radioButtonRealMake;
        private System.Windows.Forms.RadioButton radioButtonPlanMake;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ToolStripMenuItem nastaveníToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBoxSmart;
    }
}

