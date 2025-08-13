using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public class BrainOutput
    {
        public double Acceleration { get; set; }
        public double Rotation { get; set; }
        public double Eat { get; set; }
        public double Attack { get; set; }

        public double Reproduction { get; set; }

        public double PheromoneRedProduction { get; set; }
        public double PheromoneGreenProduction { get; set; }
        public double PheromoneBlueProduction { get; set; }
    }
}
