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
            Console.Title = "Paramecium Debug Console";

            ApplicationConfiguration.Initialize();

            global_FormMain = new FormMain();
            Application.Run(global_FormMain);
        }
    }
}