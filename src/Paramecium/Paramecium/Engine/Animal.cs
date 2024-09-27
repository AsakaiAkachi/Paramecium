using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

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

        public double[] NeuralNetWeights { get; set; }
        public double[] NeuralNetInputs { get; set; }
        public double[] NeuralNetOutputs { get; set; }
        public bool[] VisionRawData { get; set; }

        public const int VisionFrontRange = 9;
        public const int VisionFrontRayCount = 29;
        public const double VisionFrontAngleMin = -0.25;
        public const double VisionFrontAngleMax = 0.25;
        public const int VisionBackRange = 3;
        public const int VisionBackRayCount = 7;
        public const double VisionBackAngleMin = 0.3;
        public const double VisionBackAngleMax = 0.7;

        /**
        public Animal()
        {
            NeuralNetWeights = new double[((VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4 + 1) * 32 + (32 * 32) + (32 * 3)];
            NeuralNetInputs = new double[(VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4];
            NeuralNetOutputs = new double[3];
            VisionRawData = new bool[(VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4];
        }
        **/

        public Animal(Double2d position, double angle, double element, Random random)
        {
            SpeciesId = random.NextInt64(0, 2176782335 + 1);

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

            NeuralNetWeights = new double[((VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4 + 1) * 32 + (32 * 32) + (32 * 3)];
            for (int i = 0; i < NeuralNetWeights.Length; i++) NeuralNetWeights[i] = random.NextDouble() * 4d - 2d;
            NeuralNetInputs = new double[(VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4];
            NeuralNetOutputs = new double[3];
            VisionRawData = new bool[(VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4];
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
                for (int i = 0; i < 29; i++)
                {
                    Double2d rayDirection = Double2d.FromAngle(Angle + VisionFrontAngleMin + (VisionFrontAngleMax - VisionFrontAngleMin) * (1 / (VisionFrontRayCount - 1) * i));

                    for (int j = 0; j < 9; j++)
                    {
                        Double2d rayPosition = Position + rayDirection * (j + 1);
                        int rayPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayPosition.X)));
                        int rayPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayPosition.Y)));

                        if (rayPosition.X < 0 || rayPosition.X > g_Soup.SizeX || rayPosition.Y < 0 || rayPosition.Y > g_Soup.SizeY)
                        {
                            NeuralNetInputs[(i * VisionFrontRange + j) * 4] = 1;
                            break;
                        }

                        Tile targetTile = g_Soup.Tiles[rayPositionIntegerizedY * g_Soup.SizeX + rayPositionIntegerizedX];
                        if (targetTile.Type == TileType.Wall)
                        {
                            NeuralNetInputs[(i * VisionFrontRange + j) * 4] = 1;
                            break;
                        }
                        else if (targetTile.LocalPlantPopulation > 0)
                        {
                            NeuralNetInputs[(i * VisionFrontRange + j) * 4 + 1] = 1;
                            break;
                        }
                    }
                }

                int IndexOffset = VisionFrontRange * VisionFrontRayCount * 4;

                for (int i = 0; i < 7; i++)
                {
                    Double2d rayDirection = Double2d.FromAngle(Angle + VisionBackAngleMin + (VisionBackAngleMax - VisionBackAngleMin) * (1 / (VisionBackRayCount - 1) * i));

                    for (int j = 0; j < 3; j++)
                    {
                        Double2d rayPosition = Position + rayDirection * (j + 1);
                        int rayPositionIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(rayPosition.X)));
                        int rayPositionIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(rayPosition.Y)));

                        if (rayPosition.X < 0 || rayPosition.X > g_Soup.SizeX || rayPosition.Y < 0 || rayPosition.Y > g_Soup.SizeY)
                        {
                            NeuralNetInputs[IndexOffset + (i * VisionBackRange + j) * 4] = 1;
                            break;
                        }

                        Tile targetTile = g_Soup.Tiles[rayPositionIntegerizedY * g_Soup.SizeX + rayPositionIntegerizedX];
                        if (targetTile.Type == TileType.Wall)
                        {
                            NeuralNetInputs[IndexOffset + (i * VisionBackRange + j) * 4] = 1;
                            break;
                        }
                        else if (targetTile.LocalPlantPopulation > 0)
                        {
                            NeuralNetInputs[IndexOffset + (i * VisionBackRange + j) * 4 + 1] = 1;
                            break;
                        }
                    }
                }

                double[] NeuralNetHiddenNodesL1 = new double[32];
                for(int i = 0; i < ((VisionFrontRayCount * VisionFrontRange + VisionBackRayCount * VisionBackRange) * 4); i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        NeuralNetHiddenNodesL1[j] += NeuralNetInputs[i] * NeuralNetWeights[i * 32 + j];
                    }
                }

                double[] NeuralNetHiddenNodesL2 = new double[32];
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        NeuralNetHiddenNodesL2[j] += double.Max(0, NeuralNetHiddenNodesL1[j] * NeuralNetWeights[36096 + i * 32 + j]);
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        NeuralNetOutputs[j] += double.Max(0, NeuralNetHiddenNodesL2[j] * NeuralNetWeights[36096 + 1024 + i * 3 + j]);
                    }
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyNeuralNetOutput()
        {
            if (Initialized)
            {
                //Console.WriteLine($"{Index} : {NeuralNetOutputs[0]}");
                Velocity += Double2d.FromAngle(Angle) * double.Min(1d, NeuralNetOutputs[0]) * 0.001d;
                AngularVelocity += double.Min(1d, NeuralNetOutputs[1]) * 0.001d;
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
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.25d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.25d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.75d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.75d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.5d, y + 0.5d), 0.5d);
                        }

                        if(targetTile.LocalPlantPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                if (targetPlant.Exist) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetPlant.Position, targetPlant.Radius, targetPlant.Mass);
                            }
                        }

                        if (targetTile.LocalAnimalPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                            {
                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                if (targetAnimal.Exist && targetAnimal.Id != Id) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetAnimal.Position, targetAnimal.Radius, targetAnimal.Mass);
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
                    Velocity /= Velocity.LengthSquared / g_Soup.MaximumVelocity;
                }

                Position += Velocity;
                Angle += AngularVelocity;

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

                if (int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X))) != IntegerizedPositionX || int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y))) != IntegerizedPositionY)
                {
                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Remove(Index);

                    IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
                    IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Add(Index);
                }
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

        public Animal? CreateOffspring(in Random random)
        {
            if (Initialized)
            {
                return null;
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
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }
    }
}
