namespace Paramecium.Engine
{
    // 脳から出力されるデータが格納されるクラス
    public class BrainOutput
    {
        public double Acceleration { get; set; }
        public double Rotation { get; set; }
        public double Eat { get; set; }
        public double Attack { get; set; }

        public double Reproduction { get; set; }

        public double PheromoneRedProduction { get; set; }
        public double PheromoneGreenProduction { get; set; }
        public double PheromoneBlueProduction { get; set; }
    }
}
