using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida s oknem zobrazujici konkretni exekuci planu v ramci okna s benchmarky.
    /// </summary>
    partial class PlanTable : Form
    {
        int cellH = 40;
        int cellW = 70;
        int margin = 10;
        bool minMax = false;

        public PlanTable()
        {
            InitializeComponent();
            this.AutoScroll = true;
            this.WindowState = FormWindowState.Maximized;
        }

        private int getMaxLength(List<double>[] array)
        {
            return array.Max(l => l.Count);
        }
        private string AddEdge(double edgePart)
        {
            return minMax ? "+"+edgePart.ToString() : "";
        }

        /// <summary>
        /// Zobrazi v okne tabulku s exekuci planu s barevne oznacenymi vrcholy.
        /// </summary>
        public void ViewTable(List<double>[] vertices, List<Plan> plans, bool minMax )
        {
            this.minMax = minMax;
            cellW = minMax ? 70 : 50;
            PictureBox pictureBox1 = new PictureBox();
            this.Controls.Add(pictureBox1);
            int agents = plans.Count;
            int length = getMaxLength(vertices);
            Font drawFont = new Font(this.Font.FontFamily, 8);

            pictureBox1.Size = new Size(length * cellW + margin, agents * cellH + margin);
            Bitmap planDrawing = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(planDrawing);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            List<Vertex> last = new List<Vertex>();
            List<Vertex> current = new List<Vertex>();
            List<double> edgePart = new List<double>();
            //pro vsechny casy
            for (int i = 0; i < length; i++)
            {
                last = current;
                current = new List<Vertex>();
                edgePart = new List<double>();
                //pro vsechny agenty
                for (int j = 0; j < agents; j++)
                {
                    //nyni nakreslime i-ty vrchol, j-teho agenta, zbyva rozhodnout, jakou bude barvou
                    Vertex v = plans[j].GetNth(vertices[j][i]);
                    double edge = Math.Round(DoubleToInt.DecimalPart(vertices[j][i]), 2);
                    edgePart.Add(edge);
                    //cervena barva = vrcholovy konflikt
                    Color c = Color.Red;
                    //pokud nejde pridat, uz tam takovy vrchol je, nastal vrcholovy konflikt, vrchol bude cervenou barvou
                    if (!current.Contains(v))
                    {
                        //vrcholovy konflikt neni - takze barva bude jina nez cervena
                        if (CheckSwappingConflict(last, j, current,v))
                        {
                            //oranzova = konflikt vzajemne vymeny vrcholu
                            c = Color.Orange;
                        }
                        else
                        {
                            //zelena = hlavni cesta, bez kolize ve vrcholu
                            c = Color.Green;
                        }
                    }
                    //current obsahuje vrchol
                    else
                    {
                        //v pripade min/max robustnosti musime konflikty pocitat trochu jinak
                        //v tomto pripade kontrolujeme jen vrcholove konflikty
                        if (minMax)
                        {
                            if (!CheckMinMaxVertexConflict(current, v, edge, edgePart))
                            {
                                c = Color.Green;
                            }
                        }
                    }
                    current.Add(v);
                    Rectangle r = new Rectangle(i * cellW + margin, j * cellH + margin, cellW, cellH);
                    string text = v.ToString() + AddEdge(edge);
                    g.DrawString(text, drawFont, new SolidBrush(c), r);
                }
            }
            pictureBox1.Image = planDrawing;
        }
        private bool CheckMinMaxVertexConflict(List<Vertex> current, Vertex v, double edge, List<double> edgePart)
        {
            for (int i = 0; i < current.Count; i++)
            {
                if (current[i] == v && edge == edgePart[i])
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckSwappingConflict(List<Vertex> last, int id, List<Vertex> current, Vertex v)
        {
            for (int i = 0; i < last.Count; i++)
            {
                //pokud vrchol agenta id v case t odpovida vrcholu agenta i v case t-1 
                if (i != id && v == last[i])
                {
                    //pokud uz znam soucasnou pozici agenta i (v case t)
                    if (current.Count > i)
                    {
                        //pokud se shoduje i naopak, mam konflikt vzajemne vymeny vrcholu
                        if (current[i] == last[id])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
