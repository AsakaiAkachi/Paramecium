global using static Paramecium.Global;
using Paramecium.Simulation;

namespace Paramecium
{
    public static class Global
    {
        public static Soup SoupInstance;

        static Global()
        {
            SoupInstance = new Soup();
            SoupInstance.Initialize();
        }
    }
}
