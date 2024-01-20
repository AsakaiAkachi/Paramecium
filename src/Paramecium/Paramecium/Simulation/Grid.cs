namespace Paramecium.Simulation
{
    public class Grid
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }

        public List<int> LocalParticles { get; set; }
        public int LocalParticleCount { get; set; }

        public double Biomass { get; set; }

        public Grid(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            LocalParticles = new List<int>();
            LocalParticleCount = 0;

            Biomass = 0d;
        }
    }

    public enum TileType
    {
        None,
        Wall
    }
}
