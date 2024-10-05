using Paramecium.Engine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paramecium.Forms
{
    public partial class FormSoupSettings : Form
    {
        bool IsAllSettingsEditable;

        public FormSoupSettings(SoupSettings soupSettings, bool isAllSettingsEditable)
        {
            InitializeComponent();

            ImportPresetDialog.InitialDirectory = $"{g_PresetDefaultFilePath}";
            ExportPresetDialog.InitialDirectory = $"{g_PresetDefaultFilePath}";
            ExportPresetDialog.FileName = $"{g_PresetDefaultFileName}";

            GenerateInputsFromSoupSettings(soupSettings, true);

            IsAllSettingsEditable = isAllSettingsEditable;

            if (!isAllSettingsEditable)
            {
                Input_InitialSeed.Enabled = false;
                Randomize_InitialSeed.Enabled = false;

                Input_SizeX.Enabled = false;
                Input_SizeY.Enabled = false;

                Input_WallEnabled.Enabled = false;
                Input_WallNoiseX.Enabled = false;
                Input_WallNoiseY.Enabled = false;
                Input_WallNoiseZ.Enabled = false;
                Randomize_WallNoise.Enabled = false;
                Input_WallNoiseScale.Enabled = false;
                Input_WallNoiseOctave.Enabled = false;
                Input_WallThickness.Enabled = false;

                Input_TotalElementAmount.Enabled = false;

                Input_InitialPlantPopulation.Enabled = false;
                Input_InitialPlantElementAmount.Enabled = false;

                Input_InitialAnimalPopulation.Enabled = false;
                Input_InitialAnimalElementAmount.Enabled = false;
            }
        }

        private SoupSettings GenerateSoupSettingsFromInputs()
        {
            return new SoupSettings()
            {
                InitialSeed = (int)Input_InitialSeed.Value,

                SizeX = (int)Input_SizeX.Value,
                SizeY = (int)Input_SizeY.Value,

                WallEnabled = Input_WallEnabled.Checked,
                WallNoiseX = (double)Input_WallNoiseX.Value,
                WallNoiseY = (double)Input_WallNoiseY.Value,
                WallNoiseZ = (double)Input_WallNoiseZ.Value,
                WallNoiseScale = (double)Input_WallNoiseScale.Value,
                WallNoiseOctave = (int)Input_WallNoiseOctave.Value,
                WallThickness = (double)Input_WallThickness.Value,

                TotalElementAmount = (double)Input_TotalElementAmount.Value,
                ElementFlowRate = (double)Input_ElementFlowRate.Value,

                PheromoneFlowRate = (double)Input_PheromoneFlowRate.Value,
                PheromoneDecayRate = (double)Input_PheromoneDecayRate.Value,
                PheromoneProductionRate = (double)Input_PheromoneProductionRate.Value,

                Drag = (double)Input_Drag.Value,
                AngularVelocityDrag = (double)Input_AngularVelocityDrag.Value,
                MaximumVelocity = (double)Input_MaximumVelocity.Value,
                MaximumAngularVelocity = (double)Input_MaximumAngularVelocity.Value,
                RestitutionCoefficient = (double)Input_RestitutionCoefficient.Value,

                InitialPlantPopulation = (int)Input_InitialPlantPopulation.Value,
                InitialPlantElementAmount = (double)Input_InitialPlantElementAmount.Value,
                PlantForkCost = (double)Input_PlantForkCost.Value,
                PlantForkOffspringCountMin = (int)Input_PlantForkOffspringCountMin.Value,
                PlantForkOffspringCountMax = (int)Input_PlantForkOffspringCountMax.Value,
                PlantElementCollectRate = (double)Input_PlantElementCollectRate.Value,

                InitialAnimalPopulation = (int)Input_InitialAnimalPopulation.Value,
                InitialAnimalElementAmount = (double)Input_InitialAnimalElementAmount.Value,
                AnimalForkCost = (double)Input_AnimalForkCost.Value,
                AnimalElementBaseCost = (double)Input_AnimalElementBaseCost.Value,
                AnimalElementAccelerationCost = (double)Input_AnimalElementAccelerationCost.Value,
                AnimalElementRotationCost = (double)Input_AnimalElementRotationCost.Value,
                AnimalElementAttackCost = (double)Input_AnimalElementAttackCost.Value,
                AnimalElementPheromoneProductionCost = (double)Input_AnimalElementPheromoneProductionCost.Value,
                AnimalPlantIngestionRate = (double)Input_AnimalPlantIngestionRate.Value,
                AnimalAnimalIngestionRate = (double)Input_AnimalAnimalIngestionRate.Value,
                AnimalMaximumAge = (int)Input_AnimalMaximumAge.Value,

                AnimalMutationRate = (double)Input_AnimalMutationRate.Value,
                AnimalMaximumMutationCount = (int)Input_AnimalMaximumMutationCount.Value,
                AnimalSpeciesIdMutationRate = (double)Input_AnimalSpeciesIdMutationRate.Value,

                AnimalBrainMaximumNodeCount = (int)Input_AnimalBrainMaximumNodeCount.Value,
                AnimalBrainMaximumConnectionCount = (int)Input_AnimalBrainMaximumConnectionCount.Value
            };
        }

        private void GenerateInputsFromSoupSettings(SoupSettings soupSettings, bool isAllSettingsEditable)
        {
            if (isAllSettingsEditable)
            {
                Input_InitialSeed.Value = soupSettings.InitialSeed;

                Input_SizeX.Value = soupSettings.SizeX;
                Input_SizeY.Value = soupSettings.SizeY;

                Input_WallEnabled.Checked = soupSettings.WallEnabled;
                Input_WallNoiseX.Value = (decimal)soupSettings.WallNoiseX;
                Input_WallNoiseY.Value = (decimal)soupSettings.WallNoiseY;
                Input_WallNoiseZ.Value = (decimal)soupSettings.WallNoiseZ;
                Input_WallNoiseScale.Value = (decimal)soupSettings.WallNoiseScale;
                Input_WallNoiseOctave.Value = soupSettings.WallNoiseOctave;
                Input_WallThickness.Value = (decimal)soupSettings.WallThickness;

                Input_TotalElementAmount.Value = (decimal)soupSettings.TotalElementAmount;
            }
            Input_ElementFlowRate.Value = (decimal)soupSettings.ElementFlowRate;

            Input_PheromoneFlowRate.Value = (decimal)soupSettings.PheromoneFlowRate;
            Input_PheromoneDecayRate.Value = (decimal)soupSettings.PheromoneDecayRate;
            Input_PheromoneProductionRate.Value = (decimal)soupSettings.PheromoneProductionRate;

            Input_Drag.Value = (decimal)soupSettings.Drag;
            Input_AngularVelocityDrag.Value = (decimal)soupSettings.AngularVelocityDrag;
            Input_MaximumVelocity.Value = (decimal)soupSettings.MaximumVelocity;
            Input_MaximumAngularVelocity.Value = (decimal)soupSettings.MaximumAngularVelocity;
            Input_RestitutionCoefficient.Value = (decimal)soupSettings.RestitutionCoefficient;

            if (isAllSettingsEditable)
            {
                Input_InitialPlantPopulation.Value = soupSettings.InitialPlantPopulation;
                Input_InitialPlantElementAmount.Value = (decimal)soupSettings.InitialPlantElementAmount;
            }
            Input_PlantForkCost.Value = (decimal)soupSettings.PlantForkCost;
            Input_PlantForkOffspringCountMin.Value = soupSettings.PlantForkOffspringCountMin;
            Input_PlantForkOffspringCountMax.Value = soupSettings.PlantForkOffspringCountMax;
            Input_PlantElementCollectRate.Value = (decimal)soupSettings.PlantElementCollectRate;

            if (isAllSettingsEditable)
            {
                Input_InitialAnimalPopulation.Value = soupSettings.InitialAnimalPopulation;
                Input_InitialAnimalElementAmount.Value = (decimal)soupSettings.InitialAnimalElementAmount;
            }
            Input_AnimalForkCost.Value = (decimal)soupSettings.AnimalForkCost;
            Input_AnimalElementBaseCost.Value = (decimal)soupSettings.AnimalElementBaseCost;
            Input_AnimalElementAccelerationCost.Value = (decimal)soupSettings.AnimalElementAccelerationCost;
            Input_AnimalElementRotationCost.Value = (decimal)soupSettings.AnimalElementRotationCost;
            Input_AnimalElementAttackCost.Value = (decimal)soupSettings.AnimalElementAttackCost;
            Input_AnimalElementPheromoneProductionCost.Value = (decimal)soupSettings.AnimalElementPheromoneProductionCost;
            Input_AnimalPlantIngestionRate.Value = (decimal)soupSettings.AnimalPlantIngestionRate;
            Input_AnimalAnimalIngestionRate.Value = (decimal)soupSettings.AnimalAnimalIngestionRate;
            Input_AnimalMaximumAge.Value = soupSettings.AnimalMaximumAge;

            Input_AnimalMutationRate.Value = (decimal)soupSettings.AnimalMutationRate;
            Input_AnimalMaximumMutationCount.Value = soupSettings.AnimalMaximumMutationCount;
            Input_AnimalSpeciesIdMutationRate.Value = (decimal)soupSettings.AnimalSpeciesIdMutationRate;

            Input_AnimalBrainMaximumNodeCount.Value = soupSettings.AnimalBrainMaximumNodeCount;
            Input_AnimalBrainMaximumConnectionCount.Value = soupSettings.AnimalBrainMaximumConnectionCount;
        }

        private void Input_WallEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (Input_WallEnabled.Checked)
            {
                Input_WallNoiseX.Enabled = true;
                Input_WallNoiseY.Enabled = true;
                Input_WallNoiseZ.Enabled = true;
                Randomize_WallNoise.Enabled = true;
                Input_WallNoiseScale.Enabled = true;
                Input_WallNoiseOctave.Enabled = true;
                Input_WallThickness.Enabled = true;
            }
            else
            {
                Input_WallNoiseX.Enabled = false;
                Input_WallNoiseY.Enabled = false;
                Input_WallNoiseZ.Enabled = false;
                Randomize_WallNoise.Enabled = false;
                Input_WallNoiseScale.Enabled = false;
                Input_WallNoiseOctave.Enabled = false;
                Input_WallThickness.Enabled = false;
            }
        }

        private void ButtonImportPreset_Click(object sender, EventArgs e)
        {
            if (ImportPresetDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(ImportPresetDialog.FileName, Encoding.UTF8);
                SoupSettings? loadedSoupSettings = JsonSerializer.Deserialize<SoupSettings>(streamReader.ReadToEnd(), new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
                if (loadedSoupSettings is not null)
                {
                    GenerateInputsFromSoupSettings(loadedSoupSettings, IsAllSettingsEditable);
                }
                streamReader.Close();
            }
        }

        private void ButtonExportPreset_Click(object sender, EventArgs e)
        {
            if (ExportPresetDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter streamWriter = new StreamWriter(ExportPresetDialog.FileName, false, Encoding.UTF8);
                streamWriter.Write(JsonSerializer.Serialize(GenerateSoupSettingsFromInputs(), new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));
                streamWriter.Close();
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            if (Owner is not null && Owner.GetType() == typeof(FormCreateNewSoup))
            {
                ((FormCreateNewSoup)Owner).Settings = GenerateSoupSettingsFromInputs();
            }
            else if (Owner is not null && Owner.GetType() == typeof(FormMain))
            {
                if (g_Soup is not null && g_Soup.Initialized && g_Soup.SoupState == SoupState.Pause)
                {
                    g_Soup.Settings = GenerateSoupSettingsFromInputs();
                    g_Soup.Modified = true;
                }
            }

            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Randomize_InitialSeed_Click(object sender, EventArgs e)
        {
            Input_InitialSeed.Value = new Random().Next(int.MinValue, int.MaxValue);
        }

        private void Randomize_WallNoise_Click(object sender, EventArgs e)
        {
            Input_WallNoiseX.Value = (decimal)(new Random().NextDouble() * 256d);
            Input_WallNoiseY.Value = (decimal)(new Random().NextDouble() * 256d);
            Input_WallNoiseZ.Value = (decimal)(new Random().NextDouble() * 256d);
        }
    }
}
