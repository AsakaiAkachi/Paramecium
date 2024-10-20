using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public enum BrainNodeType
    {
        NonOperation,
        Input_Bias,
        Input_WallAvgAngle,
        Input_WallProximity,
        Input_PlantAvgAngle,
        Input_PlantProximity,
        Input_AnimalAvgAngle,
        Input_AnimalProximity,
        Input_AnimalSameSpeciesAvgAngle,
        Input_AnimalSameSpeciesProximity,
        Input_AnimalOtherSpeciesAvgAngle,
        Input_AnimalOtherSpeciesProximity,
        Input_Velocity,
        Input_AngularVelocity,
        Input_Satiety,
        Input_Damage,
        Input_PheromoneRed,
        Input_PheromoneGreen,
        Input_PheromoneBlue,
        Input_PheromoneRedAvgAngle,
        Input_PheromoneGreenAvgAngle,
        Input_PheromoneBlueAvgAngle,
        Input_Memory0,
        Input_Memory1,
        Input_Memory2,
        Input_Memory3,
        Input_Memory4,
        Input_Memory5,
        Input_Memory6,
        Input_Memory7,
        Hidden_ReLU,
        Hidden_LimitedReLU,
        Hidden_Step,
        Hidden_Sigmoid,
        Hidden_Tanh,
        Hidden_Gaussian,
        Hidden_Identity,
        Hidden_Absolute,
        Hidden_Sine,
        Hidden_Cosine,
        Hidden_Tangent,
        Hidden_LimitedTangent,
        Output_Acceleration,
        Output_Rotation,
        Output_Attack,
        Output_PheromoneRed,
        Output_PheromoneGreen,
        Output_PheromoneBlue,
        Output_Memory0,
        Output_Memory1,
        Output_Memory2,
        Output_Memory3,
        Output_Memory4,
        Output_Memory5,
        Output_Memory6,
        Output_Memory7
    }
}
