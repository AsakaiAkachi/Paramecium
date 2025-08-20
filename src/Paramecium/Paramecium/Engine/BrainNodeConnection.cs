namespace Paramecium.Engine
{
    // 脳のノード間の接続を保存する用のクラス
    public class BrainNodeConnection
    {
        public int OriginIndex { get; set; }    // 接続元のインデックス
        public int TargetIndex { get; set; }    // 接続先のインデックス
        public double Weight { get; set; }      // 接続の重み

        // 接続の更新処理
        public void UpdateConnection(List<BrainNode> brainNodes)
        {
            brainNodes[TargetIndex].Input += brainNodes[OriginIndex].Output * Weight;
        }
    }
}
