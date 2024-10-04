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
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            Brain result = Duplicate(brain);

            int mutationType = random.Next(0, 11);
            int mutationTarget = random.Next(0, result.nodes.Count);

            if (mutationType < 3) // Add Node (Weight: 3)
            {
                if (result.nodes.Count < g_Soup.Settings.AnimalBrainMaximumNodeCount)
                {
                    mutationTarget = random.Next(0, result.nodes.Count + 1);

                    result.nodes.Insert(mutationTarget, new BrainNode());
                    for (int i = 0; i < result.nodes.Count; i++)
                    {
                        for (int j = 0; j < result.nodes[i].Connections.Count; j++)
                        {
                            if (result.nodes[i].Connections[j].TargetIndex >= mutationTarget) result.nodes[i].Connections[j].TargetIndex++;
                        }
                    }

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
                        List<int> connectionTargetIndexes = new List<int>();
                        for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
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
                        for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            int connectionOrigin = random.Next(0, result.nodes.Count);

                            if (connectionOrigin != mutationTarget && !result.nodes[connectionOrigin].IsOutput && result.nodes[connectionOrigin].Connections.Count < g_Soup.Settings.AnimalBrainMaximumConnectionCount)
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
            else if (mutationType < 4) // Remove Node (Weight: 1)
            {
                if (result.nodes.Count > 0)
                {
                    result.nodes.RemoveAt(mutationTarget);

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
            else if (mutationType < 5) // Change Node Type (Weight: 1)
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

                    if (BrainNode.BrainNodeTypeIsOutput(prevBrainNodeType) && !result.nodes[mutationTarget].IsOutput)
                    {
                        List<int> connectionTargetIndexes = new List<int>();
                        for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            int connectionTarget = random.Next(0, result.nodes.Count);

                            if (connectionTarget != mutationTarget && !result.nodes[connectionTarget].IsInput && !connectionTargetIndexes.Contains(connectionTarget))
                            {
                                result.nodes[mutationTarget].Connections.Add(new BrainNodeConnection() { TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                                connectionTargetIndexes.Add(connectionTarget);
                            }

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }
                    if (BrainNode.BrainNodeTypeIsInput(prevBrainNodeType) && !result.nodes[mutationTarget].IsInput)
                    {
                        for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount / 2; i++)
                        {
                            int connectionOrigin = random.Next(0, result.nodes.Count);

                            if (connectionOrigin != mutationTarget && !result.nodes[connectionOrigin].IsOutput && result.nodes[connectionOrigin].Connections.Count < g_Soup.Settings.AnimalBrainMaximumConnectionCount)
                            {
                                List<int> connectionTargetIndexes = new List<int>();
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
            else if (mutationType < 8) // Add Connection (Weight: 3)
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count < g_Soup.Settings.AnimalBrainMaximumConnectionCount)
                        {
                            List<int> connectionTargetIndexes = new List<int>();
                            for (int i = 0; i < result.nodes[mutationTarget].Connections.Count; i++) connectionTargetIndexes.Add(result.nodes[mutationTarget].Connections[i].TargetIndex);
                            int connectionTarget = random.Next(0, result.nodes.Count);

                            if (connectionTarget != mutationTarget && !result.nodes[connectionTarget].IsInput && !connectionTargetIndexes.Contains(connectionTarget))
                            {
                                result.nodes[mutationTarget].Connections.Add(new BrainNodeConnection() { TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                                connectionTargetIndexes.Add(connectionTarget);
                            }
                        }
                    }
                }
            }
            else if (mutationType < 9) // Remove Connection (Weight: 1)
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count > 0)
                        {
                            int targetConnection = random.Next(0, result.nodes[mutationTarget].Connections.Count);

                            result.nodes[mutationTarget].Connections.RemoveAt(targetConnection);
                        }
                    }
                }
            }
            else if (mutationTarget < 10) // Change Connection Target (Weight: 1)
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
                            }
                        }
                    }
                }
            }
            else if (mutationTarget < 13) // Change Connection Weight (Weight: 3)
            {
                if (result.nodes.Count > 0)
                {
                    if (!result.nodes[mutationTarget].IsOutput)
                    {
                        if (result.nodes[mutationTarget].Connections.Count > 0)
                        {
                            int targetConnection = random.Next(0, result.nodes[mutationTarget].Connections.Count);

                            result.nodes[mutationTarget].Connections[targetConnection].Weight = random.NextDouble() * 4d - 2d;
                        }
                    }
                }
            }

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

            return result;
        }
    }
}
