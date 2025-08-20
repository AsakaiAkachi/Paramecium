using Paramecium.Engine;

namespace Paramecium
{
    // 全体で共有される変数を入れておく用のクラス
    public static class Globals
    {
        public static string AppName = "Paramecium";        // このアプリの名前
        public static string AppVersion = "0.7.0 indev-3";  // アプリのバージョン

        public static string SavesDirectoryPath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves";                // セーブデータのデフォルトの保存先
        public static string AutosavesDirectoryPath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves";  // オートセーブされたセーブデータの保存先
        public static string PresetDirectoryPath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\presets";       // スープ設定のプリセットのデフォルトの保存先

        public static string SoupFilePath = string.Empty;       // 現在開いているスープファイルのパス
        public static string SoupFileName = "untitled.soup";    // 現在開いているスープファイルのファイル名

        public static Soup? Soup = null;                // 現在のスープを保持しておく用
    }
}
