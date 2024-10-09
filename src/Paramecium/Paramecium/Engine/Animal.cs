namespace Paramecium.Engine
{
    public class Animal
    {
        public bool Exist { get; set; } = false;
        public bool Initialized { get; set; } = false;

        public int Index { get; set; } = -1;
        public long Id { get; set; } = -1;
        public long SpeciesId { get; set; }
        //public long AncestorSpeciesId { get; set; }

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
        public double CurrentStepElementCost { get; set; }

        public long LastDamageTime { get; set; }

        public int ColorRed { get; set; }
        public int ColorGreen { get; set; }
        public int ColorBlue { get; set; }

        public Brain Brain { get; set; }
        public BrainInput BrainInput { get; set; }
        public BrainOutput BrainOutput { get; set; }

        public OrganismType AttackTargetType;
        public int AttackTargetIndex;

        public Animal()
        {
            Brain = Brain.GetDefaultBrain;
            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
        }

        public Animal(Double2d position, double angle, double element, Random random)
        {
            if (g_Soup is null) throw new SoupNotCreatedOrInitializedException();

            SpeciesId = random.NextInt64(0, 2176782335 + 1);
            //AncestorSpeciesId = SpeciesId;

            Generation = 1;

            Position = position;
            Angle = angle;
            IntegerizedPositionX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(Position.X)));
            IntegerizedPositionY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(Position.Y)));

            Radius = 0.5;
            Mass = 16 + element;

            Element = element;

            LastDamageTime = int.MinValue;

            ColorRed = random.Next(0, 255 + 1);
            ColorGreen = random.Next(0, 255 + 1);
            ColorBlue = random.Next(0, 255 + 1);

            Brain = Brain.GetDefaultBrain;
            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
        }

        public Animal(Animal parent, Double2d position, double element, Random random)
        {
            if (g_Soup is null) throw new SoupNotCreatedOrInitializedException();

            SpeciesId = parent.SpeciesId;
            //AncestorSpeciesId = parent.AncestorSpeciesId;

            Generation = parent.Generation + 1;

            Position = position;
            Angle = parent.Angle + 0.5;
            if (Angle < -0.5) Angle += 1;
            if (Angle > 0.5) Angle -= 1;
            IntegerizedPositionX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(Position.X)));
            IntegerizedPositionY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(Position.Y)));

            Radius = 0.5;
            Mass = 16 + element;

            Element = element;

            LastDamageTime = int.MinValue;

            ColorRed = parent.ColorRed;
            ColorGreen = parent.ColorGreen;
            ColorBlue = parent.ColorBlue;

            Brain = Brain.Duplicate(parent.Brain);
            if (random.NextDouble() < g_Soup.Settings.AnimalMutationRate)
            {
                for (int i = 0; i < g_Soup.Settings.AnimalMaximumMutationCount; i++)
                {
                    Brain = Brain.Mutate(Brain, random);

                    if (random.NextDouble() < g_Soup.Settings.AnimalSpeciesIdMutationRate)
                    {
                        //AncestorSpeciesId = SpeciesId;
                        SpeciesId = random.NextInt64(0, 2176782335 + 1);

                        ColorRed = random.Next(0, 255 + 1);
                        ColorGreen = random.Next(0, 255 + 1);
                        ColorBlue = random.Next(0, 255 + 1);
                    }

                    if (random.NextDouble() < 0.5d) break;
                }
            }

            BrainInput = new BrainInput();
            BrainOutput = new BrainOutput();
        }

        public void Initialize(int index, Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (!Initialized)
            {
                Exist = true;

                Index = index;
                Id = random.NextInt64(0, 4738381338321616896);

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.Settings.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Add(Index);

                Initialized = true;

                g_Soup.TotalBornCount++;
            }
            else throw new InvalidOperationException("This animal has already been initialized.");
        }

        public void UpdateNeuralNet()
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                int soupSizeX = g_Soup.Settings.SizeX;
                int soupSizeY = g_Soup.Settings.SizeY;

                BrainInput = new BrainInput()
                {
                    VisionData = AnimalVision.Observe(Position, Angle, Id, SpeciesId, 9, 29, 0.5d, 3, 7, 0.4d),
                    PrevStepOutput = BrainOutput,
                    Velocity = Velocity.Length / g_Soup.Settings.MaximumVelocity,
                    AngularVelocity = AngularVelocity / g_Soup.Settings.MaximumAngularVelocity,
                    Satiety = Element / g_Soup.Settings.AnimalForkCost,
                    Damage = 1d - long.Min(g_Soup.Settings.AnimalDamageRecoveryTime, g_Soup.ElapsedTimeSteps - LastDamageTime) / (double)g_Soup.Settings.AnimalDamageRecoveryTime
                };

                BrainOutput = Brain.UpdateBrain(BrainInput);

                AttackTargetType = OrganismType.None;
                AttackTargetIndex = -1;
                double attackTargetAngleAbs = 1d;

                for (int x = int.Max(0, int.Min(soupSizeX - 1, IntegerizedPositionX - 2)); x <= int.Max(0, int.Min(soupSizeX - 1, IntegerizedPositionX + 2)); x++)
                {
                    for (int y = int.Max(0, int.Min(soupSizeY - 1, IntegerizedPositionY - 2)); y <= int.Max(0, int.Min(soupSizeY - 1, IntegerizedPositionY + 2)); y++)
                    {
                        Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

                        if (targetTile.LocalPlantPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                if (targetPlant.Exist)
                                {
                                    if (Double2d.DistanceSquared(Position, targetPlant.Position) < (Radius + targetPlant.Radius) * (Radius + targetPlant.Radius))
                                    {
                                        if (BrainOutput.Attack > 0d)
                                        {
                                            double angleAbs = double.Abs(Double2d.ToAngle(Double2d.Rotate(targetPlant.Position - Position, -Angle)));
                                            if (angleAbs < 0.125d && angleAbs < attackTargetAngleAbs)
                                            {
                                                AttackTargetType = OrganismType.Plant;
                                                AttackTargetIndex = targetPlant.Index;
                                            }
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
                                    if (Double2d.DistanceSquared(Position, targetAnimal.Position) < (Radius + targetAnimal.Radius) * (Radius + targetAnimal.Radius))
                                    {
                                        //if (BrainOutput.Attack > 0d && (targetAnimal.SpeciesId != SpeciesId && SpeciesId != targetAnimal.AncestorSpeciesId && targetAnimal.AncestorSpeciesId != SpeciesId && targetAnimal.AncestorSpeciesId != AncestorSpeciesId))
                                        if (BrainOutput.Attack > 0d && targetAnimal.SpeciesId != SpeciesId)
                                        {
                                            double angleAbs = double.Abs(Double2d.ToAngle(Double2d.Rotate(targetAnimal.Position - Position, -Angle)));
                                            if (angleAbs < 0.125d && angleAbs < attackTargetAngleAbs)
                                            {
                                                AttackTargetType = OrganismType.Animal;
                                                AttackTargetIndex = targetAnimal.Index;
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

        public void ApplyNeuralNetOutput()
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                int soupSizeX = g_Soup.Settings.SizeX;
                int soupSizeY = g_Soup.Settings.SizeY;

                Velocity += Double2d.FromAngle(Angle) * double.Max(-1d, double.Min(1d, BrainOutput.Acceleration)) * g_Soup.Settings.MaximumVelocity * g_Soup.Settings.Drag;
                AngularVelocity += double.Max(-1d, double.Min(1d, BrainOutput.Rotation)) * g_Soup.Settings.MaximumAngularVelocity * g_Soup.Settings.AngularVelocityDrag;

                if (BrainOutput.PheromoneRed > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX];
                    targetTile.PheromoneRed += double.Min(1d, BrainOutput.PheromoneRed) * g_Soup.Settings.PheromoneProductionRate;
                }
                if (BrainOutput.PheromoneGreen > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX];
                    targetTile.PheromoneGreen += double.Min(1d, BrainOutput.PheromoneGreen) * g_Soup.Settings.PheromoneProductionRate;
                }
                if (BrainOutput.PheromoneBlue > 0)
                {
                    Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX];
                    targetTile.PheromoneBlue += double.Min(1d, BrainOutput.PheromoneBlue) * g_Soup.Settings.PheromoneProductionRate;
                }

                if (AttackTargetType == OrganismType.Plant)
                {
                    Plant target = g_Soup.Plants[AttackTargetIndex];

                    Element += double.Min(target.Element, g_Soup.Settings.AnimalPlantIngestionRate) * g_Soup.ElementAmountMultiplier;
                    target.Element = double.Max(0d, target.Element - g_Soup.Settings.AnimalPlantIngestionRate);
                }
                else if (AttackTargetType == OrganismType.Animal)
                {
                    Animal target = g_Soup.Animals[AttackTargetIndex];
                    
                    Element += double.Min(target.Element, g_Soup.Settings.AnimalAnimalIngestionRate) * g_Soup.ElementAmountMultiplier;
                    target.Element = double.Max(0d, target.Element - g_Soup.Settings.AnimalAnimalIngestionRate);

                    target.LastDamageTime = g_Soup.ElapsedTimeSteps;
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void UpdateCollision()
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                int soupSizeX = g_Soup.Settings.SizeX;
                int soupSizeY = g_Soup.Settings.SizeY;

                for (int x = int.Max(0, int.Min(soupSizeX - 1, IntegerizedPositionX - 1)); x <= int.Max(0, int.Min(soupSizeX - 1, IntegerizedPositionX + 1)); x++)
                {
                    for (int y = int.Max(0, int.Min(soupSizeY - 1, IntegerizedPositionY - 1)); y <= int.Max(0, int.Min(soupSizeY - 1, IntegerizedPositionY + 1)); y++)
                    {
                        Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

                        if (targetTile.Type == TileType.Wall)
                        {
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.25d), 0.356d) * g_Soup.Settings.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.25d), 0.356d) * g_Soup.Settings.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.75d), 0.356d) * g_Soup.Settings.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.75d), 0.356d) * g_Soup.Settings.RestitutionCoefficient;
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.5d, y + 0.5d), 0.5d) * g_Soup.Settings.RestitutionCoefficient;
                        }

                        if(targetTile.LocalPlantPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                if (targetPlant.Exist) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetPlant.Position, targetPlant.Radius, targetPlant.Mass) * g_Soup.Settings.RestitutionCoefficient;
                            }
                        }

                        if (targetTile.LocalAnimalPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                            {
                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                if (targetAnimal.Exist && targetAnimal.Id != Id) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetAnimal.Position, targetAnimal.Radius, targetAnimal.Mass) * g_Soup.Settings.RestitutionCoefficient;
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
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                int soupSizeX = g_Soup.Settings.SizeX;
                int soupSizeY = g_Soup.Settings.SizeY;

                if (Velocity.LengthSquared > g_Soup.Settings.MaximumVelocity * g_Soup.Settings.MaximumVelocity)
                {
                    Velocity /= Velocity.Length / g_Soup.Settings.MaximumVelocity;
                }
                Position += Velocity;

                if (Position.X < Radius)
                {
                    Position = new Double2d(0 + Radius, Position.Y);
                    Velocity = new Double2d(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.X > soupSizeX - Radius)
                {
                    Position = new Double2d(soupSizeX - Radius, Position.Y);
                    Velocity = new Double2d(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.Y < Radius)
                {
                    Position = new Double2d(Position.X, Radius);
                    Velocity = new Double2d(Velocity.X, Velocity.Y * -1d);
                }
                if (Position.Y > soupSizeY - Radius)
                {
                    Position = new Double2d(Position.X, soupSizeY - Radius);
                    Velocity = new Double2d(Velocity.X, Velocity.Y * -1d);
                }

                if (double.Abs(AngularVelocity) > g_Soup.Settings.MaximumAngularVelocity)
                {
                    AngularVelocity /= AngularVelocity / g_Soup.Settings.MaximumAngularVelocity;
                }
                Angle += AngularVelocity;

                if (Angle < -0.5) Angle += 1;
                if (Angle > 0.5) Angle -= 1;

                if (int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(Position.X))) != IntegerizedPositionX || int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(Position.Y))) != IntegerizedPositionY)
                {
                    g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX].LocalAnimalIndexes.Remove(Index);

                    IntegerizedPositionX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(Position.X)));
                    IntegerizedPositionY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(Position.Y)));

                    g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX].LocalAnimalIndexes.Add(Index);
                }

                CurrentStepElementCost = double.Min(Element,
                    g_Soup.Settings.AnimalElementBaseCost +
                    (g_Soup.Settings.AnimalElementAccelerationCost * double.Min(1d, double.Abs(BrainOutput.Acceleration))) +
                    (g_Soup.Settings.AnimalElementRotationCost * double.Min(1d, double.Abs(BrainOutput.Rotation))) +
                    (g_Soup.Settings.AnimalElementAttackCost * double.Ceiling(double.Max(0d, double.Min(1d, BrainOutput.Attack)))) +
                    (g_Soup.Settings.AnimalElementPheromoneProductionCost * (double.Max(0d, double.Min(1d, BrainOutput.PheromoneRed)) + double.Max(0d, double.Min(1d, BrainOutput.PheromoneGreen)) + double.Max(0d, double.Min(1d, BrainOutput.PheromoneBlue))))
                );

                g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX].Element += double.Min(Element, CurrentStepElementCost) * g_Soup.ElementAmountMultiplier;
                Element = double.Max(0d, Element - CurrentStepElementCost);

                Radius = 0.5;
                Mass = 16 + Element;

                if (Element <= 0) OnDisable();
                else if (g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX].Type == TileType.Wall) OnDisable();
                else if (Age >= g_Soup.Settings.AnimalMaximumAge)
                {
                    g_Soup.Tiles[IntegerizedPositionY * soupSizeX + IntegerizedPositionX].Element += Element * g_Soup.ElementAmountMultiplier;
                    Element = 0;
                    OnDisable();
                }

                Age++;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyDrag()
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                Velocity *= 1d - g_Soup.Settings.Drag;
                AngularVelocity *= 1d - g_Soup.Settings.AngularVelocityDrag;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public Animal? CreateOffspring(Random random)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                if (Element >= g_Soup.Settings.AnimalForkCost * 2)
                {
                    Double2d OffspringPosition = Position + Double2d.FromAngle(Angle + 0.5) * 0.5;
                    if (
                        OffspringPosition.X >= 0 && OffspringPosition.X <= g_Soup.Settings.SizeX && OffspringPosition.Y >= 0 && OffspringPosition.Y <= g_Soup.Settings.SizeY && 
                        g_Soup.Tiles[int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(OffspringPosition.Y))) * g_Soup.Settings.SizeX + int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(OffspringPosition.X)))].Type == TileType.Default
                    )
                    {
                        Element -= g_Soup.Settings.AnimalForkCost;
                        OffspringCount++;
                        return new Animal(this, Position + Double2d.FromAngle(Angle + 0.5) * 0.5, g_Soup.Settings.AnimalForkCost, random);
                    }
                    else return null;
                }
                else return null;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void OnDisable()
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            if (Initialized)
            {
                Exist = false;

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.Settings.SizeX + IntegerizedPositionX].LocalAnimalIndexes.Remove(Index);
                g_Soup.AnimalUnusedIndexes.Add(Index);

                g_Soup.TotalDieCount++;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }
    }
}
