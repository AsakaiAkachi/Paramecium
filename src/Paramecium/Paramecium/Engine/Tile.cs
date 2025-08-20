using Paramecium.Variables;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // スープの空間を構成するタイルのデータを保存するクラス
    public class Tile
    {
        public int Index { get; set; } = -1;                                // タイルのインデックス
        public Int2d Position { get; set; } = Int2d.Zero;                   // タイルの位置

        public TileType Type { get; set; } = TileType.Default;              // タイルの種別

        public double Element { get; set; } = 0d;                           // タイルのエレメント量
        public double ElementBuffer = 0d;

        public double PheromoneRed { get; set; } = 0d;                      // フェロモン
        public double PheromoneRedBuffer = 0d;
        public double PheromoneGreen { get; set; } = 0d;
        public double PheromoneGreenBuffer = 0d;
        public double PheromoneBlue { get; set; } = 0d;
        public double PheromoneBlueBuffer = 0d;

        public List<int> PlantIndexes { get; set; } = new List<int>();      // タイル内にある植物のインデックス
        [JsonIgnore]
        public int PlantPopulation { get => PlantIndexes.Count; }           // タイル内の植物の数

        public List<int> AnimalIndexes { get; set; } = new List<int>();     // タイル内にある動物のインデックス
        [JsonIgnore]
        public int AnimalPopulation { get => AnimalIndexes.Count; }         // タイル内の動物の数

        public object LockObject = new object();                            // タイルの値を複数スレッドから操作する際の排他制御用オブジェクト

        // 現在のステップの処理が始まった際に最初に実行されるメソッド
        public void OnStepStart(Soup soup, SoupSettings settings)
        {
            ElementBuffer = Element;
            PheromoneRedBuffer = PheromoneRed;
            PheromoneGreenBuffer = PheromoneGreen;
            PheromoneBlueBuffer = PheromoneBlue;
        }

        // 現在のステップの処理が終わる直前に実行されるメソッド
        public void OnStepEnd(Soup soup, SoupSettings settings)
        {
            Element = ElementBuffer;
            PheromoneRed = PheromoneRedBuffer;
            PheromoneGreen = PheromoneGreenBuffer;
            PheromoneBlue = PheromoneBlueBuffer;
        }
    }
}
