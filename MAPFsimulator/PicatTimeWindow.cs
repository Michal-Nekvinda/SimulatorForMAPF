using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MAPFsimulator
{

    public partial class PicatTimeWindow : Form
    {
        /// <summary>
        /// Graficke okno s casomirou objevujici se po spusteni Picat resice.
        /// </summary>
        public PicatTimeWindow()
        {
            InitializeComponent();
        }
        Stopwatch stopwatch;

        /// <summary>
        /// Aktualni cas od zobrazeni okna.
        /// Pokud bezi dele, nez je pripustny limit, provede se akce stisknuti tlacitka buttonStop, cimz se ukonci vypocet Picat resice.
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            double time = stopwatch.ElapsedMilliseconds/1000.0;
            timeLabel.Text = time.ToString();
            if (Properties.Settings.Default.PicatLimit > 0 && time > Properties.Settings.Default.PicatLimit)
            {
                buttonStop.PerformClick();
            }            
        }

        /// <summary>
        /// Zavre toto okno, cimz ukonci vypocet Picat resice.
        /// </summary>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Spousti mereni casu.
        /// </summary>
        private void PicatTimeWindow_Shown(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer1.Enabled = true;
            timer1.Start();
        }
    }
}
