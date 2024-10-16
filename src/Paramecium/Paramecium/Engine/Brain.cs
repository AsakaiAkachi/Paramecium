using System;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    public class Brain
    {
        public List<BrainNode> Nodes { get; set; } = new List<BrainNode>();
        public List<BrainNodeConnection> Connections { get; set; } = new List<BrainNodeConnection>();

        [JsonIgnore]
        public static Brain GetDefaultBrain
        {
            get
            {
                Brain result = new Brain();

                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_Bias });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_PlantAvgAngle });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Input_PlantProximity });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Acceleration });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Rotation });
                result.Nodes.Add(new BrainNode() { Type = BrainNodeType.Output_Attack });

                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 3, Weight = 1d });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 5, Weight = -0.9d });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 1, TargetIndex = 4, Weight = 1d });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 3, Weight = -0.5d });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 5, Weight = 1d });

                return result;
            }
        }

        public BrainOutput UpdateBrain(BrainInput brainInput)
        {
            BrainOutput result = new BrainOutput();

            ApplyBrainInput(brainInput);
            CalculateNodeOutput();
            SendNodeOutput();
            ApplyBrainOutput(ref result);

            return result;
        }

        public Brain Duplicate()
        {
            Brain result = new Brain();

            for (int i = 0; i < Nodes.Count; i++) result.Nodes.Add(Nodes[i].Duplicate());
            for (int i = 0; i < Connections.Count; i++) result.Connections.Add(Connections[i].Duplicate());

            return result;
        }

        public void MutationAddNode(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Nodes.Count < g_Soup.Settings.AnimalBrainMaximumNodeCount)
            {
                int mutationTarget = random.Next(0, Nodes.Count + 1);

                Nodes.Insert(mutationTarget, new BrainNode());
                for (int i = 0; i < Connections.Count; i++)
                {
                    if (Connections[i].OriginIndex >= mutationTarget) Connections[i].OriginIndex++;
                    if (Connections[i].TargetIndex >= mutationTarget) Connections[i].TargetIndex++;
                }

                int nodeType = random.Next(0, 3 + 1);
                if (nodeType == 0)
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                }
                else if (nodeType == 1)
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                }
                else
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                }

                if (Nodes[mutationTarget].IsInput || Nodes[mutationTarget].IsHidden)
                {
                    for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount; i++)
                    {
                        int connectionTarget = random.Next(0, Nodes.Count);

                        if (connectionTarget != mutationTarget && !Nodes[connectionTarget].IsInput && !ContainsConnection(mutationTarget, connectionTarget))
                        {
                            Connections.Add(new BrainNodeConnection() { OriginIndex = mutationTarget, TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                        }

                        if (random.NextDouble() < g_Soup.Settings.AnimalMutationCountBias) break;
                    }
                }
                if (Nodes[mutationTarget].IsHidden || Nodes[mutationTarget].IsOutput)
                {
                    for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount; i++)
                    {
                        int connectionOrigin = random.Next(0, Nodes.Count);

                        if (connectionOrigin != mutationTarget && !Nodes[connectionOrigin].IsOutput && !ContainsConnection(connectionOrigin, mutationTarget))
                        {
                            Connections.Add(new BrainNodeConnection() { OriginIndex = connectionOrigin, TargetIndex = mutationTarget, Weight = random.NextDouble() * 4d - 2d });
                        }

                        if (random.NextDouble() < g_Soup.Settings.AnimalMutationCountBias) break;
                    }
                }
            }
        }

        public void MutationRemoveNode(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Nodes.Count > 0)
            {
                int mutationTarget = random.Next(0, Nodes.Count);

                Nodes.RemoveAt(mutationTarget);

                for (int i = Connections.Count - 1; i >= 0; i--)
                {
                    if (Connections[i].OriginIndex == mutationTarget) { Connections.RemoveAt(i); continue; }
                    if (Connections[i].OriginIndex > mutationTarget) Connections[i].OriginIndex--;

                    if (Connections[i].TargetIndex == mutationTarget) { Connections.RemoveAt(i); continue; }
                    if (Connections[i].TargetIndex > mutationTarget) Connections[i].TargetIndex--;
                }
            }
        }

        public void MutationDuplicateNode(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Nodes.Count > 0 && Nodes.Count < g_Soup.Settings.AnimalBrainMaximumNodeCount)
            {
                int originalNodeIndex = random.Next(0, Nodes.Count);
                int duplicatedNodeIndex = random.Next(0, Nodes.Count + 1);

                Nodes.Insert(duplicatedNodeIndex, Nodes[originalNodeIndex].Duplicate());
                for (int i = 0; i < Connections.Count; i++)
                {
                    if (Connections[i].OriginIndex >= duplicatedNodeIndex) Connections[i].OriginIndex++;
                    if (Connections[i].TargetIndex >= duplicatedNodeIndex) Connections[i].TargetIndex++;
                }

                List<BrainNodeConnection> originalIncomingConnections = EnumerateIncomingConnection(originalNodeIndex);
                for (int i = 0; i < originalIncomingConnections.Count; i++)
                {
                    Connections.Add(originalIncomingConnections[i].Duplicate());
                    Connections[Connections.Count - 1].TargetIndex = duplicatedNodeIndex;
                }

                List<BrainNodeConnection> originalOutgoingConnections = EnumerateOutgoingConnection(originalNodeIndex);
                for (int i = 0; i < originalOutgoingConnections.Count; i++)
                {
                    Connections.Add(originalOutgoingConnections[i].Duplicate());
                    Connections[Connections.Count - 1].OriginIndex = duplicatedNodeIndex;
                }
            }
        }

        public void MutationChangeNodeType(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Nodes.Count > 0)
            {
                int mutationTarget = random.Next(0, Nodes.Count);
                BrainNodeType prevBrainNodeType = Nodes[mutationTarget].Type;

                int nodeType = random.Next(0, 3 + 1);
                if (nodeType == 0)
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                }
                else if (nodeType == 1)
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                }
                else
                {
                    Nodes[mutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                }

                if (BrainNode.BrainNodeTypeIsInput(prevBrainNodeType) && !Nodes[mutationTarget].IsInput)
                {
                    for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount; i++)
                    {
                        int connectionOrigin = random.Next(0, Nodes.Count);

                        if (connectionOrigin != mutationTarget && !Nodes[connectionOrigin].IsOutput && !ContainsConnection(connectionOrigin, mutationTarget))
                        {
                            Connections.Add(new BrainNodeConnection() { OriginIndex = connectionOrigin, TargetIndex = mutationTarget, Weight = random.NextDouble() * 4d - 2d });
                        }

                        if (random.NextDouble() < g_Soup.Settings.AnimalMutationCountBias) break;
                    }
                }
                if (BrainNode.BrainNodeTypeIsOutput(prevBrainNodeType) && !Nodes[mutationTarget].IsOutput)
                {
                    for (int i = 0; i < g_Soup.Settings.AnimalBrainMaximumConnectionCount; i++)
                    {
                        int connectionTarget = random.Next(0, Nodes.Count);

                        if (connectionTarget != mutationTarget && !Nodes[connectionTarget].IsInput && !ContainsConnection(mutationTarget, connectionTarget))
                        {
                            Connections.Add(new BrainNodeConnection() { OriginIndex = mutationTarget, TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                        }

                        if (random.NextDouble() < g_Soup.Settings.AnimalMutationCountBias) break;
                    }
                }

                if (!BrainNode.BrainNodeTypeIsInput(prevBrainNodeType) && Nodes[mutationTarget].IsInput)
                {
                    List<int> enumeratedIncomingConnectionIndexes = EnumerateIncomingConnectionIndex(mutationTarget);

                    for (int i = enumeratedIncomingConnectionIndexes.Count - 1; i >= 0; i--)
                    {
                        Connections.RemoveAt(enumeratedIncomingConnectionIndexes[i]);
                    }
                }
                if (!BrainNode.BrainNodeTypeIsOutput(prevBrainNodeType) && Nodes[mutationTarget].IsOutput)
                {
                    List<int> enumeratedOutgoingConnectionIndexes = EnumerateOutgoingConnectionIndex(mutationTarget);

                    for (int i = enumeratedOutgoingConnectionIndexes.Count - 1; i >= 0; i--)
                    {
                        Connections.RemoveAt(enumeratedOutgoingConnectionIndexes[i]);
                    }
                }
            }
        }

        public void MutationAddConnection(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Nodes.Count > 0)
            {
                int connectionOrigin = random.Next(0, Nodes.Count);
                int connectionTarget = random.Next(0, Nodes.Count);

                if (connectionOrigin != connectionTarget && !Nodes[connectionOrigin].IsOutput && !Nodes[connectionTarget].IsInput && !ContainsConnection(connectionOrigin, connectionTarget) && GetOutgoingConnectionCount(connectionOrigin) < g_Soup.Settings.AnimalBrainMaximumConnectionCount)
                {
                    Connections.Add(new BrainNodeConnection() { OriginIndex = connectionOrigin, TargetIndex = connectionTarget, Weight = random.NextDouble() * 4d - 2d });
                }
            }
        }

        public void MutationRemoveConnection(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Connections.Count > 0)
            {
                int mutationTarget = random.Next(0, Connections.Count);

                Connections.RemoveAt(mutationTarget);
            }
        }

        public void MutationChangeConnectionOrigin(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Connections.Count > 0)
            {
                int mutationTarget = random.Next(0, Connections.Count);

                int newConnectionOrigin = random.Next(0, Nodes.Count);

                if (newConnectionOrigin != Connections[mutationTarget].OriginIndex && newConnectionOrigin != Connections[mutationTarget].TargetIndex && !Nodes[newConnectionOrigin].IsOutput && !ContainsConnection(newConnectionOrigin, Connections[mutationTarget].TargetIndex) && GetOutgoingConnectionCount(newConnectionOrigin) < g_Soup.Settings.AnimalBrainMaximumConnectionCount)
                {
                    Connections[mutationTarget].OriginIndex = newConnectionOrigin;
                }
            }
        }

        public void MutationChangeConnectionTarget(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Connections.Count > 0)
            {
                int mutationTarget = random.Next(0, Connections.Count);

                int newConnectionTarget = random.Next(0, Nodes.Count);

                if (newConnectionTarget != Connections[mutationTarget].OriginIndex && newConnectionTarget != Connections[mutationTarget].TargetIndex && !Nodes[newConnectionTarget].IsInput && !ContainsConnection(Connections[mutationTarget].OriginIndex, newConnectionTarget))
                {
                    Connections[mutationTarget].TargetIndex = newConnectionTarget;
                }
            }
        }

        public void MutationChangeConnectionWeight(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Connections.Count > 0)
            {
                int mutationTarget = random.Next(0, Connections.Count);

                Connections[mutationTarget].Weight = random.NextDouble() * 4d - 2d;
            }
        }

        public void MutationFinalize()
        {
            while (true)
            {
                List<int> removeNodeIndexes = new List<int>();

                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (!Nodes[i].IsOutput && !HasOutgoingConnection(i) && !removeNodeIndexes.Contains(i)) removeNodeIndexes.Add(i);
                    if (!Nodes[i].IsInput && !HasIncomingConnection(i) && !removeNodeIndexes.Contains(i)) removeNodeIndexes.Add(i);
                }

                if (removeNodeIndexes.Count == 0) break;

                for (int i = removeNodeIndexes.Count - 1; i >= 0; i--)
                {
                    Nodes.RemoveAt(removeNodeIndexes[i]);

                    for (int j = Connections.Count - 1; j >= 0; j--)
                    {
                        if (Connections[j].OriginIndex == removeNodeIndexes[i]) { Connections.RemoveAt(j); continue; }
                        if (Connections[j].OriginIndex > removeNodeIndexes[i]) Connections[j].OriginIndex--;

                        if (Connections[j].TargetIndex == removeNodeIndexes[i]) { Connections.RemoveAt(j); continue; }
                        if (Connections[j].TargetIndex > removeNodeIndexes[i]) Connections[j].TargetIndex--;
                    }
                }
            }
        }

        public static Brain ApplyMutation(Brain brain, Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            Brain result = brain.Duplicate();

            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateRemoveNode) result.MutationRemoveNode(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateRemoveConnection) result.MutationRemoveConnection(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateDuplicateNode) result.MutationDuplicateNode(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateAddNode) result.MutationAddNode(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateAddConnection) result.MutationAddConnection(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateChangeNodeType) result.MutationChangeNodeType(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateChangeConnectionOrigin) result.MutationChangeConnectionOrigin(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateChangeConnectionTarget) result.MutationChangeConnectionTarget(random);
            if (random.NextDouble() < g_Soup.Settings.AnimalBrainMutationRateChangeConnectionWeight) result.MutationChangeConnectionWeight(random);
            result.MutationFinalize();

            return result;
        }

        public List<BrainNodeConnection> EnumerateIncomingConnection(int nodeIndex)
        {
            List<BrainNodeConnection> result = new List<BrainNodeConnection>();

            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].TargetIndex == nodeIndex) result.Add(Connections[i]);
            }

            return result;
        }
        public List<BrainNodeConnection> EnumerateOutgoingConnection(int nodeIndex)
        {
            List<BrainNodeConnection> result = new List<BrainNodeConnection>();

            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].OriginIndex == nodeIndex) result.Add(Connections[i]);
            }

            return result;
        }
        public List<int> EnumerateIncomingConnectionIndex(int nodeIndex)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].TargetIndex == nodeIndex) result.Add(i);
            }

            return result;
        }
        public List<int> EnumerateOutgoingConnectionIndex(int nodeIndex)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].OriginIndex == nodeIndex) result.Add(i);
            }

            return result;
        }

        private bool HasIncomingConnection(int nodeIndex)
        {
            if (EnumerateIncomingConnection(nodeIndex).Count > 0) return true;
            else return false;
        }
        private bool HasOutgoingConnection(int nodeIndex)
        {
            if (EnumerateOutgoingConnection(nodeIndex).Count > 0) return true;
            else return false;
        }
        private bool ContainsConnection(int originIndex, int targetIndex)
        {
            List<BrainNodeConnection> connections = EnumerateOutgoingConnection(originIndex);

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].TargetIndex == targetIndex) return true;
            }

            return false;
        }

        private int GetIncomingConnectionCount(int nodeIndex)
        {
            return EnumerateIncomingConnection(nodeIndex).Count;
        }
        private int GetOutgoingConnectionCount(int nodeIndex)
        {
            return EnumerateOutgoingConnection(nodeIndex).Count;
        }

        private void ApplyBrainInput(BrainInput brainInput)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                BrainNode target = Nodes[i];

                switch (target.Type)
                {
                    case BrainNodeType.Input_Bias:
                        target.Output = 1d;
                        break;
                    case BrainNodeType.Input_WallAvgAngle:
                        target.Output = brainInput.VisionData.WallAvgAngle;
                        break;
                    case BrainNodeType.Input_WallProximity:
                        target.Output = brainInput.VisionData.WallProximity;
                        break;
                    case BrainNodeType.Input_PlantAvgAngle:
                        target.Output = brainInput.VisionData.PlantAvgAngle;
                        break;
                    case BrainNodeType.Input_PlantProximity:
                        target.Output = brainInput.VisionData.PlantProximity;
                        break;
                    case BrainNodeType.Input_AnimalSameSpeciesAvgAngle:
                        target.Output = brainInput.VisionData.AnimalSameSpeciesAvgAngle;
                        break;
                    case BrainNodeType.Input_AnimalSameSpeciesProximity:
                        target.Output = brainInput.VisionData.AnimalSameSpeciesProximity;
                        break;
                    case BrainNodeType.Input_AnimalOtherSpeciesAvgAngle:
                        target.Output = brainInput.VisionData.AnimalOtherSpeciesAvgAngle;
                        break;
                    case BrainNodeType.Input_AnimalOtherSpeciesProximity:
                        target.Output = brainInput.VisionData.AnimalOtherSpeciesProximity;
                        break;
                    case BrainNodeType.Input_Velocity:
                        target.Output = brainInput.Velocity;
                        break;
                    case BrainNodeType.Input_AngularVelocity:
                        target.Output = brainInput.AngularVelocity;
                        break;
                    case BrainNodeType.Input_Satiety:
                        target.Output = brainInput.Satiety;
                        break;
                    case BrainNodeType.Input_Damage:
                        target.Output = brainInput.Damage;
                        break;
                    case BrainNodeType.Input_PheromoneRed:
                        target.Output = brainInput.VisionData.PheromoneRed;
                        break;
                    case BrainNodeType.Input_PheromoneGreen:
                        target.Output = brainInput.VisionData.PheromoneGreen;
                        break;
                    case BrainNodeType.Input_PheromoneBlue:
                        target.Output = brainInput.VisionData.PheromoneBlue;
                        break;
                    case BrainNodeType.Input_PheromoneRedAvgAngle:
                        target.Output = brainInput.VisionData.PheromoneRedAvgAngle;
                        break;
                    case BrainNodeType.Input_PheromoneGreenAvgAngle:
                        target.Output = brainInput.VisionData.PheromoneGreenAvgAngle;
                        break;
                    case BrainNodeType.Input_PheromoneBlueAvgAngle:
                        target.Output = brainInput.VisionData.PheromoneBlueAvgAngle;
                        break;
                    case BrainNodeType.Input_Memory0:
                        target.Output = brainInput.PrevStepOutput.Memory0;
                        break;
                    case BrainNodeType.Input_Memory1:
                        target.Output = brainInput.PrevStepOutput.Memory1;
                        break;
                    case BrainNodeType.Input_Memory2:
                        target.Output = brainInput.PrevStepOutput.Memory2;
                        break;
                    case BrainNodeType.Input_Memory3:
                        target.Output = brainInput.PrevStepOutput.Memory3;
                        break;
                    case BrainNodeType.Input_Memory4:
                        target.Output = brainInput.PrevStepOutput.Memory4;
                        break;
                    case BrainNodeType.Input_Memory5:
                        target.Output = brainInput.PrevStepOutput.Memory5;
                        break;
                    case BrainNodeType.Input_Memory6:
                        target.Output = brainInput.PrevStepOutput.Memory6;
                        break;
                    case BrainNodeType.Input_Memory7:
                        target.Output = brainInput.PrevStepOutput.Memory7;
                        break;
                    default:
                        target.Output = 0d;
                        break;
                }

                if (double.IsInfinity(target.Output) || double.IsNaN(target.Output)) target.Output = 0;
                target.Output = double.Max(-100d, double.Min(100d, target.Output));
            }
        }

        private void CalculateNodeOutput()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                BrainNode target = Nodes[i];

                switch (target.Type)
                {
                    case BrainNodeType.Hidden_ReLU:
                        target.Output = double.Max(0d, target.Input);
                        break;
                    case BrainNodeType.Hidden_LimitedReLU:
                        target.Output = double.Max(0d, double.Min(1d, target.Input));
                        break;
                    case BrainNodeType.Hidden_Step:
                        if (target.Input > 0) target.Output = 1;
                        else target.Output = 0;
                        break;
                    case BrainNodeType.Hidden_Sigmoid:
                        target.Output = (Math.Tanh(target.Input / 2d) + 1d) / 2d;
                        break;
                    case BrainNodeType.Hidden_Tanh:
                        target.Output = Math.Tanh(target.Input);
                        break;
                    case BrainNodeType.Hidden_Gaussian:
                        target.Output = Math.Exp(-(target.Input * target.Input));
                        break;
                    case BrainNodeType.Hidden_Identity:
                        target.Output = target.Input;
                        break;
                    case BrainNodeType.Hidden_Absolute:
                        target.Output = double.Abs(target.Input);
                        break;
                    case BrainNodeType.Hidden_Sine:
                        target.Output = Math.Sin(target.Input * Math.PI / 2d);
                        break;
                    case BrainNodeType.Hidden_Cosine:
                        target.Output = Math.Cos(target.Input * Math.PI / 2d);
                        break;
                    case BrainNodeType.Hidden_Tangent:
                        target.Output = Math.Tan(target.Input * Math.PI / 2d);
                        if (double.IsInfinity(target.Output) || double.IsNaN(target.Output)) target.Output = 100;
                        break;
                    case BrainNodeType.Hidden_LimitedTangent:
                        target.Output = Math.Tan(target.Input * Math.PI / 2d);
                        if (double.IsInfinity(target.Output) || double.IsNaN(target.Output)) target.Output = 1;
                        target.Output = double.Max(-1d, double.Min(1d, target.Output));
                        break;
                    default:
                        break;
                }
                target.Input = 0d;

                if (double.IsInfinity(target.Output) || double.IsNaN(target.Output)) target.Output = 0;
                target.Output = double.Max(-100d, double.Min(100d, target.Output));
            }
        }

        private void SendNodeOutput()
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                BrainNodeConnection target = Connections[i];

                Nodes[target.TargetIndex].Input += Nodes[target.OriginIndex].Output * target.Weight;
            }
        }

        private void ApplyBrainOutput(ref BrainOutput brainOutput)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                BrainNode target = Nodes[i];

                switch (target.Type)
                {
                    case BrainNodeType.Output_Acceleration:
                        brainOutput.Acceleration += target.Input;
                        break;
                    case BrainNodeType.Output_Rotation:
                        brainOutput.Rotation += target.Input;
                        break;
                    case BrainNodeType.Output_Attack:
                        brainOutput.Attack += target.Input;
                        break;
                    case BrainNodeType.Output_PheromoneRed:
                        brainOutput.PheromoneRed += target.Input;
                        break;
                    case BrainNodeType.Output_PheromoneGreen:
                        brainOutput.PheromoneGreen += target.Input;
                        break;
                    case BrainNodeType.Output_PheromoneBlue:
                        brainOutput.PheromoneBlue += target.Input;
                        break;
                    case BrainNodeType.Output_Memory0:
                        brainOutput.Memory0 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory1:
                        brainOutput.Memory1 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory2:
                        brainOutput.Memory2 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory3:
                        brainOutput.Memory3 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory4:
                        brainOutput.Memory4 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory5:
                        brainOutput.Memory5 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory6:
                        brainOutput.Memory6 += target.Input;
                        break;
                    case BrainNodeType.Output_Memory7:
                        brainOutput.Memory7 += target.Input;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
