global using static Paramecium.Global;
using Paramecium.Engine;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paramecium
{
    public static class Global
    {
        public static string g_AppName;
        public static string g_AppVersion;

        public static string g_FilePath;

        public static Soup? g_Soup;

        static Global()
        {
            g_AppName = "Paramecium";
            g_AppVersion = "0.5.0";

            g_FilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\New Soup.soup";

            g_Soup = null;
        }
    }
}
