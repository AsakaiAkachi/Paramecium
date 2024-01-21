global using static Paramecium.Global;
using Paramecium.Simulation;

namespace Paramecium
{
    public static class Global
    {
        public static Soup g_Soup;

        static Global()
        {
            g_Soup = new Soup(512, 256, 0d, 0d, 0d, true, 0.03d, 4, 0.01d, 262144d, 32);
            g_Soup.SoupSetup();
        }
    }
}
