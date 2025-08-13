using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    public class BrainNode
    {
        public BrainNodeFunction Function { get; set; }
        public double Input { get; set; }
        public double Output { get; set; }

        [JsonIgnore]
        public bool IsInput { get { return (int)Function >= (int)BrainNodeFunction.Input_Bias && (int)Function <= (int)BrainNodeFunction.Input_PheromoneBlueGradAngle; } }
        [JsonIgnore]
        public bool IsHidden { get { return (int)Function >= (int)BrainNodeFunction.Hidden_ReLU && (int)Function <= (int)BrainNodeFunction.Hidden_Frac; } }
        [JsonIgnore]
        public bool IsOutput { get { return (int)Function >= (int)BrainNodeFunction.Output_Acceleration && (int)Function <= (int)BrainNodeFunction.Output_PheromoneBlueProduction; } }

        public void ApplyBrainInput(Brain brain, BrainInput brainInput)
        {
            if (IsInput)
            {
                switch (Function)
                {
                    case BrainNodeFunction.Input_Bias:
                        Output = 1d;
                        break;

                    case BrainNodeFunction.Input_Velocity:
                        Output = brainInput.Velocity;
                        break;
                    case BrainNodeFunction.Input_AngularVelocity:
                        Output = brainInput.AngularVelocity;
                        break;
                    case BrainNodeFunction.Input_Element:
                        Output = brainInput.Element;
                        break;
                    case BrainNodeFunction.Input_ReproductionProgress:
                        Output = brainInput.ReproductionProgress;
                        break;
                    case BrainNodeFunction.Input_Age:
                        Output = brainInput.Age;
                        break;

                    case BrainNodeFunction.Input_Ate:
                        Output = brainInput.Ate;
                        break;
                    case BrainNodeFunction.Input_Attacked:
                        Output = brainInput.Attacked;
                        break;
                    case BrainNodeFunction.Input_AttackdAngle:
                        Output = brainInput.AttackedAngle;
                        break;
                    case BrainNodeFunction.Input_AttackSuccessful:
                        Output = brainInput.AttackSuccessful;
                        break;

                    case BrainNodeFunction.Input_WallWAvgAngle:
                        Output = brainInput.VisionData.WallWAvgAngle;
                        break;
                    case BrainNodeFunction.Input_WallWAvgProximity:
                        Output = brainInput.VisionData.WallWAvgProximity;
                        break;
                    case BrainNodeFunction.Input_WallWAvgDistance:
                        Output = brainInput.VisionData.WallWAvgDistance;
                        break;
                    case BrainNodeFunction.Input_PlantWAvgAngle:
                        Output = brainInput.VisionData.PlantWAvgAngle;
                        break;
                    case BrainNodeFunction.Input_PlantWAvgProximity:
                        Output = brainInput.VisionData.PlantWAvgProximity;
                        break;
                    case BrainNodeFunction.Input_PlantWAvgDistance:
                        Output = brainInput.VisionData.PlantWAvgDistance;
                        break;
                    case BrainNodeFunction.Input_AnimalWAvgAngle:
                        Output = brainInput.VisionData.AnimalWAvgAngle;
                        break;
                    case BrainNodeFunction.Input_AnimalWAvgProximity:
                        Output = brainInput.VisionData.AnimalWAvgProximity;
                        break;
                    case BrainNodeFunction.Input_AnimalWAvgDistance:
                        Output = brainInput.VisionData.AnimalWAvgDistance;
                        break;
                    case BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff:
                        Output = brainInput.VisionData.AnimalWAvgSpeciesSigDiff;
                        break;

                    case BrainNodeFunction.Input_PheromoneRedConcentration:
                        Output = brainInput.VisionData.PheromoneRedConcentration;
                        break;
                    case BrainNodeFunction.Input_PheromoneRedGradAngle:
                        Output = brainInput.VisionData.PheromoneRedGradAngle;
                        break;
                    case BrainNodeFunction.Input_PheromoneGreenConcentration:
                        Output = brainInput.VisionData.PheromoneGreenConcentration;
                        break;
                    case BrainNodeFunction.Input_PheromoneGreenGradAngle:
                        Output = brainInput.VisionData.PheromoneGreenGradAngle;
                        break;
                    case BrainNodeFunction.Input_PheromoneBlueConcentration:
                        Output = brainInput.VisionData.PheromoneBlueConcentration;
                        break;
                    case BrainNodeFunction.Input_PheromoneBlueGradAngle:
                        Output = brainInput.VisionData.PheromoneBlueGradAngle;
                        break;
                }
            }
        }

        public void CalculateNodeOutput()
        {
            if (IsHidden)
            {
                switch (Function)
                {
                    case BrainNodeFunction.Hidden_ReLU:
                        Output = double.Max(0d, Input);
                        break;
                    case BrainNodeFunction.Hidden_LimitedReLU:
                        Output = double.Max(0d, double.Min(1d, Input));
                        break;
                    case BrainNodeFunction.Hidden_Step:
                        if (Input > 0) Output = 1;
                        else Output = 0;
                        break;
                    case BrainNodeFunction.Hidden_Sigmoid:
                        Output = Math.Tanh(Input);
                        break;
                    case BrainNodeFunction.Hidden_Identity:
                        Output = Input;
                        break;
                    case BrainNodeFunction.Hidden_Absolute:
                        Output = double.Abs(Input);
                        break;
                    case BrainNodeFunction.Hidden_Sine:
                        Output = Math.Sin(Input * Math.PI / 2d);
                        break;
                    case BrainNodeFunction.Hidden_Cosine:
                        Output = Math.Cos(Input * Math.PI / 2d);
                        break;
                    case BrainNodeFunction.Hidden_Tangent:
                        Output = Math.Tan(Input / Math.PI / 2d);
                        if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 100;
                        break;
                    case BrainNodeFunction.Hidden_LimitedTangent:
                        Output = Math.Tan(Input / Math.PI / 2d);
                        if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 1;
                        Output = double.Max(-1d, double.Min(1d, Output));
                        break;
                    case BrainNodeFunction.Hidden_Frac:
                        Output = Input - double.Floor(Input);
                        break;
                }
            }

            Input = 0d;

            if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 0;
            Output = double.Max(-100d, double.Min(100d, Output));
        }

        public void ApplyBrainOutput(ref BrainOutput brainOutput)
        {
            if (IsOutput)
            {
                switch (Function)
                {
                    case BrainNodeFunction.Output_Acceleration:
                        brainOutput.Acceleration += Input;
                        break;
                    case BrainNodeFunction.Output_Rotation:
                        brainOutput.Rotation += Input;
                        break;
                    case BrainNodeFunction.Output_Eat:
                        brainOutput.Eat += Input;
                        break;
                    case BrainNodeFunction.Output_Attack:
                        brainOutput.Attack += Input;
                        break;

                    case BrainNodeFunction.Output_Reproduction:
                        brainOutput.Reproduction += Input;
                        break;

                    case BrainNodeFunction.Output_PheromoneRedProduction:
                        brainOutput.PheromoneRedProduction += Input;
                        break;
                    case BrainNodeFunction.Output_PheromoneGreenProduction:
                        brainOutput.PheromoneGreenProduction += Input;
                        break;
                    case BrainNodeFunction.Output_PheromoneBlueProduction:
                        brainOutput.PheromoneBlueProduction += Input;
                        break;
                }
            }
        }
    }
}
