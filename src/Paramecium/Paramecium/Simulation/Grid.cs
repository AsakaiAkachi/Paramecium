namespace Paramecium.Simulation
{
    public class Grid
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }

        public List<int> LocalParticles { get; set; }
        public int LocalParticleCount { get; set; }
        public int LocalPlantCount { get; set; }
        public int LocalAnimalCount { get; set; }

        public double Fertility { get; set; }
        public bool NeighborhoodGridBiomass { get; set; }

        public Grid(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            LocalParticles = new List<int>();
            LocalParticleCount = 0;
            LocalPlantCount = 0;
            LocalAnimalCount = 0;

            Fertility = 0d;

            NeighborhoodGridBiomass = false;
        }
    }

    public enum TileType
    {
        None,
        Wall
    }
}
