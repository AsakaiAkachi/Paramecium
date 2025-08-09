namespace Paramecium.Engine
{
    public class BrainNodeConnection
    {
        public int OriginIndex { get; set; }
        public int TargetIndex { get; set; }
        public double Weight { get; set; }

        public void UpdateConnection(List<BrainNode> brainNodes)
        {
            brainNodes[TargetIndex].Input += brainNodes[OriginIndex].Output * Weight;
        }
    }
}
