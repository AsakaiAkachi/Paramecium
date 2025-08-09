using Paramecium.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace Paramecium
{
    // エントリポイント用のクラス
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [STAThread]
        static void Main()
        {
            // コマンドライン引数に「-debug」が指定されていた場合、デバッグ用のコンソールを表示する
            if (Environment.GetCommandLineArgs().Contains("-debug"))
            {
                AllocConsole();
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                Console.Title = $"{Globals.AppName} Debug Console";
            }

            // セーブデータを保存する用のディレクトリが存在するか確認し、存在しなければ作成する
            if (!Directory.Exists(Globals.SavesDirectoryPath))
            {
                Directory.CreateDirectory(Globals.SavesDirectoryPath);
            }
            if (!Directory.Exists(Globals.AutosavesDirectoryPath))
            {
                Directory.CreateDirectory(Globals.AutosavesDirectoryPath);
            }
            if (!Directory.Exists(Globals.PresetDirectoryPath))
            {
                Directory.CreateDirectory(Globals.PresetDirectoryPath);
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}