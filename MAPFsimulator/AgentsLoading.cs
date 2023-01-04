using System;
using System.Windows.Forms;


namespace MAPFsimulator
{
    /// <summary>
    /// Trida zobrazujici nove okno s moznosti vyberu poctu agentu pridavanych do grafu.
    /// Instance se vytvari po vyberu souboru s pozicemi agentu (.scen) v hlavnim menu.
    /// </summary>
    public partial class AgentsLoading : Form
    {
        /// <summary>
        /// vybrany pocet agentu
        /// </summary>
        public int n = 0;

        /// <summary>
        /// rozdhodnuti, zda vybrat agenty nahodne
        /// </summary>
        public bool randomChoice = false;

        /// <summary>
        /// zda jsme vyber poctu agentu potvrdili
        /// </summary>
        public bool confirmed;

        /// <summary>
        /// zda jsme vybrali smart agenty
        /// </summary>
        public bool smartAgents;

        /// <summary>
        /// Vytvori nove okno s moznosti vyberu poctu agentu do grafu.
        /// </summary>
        public AgentsLoading()
        {
            InitializeComponent();
            confirmed = false;
        }
        
        /// <summary>
        /// Zobrazi pocet nalezenych agentu. 
        /// </summary>
        public void SetText(int x)
        {
            labelAgentsCount.Text = x.ToString();
        }

        /// <summary>
        /// Akce po stisku tlacitka s vyberem poctu agentu.
        /// Vraci pocet agentu, ktere uzivatel vybral do grafu. Vysledne cislo je ulozene v promenne n.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            randomChoice = checkBoxRandom.Checked;
            smartAgents = checkBoxSmart.Checked;
            if (int.TryParse(textBox1.Text, out n) && n <= int.Parse(labelAgentsCount.Text))
            {
                confirmed = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Zadejte platné číslo!");
                n = 0;
            }
        }
    }
}
