using System.Runtime.InteropServices;
using System.Text;
using Paramecium.Forms;

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
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Paramecium";

            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations");
            }
            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\autosaves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\autosaves");
            }
            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\presets"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\presets");
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}