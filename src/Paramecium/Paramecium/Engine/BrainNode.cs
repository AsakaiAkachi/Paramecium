using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    public class BrainNode
    {
        public BrainNodeType Type { get; set; }

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

        public BrainNode Duplicate()
        {
            BrainNode result = new BrainNode();

            result.Type = Type;

            return result;
        }
    }
}
