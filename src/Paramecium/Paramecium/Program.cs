using Paramecium.Forms;
using Paramecium.Libs;
using System.IO;
using System.Runtime.InteropServices;

namespace Paramecium
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [STAThread]
        static void Main()
        {
            AllocConsole();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves");
            }
            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves");
            }

            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(g_FormMain);
            }
            catch (Exception ex)
            {
                ConsoleLog(LogLevel.Failure, ex.ToString());
            }
        }
    }
}