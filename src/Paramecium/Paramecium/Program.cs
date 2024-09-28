using System.Runtime.InteropServices;
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
            Console.Title = "Paramecium";

            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Simulations"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Simulations");
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}