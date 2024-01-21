global using static Paramecium.Global;
using Paramecium.Simulation;

namespace Paramecium
{
    public static class Global
    {
        public static Soup global_Soup;

        static Global()
        {
            global_Soup = new Soup();
            global_Soup.Initialize();
        }
    }
}
