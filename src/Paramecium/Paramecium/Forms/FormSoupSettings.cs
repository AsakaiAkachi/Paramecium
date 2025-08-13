using Paramecium.Engine;
using Paramecium.Variables;
using Paramecium.Utils;

namespace Paramecium.Forms
{
    public partial class FormSoupSettings : Form
    {
        SoupSettingsSetter _soupSettingsSetter;
        bool _soupIsCreated;

        public FormSoupSettings(SoupSettingsSetter soupSettingsSetter, bool soupIsCreated)
        {
            InitializeComponent();

            ImportPresetDialog.InitialDirectory = Globals.PresetDirectoryPath;
            ExportPresetDialog.InitialDirectory = Globals.PresetDirectoryPath;

            _soupSettingsSetter = soupSettingsSetter;
            _soupIsCreated = soupIsCreated;

            if (soupIsCreated)
            {
                SoupSettingsItem_SizeX.Editable = false;
                SoupSettingsItem_SizeY.Editable = false;

                SoupSettingsItem_WallEnabled.Editable = false;
                ButtonRandomizeWallNoiseOffset.Enabled = false;
                SoupSettingsItem_WallNoiseOffsetX.Editable = false;
                SoupSettingsItem_WallNoiseOffsetY.Editable = false;
                SoupSettingsItem_WallNoiseOffsetZ.Editable = false;
                SoupSettingsItem_WallNoiseSamplingInterval.Editable = false;
                SoupSettingsItem_WallNoiseOctave.Editable = false;
                SoupSettingsItem_WallThickness.Editable = false;

                SoupSettingsItem_TotalElementAmount.Editable = false;

                SoupSettingsItem_PlantInitialPopulation.Editable = false;

                SoupSettingsItem_AnimalInitialPopulation.Editable = false;
            }

            if (_soupSettingsSetter.SoupSettings is not null)
            {
                LoadSoupSettingsToSettingsItems(_soupSettingsSetter.SoupSettings, false);
            }
        }

        private void ButtonApplySettings_Click(object sender, EventArgs e)
        {
            _soupSettingsSetter.SoupSettings = SaveSoupSettingsFromSettingsItems();

            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            _soupSettingsSetter.SoupSettings = null;

            Close();
        }

        private void ButtonImportPreset_Click(object sender, EventArgs e)
        {
            if (ImportPresetDialog.ShowDialog() == DialogResult.OK)
            {
                SoupSettings? soupSettings = JsonFileImportAndExport.Import<SoupSettings>(ImportPresetDialog.FileName);

                if (soupSettings is not null)
                {
                    LoadSoupSettingsToSettingsItems(soupSettings, _soupIsCreated);
                }
            }
        }

        private void ButtonExportPreset_Click(object sender, EventArgs e)
        {
            if (ExportPresetDialog.ShowDialog() == DialogResult.OK)
            {
                JsonFileImportAndExport.Export(ExportPresetDialog.FileName, SaveSoupSettingsFromSettingsItems());
            }
        }

        private void ButtonRandomizeWallNoiseOffset_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            SoupSettingsItem_WallNoiseOffsetX.InputValueDouble = rand.NextDouble() * 256d;
            SoupSettingsItem_WallNoiseOffsetY.InputValueDouble = rand.NextDouble() * 256d;
            SoupSettingsItem_WallNoiseOffsetZ.InputValueDouble = rand.NextDouble() * 256d;
        }

