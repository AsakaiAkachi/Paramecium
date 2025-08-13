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
        public double Element { get; set; }
        public double ReproductionProgress { get; set; }
        public double Age { get; set; }

        public double Ate { get; set; }
        public double Attacked { get; set; }
        public double AttackedAngle { get; set; }
        public double AttackSuccessful { get; set; }
    }
}
