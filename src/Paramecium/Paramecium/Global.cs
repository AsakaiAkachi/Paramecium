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

        public static string g_SoupDefaultFilePath;
        public static string g_SoupDefaultFileName;

        public static string g_SoupAutosaveFilePath;

        public static string g_PresetDefaultFilePath;
        public static string g_PresetDefaultFileName;

        public static Soup? g_Soup;

        static Global()
        {
            g_AppName = "Paramecium";
            g_AppVersion = "0.5.9";

            g_SoupDefaultFilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations";
            g_SoupDefaultFileName = @$"Untitled.soup";

            g_SoupAutosaveFilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\simulations\autosaves";

            g_PresetDefaultFilePath = @$"{Path.GetDirectoryName(Application.ExecutablePath)}\presets";
            g_PresetDefaultFileName = @$"Untitled.souppreset";

            g_Soup = null;
        }
    }
}
