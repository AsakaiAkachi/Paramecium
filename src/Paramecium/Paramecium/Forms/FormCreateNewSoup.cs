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
    public partial class FormCreateNewSoup : Form
    {
        public FormCreateNewSoup()
        {
            InitializeComponent();

            Random random = new Random();

            inputSeed.Value = random.Next(-2147483648, 2147483647);

            inputNoiseX.Value = (decimal)(random.NextDouble() * 256d);
            inputNoiseY.Value = (decimal)(random.NextDouble() * 256d);
            inputNoiseZ.Value = (decimal)(random.NextDouble() * 256d);
        }

        private void buttonSeedRandomize_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            inputSeed.Value = random.Next(-2147483648, 2147483647);
        }

        private void buttonNoiseXYZRandomize_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            inputNoiseX.Value = (decimal)(random.NextDouble() * 256d);
            inputNoiseY.Value = (decimal)(random.NextDouble() * 256d);
            inputNoiseZ.Value = (decimal)(random.NextDouble() * 256d);
        }

        private void FormCreateNewSoup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void buttonCreateSoup_Click(object sender, EventArgs e)
        {
            Soup? prevSoup = g_Soup;
            try
            {
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Stop);

                Soup newSoup = new Soup(
                        (int)inputSeed.Value,
                        (int)inputSoupWidth.Value, (int)inputSoupHeight.Value,
                        checkboxEnableWall.Checked, (double)inputNoiseX.Value, (double)inputNoiseY.Value, (double)inputNoiseZ.Value,
                        (double)inputNoiseScale.Value, (int)inputNoiseOctave.Value, (double)inputWallThickness.Value,
                        (double)inputTotalElementAmount.Value, (double)inputCellSizeMultiplier.Value, (double)inputPlantForkCost.Value, (double)inputAnimalForkCost.Value, (int)inputPlantElementCollectionRange.Value,
                        (int)inputInitialAnimalCount.Value, (int)inputHatchingTime.Value,
                        (double)inputMutationRate.Value, (double)inputAnimalSurvivalCost.Value, (double)inputAnimalAccDecCost.Value
                    );

                g_Soup = newSoup;
                g_Soup.SoupSetup();
                g_Soup.SoupRun();

                g_FormMain.zoomFactor = 0;
                g_FormMain.zoomFactorActual = 1;
                g_FormMain.cameraX = g_Soup.SizeX / 2d;
                g_FormMain.cameraY = g_Soup.SizeY / 2d;

                g_FormMain.FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\{textboxSoupName.Text}.soup";

                g_FormAutosaveSetting.UpdateCurrentAutoSaveIntervalText();

                Hide();
            }
            catch (Exception ex)
            {
                ConsoleLog(LogLevel.Failure, ex.ToString());
                if (prevSoup is not null) g_Soup = prevSoup;
            }
        }
    }
}
