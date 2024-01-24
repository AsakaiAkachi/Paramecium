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

            SoupParameterView.Rows.Add("SizeX", 128);
            SoupParameterView.Rows.Add("SizeY", 128);
            SoupParameterView.Rows.Add("WallPerlinNoiseX", 79.14649528369992);
            SoupParameterView.Rows.Add("WallPerlinNoiseY", 97.49842312725215);
            SoupParameterView.Rows.Add("WallPerlinNoiseZ", 244.70658351525472);
            SoupParameterView.Rows.Add("WallPerlinNoiseScale", 0.1);
            SoupParameterView.Rows.Add("WallPerlinNoiseOctave", 4);
            SoupParameterView.Rows.Add("WallThickness", 0.025);
            SoupParameterView.Rows.Add("TotalBiomassAmount", 65536);
            SoupParameterView.Rows.Add("CellSizeMultiplier", 0.5);
            SoupParameterView.Rows.Add("PlantForkBiomass", 15);
            SoupParameterView.Rows.Add("AnimalForkBiomass", 60);
            SoupParameterView.Rows.Add("PlantBiomassCollectionRange", 3);
            SoupParameterView.Rows.Add("InitialAnimalCount", 1);
            SoupParameterView.Rows.Add("HatchingTime", 3000);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Soup? prevSoup = g_Soup;
            try
            {
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Stop);

                Soup newSoup = new Soup(
                    int.Parse(SoupParameterView[1, 0].Value.ToString()), int.Parse(SoupParameterView[1, 1].Value.ToString()),
                    double.Parse(SoupParameterView[1, 2].Value.ToString()), double.Parse(SoupParameterView[1, 3].Value.ToString()), double.Parse(SoupParameterView[1, 4].Value.ToString()),
                    double.Parse(SoupParameterView[1, 5].Value.ToString()), int.Parse(SoupParameterView[1, 6].Value.ToString()), double.Parse(SoupParameterView[1, 7].Value.ToString()),
                    double.Parse(SoupParameterView[1, 8].Value.ToString()),
                    double.Parse(SoupParameterView[1, 9].Value.ToString()), double.Parse(SoupParameterView[1, 10].Value.ToString()), double.Parse(SoupParameterView[1, 11].Value.ToString()), int.Parse(SoupParameterView[1, 12].Value.ToString()),
                    int.Parse(SoupParameterView[1, 13].Value.ToString()), int.Parse(SoupParameterView[1, 14].Value.ToString())
                );

                g_Soup = newSoup;
                g_Soup.SoupSetup();
                g_Soup.SoupRun();

                g_FormMain.zoomFactor = 0;
                g_FormMain.zoomFactorActual = 1;
                g_FormMain.cameraX = g_Soup.SizeX / 2d;
                g_FormMain.cameraY = g_Soup.SizeY / 2d;

                g_FormMain.FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\New Soup.soup";

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
