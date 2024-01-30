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
        Dictionary<string, int> SoupParameterViewRows = new Dictionary<string, int>();

        public FormNewSimulation()
        {
            InitializeComponent();

            int i = 0;

            SoupParameterView.Rows.Add("Size X", 256); SoupParameterViewRows.Add("SizeX", i); i++;
            SoupParameterView.Rows.Add("Size Y", 160); SoupParameterViewRows.Add("SizeY", i); i++;
            SoupParameterView.Rows.Add("Wall Perlin Noise X", new Random().NextDouble() * 256d); SoupParameterViewRows.Add("WallPerlinNoiseX", i); i++;
            SoupParameterView.Rows.Add("Wall Perlin Noise Y", new Random().NextDouble() * 256d); SoupParameterViewRows.Add("WallPerlinNoiseY", i); i++;
            SoupParameterView.Rows.Add("Wall Perlin Noise Z", new Random().NextDouble() * 256d); SoupParameterViewRows.Add("WallPerlinNoiseZ", i); i++;
            SoupParameterView.Rows.Add("Wall Perlin Noise Scale", 0.03); SoupParameterViewRows.Add("WallPerlinNoiseScale", i); i++;
            SoupParameterView.Rows.Add("Wall Perlin Noise Octave", 4); SoupParameterViewRows.Add("WallPerlinNoiseOctave", i); i++;
            SoupParameterView.Rows.Add("Wall Thickness", 0.0085); SoupParameterViewRows.Add("WallPerlinNoiseThickness", i); i++;
            SoupParameterView.Rows.Add("Total BiomassAmount", 163840); SoupParameterViewRows.Add("TotalBiomassAmount", i); i++;
            SoupParameterView.Rows.Add("Cell Size Multiplier", 0.5); SoupParameterViewRows.Add("CellSizeMultiplier", i); i++;
            SoupParameterView.Rows.Add("Plant Fork Biomass", 10); SoupParameterViewRows.Add("PlantForkBiomass", i); i++;
            SoupParameterView.Rows.Add("Animal Fork Biomass", 50); SoupParameterViewRows.Add("AnimalForkBiomass", i); i++;
            SoupParameterView.Rows.Add("Plant Biomass Collection Range", 3); SoupParameterViewRows.Add("PlantBiomassCollectionRange", i); i++;
            SoupParameterView.Rows.Add("Initial Animal Count", 1024); SoupParameterViewRows.Add("InitialAnimalCount", i); i++;
            SoupParameterView.Rows.Add("Hatching Time", 300); SoupParameterViewRows.Add("HatchingTime", i); i++;
            SoupParameterView.Rows.Add("Mutation Rate", 0.015625); SoupParameterViewRows.Add("MutationRate", i); i++;
            SoupParameterView.Rows.Add("Animal Element Lose Per Step (In Passive)", 0.025); SoupParameterViewRows.Add("AnimalElementLosePerStepInPassive", i); i++;
            SoupParameterView.Rows.Add("Animal Element Lose Per Step (In Accelerating)", 0.075); SoupParameterViewRows.Add("AnimalElementLosePerStepInAccelerating", i); i++;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Soup? prevSoup = g_Soup;
            try
            {
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Stop);

                Soup newSoup = new Soup(
                    int.Parse(SoupParameterView[1, SoupParameterViewRows["SizeX"]].Value.ToString()), int.Parse(SoupParameterView[1, SoupParameterViewRows["SizeY"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseX"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseY"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseZ"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseScale"]].Value.ToString()), int.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseOctave"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["WallPerlinNoiseThickness"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["TotalBiomassAmount"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["CellSizeMultiplier"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["PlantForkBiomass"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["AnimalForkBiomass"]].Value.ToString()), int.Parse(SoupParameterView[1, SoupParameterViewRows["PlantBiomassCollectionRange"]].Value.ToString()),
                    int.Parse(SoupParameterView[1, SoupParameterViewRows["InitialAnimalCount"]].Value.ToString()), int.Parse(SoupParameterView[1, SoupParameterViewRows["HatchingTime"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["MutationRate"]].Value.ToString()),
                    double.Parse(SoupParameterView[1, SoupParameterViewRows["AnimalElementLosePerStepInPassive"]].Value.ToString()), double.Parse(SoupParameterView[1, SoupParameterViewRows["AnimalElementLosePerStepInAccelerating"]].Value.ToString())
                );

                g_Soup = newSoup;
                g_Soup.SoupSetup();
                g_Soup.SoupRun();

                g_FormMain.zoomFactor = 0;
                g_FormMain.zoomFactorActual = 1;
                g_FormMain.cameraX = g_Soup.SizeX / 2d;
                g_FormMain.cameraY = g_Soup.SizeY / 2d;

                g_FormMain.FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\Untitled.soup";

                this.Hide();
            }
            catch (Exception ex)
            {
                ConsoleLog(LogLevel.Failure, ex.ToString());
                if (prevSoup is not null) g_Soup = prevSoup;
            }

            /**
            Global.g_Soup.SetSoupState(SoupState.Stop);
            Global.g_Soup = new Soup(
                (int)SoupSizeX.Value, (int)SoupSizeY.Value, (double)WallPerlinNoiseX.Value, (double)WallPerlinNoiseY.Value, (double)WallPerlinNoiseZ.Value, WallPerlinNoisePositionRandomize.Checked,
                (double)WallPerlinNoiseScale.Value, (int)WallPerlinNoiseOctave.Value, (double)WallThickness.Value, (double)TotalBiomassAmount.Value, (int)InitialAnimalCount.Value
            );
            Global.g_Soup.SoupSetup();
            Global.g_Soup.SoupRun();
            this.Close();
            **/
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
