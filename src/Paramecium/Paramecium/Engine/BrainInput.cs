using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public class BrainInput
    {
        public AnimalVisionOutput VisionData { get; set; } = new AnimalVisionOutput();
        public BrainOutput PrevStepOutput { get; set; } = new BrainOutput();
        public double Velocity { get; set; }
        public double AngularVelocity { get; set; }
        public double Satiety { get; set; }
        public double Attacked { get; set; }
    }
}
