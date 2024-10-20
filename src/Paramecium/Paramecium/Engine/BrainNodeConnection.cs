namespace Paramecium.Engine
{
    public class BrainNodeConnection
    {
        public int OriginIndex { get; set; }
        public int TargetIndex { get; set; }

        public double Weight { get; set; }

        public BrainNodeConnection Duplicate()
        {
            BrainNodeConnection result = new BrainNodeConnection();

            result.OriginIndex = OriginIndex;
            result.TargetIndex = TargetIndex;

            result.Weight = Weight;

            return result;
        }
    }
}
