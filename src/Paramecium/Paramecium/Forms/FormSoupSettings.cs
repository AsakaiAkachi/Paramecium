using Paramecium.Engine;
using Paramecium.Utils;
using Paramecium.Variables;

namespace Paramecium.Forms
{
    // スープの設定を変更するためのForm
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
                SoupSettingsItem_SoupSizeX.Editable = false;
                SoupSettingsItem_SoupSizeY.Editable = false;

                SoupSettingsItem_SoupWallEnabled.Editable = false;
                ButtonRandomizeWallNoiseOffset.Enabled = false;
                SoupSettingsItem_SoupWallNoiseOffsetX.Editable = false;
                SoupSettingsItem_SoupWallNoiseOffsetY.Editable = false;
                SoupSettingsItem_SoupWallNoiseOffsetZ.Editable = false;
                SoupSettingsItem_SoupWallNoiseSamplingInterval.Editable = false;
                SoupSettingsItem_SoupWallNoiseOctave.Editable = false;
                SoupSettingsItem_SoupWallThickness.Editable = false;

                SoupSettingsItem_SoupTotalElementAmount.Editable = false;

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
                SoupSettings? soupSettings = JsonImportAndExport.FileImport<SoupSettings>(ImportPresetDialog.FileName);

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
                JsonImportAndExport.FileExport(ExportPresetDialog.FileName, SaveSoupSettingsFromSettingsItems());
            }
        }

        // 壁生成用ノイズをランダム化する
        private void ButtonRandomizeWallNoiseOffset_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            SoupSettingsItem_SoupWallNoiseOffsetX.InputValueDouble = rand.NextDouble() * 256d;
            SoupSettingsItem_SoupWallNoiseOffsetY.InputValueDouble = rand.NextDouble() * 256d;
            SoupSettingsItem_SoupWallNoiseOffsetZ.InputValueDouble = rand.NextDouble() * 256d;
        }

        // スープの設定をSoupSettingsから読み込む
        private void LoadSoupSettingsToSettingsItems(SoupSettings soupSettings, bool dontLoadImmutableItems)
        {
            if (!dontLoadImmutableItems)
            {
                SoupSettingsItem_SoupSizeX.InputValueInt = soupSettings.SoupSizeX;
                SoupSettingsItem_SoupSizeY.InputValueInt = soupSettings.SoupSizeY;
            }

            if (!dontLoadImmutableItems)
            {
                SoupSettingsItem_SoupWallEnabled.Checked = soupSettings.SoupWallEnabled;
                SoupSettingsItem_SoupWallNoiseOffsetX.InputValueDouble = soupSettings.SoupWallNoiseX;
                SoupSettingsItem_SoupWallNoiseOffsetY.InputValueDouble = soupSettings.SoupWallNoiseY;
                SoupSettingsItem_SoupWallNoiseOffsetZ.InputValueDouble = soupSettings.SoupWallNoiseZ;
                SoupSettingsItem_SoupWallNoiseSamplingInterval.InputValueDouble = soupSettings.SoupWallNoiseSamplingInterval;
                SoupSettingsItem_SoupWallNoiseOctave.InputValueInt = soupSettings.SoupWallNoiseOctave;
                SoupSettingsItem_SoupWallThickness.InputValueDouble = soupSettings.SoupWallThickness;
            }

            if (!dontLoadImmutableItems) SoupSettingsItem_SoupTotalElementAmount.InputValueDouble = soupSettings.SoupTotalElementAmount;
            SoupSettingsItem_ElementFlowRate.InputValueDouble = soupSettings.SoupElementFlowRate;

            SoupSettingsItem_SoupMaximumEffectivePheromoneAmount.InputValueDouble = soupSettings.SoupMaximumEffectivePheromoneAmount;
            SoupSettingsItem_SoupMinimumEffectivePheromoneAmount.InputValueDouble = soupSettings.SoupMinimumEffectivePheromoneAmount;
            SoupSettingsItem_SoupPheromoneFlowRate.InputValueDouble = soupSettings.SoupPheromoneFlowRate;
            SoupSettingsItem_SoupPheromoneDecayRate.InputValueDouble = soupSettings.SoupPheromoneDecayRate;

            SoupSettingsItem_SoupDrag.InputValueDouble = soupSettings.SoupDrag;
            SoupSettingsItem_SoupAngularVelocityDrag.InputValueDouble = soupSettings.SoupAngularVelocityDrag;
            SoupSettingsItem_SoupMaximumEffectiveVelocity.InputValueDouble = soupSettings.SoupMaximumEffectiveVelocity;
            SoupSettingsItem_SoupMaximumEffectiveAngularVelocity.InputValueDouble = soupSettings.SoupMaximumEffectiveAngularVelocity;
            SoupSettingsItem_SoupRestitutionCoefficient.InputValueDouble = soupSettings.SoupRestitutionCoefficient;

            if (!dontLoadImmutableItems) SoupSettingsItem_PlantInitialPopulation.InputValueInt = soupSettings.PlantInitialPopulation;

            SoupSettingsItem_PlantMaximumElementAmount.InputValueDouble = soupSettings.PlantMaximumElementAmount;
            SoupSettingsItem_PlantElementCollectRate.InputValueDouble = soupSettings.PlantElementCollectRate;
            SoupSettingsItem_PlantUnderAttackTime.InputValueInt = soupSettings.PlantUnderAttackTime;

            SoupSettingsItem_PlantSpreadingTime.InputValueInt = soupSettings.PlantSpreadingTime;
            SoupSettingsItem_PlantDivisionCountMin.InputValueInt = soupSettings.PlantDivisionCountMin;
            SoupSettingsItem_PlantDivisionCountMax.InputValueInt = soupSettings.PlantDivisionCountMax;

            if (!dontLoadImmutableItems) SoupSettingsItem_AnimalInitialPopulation.InputValueInt = soupSettings.AnimalInitialPopulation;

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

            SoupSettingsItem_AnimalBrainMaximumNodeCount.InputValueInt = soupSettings.AnimalBrainMaximumNodeCount;
            SoupSettingsItem_AnimalBrainMaximumConnectionCountPerNode.InputValueInt = soupSettings.AnimalBrainMaximumConnectionCountPerNode;

            SoupSettingsItem_AnimalMutationMutationRate.InputValueDouble = soupSettings.AnimalMutationMutationRate;
            SoupSettingsItem_AnimalMutationMaximumMutationCount.InputValueInt = soupSettings.AnimalMutationMaximumMutationCount;
            SoupSettingsItem_AnimalMutationMutationCountFactor.InputValueDouble = soupSettings.AnimalMutationMutationCountFactor;
            SoupSettingsItem_AnimalMutationDisableSpeciesSigChangeByMutation.Checked = soupSettings.AnimalMutationDisableSpeciesSigChangeByMutation;

            SoupSettingsItem_AnimalMutationMutationTypeAddNodeWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeAddNodeWeight;
            SoupSettingsItem_AnimalMutationMutationTypeRemoveNodeWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeRemoveNodeWeight;
            SoupSettingsItem_AnimalMutationMutationTypeChangeNodeTypeWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeChangeNodeTypeWeight;
            SoupSettingsItem_AnimalMutationMutationTypeAddConnectionWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeAddConnectionWeight;
            SoupSettingsItem_AnimalMutationMutationTypeRemoveConnectionWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeRemoveConnectionWeight;
            SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionOriginWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeChangeConnectionOriginWeight;
            SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionTargetWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeChangeConnectionTargetWeight;
            SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionWeightWeight.InputValueDouble = soupSettings.AnimalMutationMutationTypeChangeConnectionWeightWeight;

            SoupSettingsItem_AnimalMutationNodeTypeInputWeight.InputValueDouble = soupSettings.AnimalMutationNodeTypeInputWeight;
            SoupSettingsItem_AnimalMutationNodeTypeHiddenWeight.InputValueDouble = soupSettings.AnimalMutationNodeTypeHiddenWeight;
            SoupSettingsItem_AnimalMutationNodeTypeOutputWeight.InputValueDouble = soupSettings.AnimalMutationNodeTypeOutputWeight;
        }

        // スープの設定をSoupSettingsに書き込む
        private SoupSettings SaveSoupSettingsFromSettingsItems()
        {
            SoupSettings result = new SoupSettings();

            result.SoupSizeX = SoupSettingsItem_SoupSizeX.InputValueInt;
            result.SoupSizeY = SoupSettingsItem_SoupSizeY.InputValueInt;

            result.SoupWallEnabled = SoupSettingsItem_SoupWallEnabled.Checked;
            result.SoupWallNoiseX = SoupSettingsItem_SoupWallNoiseOffsetX.InputValueDouble;
            result.SoupWallNoiseY = SoupSettingsItem_SoupWallNoiseOffsetY.InputValueDouble;
            result.SoupWallNoiseZ = SoupSettingsItem_SoupWallNoiseOffsetZ.InputValueDouble;
            result.SoupWallNoiseSamplingInterval = SoupSettingsItem_SoupWallNoiseSamplingInterval.InputValueDouble;
            result.SoupWallNoiseOctave = SoupSettingsItem_SoupWallNoiseOctave.InputValueInt;
            result.SoupWallThickness = SoupSettingsItem_SoupWallThickness.InputValueDouble;

            result.SoupTotalElementAmount = SoupSettingsItem_SoupTotalElementAmount.InputValueDouble;
            result.SoupElementFlowRate = SoupSettingsItem_ElementFlowRate.InputValueDouble;

            result.SoupMaximumEffectivePheromoneAmount = SoupSettingsItem_SoupMaximumEffectivePheromoneAmount.InputValueDouble;
            result.SoupMinimumEffectivePheromoneAmount = SoupSettingsItem_SoupMinimumEffectivePheromoneAmount.InputValueDouble;
            result.SoupPheromoneFlowRate = SoupSettingsItem_SoupPheromoneFlowRate.InputValueDouble;
            result.SoupPheromoneDecayRate = SoupSettingsItem_SoupPheromoneDecayRate.InputValueDouble;

            result.SoupDrag = SoupSettingsItem_SoupDrag.InputValueDouble;
            result.SoupAngularVelocityDrag = SoupSettingsItem_SoupAngularVelocityDrag.InputValueDouble;
            result.SoupMaximumEffectiveVelocity = SoupSettingsItem_SoupMaximumEffectiveVelocity.InputValueDouble;
            result.SoupMaximumEffectiveAngularVelocity = SoupSettingsItem_SoupMaximumEffectiveAngularVelocity.InputValueDouble;
            result.SoupRestitutionCoefficient = SoupSettingsItem_SoupRestitutionCoefficient.InputValueDouble;

            result.PlantInitialPopulation = SoupSettingsItem_PlantInitialPopulation.InputValueInt;

            result.PlantMaximumElementAmount = SoupSettingsItem_PlantMaximumElementAmount.InputValueDouble;
            result.PlantElementCollectRate = SoupSettingsItem_PlantElementCollectRate.InputValueDouble;
            result.PlantUnderAttackTime = SoupSettingsItem_PlantUnderAttackTime.InputValueInt;

            result.PlantSpreadingTime = SoupSettingsItem_PlantSpreadingTime.InputValueInt;
            result.PlantDivisionCountMin = SoupSettingsItem_PlantDivisionCountMin.InputValueInt;
            result.PlantDivisionCountMax = SoupSettingsItem_PlantDivisionCountMax.InputValueInt;

            result.AnimalInitialPopulation = SoupSettingsItem_AnimalInitialPopulation.InputValueInt;

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

            result.AnimalBrainMaximumNodeCount = SoupSettingsItem_AnimalBrainMaximumNodeCount.InputValueInt;
            result.AnimalBrainMaximumConnectionCountPerNode = SoupSettingsItem_AnimalBrainMaximumConnectionCountPerNode.InputValueInt;

            result.AnimalMutationMutationRate = SoupSettingsItem_AnimalMutationMutationRate.InputValueDouble;
            result.AnimalMutationMaximumMutationCount = SoupSettingsItem_AnimalMutationMaximumMutationCount.InputValueInt;
            result.AnimalMutationMutationCountFactor = SoupSettingsItem_AnimalMutationMutationCountFactor.InputValueDouble;
            result.AnimalMutationDisableSpeciesSigChangeByMutation = SoupSettingsItem_AnimalMutationDisableSpeciesSigChangeByMutation.Checked;

            result.AnimalMutationMutationTypeAddNodeWeight = SoupSettingsItem_AnimalMutationMutationTypeAddNodeWeight.InputValueDouble;
            result.AnimalMutationMutationTypeRemoveNodeWeight = SoupSettingsItem_AnimalMutationMutationTypeRemoveNodeWeight.InputValueDouble;
            result.AnimalMutationMutationTypeChangeNodeTypeWeight = SoupSettingsItem_AnimalMutationMutationTypeChangeNodeTypeWeight.InputValueDouble;
            result.AnimalMutationMutationTypeAddConnectionWeight = SoupSettingsItem_AnimalMutationMutationTypeAddConnectionWeight.InputValueDouble;
            result.AnimalMutationMutationTypeRemoveConnectionWeight = SoupSettingsItem_AnimalMutationMutationTypeRemoveConnectionWeight.InputValueDouble;
            result.AnimalMutationMutationTypeChangeConnectionOriginWeight = SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionOriginWeight.InputValueDouble;
            result.AnimalMutationMutationTypeChangeConnectionTargetWeight = SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionTargetWeight.InputValueDouble;
            result.AnimalMutationMutationTypeChangeConnectionWeightWeight = SoupSettingsItem_AnimalMutationMutationTypeChangeConnectionWeightWeight.InputValueDouble;

            result.AnimalMutationNodeTypeInputWeight = SoupSettingsItem_AnimalMutationNodeTypeInputWeight.InputValueDouble;
            result.AnimalMutationNodeTypeHiddenWeight = SoupSettingsItem_AnimalMutationNodeTypeHiddenWeight.InputValueDouble;
            result.AnimalMutationNodeTypeOutputWeight = SoupSettingsItem_AnimalMutationNodeTypeOutputWeight.InputValueDouble;

            return result;
        }
    }
}
