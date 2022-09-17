using System;
using System.IO;
using System.Windows.Forms;

namespace MAPFsimulator
{
    /// <summary>
    /// Trida definujici okno, ktere se zobrazi po stisku polozky Soubor -> Nastaveni v Menu programu.
    /// </summary>
    public partial class Settings : Form
    {
        /// <summary>
        /// Ve vytvorenem okne se zobrazi defaultni hodnoty vsech parametru k nastaveni.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            textBoxCBS.Text = Properties.Settings.Default.CBSLimit.ToString();
            textBoxPicat.Text = Properties.Settings.Default.PicatLimit.ToString();
            textBoxPicatPath.Text = Properties.Settings.Default.PicatPath;
            numericUpDownInterval.Value = Properties.Settings.Default.TimerInterval;
        }

        /// <summary>
        /// Po stisku tlacitka se ulozi uzivatelem upravene hodnoty.
        /// </summary>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            int newVal;
            if (int.TryParse(textBoxCBS.Text,out newVal) && newVal != Properties.Settings.Default.CBSLimit)
            {
                Properties.Settings.Default.CBSLimit = newVal;
                Properties.Settings.Default.Save();
            }
            if (int.TryParse(textBoxPicat.Text, out newVal) && newVal != Properties.Settings.Default.PicatLimit)
            {
                Properties.Settings.Default.PicatLimit = newVal;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.PicatPath != textBoxPicatPath.Text)
            {
                Properties.Settings.Default.PicatPath = textBoxPicatPath.Text;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.TimerInterval != numericUpDownInterval.Value)
            {
                Properties.Settings.Default.TimerInterval = (int)numericUpDownInterval.Value;
                Properties.Settings.Default.Save();
            }
            this.Close();
        }
        /// <summary>
        /// Otevre dialogove okno s vyberem umisteni resice Picat.
        /// </summary>
        private void buttonChange_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxPicatPath.Text = openFileDialog1.FileName;
            }
        }
    }
}
