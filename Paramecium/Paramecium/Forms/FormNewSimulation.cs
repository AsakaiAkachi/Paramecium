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
            Variables.SoupInstance.SoupStop = true;
            Variables.SoupInstance = new Soup(
                (int)SoupSizeX.Value, (int)SoupSizeY.Value, (double)WallPerlinNoiseX.Value, (double)WallPerlinNoiseY.Value, (double)WallPerlinNoiseZ.Value, WallPerlinNoisePositionRandomize.Checked,
                (double)WallPerlinNoiseScale.Value, (int)WallPerlinNoiseOctave.Value, (double)WallThickness.Value, (int)ParticleCountLimit.Value, (double)TotalBiomassAmount.Value, (int)InitialAnimalCount.Value
            );
            Variables.SoupInstance.SoupSetup();
            Variables.SoupInstance.SoupRun();
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
