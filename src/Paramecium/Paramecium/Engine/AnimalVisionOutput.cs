using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public class AnimalVisionOutput
    {
        public double WallAvgAngle { get; set; }
        public double WallProximity { get; set; }
        public double PlantAvgAngle { get; set; }
        public double PlantProximity { get; set; }
        public double AnimalSameSpeciesAvgAngle { get; set; }
        public double AnimalSameSpeciesProximity { get; set; }
        public double AnimalOtherSpeciesAvgAngle { get; set; }
        public double AnimalOtherSpeciesProximity { get; set; }
        public double PheromoneRed { get; set; }
        public double PheromoneGreen { get; set; }
        public double PheromoneBlue { get; set; }
        public double PheromoneRedAvgAngle { get; set; }
        public double PheromoneGreenAvgAngle { get; set; }
        public double PheromoneBlueAvgAngle { get; set; }
    }
}
