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
            Brain result = Duplicate(brain);

            for (int i = 0; i < g_Soup.AnimalMaximumMutationCount; i++)
            {
                int MutationType = random.Next(0, 6 + 1);
                int MutationTarget;

                if (MutationType == 0) // Add Node
                {
                    if (result.nodes.Count < g_Soup.AnimalBrainMaximumNodeCount)
                    {
                        MutationTarget = random.Next(0, result.nodes.Count + 1);
                        result.nodes.Insert(MutationTarget, new BrainNode());

                        for (int j = 0; j < result.nodes.Count; j++)
                        {
                            for (int k = 0; k < result.nodes[j].Connections.Count; k++)
                            {
                                if (result.nodes[j].Connections[k].TargetIndex >= MutationTarget) result.nodes[j].Connections[k].TargetIndex++;
                            }
                        }

                        int NodeType = random.Next(0, 2 + 1);
                        switch (NodeType)
                        {
                            case 0:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                                break;
                            case 1:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                                break;
                            case 2:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                                break;
                        }

                        for (int j = 0; j < 8; j++)
                        {
                            result.nodes[MutationTarget].Connections.Add(new BrainNodeConnection());
                            result.nodes[MutationTarget].Connections[j].TargetIndex = random.Next(0, result.nodes.Count);
                            result.nodes[MutationTarget].Connections[j].Weight = random.NextDouble() * 4d - 2d;

                            if (random.NextDouble() < 0.5d) break;
                        }
                    }
                }
                else if (MutationType == 1) // Remove Node
                {
                    if (result.nodes.Count > 1)
                    {
                        MutationTarget = random.Next(0, result.nodes.Count);
                        result.nodes.RemoveAt(MutationTarget);

                        for (int j = 0; j < result.nodes.Count; j++)
                        {
                            if (result.nodes[j].Connections.Count > 0)
                            {
                                for (int k = result.nodes[j].Connections.Count - 1; k >= 0; k--)
                                {
                                    if (result.nodes[j].Connections[k].TargetIndex == MutationTarget)
                                    {
                                        result.nodes[j].Connections.RemoveAt(k);
                                    }
                                    else if (result.nodes[j].Connections[k].TargetIndex > MutationTarget)
                                    {
                                        result.nodes[j].Connections[k].TargetIndex--;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (MutationType == 2) // Change Node Type
                {
                    if (result.nodes.Count > 1)
                    {
                        MutationTarget = random.Next(0, result.nodes.Count);

                        int NodeType = random.Next(0, 2 + 1);
                        switch (NodeType)
                        {
                            case 0:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                                break;
                            case 1:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                                break;
                            case 2:
                                result.nodes[MutationTarget].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                                break;
                        }
                    }
                }
                else if (MutationType == 3) // Add Connection
                {
                    MutationTarget = random.Next(0, result.nodes.Count);

                    for (int j = 0; j < 8; j++)
                    {
                        if (result.nodes[MutationTarget].Connections.Count >= g_Soup.AnimalBrainMaximumConnectionCount) break;

                        int ConnectionMutationTarget = random.Next(0, result.nodes[MutationTarget].Connections.Count + 1);

                        result.nodes[MutationTarget].Connections.Insert(ConnectionMutationTarget, new BrainNodeConnection());
                        result.nodes[MutationTarget].Connections[ConnectionMutationTarget].TargetIndex = random.Next(0, result.nodes.Count);
                        result.nodes[MutationTarget].Connections[ConnectionMutationTarget].Weight = random.NextDouble() * 4d - 2d;

                        if (random.NextDouble() < 0.5d) break;
                    }
                }
                else if (MutationType == 4) // Remove Connection
                {
                    MutationTarget = random.Next(0, result.nodes.Count);

                    for (int j = 0; j < 8; j++)
                    {
                        if (result.nodes[MutationTarget].Connections.Count == 0) break;

                        int ConnectionMutationTarget = random.Next(0, result.nodes[MutationTarget].Connections.Count);

                        result.nodes[MutationTarget].Connections.RemoveAt(ConnectionMutationTarget);

                        if (random.NextDouble() < 0.5d) break;
                    }
                }
                else if (MutationType == 5) // Change Connection Target
                {
                    MutationTarget = random.Next(0, result.nodes.Count);

                    for (int j = 0; j < 8; j++)
                    {
                        if (result.nodes[MutationTarget].Connections.Count == 0) break;

                        int ConnectionMutationTarget = random.Next(0, result.nodes[MutationTarget].Connections.Count);

                        result.nodes[MutationTarget].Connections[ConnectionMutationTarget].TargetIndex = random.Next(0, result.nodes.Count);

                        if (random.NextDouble() < 0.5d) break;
                    }
                }
                else if (MutationType == 6) // Change Connection Weight
                {
                    MutationTarget = random.Next(0, result.nodes.Count);

                    for (int j = 0; j < 8; j++)
                    {
                        if (result.nodes[MutationTarget].Connections.Count == 0) break;

                        int ConnectionMutationTarget = random.Next(0, result.nodes[MutationTarget].Connections.Count);

                        result.nodes[MutationTarget].Connections[ConnectionMutationTarget].Weight = random.NextDouble() * 4d - 2d;

                        if (random.NextDouble() < 0.5d) break;
                    }
                }

                if (random.NextDouble() < 0.5d) break;
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

            int WallAvgAngleEntryCount = 0;
            int PlantAvgAngleEntryCount = 0;
            int AnimalSameSpeciesAvgAngleEntryCount = 0;
            int AnimalOtherSpeciesAvgAngleEntryCount = 0;

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
                                //result.WallAvgAngle += Double2d.ToAngle(Double2d.Rotate(rayScanPosition - originPosition, -angle)) * 2d * ((frontViewRange - j) / frontViewRange);
                                result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRange);
                                //result.WallProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(rayScanPosition, originPosition) / frontViewRange));
                                result.WallProximity = double.Max(result.WallProximity, 1d - ((frontViewRange - j) / frontViewRange));
                                WallAvgAngleEntryCount++;
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    //result.WallAvgAngle += Double2d.ToAngle(Double2d.Rotate(rayScanPosition - originPosition, -angle)) * 2d * ((frontViewRange - j) / frontViewRange);
                                    result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRange);
                                    //result.WallProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(rayScanPosition, originPosition) / frontViewRange));
                                    result.WallProximity = double.Max(result.WallProximity, 1d - ((frontViewRange - j) / frontViewRange));
                                    WallAvgAngleEntryCount++;
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
                                            //result.PlantAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - originPosition, -angle)) * 2d * ((frontViewRange - j) / frontViewRange);
                                            result.PlantAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRange);
                                            //result.PlantProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetPlant.Position, originPosition) / frontViewRange));
                                            result.PlantProximity = double.Max(result.WallProximity, 1d - ((frontViewRange - j) / frontViewRange));
                                            PlantAvgAngleEntryCount++;
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
                                                    //result.AnimalSameSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - originPosition, -angle)) * 2d * ((frontViewRange - j) / frontViewRange);
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRange);
                                                    //result.AnimalSameSpeciesProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetAnimal.Position, originPosition) / frontViewRange));
                                                    result.AnimalSameSpeciesProximity = double.Max(result.WallProximity, 1d - ((frontViewRange - j) / frontViewRange));
                                                    AnimalSameSpeciesAvgAngleEntryCount++;
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    //result.AnimalOtherSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - originPosition, -angle)) * 2d * ((frontViewRange - j) / frontViewRange);
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRange);
                                                    //result.AnimalOtherSpeciesProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetAnimal.Position, originPosition) / frontViewRange));
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.WallProximity, 1d - ((frontViewRange - j) / frontViewRange));
                                                    AnimalOtherSpeciesAvgAngleEntryCount++;
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
                                //result.WallAvgAngle += Double2d.ToAngle(Double2d.Rotate(rayScanPosition - originPosition, -angle)) * 2d * ((backViewRange - j) / backViewRange);
                                result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRange);
                                //result.WallProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(rayScanPosition, originPosition) / backViewRange));
                                result.WallProximity = double.Max(result.WallProximity, 1d - ((backViewRange - j) / backViewRange));
                                WallAvgAngleEntryCount++;
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    //result.WallAvgAngle += Double2d.ToAngle(Double2d.Rotate(rayScanPosition - originPosition, -angle)) * 2d * ((backViewRange - j) / backViewRange);
                                    result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRange);
                                    //result.WallProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(rayScanPosition, originPosition) / backViewRange));
                                    result.WallProximity = double.Max(result.WallProximity, 1d - ((backViewRange - j) / backViewRange));
                                    WallAvgAngleEntryCount++;
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
                                            //result.PlantAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - originPosition, -angle)) * 2d * ((backViewRange - j) / backViewRange);
                                            result.PlantAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRange);
                                            //result.PlantProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetPlant.Position, originPosition) / backViewRange));
                                            result.PlantProximity = double.Max(result.WallProximity, 1d - ((backViewRange - j) / backViewRange));
                                            PlantAvgAngleEntryCount++;
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
                                                    //result.AnimalSameSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - originPosition, -angle)) * 2d * ((backViewRange - j) / backViewRange);
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRange);
                                                    //result.AnimalSameSpeciesProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetAnimal.Position, originPosition) / backViewRange));
                                                    result.AnimalSameSpeciesProximity = double.Max(result.WallProximity, 1d - ((backViewRange - j) / backViewRange));
                                                    AnimalSameSpeciesAvgAngleEntryCount++;
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    //result.AnimalOtherSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - originPosition, -angle)) * 2d * ((backViewRange - j) / backViewRange);
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRange);
                                                    //result.AnimalOtherSpeciesProximity = double.Max(result.WallProximity, 1d - (Double2d.Distance(targetAnimal.Position, originPosition) / backViewRange));
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.WallProximity, 1d - ((backViewRange - j) / backViewRange));
                                                    AnimalOtherSpeciesAvgAngleEntryCount++;
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

            if (WallAvgAngleEntryCount > 0) result.WallAvgAngle /= WallAvgAngleEntryCount;
            if (PlantAvgAngleEntryCount > 0) result.PlantAvgAngle /= PlantAvgAngleEntryCount;
            if (AnimalSameSpeciesAvgAngleEntryCount > 0) result.AnimalSameSpeciesAvgAngle /= AnimalSameSpeciesAvgAngleEntryCount;
            if (AnimalOtherSpeciesAvgAngleEntryCount > 0) result.AnimalOtherSpeciesAvgAngle /= AnimalOtherSpeciesAvgAngleEntryCount;

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
