global using static Paramecium.Global;
using Paramecium.Engine;

namespace Paramecium
{
    public static class Global
    {
        public static Soup g_Soup;

        static Global()
        {
            g_Soup = new Soup();
            g_Soup.InitializeSoup();
            g_Soup.StartSoupThread();
        }
    }
}
