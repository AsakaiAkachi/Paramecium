namespace Paramecium.Engine
{
    public class Tile
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public TileType Type { get; set; } = TileType.Default;

        public double Element { get; set; } = 0d;

        public List<int> LocalPlantIndexes { get; set; } = new List<int>();
        public int LocalPlantPopulation { get => LocalPlantIndexes.Count; }

        public List<int> LocalAnimalIndexes { get; set; } = new List<int>();
        public int LocalAnimalPopulation { get => LocalAnimalIndexes.Count; }

        public Tile(int positionX, int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }
    }

    public enum TileType
    {
        Default,
        Wall
    }
}
