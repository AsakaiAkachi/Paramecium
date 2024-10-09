using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public class BrainInput
    {
        public AnimalVisionOutput VisionData = new AnimalVisionOutput();
        public BrainOutput PrevStepOutput = new BrainOutput();
        public double Velocity;
        public double AngularVelocity;
        public double Satiety;
        public double Damage;
    }
}
