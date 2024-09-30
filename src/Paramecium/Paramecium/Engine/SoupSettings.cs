namespace Paramecium.Engine
{
    public class SoupSettings
    {
        // Seed Settings
        public int InitialSeed { get; set; } = 0;

        // Soup Size Settings
        public int SizeX { get; set; } = 128;
        public int SizeY { get; set; } = 128;

        // Wall Settings
        public bool WallEnabled { get; set; } = true;
        public double WallNoiseX { get; set; } = 0d;
        public double WallNoiseY { get; set; } = 0d;
        public double WallNoiseZ { get; set; } = 0d;
        public double WallNoiseScale { get; set; } = 0.03d;
        public int WallNoiseOctave { get; set; } = 4;
        public double WallThickness { get; set; } = 0.008d; // Upper Limit : 0.0125d

        // Element Settings
        public double TotalElementAmount { get; set; } = 128 * 128 * 2;
        public double ElementFlowRate { get; set; } = 0.01d;
        public double PlantElementEfficiency { get; set; } = 1d;
        public double AnimalElementEfficiency { get; set; } = 5d;

        // Pheromone Settings
        public double PheromoneFlowRate { get; set; } = 0.1d;
        public double PheromoneDecayRate { get; set; } = 0.01d;
        public double PheromoneProductionRate { get; set; } = 0.1d;

        // Physical Settings
        public double Drag { get; set; } = 0.025d;
        public double AngularVelocityDrag { get; set; } = 0.01d;
        public double MaximumVelocity { get; set; } = 0.1d;
        public double MaximumAngularVelocity { get; set; } = 0.1d;
        public double RestitutionCoefficient { get; set; } = 0.1d;

        // Plant Settings
        public int InitialPlantPopulation { get; set; } = 128 * 128 * 2 / 2 / 16;
        public double InitialPlantElementAmount { get; set; } = 16d;
        public double PlantForkCost { get; set; } = 16d;
        public int PlantForkOffspringCountMin { get; set; } = 4;
        public int PlantForkOffspringCountMax { get; set; } = 8;
        public double PlantElementCollectRate { get; set; } = 0.01d;

        // Animal Basic Settings
        public int InitialAnimalPopulation { get; set; } = 128 * 128 * 2 / 2 / 64;
        public double InitialAnimalElementAmount { get; set; } = 64d;
        public double AnimalForkCost { get; set; } = 64d;
        public double AnimalElementBaseCost { get; set; } = 0.01d;
        public double AnimalElementAccelerationCost { get; set; } = 0.02d;
        public double AnimalElementRotationCost { get; set; } = 0.01d;
        public double AnimalElementAttackCost { get; set; } = 0.02d;
        public double AnimalElementPheromoneProductionCost { get; set; } = 0.0025d;
        public double AnimalPlantIngestionRate { get; set; } = 0.2d;
        public double AnimalAnimalIngestionRate { get; set; } = 0.8d;
        public int AnimalMaximumAge { get; set; } = 15000;

        // Animal Mutation Settings
        public double AnimalMutationRate { get; set; } = 0.25d;
        public int AnimalMaximumMutationCount { get; set; } = 8;
        public double AnimalSpeciesIdMutationRate { get; set; } = 0.25d;

        // Animal Brain Settings
        public int AnimalBrainMaximumNodeCount { get; set; } = 64;
        public int AnimalBrainMaximumConnectionCount { get; set; } = 8;
    }
}
