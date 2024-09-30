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

            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations");
            }
            if (!Directory.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\autosaves"))
            {
                Directory.CreateDirectory($@"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\autosaves");
            }

            List<int> test = new List<int>();
            test.Clear();
            List<int> test2 = new List<int>(test);
            Console.WriteLine(test2.Count);

            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}