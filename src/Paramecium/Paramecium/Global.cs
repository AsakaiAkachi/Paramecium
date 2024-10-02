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

        public static string g_SoupFilePath;

        public static string g_PresetDefaultFilePath;
        public static string g_PresetDefaultFileName;

        public static Soup? g_Soup;

        static Global()
        {
            g_AppName = "Paramecium";
            g_AppVersion = "0.5.7";

            g_SoupFilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\New Soup.soup";

            g_PresetDefaultFilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\presets\";
            g_PresetDefaultFileName = @$"New Preset.souppreset";

            g_Soup = null;
        }
    }
}
