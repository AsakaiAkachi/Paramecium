namespace Paramecium.Engine
{
    // タイルの種別を表す列挙型
    public enum TileType
    {
        Default,    // デフォルト、特に効果なし
        Wall        // 壁、このタイルの中に動植物は進入できず、エレメントやフェロモンも透過しない
    }
}
