using Paramecium.Variables;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // スープの空間を構成するタイルのデータを保存するクラス
    public class Tile
    {
        public int Index { get; set; } = -1;
        public Int2d Position { get; set; } = Int2d.Zero;

        public TileType Type { get; set; } = TileType.Default;              // タイルの種別

        public double Element { get; set; } = 0d;                           // タイルのエレメント量

        public double PheromoneRed { get; set; } = 0d;                      // フェロモン
        public double PheromoneGreen { get; set; } = 0d;
        public double PheromoneBlue { get; set; } = 0d;

        public double Temperature { get; set; } = 1d;                       // タイルの温度
        public double Fertility { get; set; } = 1d;                         // タイルの肥沃度

        public List<int> PlantIndexes { get; set; } = new List<int>();      // タイル内にある植物のインデックス
        [JsonIgnore]
        public int PlantPopulation { get => PlantIndexes.Count; }           // タイル内の植物の数

        public List<int> AnimalIndexes { get; set; } = new List<int>();     // タイル内にある動物のインデックス
        [JsonIgnore]
        public int AnimalPopulation { get => AnimalIndexes.Count; }         // タイル内の動物の数

        public object LockObject = new object();                            // タイルの値を複数スレッドから操作する際の排他制御用オブジェクト
    }
}
