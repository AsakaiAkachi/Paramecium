using Paramecium.Variables;

namespace Paramecium.Engine
{
    // 動物の周辺認識の情報が格納されているクラス
    public class AnimalVisionOutput
    {
        // 視覚情報
        public double WallWAvgAngle { get; set; }
        public double WallWAvgProximity { get; set; }
        public double WallWAvgDistance { get; set; }
        public double PlantWAvgAngle { get; set; }
        public double PlantWAvgProximity { get; set; }
        public double PlantWAvgDistance { get; set; }
        public double AnimalWAvgAngle { get; set; }
        public double AnimalWAvgProximity { get; set; }
        public double AnimalWAvgDistance { get; set; }
        public double AnimalWAvgSpeciesSigDiff { get; set; }

        // フェロモン情報
        public double PheromoneRedConcentration { get; set; }
        public double PheromoneGreenConcentration { get; set; }
        public double PheromoneBlueConcentration { get; set; }
        public double PheromoneGreenGradAngle { get; set; }
        public double PheromoneRedGradAngle { get; set; }
        public double PheromoneBlueGradAngle { get; set; }
    }
}
