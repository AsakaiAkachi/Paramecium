using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    public class Brain
    {
        public List<BrainNode> Nodes { get { return nodes; } set { nodes = value; } }
        private List<BrainNode> nodes = new List<BrainNode>();

        [JsonIgnore]
        public static Brain GetDefaultBrain 
        { 
            get 
            {
                Brain result = new Brain();

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Input_Bias;
                result.nodes[result.nodes.Count - 1].Connections.Add(new BrainNodeConnection() { TargetIndex = 3, Weight = 1 });
                result.nodes[result.nodes.Count - 1].Connections.Add(new BrainNodeConnection() { TargetIndex = 5, Weight = -0.9 });

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Input_PlantAvgAngle;
                result.nodes[result.nodes.Count - 1].Connections.Add(new BrainNodeConnection() { TargetIndex = 4, Weight = 1 });

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Input_PlantProximity;
                result.nodes[result.nodes.Count - 1].Connections.Add(new BrainNodeConnection() { TargetIndex = 3, Weight = -0.5 });
                result.nodes[result.nodes.Count - 1].Connections.Add(new BrainNodeConnection() { TargetIndex = 5, Weight = 1 });

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Output_Acceleration;

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Output_Rotation;

                result.nodes.Add(new BrainNode());
                result.nodes[result.nodes.Count - 1].Type = BrainNodeType.Output_Attack;

                return result;
            } 
        }

        public BrainOutput UpdateBrain(BrainInput brainInput)
        {
            BrainOutput result = new BrainOutput();

            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainInput(brainInput);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].CalculateNodeOutput();
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].SendNodeOutput(ref nodes);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainOutput(ref result);

            return result;
        }

        public static Brain Duplicate(Brain brain)
        {
            Brain result = new Brain();

            for (int i = 0; i < brain.nodes.Count; i++)
            {
                result.nodes.Add(new BrainNode());
                result.nodes[i].Type = brain.nodes[i].Type;

                for (int j = 0; j < brain.nodes[i].Connections.Count; j++)
                {
                    result.nodes[i].Connections.Add(new BrainNodeConnection());
                    result.nodes[i].Connections[j].TargetIndex = brain.nodes[i].Connections[j].TargetIndex;
                    result.nodes[i].Connections[j].Weight = brain.nodes[i].Connections[j].Weight;
                }
            }

            return result;
        }

        public static Brain Mutate(Brain brain, Random random)
        {
            return Mutate(brain, random, true, out bool mutationSuccessFlag);
        }
        public static Brain Mutate(Brain brain, Random random, bool applyMutationRule)
        {
            return Mutate(brain, random, applyMutationRule, out bool mutationSuccessFlag);
        }
        public static Brain Mutate(Brain brain, Random random, out bool mutationSuccessFlag)
        {
            return Mutate(brain, random, true, out mutationSuccessFlag);
        }

        public static Brain Mutate(Brain brain, Random random, bool applyMutationRule, out bool mutationSuccessFlag)
        {
            Brain result = Duplicate(brain);
            mutationSuccessFlag = false;

            int mutationType = random.Next(0, 6 + 1);
            int mutationTarget = random.Next(0, result.nodes.Count);

            if (mutationType == 0) // Add Node
            {
                if (result.nodes.Count < g_Soup.AnimalBrainMaximumNodeCount)
                {
                    mutationTarget = result.nodes.Count;

                    result.nodes.Add(new BrainNode());
                    mutationSuccessFlag = true;

                    int nodeType = random.Next(0, 3 + 1);
                    if (nodeType == 0)
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                    }
                    else if (nodeType == 1)
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                    }
                    else
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                    }

                    if (result.nodes[mutationTarget].IsInput || result.nodes[mutationTarget].IsHidden)
                    {
                        for (int i = 0; i < g_Soup.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            int connectionTarget = random.Next(0, result.nodes.Count);

                            if (connectionTarget != mutationTarget && !result.nodes[connectionTarget].IsInput && !connectionTargetIndexes.Contains(connectionTarget))
                            {
                                result.nodes[mutationTarget].Connections.Add(new BrainNodeConnection() { TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                                connectionTargetIndexes.Add(connectionTarget);
                            }

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }
                    if (result.nodes[mutationTarget].IsHidden || result.nodes[mutationTarget].IsOutput)
                    {
                        for (int i = 0; i < g_Soup.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            int connectionOrigin = random.Next(0, result.nodes.Count);

                            if (connectionOrigin != mutationTarget && !result.nodes[connectionOrigin].IsOutput && result.nodes[connectionOrigin].Connections.Count < g_Soup.AnimalBrainMaximumConnectionCount)
                            {
                                for (int j = 0; j < result.nodes[connectionOrigin].Connections.Count; j++) connectionTargetIndexes.Add(result.nodes[connectionOrigin].Connections[j].TargetIndex);

                                if (!connectionTargetIndexes.Contains(mutationTarget))
                                {
                                    result.nodes[connectionOrigin].Connections.Add(new BrainNodeConnection() { TargetIndex = mutationTarget, Weight = random.NextDouble() * 4d - 2d });
                                }
                            }

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }
                }
            }
            else if (mutationType == 1) // Remove Node
            {
                if (result.nodes.Count > 0)
                {
                    result.nodes.RemoveAt(mutationTarget);
                    mutationSuccessFlag = true;

                    for (int i = 0; i < result.nodes.Count; i++)
                    {
                        for (int j = result.nodes[i].Connections.Count - 1; j >= 0; j--)
                        {
                            if (result.nodes[i].Connections[j].TargetIndex == mutationTarget) result.nodes[i].Connections.RemoveAt(j);
                            else if (result.nodes[i].Connections[j].TargetIndex > mutationTarget) result.nodes[i].Connections[j].TargetIndex--;
                        }
                    }
                }
            }
            else if (mutationType == 2) // Change Node Type
            {
                if (result.nodes.Count > 0)
                {
                    BrainNodeType prevBrainNodeType = result.nodes[mutationTarget].Type;

                    int nodeType = random.Next(0, 3 + 1);
                    if (nodeType == 0)
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                    }
                    else if (nodeType == 1)
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                    }
                    else
                    {
                        result.nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                    }
                    mutationSuccessFlag = true;

                    if (BrainNode.BrainNodeTypeIsOutput(prevBrainNodeType) && !result.nodes[mutationTarget].IsOutput)
                    {
                        for (int i = 0; i < g_Soup.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            int connectionTarget = random.Next(0, result.nodes.Count);

                            if (connectionTarget != mutationTarget && !result.nodes[connectionTarget].IsInput && !connectionTargetIndexes.Contains(connectionTarget))
                            {
                                result.nodes[mutationTarget].Connections.Add(new BrainNodeConnection() { TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                            }

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }
                    if (BrainNode.BrainNodeTypeIsInput(prevBrainNodeType) && !result.nodes[mutationTarget].IsInput)
                    {
                        for (int i = 0; i < g_Soup.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            int connectionOrigin = random.Next(0, result.nodes.Count);

                            if (connectionOrigin != mutationTarget && !result.nodes[connectionOrigin].IsOutput && result.nodes[connectionOrigin].Connections.Count < g_Soup.AnimalBrainMaximumConnectionCount)
                            {
                                for (int j = 0; j < result.nodes[connectionOrigin].Connections.Count; j++) connectionTargetIndexes.Add(result.nodes[connectionOrigin].Connections[j].TargetIndex);

                                if (!connectionTargetIndexes.Contains(mutationTarget))
                                {
                                    result.nodes[connectionOrigin].Connections.Add(new BrainNodeConnection() { TargetIndex = mutationTarget, Weight = random.NextDouble() * 4d - 2d });
                                }
                            }

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }

                    if (!BrainNode.BrainNodeTypeIsOutput(prevBrainNodeType) && result.nodes[mutationTarget].IsOutput)
                    {
                        result.nodes[mutationTarget].Connections.Clear();
                    }
                    if (!BrainNode.BrainNodeTypeIsInput(prevBrainNodeType) && result.nodes[mutationTarget].IsInput)
                    {
                        for (int i = 0; i < result.nodes.Count; i++)
                        {
                            for (int j = result.nodes[i].Connections.Count - 1; j >= 0; j--)
                            {
                                if (result.nodes[i].Connections[j].TargetIndex == mutationTarget) result.nodes[i].Connections.RemoveAt(j);
                            }
                        }
                    }
                }
            }
            else if (mutationType == 3) // Add Connection
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count < g_Soup.AnimalBrainMaximumConnectionCount)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            for (int i = 0; i < result.nodes[mutationTarget].Connections.Count; i++) connectionTargetIndexes.Add(result.nodes[mutationTarget].Connections[i].TargetIndex);
                            int connectionTarget = random.Next(0, result.nodes.Count);

                            if (connectionTarget != mutationTarget && !result.nodes[connectionTarget].IsInput && !connectionTargetIndexes.Contains(connectionTarget))
                            {
                                result.nodes[mutationTarget].Connections.Add(new BrainNodeConnection() { TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                                connectionTargetIndexes.Add(connectionTarget);
                                mutationSuccessFlag = true;
                            }
                        }
                    }
                }
            }
            else if (mutationType == 4) // Remove Connection
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count > 0)
                        {
                            int targetConnection = random.Next(0, result.nodes[mutationTarget].Connections.Count);

                            result.nodes[mutationTarget].Connections.RemoveAt(targetConnection);
                            mutationSuccessFlag = true;
                        }
                    }
                }
            }
            else if (mutationTarget == 5) // Change Connection Target
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count > 0)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            for (int i = 0; i < result.nodes[mutationTarget].Connections.Count; i++) connectionTargetIndexes.Add(result.nodes[mutationTarget].Connections[i].TargetIndex);

                            int targetConnection = random.Next(0, result.nodes[mutationTarget].Connections.Count);
                            int targetConnectionNewConnectionTargetIndex = random.Next(0, result.nodes.Count);

                            if (targetConnectionNewConnectionTargetIndex != mutationTarget && !result.nodes[targetConnectionNewConnectionTargetIndex].IsInput && !connectionTargetIndexes.Contains(targetConnectionNewConnectionTargetIndex))
                            {
                                result.nodes[mutationTarget].Connections[targetConnection].TargetIndex = targetConnectionNewConnectionTargetIndex;
                                mutationSuccessFlag = true;
                            }
                        }
                    }
                }
            }
            else if (mutationTarget == 6) // Change Connection Weight
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count > 0)
                        {
                            int targetConnection = random.Next(0, result.nodes[mutationTarget].Connections.Count);

                            result.nodes[mutationTarget].Connections[targetConnection].Weight = random.NextDouble() * 4d - 2d;
                            mutationSuccessFlag = true;
                        }
                    }
                }
            }

            if (applyMutationRule)
            {
                while (true)
                {
                    List<int> removeNodeIndexes = new List<int>();

                    for (int i = 0; i < result.nodes.Count; i++)
                    {
                        if (!result.nodes[i].IsOutput && result.nodes[i].Connections.Count == 0) if (!removeNodeIndexes.Contains(i)) removeNodeIndexes.Add(i);

                        if (!result.nodes[i].IsInput)
                        {
                            bool hasIncomingConnection = false;
                            for (int j = 0; j < result.nodes.Count; j++)
                            {
                                for (int k = 0; k < result.nodes[j].Connections.Count; k++)
                                {
                                    if (result.nodes[j].Connections[k].TargetIndex == i)
                                    {
                                        hasIncomingConnection = true;
                                        break;
                                    }
                                }
                                if (hasIncomingConnection) break;
                            }
                            if (!hasIncomingConnection) if (!removeNodeIndexes.Contains(i)) removeNodeIndexes.Add(i);
                        }
                    }

                    for (int i = result.nodes.Count - 1; i >= 0; i--)
                    {
                        if (removeNodeIndexes.Contains(i))
                        {
                            result.nodes.RemoveAt(i);

                            for (int j = 0; j < result.nodes.Count; j++)
                            {
                                for (int k = result.nodes[j].Connections.Count - 1; k >= 0; k--)
                                {
                                    if (result.nodes[j].Connections[k].TargetIndex == i) result.nodes[j].Connections.RemoveAt(k);
                                    else if (result.nodes[j].Connections[k].TargetIndex > i) result.nodes[j].Connections[k].TargetIndex--;
                                }
                            }
                        }
                    }

                    if (removeNodeIndexes.Count == 0) break;
                }
            }

            return result;
        }
    }

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
                    Output = brainInput.PheromoneRed;
                    break;
                case BrainNodeType.Input_PheromoneGreen:
                    Output = brainInput.PheromoneGreen;
                    break;
                case BrainNodeType.Input_PheromoneBlue:
                    Output = brainInput.PheromoneBlue;
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

            if (Output > 100d) Output = 100d;
            else if (Output < -100d) Output = -100d;
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
                    if (Output == double.PositiveInfinity || Output == double.NegativeInfinity || Output == double.NaN) Output = 100;
                    break;
                case BrainNodeType.Hidden_LimitedTangent:
                    Output = Math.Tan(Input * Math.PI / 2d);
                    if (Output == double.PositiveInfinity || Output == double.NegativeInfinity || Output == double.NaN) Output = 100;
                    Output = double.Max(-1d, double.Min(1d, Output));
                    break;
                default:
                    break;
            }
            Input = 0d;
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

    public class BrainNodeConnection
    {
        public int TargetIndex { get; set; }
        public double Weight { get; set; }
    }

    public class BrainInput
    {
        public AnimalVisionOutput VisionData = new AnimalVisionOutput();
        public BrainOutput PrevStepOutput = new BrainOutput();
        public double Velocity;
        public double AngularVelocity;
        public double Satiety;
        public double PheromoneRed;
        public double PheromoneGreen;
        public double PheromoneBlue;
    }

    public class BrainOutput
    {
        public double Acceleration { get; set; }
        public double Rotation { get; set; }
        public double Attack { get; set; }
        public double PheromoneRed { get; set; }
        public double PheromoneGreen { get; set; }
        public double PheromoneBlue { get; set; }
        public double Memory0 { get; set; }
        public double Memory1 { get; set; }
        public double Memory2 { get; set; }
        public double Memory3 { get; set; }
        public double Memory4 { get; set; }
        public double Memory5 { get; set; }
        public double Memory6 { get; set; }
        public double Memory7 { get; set; }
    }

    public enum BrainNodeType
    {
        NonOperation,
        Input_Bias,
        Input_WallAvgAngle,
        Input_WallProximity,
        Input_PlantAvgAngle,
        Input_PlantProximity,
        Input_AnimalSameSpeciesAvgAngle,
        Input_AnimalSameSpeciesProximity,
        Input_AnimalOtherSpeciesAvgAngle,
        Input_AnimalOtherSpeciesProximity,
        Input_Velocity,
        Input_AngularVelocity,
        Input_Satiety,
        Input_PheromoneRed,
        Input_PheromoneGreen,
        Input_PheromoneBlue,
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

    public static class AnimalVision
    {
        public static AnimalVisionOutput Observe(Double2d originPosition, double angle, long originId, long originSpeciesId, int frontViewRange, int frontViewRayCount, double frontViewAngleRange, int backViewRange, int backViewRayCount, double backViewAngleRange)
        {
            AnimalVisionOutput result = new AnimalVisionOutput();

            double WallAvgAngleDenominator = 0;
            double PlantAvgAngleDenominator = 0;
            double AnimalSameSpeciesAvgAngleDenominator = 0;
            double AnimalOtherSpeciesAvgAngleDenominator = 0;

            double frontViewRangeDouble = frontViewRange;

            for (int i = 0; i < frontViewRayCount; i++)
            {
                double rayAngle = -(frontViewAngleRange / 2d) + frontViewAngleRange / (frontViewRayCount - 1) * i;
                if (rayAngle > 0.5d) rayAngle -= 1;
                if (rayAngle < -0.5d) rayAngle += 1;
                Double2d rayVector = Double2d.FromAngle(angle + rayAngle);

                for (int j = 0; j < frontViewRange; j++)
                {
                    Double2d rayPosition = originPosition + rayVector * (j + 1);
                    bool WallHitFlag = false;

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                            if (rayScanPosition.X < 0d || rayScanPosition.X > g_Soup.SizeX || rayScanPosition.Y < 0d || rayScanPosition.Y > g_Soup.SizeY)
                            {
                                result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                result.WallProximity = double.Max(result.WallProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                WallAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                    result.WallProximity = double.Max(result.WallProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                    WallAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                    WallHitFlag = true;
                                }
                            }
                        }
                    }

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            if (
                                !WallHitFlag ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y >= originPosition.Y && x == 0 && y == 0) ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y < originPosition.Y && x == 0 && y == 1) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y >= originPosition.Y && x == 1 && y == 0) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y < originPosition.Y && x == 1 && y == 1)
                            )
                            {
                                Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.LocalPlantPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                    {
                                        Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                        if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5d * 0.5d)
                                        {
                                            result.PlantAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                            result.PlantProximity = double.Max(result.PlantProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                            PlantAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                        }
                                    }
                                }
                                if (targetTile.LocalAnimalPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                    {
                                        Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                        if (targetAnimal.Id != originId)
                                        {
                                            if (targetAnimal.SpeciesId == originSpeciesId)
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                                    result.AnimalSameSpeciesProximity = double.Max(result.AnimalSameSpeciesProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                                    AnimalSameSpeciesAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.AnimalOtherSpeciesProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                                    AnimalOtherSpeciesAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (WallHitFlag) break;
                }
            }
            for (int i = 0; i < backViewRayCount; i++)
            {
                double rayAngle = 0.5d + -(backViewAngleRange / 2d) + backViewAngleRange / (backViewRayCount - 1) * i;
                if (rayAngle > 0.5d) rayAngle -= 1;
                if (rayAngle < -0.5d) rayAngle += 1;
                Double2d rayVector = Double2d.FromAngle(angle + rayAngle);

                double backViewRangeDouble = backViewRange;

                for (int j = 0; j < backViewRange; j++)
                {
                    Double2d rayPosition = originPosition + rayVector * (j + 1);
                    bool WallHitFlag = false;

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                            if (rayScanPosition.X < 0d || rayScanPosition.X > g_Soup.SizeX || rayScanPosition.Y < 0d || rayScanPosition.Y > g_Soup.SizeY)
                            {
                                result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                result.WallProximity = double.Max(result.WallProximity, ((backViewRange - j) / backViewRangeDouble));
                                WallAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                    result.WallProximity = double.Max(result.WallProximity, ((backViewRange - j) / backViewRangeDouble));
                                    WallAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                    WallHitFlag = true;
                                }
                            }
                        }
                    }

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            if (
                                !WallHitFlag ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y >= originPosition.Y && x == 0 && y == 0) ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y < originPosition.Y && x == 0 && y == 1) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y >= originPosition.Y && x == 1 && y == 0) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y < originPosition.Y && x == 1 && y == 1)
                            )
                            {
                                Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.LocalPlantPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                    {
                                        Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                        if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5d * 0.5d)
                                        {
                                            result.PlantAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                            result.PlantProximity = double.Max(result.PlantProximity, ((backViewRange - j) / backViewRangeDouble));
                                            PlantAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                        }
                                    }
                                }
                                if (targetTile.LocalAnimalPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                    {
                                        Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                        if (targetAnimal.Id != originId)
                                        {
                                            if (targetAnimal.SpeciesId == originSpeciesId)
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                                    result.AnimalSameSpeciesProximity = double.Max(result.AnimalSameSpeciesProximity, ((backViewRange - j) / backViewRangeDouble));
                                                    AnimalSameSpeciesAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.AnimalOtherSpeciesProximity, ((backViewRange - j) / backViewRangeDouble));
                                                    AnimalOtherSpeciesAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (WallHitFlag) break;
                }
            }

            if (WallAvgAngleDenominator > 0) result.WallAvgAngle /= WallAvgAngleDenominator;
            if (PlantAvgAngleDenominator > 0) result.PlantAvgAngle /= PlantAvgAngleDenominator;
            if (AnimalSameSpeciesAvgAngleDenominator > 0) result.AnimalSameSpeciesAvgAngle /= AnimalSameSpeciesAvgAngleDenominator;
            if (AnimalOtherSpeciesAvgAngleDenominator > 0) result.AnimalOtherSpeciesAvgAngle /= AnimalOtherSpeciesAvgAngleDenominator;

            //Console.WriteLine(result.PlantAvgAngle);

            return result;
        }
    }

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
    }
}
