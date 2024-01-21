global using static Paramecium.Global;
using Paramecium.Simulation;

namespace Paramecium
{
    public static class Global
    {
        public static Soup SoupInstance;

        static Global()
        {
            SoupInstance = new Soup(512, 256, 0d, 0d, 0d, true, 0.03d, 4, 0.01d, 655360, 262144d, 8);
            SoupInstance.SoupSetup();
        }
    }
}
