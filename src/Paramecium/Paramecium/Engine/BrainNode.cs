using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public class BrainNode
    {
        public BrainNodeType Type { get; set; }
        public List<BrainNodeConnection> Connections { get; set; } = new List<BrainNodeConnection>();

        public double Input { get; set; }
        public double Output { get; set; }

        [JsonIgnore]
        public bool IsInput { get { return (int)Type >= (int)BrainNodeType.Input_Bias && (int)Type <= (int)BrainNodeType.Input_Memory7; } }
        [JsonIgnore]
        public bool IsHidden { get { return (int)Type >= (int)BrainNodeType.Hidden_ReLU && (int)Type <= (int)BrainNodeType.Hidden_LimitedTangent; } }
        [JsonIgnore]
        public bool IsOutput { get { return (int)Type >= (int)BrainNodeType.Output_Acceleration && (int)Type <= (int)BrainNodeType.Output_Memory7; } }

        public static bool BrainNodeTypeIsInput(BrainNodeType type)
        {
            if ((int)type >= (int)BrainNodeType.Input_Bias && (int)type <= (int)BrainNodeType.Input_Memory7) return true;
            else return false;
        }
        public static bool BrainNodeTypeIsHidden(BrainNodeType type)
        {
            if ((int)type >= (int)BrainNodeType.Hidden_ReLU && (int)type <= (int)BrainNodeType.Hidden_LimitedTangent) return true;
            else return false;
        }
        public static bool BrainNodeTypeIsOutput(BrainNodeType type)
        {
            if ((int)type >= (int)BrainNodeType.Output_Acceleration && (int)type <= (int)BrainNodeType.Output_Memory7) return true;
            else return false;
        }

        public void ApplyBrainInput(BrainInput brainInput)
        {
            switch (Type)
            {
                case BrainNodeType.Input_Bias:
                    Output = 1d;
                    break;
                case BrainNodeType.Input_WallAvgAngle:
                    Output = brainInput.VisionData.WallAvgAngle;
                    break;
                case BrainNodeType.Input_WallProximity:
                    Output = brainInput.VisionData.WallProximity;
                    break;
                case BrainNodeType.Input_PlantAvgAngle:
                    Output = brainInput.VisionData.PlantAvgAngle;
                    break;
                case BrainNodeType.Input_PlantProximity:
                    Output = brainInput.VisionData.PlantProximity;
                    break;
                case BrainNodeType.Input_AnimalSameSpeciesAvgAngle:
                    Output = brainInput.VisionData.AnimalSameSpeciesAvgAngle;
                    break;
                case BrainNodeType.Input_AnimalSameSpeciesProximity:
                    Output = brainInput.VisionData.AnimalSameSpeciesProximity;
                    break;
                case BrainNodeType.Input_AnimalOtherSpeciesAvgAngle:
                    Output = brainInput.VisionData.AnimalOtherSpeciesAvgAngle;
                    break;
                case BrainNodeType.Input_AnimalOtherSpeciesProximity:
                    Output = brainInput.VisionData.AnimalOtherSpeciesProximity;
                    break;
                case BrainNodeType.Input_Velocity:
                    Output = brainInput.Velocity;
                    break;
                case BrainNodeType.Input_AngularVelocity:
                    Output = brainInput.AngularVelocity;
                    break;
                case BrainNodeType.Input_Satiety:
                    Output = brainInput.Satiety;
                    break;
                case BrainNodeType.Input_PheromoneRed:
                    Output = brainInput.VisionData.PheromoneRed;
                    break;
                case BrainNodeType.Input_PheromoneGreen:
                    Output = brainInput.VisionData.PheromoneGreen;
                    break;
                case BrainNodeType.Input_PheromoneBlue:
                    Output = brainInput.VisionData.PheromoneBlue;
                    break;
                case BrainNodeType.Input_PheromoneRedAvgAngle:
                    Output = brainInput.VisionData.PheromoneRedAvgAngle;
                    break;
                case BrainNodeType.Input_PheromoneGreenAvgAngle:
                    Output = brainInput.VisionData.PheromoneGreenAvgAngle;
                    break;
                case BrainNodeType.Input_PheromoneBlueAvgAngle:
                    Output = brainInput.VisionData.PheromoneBlueAvgAngle;
                    break;
                case BrainNodeType.Input_Memory0:
                    Output = brainInput.PrevStepOutput.Memory0;
                    break;
                case BrainNodeType.Input_Memory1:
                    Output = brainInput.PrevStepOutput.Memory1;
                    break;
                case BrainNodeType.Input_Memory2:
                    Output = brainInput.PrevStepOutput.Memory2;
                    break;
                case BrainNodeType.Input_Memory3:
                    Output = brainInput.PrevStepOutput.Memory3;
                    break;
                case BrainNodeType.Input_Memory4:
                    Output = brainInput.PrevStepOutput.Memory4;
                    break;
                case BrainNodeType.Input_Memory5:
                    Output = brainInput.PrevStepOutput.Memory5;
                    break;
                case BrainNodeType.Input_Memory6:
                    Output = brainInput.PrevStepOutput.Memory6;
                    break;
                case BrainNodeType.Input_Memory7:
                    Output = brainInput.PrevStepOutput.Memory7;
                    break;
                default:
                    Output = 0d;
                    break;
            }

            if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 0;
            Output = double.Max(-100d, double.Min(100d, Output));
        }

        public void CalculateNodeOutput()
        {
            switch (Type)
            {
                case BrainNodeType.Hidden_ReLU:
                    Output = double.Max(0d, Input);
                    break;
                case BrainNodeType.Hidden_LimitedReLU:
                    Output = double.Max(0d, double.Min(1d, Input));
                    break;
                case BrainNodeType.Hidden_Step:
                    if (Input > 0) Output = 1;
                    else Output = 0;
                    break;
                case BrainNodeType.Hidden_Sigmoid:
                    Output = Math.Tanh(Input);
                    break;
                case BrainNodeType.Hidden_Identity:
                    Output = Input;
                    break;
                case BrainNodeType.Hidden_Absolute:
                    Output = double.Abs(Input);
                    break;
                case BrainNodeType.Hidden_Sine:
                    Output = Math.Sin(Input * Math.PI / 2d);
                    break;
                case BrainNodeType.Hidden_Cosine:
                    Output = Math.Cos(Input * Math.PI / 2d);
                    break;
                case BrainNodeType.Hidden_Tangent:
                    Output = Math.Tan(Input * Math.PI / 2d);
                    if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 100;
                    break;
                case BrainNodeType.Hidden_LimitedTangent:
                    Output = Math.Tan(Input * Math.PI / 2d);
                    if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 1;
                    Output = double.Max(-1d, double.Min(1d, Output));
                    break;
                default:
                    break;
            }
            Input = 0d;

            if (double.IsInfinity(Output) || double.IsNaN(Output)) Output = 0;
            Output = double.Max(-100d, double.Min(100d, Output));
        }

        public void SendNodeOutput(ref List<BrainNode> brainNodes)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                brainNodes[Connections[i].TargetIndex].Input += Output * Connections[i].Weight;
            }
        }

        public void ApplyBrainOutput(ref BrainOutput brainOutput)
        {
            switch (Type)
            {
                case BrainNodeType.Output_Acceleration:
                    brainOutput.Acceleration += Input;
                    break;
                case BrainNodeType.Output_Rotation:
                    brainOutput.Rotation += Input;
                    break;
                case BrainNodeType.Output_Attack:
                    brainOutput.Attack += Input;
                    break;
                case BrainNodeType.Output_PheromoneRed:
                    brainOutput.PheromoneRed += Input;
                    break;
                case BrainNodeType.Output_PheromoneGreen:
                    brainOutput.PheromoneGreen += Input;
                    break;
                case BrainNodeType.Output_PheromoneBlue:
                    brainOutput.PheromoneBlue += Input;
                    break;
                //case BrainNodeType.Output_ShareElement:
                //    brainOutput.ShareElement += Input;
                //    break;
                case BrainNodeType.Output_Memory0:
                    brainOutput.Memory0 += Input;
                    break;
                case BrainNodeType.Output_Memory1:
                    brainOutput.Memory1 += Input;
                    break;
                case BrainNodeType.Output_Memory2:
                    brainOutput.Memory2 += Input;
                    break;
                case BrainNodeType.Output_Memory3:
                    brainOutput.Memory3 += Input;
                    break;
                case BrainNodeType.Output_Memory4:
                    brainOutput.Memory4 += Input;
                    break;
                case BrainNodeType.Output_Memory5:
                    brainOutput.Memory5 += Input;
                    break;
                case BrainNodeType.Output_Memory6:
                    brainOutput.Memory6 += Input;
                    break;
                case BrainNodeType.Output_Memory7:
                    brainOutput.Memory7 += Input;
                    break;
                default:
                    break;
            }
        }
    }
}
