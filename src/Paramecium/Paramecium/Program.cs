using Paramecium.Forms;
using Paramecium.Libs;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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

            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}