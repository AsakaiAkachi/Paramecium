using Paramecium.Forms;
using Paramecium.Libraries;
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
            Console.Title = "Paramecium";

            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves");
            }
            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves");
            }

            Console.WriteLine(Vector2D.ToAngle(new Vector2D(1, 0)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(1, 1)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(0, 1)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(-1, 1)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(-1, 0)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(-1, -1)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(0, -1)));
            Console.WriteLine(Vector2D.ToAngle(new Vector2D(1, -1)));

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