using System.Linq;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    public class Brain
    {

        public List<BrainNode> Nodes { get; set; } = new List<BrainNode>();
        public List<BrainNodeConnection> Connections { get; set; } = new List<BrainNodeConnection>();

        /**
        public double InheritedMemory0 { get; set; }
        public double InheritedMemory1 { get; set; }
        public double InheritedMemory2 { get; set; }
        public double InheritedMemory3 { get; set; }
        public double InheritedMemory4 { get; set; }
        public double InheritedMemory5 { get; set; }
        public double InheritedMemory6 { get; set; }
        public double InheritedMemory7 { get; set; }
        **/

        public BrainInput Input { get; set; } = new BrainInput();
        public BrainOutput Output { get; set; } = new BrainOutput();

        [JsonIgnore]
        public int[] InputNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsInput) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] HiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] OutputNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsOutput) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] InputAndHiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsInput || Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] OutputAndHiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsOutput || Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }

        [JsonIgnore]
        public static Brain DefaultBrain
        {
            get
            {
                Brain result = new Brain();

                /**
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_Bias });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_PlantAvgAngle });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_PlantAvgDistance });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Acceleration });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Rotation });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Attack });

                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 4, Weight = 0.01 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 5, Weight = 0.25 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 1, TargetIndex = 4, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 3, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 5, Weight = -1 });
                **/

                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_Bias });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_PlantWAvgAngle });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_PlantWAvgProximity });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Acceleration });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Rotation });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Eat });

                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 3, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 5, Weight = -0.5 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 1, TargetIndex = 4, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 3, Weight = -0.95 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 5, Weight = 1 });

                return result;
            }
        }

        public Brain() { }
        public Brain(Brain parentBrain)
        {
            for (int i = 0; i < parentBrain.Nodes.Count; i++)
            {
                Nodes.Add(new BrainNode() { Function = parentBrain.Nodes[i].Function });
            }
            for (int i = 0; i < parentBrain.Connections.Count; i++)
            {
                Connections.Add(new BrainNodeConnection() { OriginIndex = parentBrain.Connections[i].OriginIndex, TargetIndex = parentBrain.Connections[i].TargetIndex, Weight = parentBrain.Connections[i].Weight });
            }

            /**
            InheritedMemory0 = parentBrain.Output.InheritedMemory0;
            InheritedMemory1 = parentBrain.Output.InheritedMemory1;
            InheritedMemory2 = parentBrain.Output.InheritedMemory2;
            InheritedMemory3 = parentBrain.Output.InheritedMemory3;
            InheritedMemory4 = parentBrain.Output.InheritedMemory4;
            InheritedMemory5 = parentBrain.Output.InheritedMemory5;
            InheritedMemory6 = parentBrain.Output.InheritedMemory6;
            InheritedMemory7 = parentBrain.Output.InheritedMemory7;
            **/
        }

        public bool Mutate(SoupSettings settings)
        {
            int mutationType = WeightedSelector(settings.AnimalMutationTypeWeights);

            bool mutationSuccessful = false;

            if (mutationType == 0) mutationSuccessful = AddNodeMutation(settings);
            if (mutationType == 1) mutationSuccessful = RemoveNodeMutation();
            if (mutationType == 2) mutationSuccessful = ChangeNodeTypeMutation(settings);
            if (mutationType == 3) mutationSuccessful = AddConnectionMutation();
            if (mutationType == 4) mutationSuccessful = RemoveConnectionMutation();
            if (mutationType == 5) mutationSuccessful = ChangeConnectionOriginMutation();
            if (mutationType == 6) mutationSuccessful = ChangeConnectionTargetMutation();
            if (mutationType == 7) mutationSuccessful = ChangeConnectionWeightMutation();

            RemoveInvalidNode();

            return mutationSuccessful;
        }

        public bool AddNodeMutation(SoupSettings settings)
        {
            if (Nodes.Count < settings.AnimalMaximumNodeCount)
            {
                if (InputNodeIndexes.Length > 0 && OutputNodeIndexes.Length > 0)
                {
                    Random rand = new Random();

                    BrainNode targetNode = new BrainNode();
                    int targetNodeIndex = Nodes.Count;

                    int nodeType = WeightedSelector(settings.AnimalNodeTypeWeights);
                    //if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Input_Bias, (int)BrainNodeFunction.Input_InheritedMemory7 + 1);
                    if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Input_Bias, (int)BrainNodeFunction.Input_PheromoneBlueGradAngle + 1);
                    if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Hidden_ReLU, (int)BrainNodeFunction.Hidden_Frac + 1);
                    //if (nodeType == 2) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Output_Acceleration, (int)BrainNodeFunction.Output_InheritedMemory7 + 1);
                    if (nodeType == 2) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Output_Acceleration, (int)BrainNodeFunction.Output_PheromoneBlueProduction + 1);

                    if (nodeType == 0 || nodeType == 1)
                    {
                        int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = targetNodeIndex, TargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)], Weight = rand.NextDouble() * 4d - 2d });
                    }
                    if (nodeType == 1 || nodeType == 2)
                    {
                        int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)], TargetIndex = targetNodeIndex, Weight = rand.NextDouble() * 4d - 2d });
                    }

                    Nodes.Add(targetNode);
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool RemoveNodeMutation()
        {
            if (Nodes.Count > 0)
            {
                Random rand = new Random();

                int targetNodeIndex = rand.Next(0, Nodes.Count);

                Nodes.RemoveAt(targetNodeIndex);
                for (int i = Connections.Count - 1; i >= 0; i--)
                {
                    if (Connections[i].OriginIndex == targetNodeIndex || Connections[i].TargetIndex == targetNodeIndex) Connections.RemoveAt(i);
                    else
                    {
                        if (Connections[i].OriginIndex > targetNodeIndex) Connections[i].OriginIndex--;
                        if (Connections[i].TargetIndex > targetNodeIndex) Connections[i].TargetIndex--;
                    }
                }

                return true;
            }
            else return false;
        }
        public bool ChangeNodeTypeMutation(SoupSettings settings)
        {
            if (Nodes.Count > 0)
            {
                Random rand = new Random();

                int targetNodeIndex = rand.Next(0, Nodes.Count);
                BrainNode targetNode = Nodes[targetNodeIndex];

                int targetNodePrevNodeType = -1;
                if (targetNode.IsInput) targetNodePrevNodeType = 0;
                if (targetNode.IsHidden) targetNodePrevNodeType = 1;
                if (targetNode.IsOutput) targetNodePrevNodeType = 2;

                int nodeType = WeightedSelector(settings.AnimalNodeTypeWeights);

                if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Input_Bias, (int)BrainNodeFunction.Input_PheromoneBlueGradAngle + 1);
                if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Hidden_ReLU, (int)BrainNodeFunction.Hidden_Frac + 1);
                if (nodeType == 2) targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Output_Acceleration, (int)BrainNodeFunction.Output_PheromoneBlueProduction + 1);

                if (targetNodePrevNodeType == 0 && nodeType == 1)
                {
                    if (InputAndHiddenNodeIndexes.Length > 1)
                    {
                        int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)], TargetIndex = targetNodeIndex, Weight = rand.NextDouble() * 4d - 2d });
                    }
                    else
                    {
                        targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Input_Bias, (int)BrainNodeFunction.Input_PheromoneBlueGradAngle + 1);
                        return true;
                    }
                }
                if (targetNodePrevNodeType == 2 && nodeType == 1)
                {
                    if (OutputAndHiddenNodeIndexes.Length > 1)
                    {
                        int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = targetNodeIndex, TargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)], Weight = rand.NextDouble() * 4d - 2d });
                    }
                    else
                    {
                        targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Output_Acceleration, (int)BrainNodeFunction.Output_PheromoneBlueProduction + 1);
                        return true;
                    }
                }

                if (targetNodePrevNodeType != 0 && nodeType == 0)
                {
                    for (int i = Connections.Count - 1; i >= 0; i--)
                    {
                        if (Connections[i].TargetIndex == targetNodeIndex) Connections.RemoveAt(i);
                    }
                }
                if (targetNodePrevNodeType != 2 && nodeType == 2)
                {
                    for (int i = Connections.Count - 1; i >= 0; i--)
                    {
                        if (Connections[i].OriginIndex == targetNodeIndex) Connections.RemoveAt(i);
                    }
                }

                return true;
            }
            else return false;
        }
        public bool AddConnectionMutation()
        {
            if (Nodes.Count >= 2)
            {
                Random rand = new Random();

                int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;

                int connectionOriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)];
                int connectionTargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)];

                if (connectionOriginIndex != connectionTargetIndex && !ContainConnection(connectionOriginIndex, connectionTargetIndex))
                {
                    Connections.Add(new BrainNodeConnection() { OriginIndex = connectionOriginIndex, TargetIndex = connectionTargetIndex, Weight = rand.NextDouble() * 4d - 2d });
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool RemoveConnectionMutation()
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                Connections.RemoveAt(targetConnectionIndex);

                return true;
            }
            else return false;
        }
        public bool ChangeConnectionOriginMutation()
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                int targetConnectionNewOriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)];

                if (targetConnection.OriginIndex != targetConnectionNewOriginIndex && !ContainConnection(targetConnectionNewOriginIndex, targetConnection.TargetIndex))
                {
                    targetConnection.OriginIndex = targetConnectionNewOriginIndex;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool ChangeConnectionTargetMutation()
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                int targetConnectionNewTargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)];

                if (targetConnection.TargetIndex != targetConnectionNewTargetIndex && !ContainConnection(targetConnection.OriginIndex, targetConnectionNewTargetIndex))
                {
                    targetConnection.TargetIndex = targetConnectionNewTargetIndex;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool ChangeConnectionWeightMutation()
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                targetConnection.Weight = rand.NextDouble() * 4d - 2d;

                return true;
            }
            else return false;
        }

        public void RemoveInvalidNode()
        {
            List<int>[] nodeIncomingConnections = new List<int>[Nodes.Count];
            List<int>[] nodeOutgoingConnections = new List<int>[Nodes.Count];
            List<int> invalidNodeIndexes = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                nodeIncomingConnections[i] = new List<int>();
                nodeOutgoingConnections[i] = new List<int>();
                invalidNodeIndexes.Add(i);
            }
            for (int i = 0; i < Connections.Count; i++)
            {
                nodeIncomingConnections[Connections[i].TargetIndex].Add(Connections[i].OriginIndex);
                nodeOutgoingConnections[Connections[i].OriginIndex].Add(Connections[i].TargetIndex);
            }

            List<int> connectedFromInputNodeIndexes = new List<int>();
            List<int> connectedFromOutputNodeIndexes = new List<int>();

            List<int> exploreNodeIndexes = InputNodeIndexes.ToList<int>();
            while (exploreNodeIndexes.Count > 0)
            {
                List<int> nextStepExploreNodeIndexes = new List<int>();

                for (int i = 0; i < exploreNodeIndexes.Count; i++)
                {
                    connectedFromInputNodeIndexes.Add(exploreNodeIndexes[i]);

                    for (int j = 0; j < nodeOutgoingConnections[exploreNodeIndexes[i]].Count; j++)
                    {
                        int targetConnectionTargetIndex = nodeOutgoingConnections[exploreNodeIndexes[i]][j];

                        if (!nextStepExploreNodeIndexes.Contains(targetConnectionTargetIndex) && !connectedFromInputNodeIndexes.Contains(targetConnectionTargetIndex)) nextStepExploreNodeIndexes.Add(targetConnectionTargetIndex);
                    }
                }

                exploreNodeIndexes = nextStepExploreNodeIndexes;
            }

            exploreNodeIndexes = OutputNodeIndexes.ToList<int>();
            while (exploreNodeIndexes.Count > 0)
            {
                List<int> nextStepExploreNodeIndexes = new List<int>();

                for (int i = 0; i < exploreNodeIndexes.Count; i++)
                {
                    connectedFromOutputNodeIndexes.Add(exploreNodeIndexes[i]);

                    for (int j = 0; j < nodeIncomingConnections[exploreNodeIndexes[i]].Count; j++)
                    {
                        int targetConnectionOriginIndex = nodeIncomingConnections[exploreNodeIndexes[i]][j];

                        if (!nextStepExploreNodeIndexes.Contains(targetConnectionOriginIndex) && !connectedFromOutputNodeIndexes.Contains(targetConnectionOriginIndex)) nextStepExploreNodeIndexes.Add(targetConnectionOriginIndex);
                    }
                }

                exploreNodeIndexes = nextStepExploreNodeIndexes;
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (connectedFromInputNodeIndexes.Contains(i) && connectedFromOutputNodeIndexes.Contains(i)) invalidNodeIndexes.Remove(i);
            }

            for (int i = invalidNodeIndexes.Count - 1; i >= 0; i--)
            {
                Nodes.RemoveAt(invalidNodeIndexes[i]);

                for (int j = Connections.Count - 1; j >= 0; j--)
                {
                    if (Connections[j].OriginIndex == invalidNodeIndexes[i] || Connections[j].TargetIndex == invalidNodeIndexes[i]) Connections.RemoveAt(j);
                    else
                    {
                        if (Connections[j].OriginIndex > invalidNodeIndexes[i]) Connections[j].OriginIndex--;
                        if (Connections[j].TargetIndex > invalidNodeIndexes[i]) Connections[j].TargetIndex--;
                    }
                }
            }
        }

        private int WeightedSelector(double[] weight)
        {
            double totalWeight = 0d;
            for (int i = 0; i < weight.Length; i++) totalWeight += weight[i];

            double weightedSelectedIndex = new Random().NextDouble() * totalWeight;

            for (int i = 0; i < weight.Length - 1; i++)
            {
                if (weightedSelectedIndex < weight[i]) return i;
                else weightedSelectedIndex -= weight[i];
            }
            return weight.Length - 1;
        }

        private bool ContainConnection(int originIndex, int targetIndex)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].OriginIndex == originIndex && Connections[i].TargetIndex == targetIndex) return true;
            }

            return false;
        }

        public bool ContainNodeFunction(BrainNodeFunction function)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Function == function) return true;
            }

            return false;
        }
        public bool ContainNodeFunction(BrainNodeFunction[] functions)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (functions.Contains(Nodes[i].Function)) return true;
            }

            return false;
        }

        public void UpdateBrain(BrainInput brainInput)
        {
            Input = brainInput;
            Input.PrevStepOutput = Output;

            BrainOutput brainOutput = new BrainOutput();

            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainInput(this, Input);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].CalculateNodeOutput();
            for (int i = 0; i < Connections.Count; i++) Connections[i].UpdateConnection(Nodes);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainOutput(ref brainOutput);

            Output = brainOutput;
        }
    }
}
