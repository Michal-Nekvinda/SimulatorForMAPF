using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida poskytujici grafickou reprezentaci dat o MAPF problemu.
    /// </summary>
    class MapfView
    {
        private MapfModel model;
        int gWidth;
        int gHeight;
        int vertexSize;
        int betweenVertices;
        int margin = 6;

        //souradnice polohy jednotlivych agentu
        public List<Rectangle> currentAgentsPositions { get; }

        /// <summary>
        /// Vytvori novy pohled pro data ulozena v MAPF modelu model
        /// </summary>
        /// <param name="model"></param>
        public MapfView(MapfModel model)
        {
            this.model = model;
            currentAgentsPositions = new List<Rectangle>();
        }

        /// <summary>
        /// Nakresli vybrany graf na panel v uvodnim okne programu.
        /// </summary>
        public PictureBox VisualizeGraph(Panel panel1)
        {
            gWidth = model.graph.width;
            gHeight = model.graph.height;
            //delka hrany
            int edgeLength = 2;
            //kolik vrcholu se musi vejit do obrazku na vysku resp. na sirku
            int h = (panel1.Height-2* margin) / ((edgeLength+1) * gHeight - edgeLength);
            int w = (panel1.Width - 2 * margin) / ((edgeLength+1) * gWidth - edgeLength);

            //vypocet optimalni velikosti vrcholu pro zobrazeni v panelu
            vertexSize = Math.Max(Math.Min(h,w), 8);
            betweenVertices = (edgeLength+1) * vertexSize;

            PictureBox pictureBox1 = new PictureBox();
            pictureBox1.Size = new Size(2 * margin + betweenVertices * gWidth - edgeLength * vertexSize + 60, 2 * margin + betweenVertices * gHeight - edgeLength * vertexSize);
            panel1.Controls.Clear();
            panel1.Controls.Add(pictureBox1);
            Bitmap loadedMap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(loadedMap);

            //vrcholy zelene, prekazky cervene, hrany cerne
            Pen normal = new Pen(Color.Green, 1);
            Pen obstacle = new Pen(Color.Red, 1);
            Pen edge = new Pen(Color.Black, 1);

            for (int j = 0; j < gHeight; j++)
            {
                for (int i = 0; i < gWidth; i++)
                {
                    Rectangle rect = new Rectangle(i * betweenVertices + margin, j * betweenVertices + margin, vertexSize, vertexSize);
                    if (model.graph.IsVertex(i, j))
                    {
                        g.DrawEllipse(normal, rect);

                        float centre = vertexSize / 2;
                        //neni prekazka vpravo
                        if (i != gWidth - 1 && !model.graph.IsObstacle(i +1, j))
                        {
                            float endX = (i + 1) * betweenVertices + margin;
                            float endY = j * betweenVertices + centre + margin;
                            g.DrawLine(edge, i * betweenVertices + vertexSize + margin, j * betweenVertices + centre + margin, endX, endY);
                        }
                        //neni prekazka dole
                        if (j != gHeight - 1 && !model.graph.IsObstacle(i, j +1))
                        {
                            g.DrawLine(edge, i * betweenVertices + centre + margin, j * betweenVertices + vertexSize + margin, i * betweenVertices + centre + margin, (j + 1) * betweenVertices + margin);

                        }
                    }
                    else
                    {
                        g.DrawEllipse(obstacle, rect);
                    }
                }
            }
            pictureBox1.Image = loadedMap;
            return pictureBox1;
        }

        /// <summary>
        /// Vraci pozici agenta k vykresleni na obrazovku na zaklade vrcholu grafu.
        /// </summary>
        private Rectangle VertexToRealPosition(Vertex v, int shift = 0)
        {
            return new Rectangle(v.x * betweenVertices + margin - shift, v.y * betweenVertices + margin - shift, vertexSize + 2 * shift, vertexSize + 2 * shift);
        }

        /// <summary>
        /// Vraci pozice cilovych vrcholu vsech agentu k vykreselni na obrazku (vykresleny jako barevne ctverce okolo vrcholu).
        /// </summary>
        public Rectangle[] GetTargets()
        {
            Rectangle[] rect = new Rectangle[model.agents.Count];
            for (int i = 0; i < rect.Length; i++)
            {
                rect[i] = VertexToRealPosition(model.agents[i].target,margin-3);
            }
            return rect;
        }

        /// <summary>
        /// Na zaklade abstraktnich pozic (udaju z exekuce planu) spocita pozice agentu k vykresleni na obrazovku v case timestep. 
        /// </summary>
        public void ComputeAgentPositions(int timeStep, double[] abstractPositionsInTime)
        {
            currentAgentsPositions.Clear();
            //pro agenty na startu zname dokonce konkretni vrcholy, tedy abstraktni nepotrebujeme
            if (timeStep==0)
            {
                for (int i = 0; i < model.agents.Count; i++)
                {
                    Rectangle r = VertexToRealPosition(model.agents[i].start);
                    currentAgentsPositions.Add(r);
                }
                return;
            }
            if (abstractPositionsInTime == null)
            {
                return;
            }
            //jinak prevedeme abstraktni pozice na vrcholy a vrcholy na souradnice k vykresleni na obrazovku
            for (int i = 0; i < abstractPositionsInTime.Length; i++)
            {
                int number = DoubleToInt.ToInt(abstractPositionsInTime[i]);
                double part = DoubleToInt.DecimalPart(abstractPositionsInTime[i]);
                Vertex v = model.solution[i].GetNth(number);
                Vertex vNext = model.solution[i].GetNth(number + 1);
                Vertex move = new Vertex(vNext.x - v.x, vNext.y - v.y);
                Rectangle r = VertexToRealPosition(v);
                //min/max robustnost - zde se agenti mohou nachazet mezi vrcholy
                if (part > 0)
                {
                    double minPart = 1.0 / model.solParameter2;
                    double maxPart = 1 - minPart;
                    double currentPart = minPart;
                    if (Math.Abs(minPart-maxPart)>DoubleToInt.epsilon)
                    {
                        currentPart = (part - minPart) / (maxPart - minPart);
                    }
                    r.X += (int)(move.x * (vertexSize + currentPart*vertexSize));
                    r.Y += (int)(move.y * (vertexSize + currentPart*vertexSize));

                }
                currentAgentsPositions.Add(r);
            }

        }

        /// <summary>
        /// Vypise plany agentu do prislusneho listboxu l.
        /// Na kazdy plan vola metodu ToString().
        /// </summary>
        public void ViewPlans(ListBox l)
        {
            l.Items.Clear();
            int i = 0;
            if (model.solution==null)
            {
                l.Items.Add("Řešení nenalezeno!");
                return;
            }
            foreach (var path in model.solution)
            {
                l.Items.Add("Agent " + i + ": " + path.ToString());
                i++;
            }
        }

        /// <summary>
        /// Do listboxu vypise posloupnost vrcholu, kterou agenti v ramci svych planu projeli behem exekuce.
        /// Oproti nalezenemu planu navic vstupuje do hry zpozdeni.
        /// </summary>
        public void ViewExecution(ListBox l, List<double>[] abstractPositions)
        {
            l.Items.Clear();
            if (abstractPositions == null || !model.HasSolution())
            {
                l.Items.Add("Řešení nenalezeno!");
                return;
            }
            int i = 0;
            foreach (var abstractPath in abstractPositions)
            {
                List<Vertex> v = new List<Vertex>();
                foreach (var vertex in abstractPath)
                {
                    v.Add(model.solution[i].GetNth(vertex));
                }
                Plan path = new Plan(v);
                l.Items.Add("Agent " + i + ": " + path.ToString());
                i++;
            }
        }
        
        /// <summary>
        /// Na zaklade id agenta mu priradi urcitou barvu, kterou je vykreslovan v grafu.
        /// Podporuje az 104 unikatnich barev, dale uz se opakuje.
        /// </summary>
        public Color GetColor(int agentID)
        {
            return SetOfColors.GetColor(agentID, model.agents.Count);
        }
    }
}
