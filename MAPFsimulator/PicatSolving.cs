using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida pro hledani reseni MAPF problemu v deklarativnim jazyce Picat.
    /// </summary>
    class PicatSolving
    {
        string tempFile;
        /// <summary>
        /// Prevede graf a pozice agentu do souboru, ktery je mozne predlozit Picat resici k vyreseni.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="picatGraph">verze grafu upravena pro Picat - vznikne volanim metody ConvertForPicat() na prislusnem grafu</param>
        /// <param name="agents"></param>
        /// <returns>true, pokud byl prevod uspesny</returns>
        private bool MapfToPicat(int r, int[,] picatGraph, List<Agent> agents)
        {
            try
            {
                tempFile = Path.GetTempFileName();
                using (StreamWriter stw = new StreamWriter(tempFile))
                {
                    //robustnost
                    if (r > 0)
                    {
                        stw.WriteLine(r);
                    }
                    //graf
                    stw.WriteLine("{");
                    for (int j = 0; j < picatGraph.GetLength(1); j++)
                    {
                        stw.Write("{");
                        for (int i = 0; i < picatGraph.GetLength(0); i++)
                        {
                            stw.Write(picatGraph[i, j]);
                            if (i < picatGraph.GetLength(0) - 1)
                            {
                                stw.Write(",");
                            }
                        }
                        stw.Write("}");
                        if (j < picatGraph.GetLength(1) - 1)
                        {
                            stw.WriteLine(",");
                        }
                    }
                    stw.WriteLine("}.");
                    //agenti
                    stw.Write("[");
                    int x = 0;
                    foreach (var ag in agents)
                    {
                        stw.Write("(" + picatGraph[ag.start.x, ag.start.y] + "," + picatGraph[ag.target.x, ag.target.y] + ")");
                        if (x < agents.Count - 1)
                        {
                            stw.Write(",");
                        }
                        x++;
                    }
                    stw.Write("].");

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Najde k-robustni reseni MAPF problemu pro graf graph s agenty agents, pomoci SAT resice v jazyce Picat.
        /// V novem vlakne vytvori proces, ve kterem spusti program s Picat resicem a preda mu soubor s deklarativnim modelem a souborem 
        /// s definici grafu a polohou agentu.
        /// </summary>
        public List<Plan> SolveByPicat(int k, int[,] graph, List<Agent> agents)
        {
            if (!MapfToPicat(k, graph, agents))
                return null;
            string output = RunProcess(k);
            List<Plan> newSol = new List<Plan>();
            if (output != "")
            {
                string[] paths = output.Split('.');
                //posledni je prazdny radek
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    Plan pl = new Plan();
                    if (pl.LoadFromString(paths[i]))
                    {
                        newSol.Add(pl);
                    }
                }
                return newSol;
            }
            return null;
        }

        /// <summary>
        /// Spousti v novem vlakne proces, ve kterem se hleda reseni MAPF problemu v Picatu.
        /// </summary>
        private string RunProcess(int robustness)
        {
            try
            {
                //pripravime parametry procesu
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = Properties.Settings.Default.PicatPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;

                //jako parametr predame jmeno souboru s modelem v jazyce Picat a textovy soubor s grafem a pozicemi agentu
                var fPath = "PicatFiles"+Path.DirectorySeparatorChar;
                string commandLineArgs = fPath+"picatMAPF "+ tempFile;
                if (robustness > 0)
                {
                    commandLineArgs = fPath+"picatMAPFrobust "+tempFile;
                }
                p.StartInfo.Arguments = commandLineArgs;
                p.EnableRaisingEvents = true;
                //zobrazime nove okno s informaci o delce behu a moznosti tento process ukoncit
                PicatTimeWindow w = new PicatTimeWindow();
                //pokud okno zavreme, dojde k ukonceni procesu 
                w.FormClosed += (a, e) =>
                {
                    try { p.Kill(); p.WaitForExit();}
                    catch (InvalidOperationException)
                    { }
                };
                //vytvoreni noveho vlakna s procesem po zobrazeni okna
                w.Shown += (a, e) =>
                {
                    ThreadStart st = new ThreadStart(() => p.Start());
                    Thread t = new Thread(st);
                    t.Start();
                    
                };
                //cekame, dokud proces neskonci a my obdrzime reseni
                p.Exited += (a, x) =>
                {
                    if (w.Visible)
                    {
                        w.BeginInvoke((Action)(() => w.Close()));
                    }

                };
                w.ShowDialog();

                //vracime vypis z programu, kde by se melo nachazet reseni
                return p.StandardOutput.ReadToEnd();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