        private void LoadSoupSettingsToSettingsItems(SoupSettings soupSettings, bool dontLoadImmutableItems)
        {
            if (!dontLoadImmutableItems)
            {
                SoupSettingsItem_SizeX.InputValueInt = soupSettings.SizeX;
                SoupSettingsItem_SizeY.InputValueInt = soupSettings.SizeY;
            }

            if (!dontLoadImmutableItems)
            {
                SoupSettingsItem_WallEnabled.Checked = soupSettings.WallEnabled;
                SoupSettingsItem_WallNoiseOffsetX.InputValueDouble = soupSettings.WallNoiseX;
                SoupSettingsItem_WallNoiseOffsetY.InputValueDouble = soupSettings.WallNoiseY;
                SoupSettingsItem_WallNoiseOffsetZ.InputValueDouble = soupSettings.WallNoiseZ;
                SoupSettingsItem_WallNoiseSamplingInterval.InputValueDouble = soupSettings.WallNoiseSamplingInterval;
                SoupSettingsItem_WallNoiseOctave.InputValueInt = soupSettings.WallNoiseOctave;
                SoupSettingsItem_WallThickness.InputValueDouble = soupSettings.WallThickness;
            }

            if (!dontLoadImmutableItems) SoupSettingsItem_TotalElementAmount.InputValueDouble = soupSettings.TotalElementAmount;
            SoupSettingsItem_ElementFlowRate.InputValueDouble = soupSettings.ElementFlowRate;

            SoupSettingsItem_MaximumEffectivePheromoneAmount.InputValueDouble = soupSettings.MaximumEffectivePheromoneAmount;
            SoupSettingsItem_MinimumEffectivePheromoneAmount.InputValueDouble = soupSettings.MinimumEffectivePheromoneAmount;
            SoupSettingsItem_PheromoneFlowRate.InputValueDouble = soupSettings.PheromoneFlowRate;
            SoupSettingsItem_PheromoneDecayRate.InputValueDouble = soupSettings.PheromoneDecayRate;

            SoupSettingsItem_Drag.InputValueDouble = soupSettings.Drag;
            SoupSettingsItem_AngularVelocityDrag.InputValueDouble = soupSettings.AngularVelocityDrag;
            SoupSettingsItem_MaximumEffectiveVelocity.InputValueDouble = soupSettings.MaximumEffectiveVelocity;
            SoupSettingsItem_MaximumEffectiveAngularVelocity.InputValueDouble = soupSettings.MaximumEffectiveAngularVelocity;
            SoupSettingsItem_RestitutionCoefficient.InputValueDouble = soupSettings.RestitutionCoefficient;

            if (!dontLoadImmutableItems) SoupSettingsItem_PlantInitialPopulation.InputValueInt = soupSettings.InitialPlantPopulation;

            SoupSettingsItem_PlantMaximumElementAmount.InputValueDouble = soupSettings.PlantMaximumElementAmount;
            SoupSettingsItem_ElementCollectRate.InputValueDouble = soupSettings.PlantElementCollectRate;
            SoupSettingsItem_PlantUnderAttackTime.InputValueInt = soupSettings.PlantUnderAttackTime;

            SoupSettingsItem_PlantSpreadingTime.InputValueInt = soupSettings.PlantSpreadingTime;
            SoupSettingsItem_PlantDivisionCountMin.InputValueInt = soupSettings.PlantDivisionCountMin;
            SoupSettingsItem_PlantDivisionCountMax.InputValueInt = soupSettings.PlantDivisionCountMax;

            if (!dontLoadImmutableItems) SoupSettingsItem_AnimalInitialPopulation.InputValueInt = soupSettings.InitialAnimalPopulation;

            SoupSettingsItem_AnimalMaximumElementAmount.InputValueDouble = soupSettings.AnimalMaximumElementAmount;
            SoupSettingsItem_AnimalElementBaseCost.InputValueDouble = soupSettings.AnimalElementBaseCost;
            SoupSettingsItem_AnimalElementAccelerationCost.InputValueDouble = soupSettings.AnimalElementAccelerationCost;
            SoupSettingsItem_AnimalElementRotationCost.InputValueDouble = soupSettings.AnimalElementRotationCost;
            SoupSettingsItem_AnimalElementEatCost.InputValueDouble = soupSettings.AnimalElementEatCost;
            SoupSettingsItem_AnimalElementAttackCost.InputValueDouble = soupSettings.AnimalElementAttackCost;
            SoupSettingsItem_AnimalElementPheromoneProductionCost.InputValueDouble = soupSettings.AnimalElementPheromoneProductionCost;

            SoupSettingsItem_AnimalPheromoneProductionRate.InputValueDouble = soupSettings.AnimalPheromoneProductionRate;

            SoupSettingsItem_AnimalMaximumAcceleration.InputValueDouble = soupSettings.AnimalMaximumAcceleration;
            SoupSettingsItem_AnimalMaximumAngularAcceleration.InputValueDouble = soupSettings.AnimalMaximumAngularAcceleration;

            SoupSettingsItem_AnimalDisableSameSpeciesAttack.Checked = soupSettings.AnimalDisableSameSpeciesAttack;
            SoupSettingsItem_AnimalUnderAttackTime.InputValueInt = soupSettings.AnimalUnderAttackTime;
            SoupSettingsItem_AnimalPlantIngestionRate.InputValueDouble = soupSettings.AnimalPlantIngestionRate;
            SoupSettingsItem_AnimalAnimalIngestionRate.InputValueDouble = soupSettings.AnimalAnimalIngestionRate;

            SoupSettingsItem_AnimalReproductionCost.InputValueDouble = soupSettings.AnimalReproductionCost;
            SoupSettingsItem_AnimalMaximumReproductionRate.InputValueDouble = soupSettings.AnimalMaximumReproductionRate;
            SoupSettingsItem_AnimalEggHatchingTime.InputValueInt = soupSettings.AnimalEggHatchingTime;
            SoupSettingsItem_AnimalEggRadiusRatio.InputValueDouble = soupSettings.AnimalEggRadiusRatio;

            SoupSettingsItem_AnimalLifespan.InputValueInt = soupSettings.AnimalLifespan;

            SoupSettingsItem_AnimalMaximumNodeCount.InputValueInt = soupSettings.AnimalMaximumNodeCount;
            SoupSettingsItem_AnimalMaximumConnectionCountPerNode.InputValueInt = soupSettings.AnimalMaximumConnectionCountPerNode;

            SoupSettingsItem_AnimalMutationRate.InputValueDouble = soupSettings.AnimalMutationRate;
            SoupSettingsItem_AnimalMaximumMutationCount.InputValueInt = soupSettings.AnimalMaximumMutationCount;
            SoupSettingsItem_AnimalMutationCountFactor.InputValueDouble = soupSettings.AnimalMutationCountFactor;
            SoupSettingsItem_AnimalDisableSpeciesSigChangeByMutation.Checked = soupSettings.AnimalDisableSpeciesSigChangeByMutation;

            SoupSettingsItem_AnimalMutationAddNodeWeight.InputValueDouble = soupSettings.AnimalMutationAddNodeWeight;
            SoupSettingsItem_AnimalMutationRemoveNodeWeight.InputValueDouble = soupSettings.AnimalMutationRemoveNodeWeight;
            SoupSettingsItem_AnimalMutationChangeNodeTypeWeight.InputValueDouble = soupSettings.AnimalMutationChangeNodeTypeWeight;
            SoupSettingsItem_AnimalMutationAddConnectionWeight.InputValueDouble = soupSettings.AnimalMutationAddConnectionWeight;
            SoupSettingsItem_AnimalMutationRemoveConnectionWeight.InputValueDouble = soupSettings.AnimalMutationRemoveConnectionWeight;
            SoupSettingsItem_AnimalMutationChangeConnectionOrigin.InputValueDouble = soupSettings.AnimalMutationChangeConnectionOriginWeight;
            SoupSettingsItem_AnimalMutationChangeConnectionTarget.InputValueDouble = soupSettings.AnimalMutationChangeConnectionTargetWeight;
            SoupSettingsItem_AnimalMutationChangeConnectionWeight.InputValueDouble = soupSettings.AnimalMutationChangeConnectionWeightWeight;

            SoupSettingsItem_AnimalNodeTypeInputWeight.InputValueDouble = soupSettings.AnimalNodeTypeInputWeight;
            SoupSettingsItem_AnimalNodeTypeHiddenWeight.InputValueDouble = soupSettings.AnimalNodeTypeHiddenWeight;
            SoupSettingsItem_AnimalNodeTypeOutputWeight.InputValueDouble = soupSettings.AnimalNodeTypeOutputWeight;
        }

