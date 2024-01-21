namespace Paramecium.Simulation
{
    public class Grid
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }

        public List<int> LocalPlants { get; set; }
        public List<int> LocalAnimals { get; set; }
        public List<int> LocalParticles { get; set; }

        public double Fertility { get; set; }

        public Grid(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            LocalPlants = new List<int>();
            LocalAnimals = new List<int>();
            LocalParticles = new List<int>();

            Fertility = 0d;
        }
    }

    public enum TileType
    {
        None,
        Wall
    }
}
