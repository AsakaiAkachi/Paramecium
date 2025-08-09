namespace Paramecium.Engine
{
    // スープの状態を管理する用の列挙型
    public enum SoupState
    {
        Stop,       // 停止
        Pause,      // 一時停止
        Running,    // 実行中
        StepRun,    // 1ステップだけ実行中
        Saving      // 保存中
    }
}
