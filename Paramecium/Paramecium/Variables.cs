using Paramecium.Simulation;

namespace Paramecium
{
    public static class Variables
    {
        public static Soup SoupInstance;

        static Variables()
        {
            SoupInstance = new Soup(256, 256, 0d, 0d, 0d, true, 0.03d, 4, 0.0085d, 65536, 65536d, 16);
            SoupInstance.SoupSetup();
        }
    }
}
