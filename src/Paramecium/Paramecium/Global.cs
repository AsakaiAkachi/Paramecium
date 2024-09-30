global using static Paramecium.Global;
using Paramecium.Engine;

namespace Paramecium
{
    public static class Global
    {
        public static string g_AppName;
        public static string g_AppVersion;

        public static string g_FilePath;

        public static Soup g_Soup;

        static Global()
        {
            g_AppName = "Paramecium";
            g_AppVersion = "0.5.0 indev-5";

            g_FilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\New Soup.soup";

            g_Soup = new Soup();
            g_Soup.InitializeSoup();
            g_Soup.StartSoupThread();
        }
    }
}