        private SoupSettings SaveSoupSettingsFromSettingsItems()
        {
            SoupSettings result = new SoupSettings();

            result.SizeX = SoupSettingsItem_SizeX.InputValueInt;
            result.SizeY = SoupSettingsItem_SizeY.InputValueInt;

            result.WallEnabled = SoupSettingsItem_WallEnabled.Checked;
            result.WallNoiseX = SoupSettingsItem_WallNoiseOffsetX.InputValueDouble;
            result.WallNoiseY = SoupSettingsItem_WallNoiseOffsetY.InputValueDouble;
            result.WallNoiseZ = SoupSettingsItem_WallNoiseOffsetZ.InputValueDouble;
            result.WallNoiseSamplingInterval = SoupSettingsItem_WallNoiseSamplingInterval.InputValueDouble;
            result.WallNoiseOctave = SoupSettingsItem_WallNoiseOctave.InputValueInt;
            result.WallThickness = SoupSettingsItem_WallThickness.InputValueDouble;

            result.TotalElementAmount = SoupSettingsItem_TotalElementAmount.InputValueDouble;
            result.ElementFlowRate = SoupSettingsItem_ElementFlowRate.InputValueDouble;

            result.MaximumEffectivePheromoneAmount = SoupSettingsItem_MaximumEffectivePheromoneAmount.InputValueDouble;
            result.MinimumEffectivePheromoneAmount = SoupSettingsItem_MinimumEffectivePheromoneAmount.InputValueDouble;
            result.PheromoneFlowRate = SoupSettingsItem_PheromoneFlowRate.InputValueDouble;
            result.PheromoneDecayRate = SoupSettingsItem_PheromoneDecayRate.InputValueDouble;

            result.Drag = SoupSettingsItem_Drag.InputValueDouble;
            result.AngularVelocityDrag = SoupSettingsItem_AngularVelocityDrag.InputValueDouble;
            result.MaximumEffectiveVelocity = SoupSettingsItem_MaximumEffectiveVelocity.InputValueDouble;
            result.MaximumEffectiveAngularVelocity = SoupSettingsItem_MaximumEffectiveAngularVelocity.InputValueDouble;
            result.RestitutionCoefficient = SoupSettingsItem_RestitutionCoefficient.InputValueDouble;

            result.InitialPlantPopulation = SoupSettingsItem_PlantInitialPopulation.InputValueInt;

            result.PlantMaximumElementAmount = SoupSettingsItem_PlantMaximumElementAmount.InputValueDouble;
            result.PlantElementCollectRate = SoupSettingsItem_ElementCollectRate.InputValueDouble;
            result.PlantUnderAttackTime = SoupSettingsItem_PlantUnderAttackTime.InputValueInt;

            result.PlantSpreadingTime = SoupSettingsItem_PlantSpreadingTime.InputValueInt;
            result.PlantDivisionCountMin = SoupSettingsItem_PlantDivisionCountMin.InputValueInt;
            result.PlantDivisionCountMax = SoupSettingsItem_PlantDivisionCountMax.InputValueInt;

            result.InitialAnimalPopulation = SoupSettingsItem_AnimalInitialPopulation.InputValueInt;

            result.AnimalMaximumElementAmount = SoupSettingsItem_AnimalMaximumElementAmount.InputValueDouble;
            result.AnimalElementBaseCost = SoupSettingsItem_AnimalElementBaseCost.InputValueDouble;
            result.AnimalElementAccelerationCost = SoupSettingsItem_AnimalElementAccelerationCost.InputValueDouble;
            result.AnimalElementRotationCost = SoupSettingsItem_AnimalElementRotationCost.InputValueDouble;
            result.AnimalElementEatCost = SoupSettingsItem_AnimalElementEatCost.InputValueDouble;
            result.AnimalElementAttackCost = SoupSettingsItem_AnimalElementAttackCost.InputValueDouble;
            result.AnimalElementPheromoneProductionCost = SoupSettingsItem_AnimalElementPheromoneProductionCost.InputValueDouble;

            result.AnimalPheromoneProductionRate = SoupSettingsItem_AnimalPheromoneProductionRate.InputValueDouble;

            result.AnimalMaximumAcceleration = SoupSettingsItem_AnimalMaximumAcceleration.InputValueDouble;
            result.AnimalMaximumAngularAcceleration = SoupSettingsItem_AnimalMaximumAngularAcceleration.InputValueDouble;

            result.AnimalDisableSameSpeciesAttack = SoupSettingsItem_AnimalDisableSameSpeciesAttack.Checked;
            result.AnimalUnderAttackTime = SoupSettingsItem_AnimalUnderAttackTime.InputValueInt;
            result.AnimalPlantIngestionRate = SoupSettingsItem_AnimalPlantIngestionRate.InputValueDouble;
            result.AnimalAnimalIngestionRate = SoupSettingsItem_AnimalAnimalIngestionRate.InputValueDouble;

            result.AnimalReproductionCost = SoupSettingsItem_AnimalReproductionCost.InputValueDouble;
            result.AnimalMaximumReproductionRate = SoupSettingsItem_AnimalMaximumReproductionRate.InputValueDouble;
            result.AnimalEggHatchingTime = SoupSettingsItem_AnimalEggHatchingTime.InputValueInt;
            result.AnimalEggRadiusRatio = SoupSettingsItem_AnimalEggRadiusRatio.InputValueDouble;

            result.AnimalLifespan = SoupSettingsItem_AnimalLifespan.InputValueInt;

            result.AnimalMaximumNodeCount = SoupSettingsItem_AnimalMaximumNodeCount.InputValueInt;
            result.AnimalMaximumConnectionCountPerNode = SoupSettingsItem_AnimalMaximumConnectionCountPerNode.InputValueInt;

            result.AnimalMutationRate = SoupSettingsItem_AnimalMutationRate.InputValueDouble;
            result.AnimalMaximumMutationCount = SoupSettingsItem_AnimalMaximumMutationCount.InputValueInt;
            result.AnimalMutationCountFactor = SoupSettingsItem_AnimalMutationCountFactor.InputValueDouble;
            result.AnimalDisableSpeciesSigChangeByMutation = SoupSettingsItem_AnimalDisableSpeciesSigChangeByMutation.Checked;

            result.AnimalMutationAddNodeWeight = SoupSettingsItem_AnimalMutationAddNodeWeight.InputValueDouble;
            result.AnimalMutationRemoveNodeWeight = SoupSettingsItem_AnimalMutationRemoveNodeWeight.InputValueDouble;
            result.AnimalMutationChangeNodeTypeWeight = SoupSettingsItem_AnimalMutationChangeNodeTypeWeight.InputValueDouble;
            result.AnimalMutationAddConnectionWeight = SoupSettingsItem_AnimalMutationAddConnectionWeight.InputValueDouble;
            result.AnimalMutationRemoveConnectionWeight = SoupSettingsItem_AnimalMutationRemoveConnectionWeight.InputValueDouble;
            result.AnimalMutationChangeConnectionOriginWeight = SoupSettingsItem_AnimalMutationChangeConnectionOrigin.InputValueDouble;
            result.AnimalMutationChangeConnectionTargetWeight = SoupSettingsItem_AnimalMutationChangeConnectionTarget.InputValueDouble;
            result.AnimalMutationChangeConnectionWeightWeight = SoupSettingsItem_AnimalMutationChangeConnectionWeight.InputValueDouble;

            result.AnimalNodeTypeInputWeight = SoupSettingsItem_AnimalNodeTypeInputWeight.InputValueDouble;
            result.AnimalNodeTypeHiddenWeight = SoupSettingsItem_AnimalNodeTypeHiddenWeight.InputValueDouble;
            result.AnimalNodeTypeOutputWeight = SoupSettingsItem_AnimalNodeTypeOutputWeight.InputValueDouble;

            return result;
        }
    }
}
