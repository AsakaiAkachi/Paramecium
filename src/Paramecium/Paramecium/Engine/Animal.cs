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
        public int OffspringCount { get; set; }

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
        public double CurrentStepElementCost { get; set; }

        public int ColorRed { get; set; }
        public int ColorGreen { get; set; }
        public int ColorBlue { get; set; }

        public Brain Brain { get; set; }
        public BrainInput BrainInput { get; set; }
        public BrainOutput BrainOutput { get; set; }

        public Animal()
        {
            Brain = Brain.GetDefaultBrain;
            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
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
            Efficiency = 1d;

            ColorRed = random.Next(0, 255 + 1);
            ColorGreen = random.Next(0, 255 + 1);
            ColorBlue = random.Next(0, 255 + 1);

            //Brain = Brain.GetDefaultBrain;
            Brain = new Brain();
            for (int i = 0; i < 64; i++) Brain = Brain.Mutate(Brain, random, false);
            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
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
            Efficiency = parent.Efficiency;

            ColorRed = parent.ColorRed;
            ColorGreen = parent.ColorGreen;
            ColorBlue = parent.ColorBlue;

            Brain = Brain.Duplicate(parent.Brain);
            if (random.NextDouble() < g_Soup.AnimalMutationRate)
            {
                for (int i = 0; i < g_Soup.AnimalMaximumMutationCount; i++)
                {
                    bool mutationSuccessFlag;
                    Brain = Brain.Mutate(Brain, random, out mutationSuccessFlag);

                    if (mutationSuccessFlag)
                    {
                        if (random.NextDouble() < g_Soup.AnimalSpeciesIdMutationRate)
                        {
                            SpeciesId = random.NextInt64(0, 2176782335 + 1);

                            ColorRed = random.Next(0, 255 + 1);
                            ColorGreen = random.Next(0, 255 + 1);
                            ColorBlue = random.Next(0, 255 + 1);
                        }
                    }

                    if (random.NextDouble() < 0.5d) break;
                }
            }

            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
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

                g_Soup.TotalBornCount++;
            }
            else throw new InvalidOperationException("This animal has already been initialized.");
        }

        public void UpdateNeuralNet()
        {
            if (Initialized)
            {
                Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX];

                BrainInput = new BrainInput()
                {
                    VisionData = AnimalVision.Observe(Position, Angle, Id, SpeciesId, 9, 29, 0.5d, 3, 7, 0.4d),
                    PrevStepOutput = BrainOutput,
                    Velocity = Velocity.Length / g_Soup.MaximumVelocity,
                    AngularVelocity = AngularVelocity / g_Soup.MaximumAngularVelocity,
                    Satiety = Element / g_Soup.AnimalForkCost,
                    PheromoneRed = targetTile.PheromoneRed,
                    PheromoneGreen = targetTile.PheromoneGreen,
                    PheromoneBlue = targetTile.PheromoneBlue
                };

                BrainOutput = Brain.UpdateBrain(BrainInput);
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyNeuralNetOutput()
        {
            if (Initialized)
            {
                Velocity += Double2d.FromAngle(Angle) * double.Max(-1d, double.Min(1d, BrainOutput.Acceleration)) * g_Soup.MaximumVelocity * g_Soup.Drag;
                AngularVelocity += double.Max(-1d, double.Min(1d, BrainOutput.Rotation)) * g_Soup.MaximumAngularVelocity * g_Soup.AngularVelocityDrag;

                if (BrainOutput.PheromoneRed > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX];
                    targetTile.PheromoneRed += double.Min(1d, BrainOutput.PheromoneRed) * g_Soup.PheromoneProductionRate;
                }
                if (BrainOutput.PheromoneGreen > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX];
                    targetTile.PheromoneGreen += double.Min(1d, BrainOutput.PheromoneGreen) * g_Soup.PheromoneProductionRate;
                }
                if (BrainOutput.PheromoneBlue > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX];
                    targetTile.PheromoneBlue += double.Min(1d, BrainOutput.PheromoneBlue) * g_Soup.PheromoneProductionRate;
                }

                if (BrainOutput.Attack > 0)
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
                                                //Efficiency = (Efficiency * Element + g_Soup.PlantElementEfficiency * double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate)) / (Element + double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate));

                                                Element += double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate);
                                                targetPlant.Element -= double.Min(targetPlant.Element, g_Soup.AnimalPlantIngestionRate);
                                            }
                                        }
                                    }
                                }
                            }

                            if (targetTile.LocalAnimalPopulation > 0 && BrainOutput.Attack >= 1)
                            {
                                for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                                {
                                    Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                    if (targetAnimal.Exist && targetAnimal.Id != Id && targetAnimal.SpeciesId != SpeciesId)
                                    {
                                        if (Double2d.DistanceSquared(Position, targetAnimal.Position) < (Radius + targetAnimal.Radius + 0.1) * (Radius + targetAnimal.Radius + 0.1))
                                        {
                                            if (double.Abs(Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle))) < 0.167d)
                                            {
                                                //Efficiency = (Efficiency * Element + g_Soup.AnimalElementEfficiency * double.Min(targetAnimal.Element, g_Soup.AnimalPlantIngestionRate)) / (Element + double.Min(targetAnimal.Element, g_Soup.AnimalPlantIngestionRate));

                                                Element += double.Min(targetAnimal.Element, g_Soup.AnimalAnimalIngestionRate);
                                                targetAnimal.Element -= double.Min(targetAnimal.Element, g_Soup.AnimalAnimalIngestionRate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Efficiency < double.Min(g_Soup.PlantElementEfficiency, g_Soup.AnimalElementEfficiency)) Efficiency = double.Min(g_Soup.PlantElementEfficiency, g_Soup.AnimalElementEfficiency);
                    if (Efficiency > double.Max(g_Soup.PlantElementEfficiency, g_Soup.AnimalElementEfficiency)) Efficiency = double.Max(g_Soup.PlantElementEfficiency, g_Soup.AnimalElementEfficiency);
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

                CurrentStepElementCost = double.Min(Element,
                    g_Soup.AnimalElementBaseCost +
                    (g_Soup.AnimalElementAccelerationCost * double.Min(1d, double.Abs(BrainOutput.Acceleration))) +
                    (g_Soup.AnimalElementRotationCost * double.Min(1d, double.Abs(BrainOutput.Rotation))) +
                    (g_Soup.AnimalElementAttackCost * double.Max(0d, double.Min(1d, BrainOutput.Attack))) +
                    (g_Soup.AnimalElementPheromoneProductionCost * (double.Max(0d, double.Min(1d, BrainOutput.PheromoneRed)) + double.Max(0d, double.Min(1d, BrainOutput.PheromoneGreen)) + double.Max(0d, double.Min(1d, BrainOutput.PheromoneBlue))))
                );

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].Element += CurrentStepElementCost;
                Element -= CurrentStepElementCost;

                Radius = 0.5;
                Mass = 16 + Element;

                if (Age >= g_Soup.AnimalMaximumAge)
                {
                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].Element = Element;
                    Element = 0;
                }

                if (Element <= 0) OnDisable();
                if (g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].Type == TileType.Wall) OnDisable();

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
                        OffspringCount++;
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
}
