global using static Paramecium.Global;
global using Paramecium.Forms;
global using Paramecium.Simulation;
global using Paramecium.Libraries;

namespace Paramecium
{
    public static class Global
    {
        public static Soup g_Soup;
        public static FormMain g_FormMain;
        public static FormNewSimulation g_FormNewSimulation;

        public static SolidBrush SolidBrushPlaceholder = new SolidBrush(Color.FromArgb(255, 0, 255));

        static Global()
        {
            /**
            g_Soup = new Soup(
                512, 256,
                79.14649528369992, 97.49842312725215, 244.70658351525472,
                0.03d, 4, 0.01,
                262144,
                0.5, 15, 60, 3,
                32, 3000
            );
            g_Soup.SoupSetup();
            **/
            g_FormMain = new FormMain();
            g_FormNewSimulation = new FormNewSimulation();
        }

        public static void ConsoleLog(LogLevel logLevel, string logText)
        {
            string LogLevelText = "";
            if (logLevel == LogLevel.Information) LogLevelText = "INFO";
            else if (logLevel == LogLevel.Warning) LogLevelText = "WARN";
            else if (logLevel == LogLevel.Failure) LogLevelText = "FAIL";

            Console.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}] [{LogLevelText}] : {logText}");
        }
    }

    public enum LogLevel
    {
        Information,
        Warning,
        Failure
    }
}
