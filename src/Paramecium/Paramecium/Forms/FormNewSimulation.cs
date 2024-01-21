using Paramecium.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paramecium.Forms
{
    public partial class FormNewSimulation : Form
    {
        public FormNewSimulation()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Global.g_Soup.SetSoupState(SoupState.Stop);
            Global.g_Soup = new Soup(
                (int)SoupSizeX.Value, (int)SoupSizeY.Value, (double)WallPerlinNoiseX.Value, (double)WallPerlinNoiseY.Value, (double)WallPerlinNoiseZ.Value, WallPerlinNoisePositionRandomize.Checked,
                (double)WallPerlinNoiseScale.Value, (int)WallPerlinNoiseOctave.Value, (double)WallThickness.Value, (double)TotalBiomassAmount.Value, (int)InitialAnimalCount.Value
            );
            Global.g_Soup.SoupSetup();
            Global.g_Soup.SoupRun();
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
