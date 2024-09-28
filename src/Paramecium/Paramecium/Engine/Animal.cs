using System;
using System.Formats.Tar;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Paramecium.Engine
{
    public class Animal
    {
        public bool Exist { get; set; } = false;
        public bool Initialized { get; set; } = false;

        public int Index { get; set; } = -1;
        public long Id { get; set; } = -1;
        public long SpeciesId { get; set; }

        public int Generation { get; set; }
        public int Age { get; set; }

        public Double2d Position { get; set; }
        public Double2d Velocity { get; set; }
        public double Angle { get; set; }
        public double AngularVelocity { get; set; }
        public int IntegerizedPositionX { get; set; }
        public int IntegerizedPositionY { get; set; }

        public double Radius { get; set; }
        public double Mass { get; set; }

        public double Element { get; set; }
        public double Efficiency { get; set; }
        public double Fertility { get; set; }

        public int ColorRed { get; set; }
        public int ColorGreen { get; set; }
        public int ColorBlue { get; set; }

        public BrainNode[] Brain { get; set; }
        public double[] NeuralNetOutputs { get; set; }

        public Animal()
        {
            Brain = new BrainNode[0];
            NeuralNetOutputs = new double[0];
        }

        public Animal(Double2d position, double angle, double element, Random random)
        {
            SpeciesId = random.NextInt64(0, 2176782335 + 1);

            Generation = 1;

            Position = position;
            Angle = angle;
            IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
            IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

            Radius = 0.5;
            Mass = 16 + element;

            Element = element;
            Efficiency = 10d;

            ColorRed = random.Next(0, 255 + 1);
            ColorGreen = random.Next(0, 255 + 1);
            ColorBlue = random.Next(0, 255 + 1);

            Brain = new BrainNode[64];

            for (int i = 0; i < Brain.Length; i++)
            {
                Brain[i] = new BrainNode();
            }
            Brain[0] = new BrainNode
            {
                Type = BrainNodeType.Input_Bias,
                Connections = [10, 12, -1, -1],
                ConnectionWeights = [1, -0.9, 0, 0]
            };
            Brain[1] = new BrainNode
            {
                Type = BrainNodeType.Input_PlantAvgAngle,
                Connections = [11, -1, -1, -1],
                ConnectionWeights = [1, 0, 0, 0]
            };
            Brain[2] = new BrainNode
            {
                Type = BrainNodeType.Input_PlantProximity,
                Connections = [10, 12, -1, -1],
                ConnectionWeights = [-0.5, 1, 0, 0]
            };
            Brain[10] = new BrainNode
            {
                Type = BrainNodeType.Output_Acceleration,
                Connections = [-1, -1, -1, -1],
                ConnectionWeights = [0, 0, 0, 0]
            };
            Brain[11] = new BrainNode
            {
                Type = BrainNodeType.Output_Rotation,
                Connections = [-1, -1, -1, -1],
                ConnectionWeights = [0, 0, 0, 0]
            };
            Brain[12] = new BrainNode
            {
                Type = BrainNodeType.Output_Attack,
                Connections = [-1, -1, -1, -1],
                ConnectionWeights = [0, 0, 0, 0]
            };

            NeuralNetOutputs = new double[14];
        }

        public Animal(Animal parent, Double2d position, double element, Random random)
        {
            SpeciesId = parent.SpeciesId;

            Generation = parent.Generation + 1;

            Position = position;
            Angle = parent.Angle + 0.5;
            if (Angle < -0.5) Angle += 1;
            if (Angle > 0.5) Angle -= 1;
            IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
            IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

            Radius = 0.5;
            Mass = 16 + element;

            Element = element;
            Efficiency = 10d;

            ColorRed = parent.ColorRed;
            ColorGreen = parent.ColorGreen;
            ColorBlue = parent.ColorBlue;

            Brain = new BrainNode[64];
            for (int i = 0; i < Brain.Length; i++) Brain[i] = new BrainNode(parent.Brain[i]);

            if (random.NextDouble() < g_Soup.AnimalMutationRate)
            {
                if (random.NextDouble() < g_Soup.AnimalSpeciesIdMutationRate)
                {
                    SpeciesId = random.NextInt64(0, 2176782335 + 1);

                    ColorRed = random.Next(0, 255 + 1);
                    ColorGreen = random.Next(0, 255 + 1);
                    ColorBlue = random.Next(0, 255 + 1);
                }

                for (int i = 0; i < Brain.Length; i++)
                {
                    if (random.NextDouble() < g_Soup.AnimalBrainNodeMutationRate)
                    {
                        if (random.NextDouble() < 0.25d)
                        {
                            double randomValue = random.NextDouble();

                            if (randomValue < 0.5)
                            {
                                Brain[i].Type = (BrainNodeType)random.Next((int)BrainNodeType.Input_Bias, (int)BrainNodeType.Input_Memory7 + 1);
                            }
                            else if (randomValue < 0.75)
                            {
                                Brain[i].Type = (BrainNodeType)random.Next((int)BrainNodeType.Hidden_ReLU, (int)BrainNodeType.Hidden_LimitedTangent + 1);
                            }
                            else
                            {
                                Brain[i].Type = (BrainNodeType)random.Next((int)BrainNodeType.Output_Acceleration, (int)BrainNodeType.Output_Memory7 + 1);
                            }
                        }
                        if (random.NextDouble() < 0.5d)
                        {
                            for (int j = 0; j < Brain[i].Connections.Length; j++)
                            {
                                if (random.NextDouble() < 0.5d)
                                {
                                    if (Brain[i].Connections[j] != -1)
                                    {
                                        if (random.NextDouble() < 0.5d)
                                        {
                                            Brain[i].Connections[j] = -1;
                                            Brain[i].ConnectionWeights[j] = 0;
                                        }
                                        else
                                        {
                                            if (random.NextDouble() < 0.5d)
                                            {
                                                Brain[i].Connections[j] = random.Next(0, Brain.Length);
                                            }
                                            if (random.NextDouble() < 0.5d)
                                            {
                                                Brain[i].ConnectionWeights[j] = random.NextDouble() * 4d - 2d;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Brain[i].Connections[j] = random.Next(0, Brain.Length);
                                        Brain[i].ConnectionWeights[j] = random.NextDouble() * 4d - 2d;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            NeuralNetOutputs = new double[14];
        }

        public void Initialize(int index, Random random)
        {
            if (!Initialized)
            {
                Exist = true;

                Index = index;
                Id = random.NextInt64(0, 4738381338321616896);

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Add(Index);

                Initialized = true;
            }
            else throw new InvalidOperationException("This animal has already been initialized.");
        }

        public void UpdateNeuralNet()
        {
            if (Initialized)
            {
                double WallAvgAngle = 0d;
                double WallProximity = 0d;
                int WallAvgAngleEntryCount = 0;

                double PlantAvgAngle = 0d;
                double PlantProximity = 0d;
                int PlantAvgAngleEntryCount = 0;

                double AnimalSameSpeciesAvgAngle = 0d;
                double AnimalSameSpeciesProximity = 0d;
                int AnimalSameSpeciesAvgAngleEntryCount = 0;

                double AnimalOtherSpeciesAvgAngle = 0d;
                double AnimalOtherSpeciesProximity = 0d;
                int AnimalOtherSpeciesAvgAngleEntryCount = 0;

                for (int i = 0; i < 29; i++)
                {
                    double angle = -0.25d + (0.25 - -0.25) / 28 * i;
                    if (angle > 0.5d) angle -= 1d;
                    if (angle < -0.5d) angle += 1d;
                    Double2d rayDirection = Double2d.FromAngle(Angle + angle);

                    for (int j = 0; j < 9; j++)
                    {
                        Double2d rayPosition = Position + rayDirection * (j + 1);
                        int rayPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayPosition.X)));
                        int rayPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayPosition.Y)));

                        if (rayPosition.X < 0 || rayPosition.X > g_Soup.SizeX || rayPosition.Y < 0 || rayPosition.Y > g_Soup.SizeY)
                        {
                            WallAvgAngle += angle * 2d * ((9 - j) / 9d);
                            WallProximity = double.Max(WallProximity, ((9 - j) / 9d));
                            WallAvgAngleEntryCount++;
                            break;
                        }
                        else
                        {
                            bool wallHitFlag = false;

                            for (int x = 0; x <= 1; x++)
                            {
                                for (int y = 0; y <= 1; y++)
                                {
                                    Double2d rayScanPosition = rayPosition + new Double2d(-0.5, -0.5) + new Double2d(x, y);

                                    if (rayScanPosition.X < 0 || rayScanPosition.X > g_Soup.SizeX || rayScanPosition.Y < 0 || rayScanPosition.Y > g_Soup.SizeY)
                                    {
                                        WallAvgAngle += angle * 2d * ((9 - j) / 9d);
                                        WallProximity = double.Max(WallProximity, ((9 - j) / 9d));
                                        WallAvgAngleEntryCount++;
                                        wallHitFlag = true;
                                    }
                                    else
                                    {
                                        int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                        int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                        Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];
                                        
                                        if (targetTile.Type == TileType.Wall)
                                        {
                                            WallAvgAngle += angle * 2d * ((9 - j) / 9d);
                                            WallProximity = double.Max(WallProximity, ((9 - j) / 9d));
                                            WallAvgAngleEntryCount++;
                                            wallHitFlag = true;
                                        }
                                    }
                                }
                            }

                            if (wallHitFlag) break;
                            else
                            {
                                for (int x = 0; x <= 1; x++)
                                {
                                    for (int y = 0; y <= 1; y++)
                                    {
                                        Double2d rayScanPosition = rayPosition + new Double2d(-0.5, -0.5) + new Double2d(x, y);

                                        int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                        int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                        Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                        if (targetTile.LocalPlantPopulation > 0)
                                        {
                                            for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                            {
                                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                                if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5 * 0.5)
                                                {
                                                    PlantAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - Position, -Angle)) * 2d * ((9 - j) / 9d);
                                                    PlantProximity = double.Max(PlantProximity, ((9 - j) / 9d));
                                                    PlantAvgAngleEntryCount++;
                                                }
                                            }
                                        }
                                        if (targetTile.LocalAnimalPopulation > 0)
                                        {
                                            for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                            {
                                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                                if (targetAnimal.Id != Id)
                                                {
                                                    if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5 * 0.5)
                                                    {
                                                        if (targetAnimal.SpeciesId == SpeciesId)
                                                        {
                                                            AnimalSameSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle)) * 2d * ((9 - j) / 9d);
                                                            AnimalSameSpeciesProximity = double.Max(PlantProximity, ((9 - j) / 9d));
                                                            AnimalSameSpeciesAvgAngleEntryCount++;
                                                        }
                                                        else
                                                        {
                                                            AnimalOtherSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle)) * 2d * ((9 - j) / 9d);
                                                            AnimalOtherSpeciesProximity = double.Max(PlantProximity, ((9 - j) / 9d));
                                                            AnimalOtherSpeciesAvgAngleEntryCount++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    double angle = 0.3d + (0.7 - 0.3) / 6 * i;
                    if (angle > 0.5d) angle -= 1d;
                    if (angle < -0.5d) angle += 1d;
                    Double2d rayDirection = Double2d.FromAngle(Angle + angle);

                    for (int j = 0; j < 3; j++)
                    {
                        Double2d rayPosition = Position + rayDirection * (j + 1);

                        if (rayPosition.X < 0 || rayPosition.X > g_Soup.SizeX || rayPosition.Y < 0 || rayPosition.Y > g_Soup.SizeY)
                        {
                            WallAvgAngle += angle * 2d * ((9 - j) / 9d);
                            WallProximity = double.Max(WallProximity, ((9 - j) / 9d));
                            WallAvgAngleEntryCount++;
                            break;
                        }
                        else
                        {
                            bool wallHitFlag = false;

                            for (int x = 0; x <= 1; x++)
                            {
                                for (int y = 0; y <= 1; y++)
                                {
                                    Double2d rayScanPosition = rayPosition + new Double2d(-0.5, -0.5) + new Double2d(x, y);

                                    if (rayScanPosition.X < 0 || rayScanPosition.X > g_Soup.SizeX || rayScanPosition.Y < 0 || rayScanPosition.Y > g_Soup.SizeY)
                                    {
                                        WallAvgAngle += angle * 2d * ((3 - j) / 3d);
                                        WallProximity = double.Max(WallProximity, ((3 - j) / 3d));
                                        WallAvgAngleEntryCount++;
                                        wallHitFlag = true;
                                    }
                                    else
                                    {
                                        int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                        int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                        Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                        if (targetTile.Type == TileType.Wall)
                                        {
                                            WallAvgAngle += angle * 2d * ((3 - j) / 3d);
                                            WallProximity = double.Max(WallProximity, ((3 - j) / 3d));
                                            WallAvgAngleEntryCount++;
                                            wallHitFlag = true;
                                        }
                                    }
                                }
                            }

                            for (int x = 0; x <= 1; x++)
                            {
                                for (int y = 0; y <= 1; y++)
                                {
                                    if (
                                        !wallHitFlag ||
                                        (rayPosition.X >= Position.X && rayPosition.Y >= Position.Y && x == 0 && y == 0) ||
                                        (rayPosition.X >= Position.X && rayPosition.Y < Position.Y && x == 0 && y == 1) ||
                                        (rayPosition.X < Position.X && rayPosition.Y >= Position.Y && x == 1 && y == 0) ||
                                        (rayPosition.X < Position.X && rayPosition.Y < Position.Y && x == 1 && y == 1)
                                    )
                                    {
                                        Double2d rayScanPosition = rayPosition + new Double2d(-0.5, -0.5) + new Double2d(x, y);

                                        int rayScanPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                        int rayScanPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                        Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * g_Soup.SizeX + rayScanPositionIntegerizedX];

                                        if (targetTile.LocalPlantPopulation > 0)
                                        {
                                            for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                            {
                                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                                if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5 * 0.5)
                                                {
                                                    PlantAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - Position, -Angle)) * 2d * ((3 - j) / 3d);
                                                    PlantProximity = double.Max(PlantProximity, ((3 - j) / 3d));
                                                    PlantAvgAngleEntryCount++;
                                                }
                                            }
                                        }
                                        if (targetTile.LocalAnimalPopulation > 0)
                                        {
                                            for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                            {
                                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                                if (targetAnimal.Id != Id)
                                                {
                                                    if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5 * 0.5)
                                                    {
                                                        if (targetAnimal.SpeciesId == SpeciesId)
                                                        {
                                                            AnimalSameSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle)) * 2d * ((3 - j) / 3d);
                                                            AnimalSameSpeciesProximity = double.Max(PlantProximity, ((3 - j) / 3d));
                                                            AnimalSameSpeciesAvgAngleEntryCount++;
                                                        }
                                                        else
                                                        {
                                                            AnimalOtherSpeciesAvgAngle += Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle)) * 2d * ((3 - j) / 3d);
                                                            AnimalOtherSpeciesProximity = double.Max(PlantProximity, ((3 - j) / 3d));
                                                            AnimalOtherSpeciesAvgAngleEntryCount++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (wallHitFlag) break;
                        }
                    }
                }

                if (WallAvgAngleEntryCount > 0) WallAvgAngle /= WallAvgAngleEntryCount;
                if (PlantAvgAngleEntryCount > 0) PlantAvgAngle /= PlantAvgAngleEntryCount;
                if (AnimalSameSpeciesAvgAngleEntryCount > 0) AnimalSameSpeciesAvgAngle /= AnimalSameSpeciesAvgAngleEntryCount;
                if (AnimalOtherSpeciesAvgAngleEntryCount > 0) AnimalOtherSpeciesAvgAngle /= AnimalOtherSpeciesAvgAngleEntryCount;

                for (int i = 0; i < Brain.Length; i++)
                {
                    switch (Brain[i].Type)
                    {
                        case BrainNodeType.Input_Bias:
                            Brain[i].Output = 1d;
                            break;
                        case BrainNodeType.Input_WallAvgAngle:
                            Brain[i].Output = WallAvgAngle;
                            break;
                        case BrainNodeType.Input_WallProximity:
                            Brain[i].Output = WallProximity;
                            break;
                        case BrainNodeType.Input_PlantAvgAngle:
                            Brain[i].Output = PlantAvgAngle;
                            break;
                        case BrainNodeType.Input_PlantProximity:
                            Brain[i].Output = PlantProximity;
                            break;
                        case BrainNodeType.Input_AnimalSameSpeciesAvgAngle:
                            Brain[i].Output = AnimalSameSpeciesAvgAngle;
                            break;
                        case BrainNodeType.Input_AnimalSameSpeciesProximity:
                            Brain[i].Output = AnimalSameSpeciesProximity;
                            break;
                        case BrainNodeType.Input_AnimalOtherSpeciesAvgAngle:
                            Brain[i].Output = AnimalOtherSpeciesAvgAngle;
                            break;
                        case BrainNodeType.Input_AnimalOtherSpeciesProximity:
                            Brain[i].Output = AnimalOtherSpeciesProximity;
                            break;
                        case BrainNodeType.Input_Velocity:
                            Brain[i].Output = Velocity.Length / g_Soup.MaximumVelocity;
                            break;
                        case BrainNodeType.Input_AngularVelocity:
                            Brain[i].Output = AngularVelocity / g_Soup.MaximumAngularVelocity;
                            break;
                        case BrainNodeType.Input_Satiety:
                            Brain[i].Output = Element / g_Soup.AnimalForkCost;
                            break;
                        case BrainNodeType.Input_Memory0:
                            Brain[i].Output = NeuralNetOutputs[0];
                            break;
                        case BrainNodeType.Input_Memory1:
                            Brain[i].Output = NeuralNetOutputs[1];
                            break;
                        case BrainNodeType.Input_Memory2:
                            Brain[i].Output = NeuralNetOutputs[2];
                            break;
                        case BrainNodeType.Input_Memory3:
                            Brain[i].Output = NeuralNetOutputs[3];
                            break;
                        case BrainNodeType.Input_Memory4:
                            Brain[i].Output = NeuralNetOutputs[4];
                            break;
                        case BrainNodeType.Input_Memory5:
                            Brain[i].Output = NeuralNetOutputs[5];
                            break;
                        case BrainNodeType.Input_Memory6:
                            Brain[i].Output = NeuralNetOutputs[6];
                            break;
                        case BrainNodeType.Input_Memory7:
                            Brain[i].Output = NeuralNetOutputs[7];
                            break;

                        case BrainNodeType.Hidden_ReLU:
                            Brain[i].Output = double.Max(0d, Brain[i].Input);
                            break;
                        case BrainNodeType.Hidden_LimitedReLU:
                            Brain[i].Output = double.Max(0d, double.Min(1d, Brain[i].Input));
                            break;
                        case BrainNodeType.Hidden_Step:
                            if (Brain[i].Input > 0) Brain[i].Output = 1;
                            else Brain[i].Output = 0;
                            break;
                        case BrainNodeType.Hidden_Sigmoid:
                            Brain[i].Output = Math.Tanh(Brain[i].Input);
                            break;
                        case BrainNodeType.Hidden_Identity:
                            Brain[i].Output = Brain[i].Input;
                            break;
                        case BrainNodeType.Hidden_Absolute:
                            Brain[i].Output = double.Abs(Brain[i].Input);
                            break;
                        case BrainNodeType.Hidden_Sine:
                            Brain[i].Output = Math.Sin(Brain[i].Input * Math.PI / 2d);
                            break;
                        case BrainNodeType.Hidden_Cosine:
                            Brain[i].Output = Math.Cos(Brain[i].Input * Math.PI / 2d);
                            break;
                        case BrainNodeType.Hidden_Tangent:
                            Brain[i].Output = Math.Tan(Brain[i].Input * Math.PI / 2d);
                            if (Brain[i].Output == double.PositiveInfinity || Brain[i].Output == double.NegativeInfinity || Brain[i].Output == double.NaN) Brain[i].Output = 100;
                            break;
                        case BrainNodeType.Hidden_LimitedTangent:
                            Brain[i].Output = Math.Tan(Brain[i].Input * Math.PI / 2d);
                            if (Brain[i].Output == double.PositiveInfinity || Brain[i].Output == double.NegativeInfinity || Brain[i].Output == double.NaN) Brain[i].Output = 0;
                            Brain[i].Output = double.Max(-1d, double.Min(1d, Brain[i].Output));
                            break;

                        default:
                            Brain[i].Output = 0;
                            break;
                    }
                    Brain[i].Input = 0d;

                    if (Brain[i].Output > 100d) Brain[i].Output = 100d;
                    else if (Brain[i].Output < -100d) Brain[i].Output = -100d;
                }
                for (int i = 0; i < Brain.Length; i++)
                {
                    for (int j = 0; j < Brain[i].Connections.Length; j++)
                    {
                        if (Brain[i].Connections[j] != -1)
                        {
                            Brain[Brain[i].Connections[j]].Input += Brain[i].Output * Brain[i].ConnectionWeights[j];
                        }
                    }
                }
                for (int i = 0; i < Brain.Length; i++)
                {
                    if (Brain[i].Input > 100d) Brain[i].Input = 100d;
                    if (Brain[i].Input < -100d) Brain[i].Input = -100d;
                }

                for (int i = 0; i < NeuralNetOutputs.Length; i++) NeuralNetOutputs[i] = 0;
                for (int i = 0; i < Brain.Length; i++)
                {
                    switch (Brain[i].Type)
                    {
                        case BrainNodeType.Output_Memory0:
                            NeuralNetOutputs[0] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory1:
                            NeuralNetOutputs[1] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory2:
                            NeuralNetOutputs[2] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory3:
                            NeuralNetOutputs[3] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory4:
                            NeuralNetOutputs[4] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory5:
                            NeuralNetOutputs[5] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory6:
                            NeuralNetOutputs[6] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Memory7:
                            NeuralNetOutputs[7] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Acceleration:
                            NeuralNetOutputs[8] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Rotation:
                            NeuralNetOutputs[9] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_Attack:
                            NeuralNetOutputs[10] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_PheromoneRed:
                            NeuralNetOutputs[11] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_PheromoneGreen:
                            NeuralNetOutputs[12] += Brain[i].Input;
                            break;
                        case BrainNodeType.Output_PheromoneBlue:
                            NeuralNetOutputs[13] += Brain[i].Input;
                            break;
                        default:
                            break;
                    }
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyNeuralNetOutput()
        {
            if (Initialized)
            {
                Velocity += Double2d.FromAngle(Angle) * double.Max(-1d, double.Min(1d, NeuralNetOutputs[8])) * g_Soup.MaximumVelocity * g_Soup.Drag;
                AngularVelocity += double.Max(-1d, double.Min(1d, NeuralNetOutputs[9])) * g_Soup.MaximumAngularVelocity * g_Soup.AngularVelocityDrag;

                if (NeuralNetOutputs[10] > 0)
                {
                    for (int x = int.Max(0, int.Min(g_Soup.SizeX - 1, IntegerizedPositionX - 1)); x <= int.Max(0, int.Min(g_Soup.SizeX - 1, IntegerizedPositionX + 1)); x++)
                    {
                        for (int y = int.Max(0, int.Min(g_Soup.SizeY - 1, IntegerizedPositionY - 1)); y <= int.Max(0, int.Min(g_Soup.SizeY - 1, IntegerizedPositionY + 1)); y++)
                        {
                            Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

                            if (targetTile.LocalPlantPopulation > 0)
                            {
                                for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                                {
                                    Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                    if (targetPlant.Exist)
                                    {
                                        if (Double2d.DistanceSquared(Position, targetPlant.Position) < (Radius + targetPlant.Radius + 0.1) * (Radius + targetPlant.Radius + 0.1))
                                        {
                                            if (double.Abs(Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - Position, -Angle))) < 0.167d)
                                            {
                                                Element += double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate);
                                                targetPlant.Element -= double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate);
                                            }
                                        }
                                    }
                                }
                            }

                            if (targetTile.LocalAnimalPopulation > 0)
                            {
                                for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                                {
                                    Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                    if (targetAnimal.Exist && targetAnimal.Id != Id)
                                    {
                                        if (Double2d.DistanceSquared(Position, targetAnimal.Position) < (Radius + targetAnimal.Radius + 0.1) * (Radius + targetAnimal.Radius + 0.1))
                                        {
                                            if (double.Abs(Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle))) < 0.167d)
                                            {
                                                Element += double.Min(targetAnimal.Element, g_Soup.AnimalAnimalIngestionRate);
                                                targetAnimal.Element -= double.Min(targetAnimal.Element, g_Soup.AnimalAnimalIngestionRate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void UpdateCollision()
        {
            if (Initialized)
            {
                for (int x = int.Max(0, int.Min(g_Soup.SizeX - 1, IntegerizedPositionX - 1)); x <= int.Max(0, int.Min(g_Soup.SizeX - 1, IntegerizedPositionX + 1)); x++)
                {
                    for (int y = int.Max(0, int.Min(g_Soup.SizeY - 1, IntegerizedPositionY - 1)); y <= int.Max(0, int.Min(g_Soup.SizeY - 1, IntegerizedPositionY + 1)); y++)
                    {
                        Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

                        if (targetTile.Type == TileType.Wall)
                        {
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.25d), 0.356d) * g_Soup.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.25d), 0.356d) * g_Soup.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.75d), 0.356d) * g_Soup.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.75d), 0.356d) * g_Soup.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.5d, y + 0.5d), 0.5d) * g_Soup.RestitutionCoefficient;
                        }

                        if(targetTile.LocalPlantPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                if (targetPlant.Exist) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetPlant.Position, targetPlant.Radius, targetPlant.Mass) * g_Soup.RestitutionCoefficient;
                            }
                        }

                        if (targetTile.LocalAnimalPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                            {
                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                if (targetAnimal.Exist && targetAnimal.Id != Id) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetAnimal.Position, targetAnimal.Radius, targetAnimal.Mass) * g_Soup.RestitutionCoefficient;
                            }
                        }
                    }
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }
        public Double2d CalculateCollisionTwoObjects(Double2d obj1Pos, double obj1Radius, double obj1Mass, Double2d obj2Pos, double obj2Radius, double obj2Mass)
        {
            if (Double2d.DistanceSquared(obj1Pos, obj2Pos) < (obj1Radius + obj2Radius) * (obj1Radius + obj2Radius)) return (obj1Pos - obj2Pos).Normalized * (1d - Double2d.Distance(obj1Pos, obj2Pos) / (obj1Radius + obj2Radius)) * Math.Min(1d, obj2Mass / obj1Mass);
            else return Double2d.Zero;
        }
        public Double2d CalculateCollisionTwoObjects(Double2d obj1Pos, double obj1Radius, Double2d obj2Pos, double obj2Radius)
        {
            if (Double2d.DistanceSquared(obj1Pos, obj2Pos) < (obj1Radius + obj2Radius) * (obj1Radius + obj2Radius)) return (obj1Pos - obj2Pos).Normalized * (1d - Double2d.Distance(obj1Pos, obj2Pos) / (obj1Radius + obj2Radius));
            else return Double2d.Zero;
        }

        public void UpdatePosition()
        {
            if (Initialized)
            {
                if (Velocity.LengthSquared > g_Soup.MaximumVelocity * g_Soup.MaximumVelocity)
                {
                    Velocity /= Velocity.Length / g_Soup.MaximumVelocity;
                }
                Position += Velocity;

                if (Position.X < Radius)
                {
                    Position = new Double2d(0 + Radius, Position.Y);
                    Velocity = new Double2d(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.X > g_Soup.SizeX - Radius)
                {
                    Position = new Double2d(g_Soup.SizeX - Radius, Position.Y);
                    Velocity = new Double2d(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.Y < Radius)
                {
                    Position = new Double2d(Position.X, Radius);
                    Velocity = new Double2d(Velocity.X, Velocity.Y * -1d);
                }
                if (Position.Y > g_Soup.SizeY - Radius)
                {
                    Position = new Double2d(Position.X, g_Soup.SizeY - Radius);
                    Velocity = new Double2d(Velocity.X, Velocity.Y * -1d);
                }

                if (double.Abs(AngularVelocity) > g_Soup.MaximumAngularVelocity)
                {
                    AngularVelocity /= AngularVelocity / g_Soup.MaximumAngularVelocity;
                }
                Angle += AngularVelocity;

                if (Angle < -0.5) Angle += 1;
                if (Angle > 0.5) Angle -= 1;

                if (int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X))) != IntegerizedPositionX || int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y))) != IntegerizedPositionY)
                {
                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Remove(Index);

                    IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
                    IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Add(Index);
                }

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].Element += double.Min(Element, g_Soup.AnimalElementUpkeep);
                Element -= double.Min(Element, g_Soup.AnimalElementUpkeep);

                Radius = 0.5;
                Mass = 16 + Element;

                if (Age >= g_Soup.AnimalMaximumAge)
                {
                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].Element = Element;
                    Element = 0;
                }

                if (Element <= 0) OnDisable();

                Age++;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyDrag()
        {
            if (Initialized)
            {
                Velocity *= 1d - g_Soup.Drag;
                AngularVelocity *= 1d - g_Soup.AngularVelocityDrag;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public Animal? CreateOffspring(Random random)
        {
            if (Initialized)
            {
                if (Element >= g_Soup.AnimalForkCost * 2)
                {
                    Double2d OffspringPosition = Position + Double2d.FromAngle(Angle + 0.5) * 0.5;
                    if (
                        OffspringPosition.X >= 0 && OffspringPosition.X <= g_Soup.SizeX && OffspringPosition.Y >= 0 && OffspringPosition.Y <= g_Soup.SizeY && 
                        g_Soup.Tiles[int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(OffspringPosition.Y))) * g_Soup.SizeX + int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(OffspringPosition.X)))].Type == TileType.Default
                    )
                    {
                        Element -= g_Soup.AnimalForkCost;
                        g_Soup.TotalBornCount++;
                        return new Animal(this, Position + Double2d.FromAngle(Angle + 0.5) * 0.5, g_Soup.AnimalForkCost, random);
                    }
                    else return null;
                }
                else return null;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void OnDisable()
        {
            if (Initialized)
            {
                Exist = false;

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Remove(Index);
                g_Soup.AnimalUnusedIndexes.Add(Index);

                g_Soup.TotalDieCount++;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }
    }

    public class BrainNode
    {
        public double Input { get; set; }
        public double Output { get; set; }

        public BrainNodeType Type { get; set; } = BrainNodeType.NonOperation;

        public int[] Connections { get; set; }
        public double[] ConnectionWeights { get; set; }

        public BrainNode()
        {
            Connections = new int[4];
            for (int i = 0; i < Connections.Length; i++) Connections[i] = -1;

            ConnectionWeights = new double[4];
            for (int i = 0; i < ConnectionWeights.Length; i++) ConnectionWeights[i] = 0;
        }
        public BrainNode(BrainNode neuron)
        {
            Type = neuron.Type;
            Connections = new int[neuron.Connections.Length];
            for (int i = 0; i < Connections.Length; i++) Connections[i] = neuron.Connections[i];
            ConnectionWeights = new double[neuron.ConnectionWeights.Length];
            for (int i = 0; i < ConnectionWeights.Length; i++) ConnectionWeights[i] = neuron.ConnectionWeights[i];
        }
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
}
