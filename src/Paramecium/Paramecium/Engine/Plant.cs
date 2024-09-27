﻿namespace Paramecium.Engine
{
    public class Plant
    {
        public bool Exist { get; set; } = false;
        public bool Initialized { get; set; } = false;

        public int Index { get; set; } = -1;
        public long Id { get; set; } = -1;

        public Double2d Position { get; set; }
        public Double2d Velocity { get; set; }
        public int IntegerizedPositionX { get; set; }
        public int IntegerizedPositionY { get; set; }

        public double Radius { get; set; }
        public double Mass { get; set; }

        public double Element { get; set; }

        public Plant(Double2d position, double element)
        {
            Position = position;
            IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
            IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

            Radius = Math.Sqrt(element / g_Soup.PlantForkCost) * 0.5d;
            Mass = element;

            Element = element;
        }

        public void Initialize(int index, Random random)
        {
            if (!Initialized)
            {
                Exist = true;

                Index = index;
                Id = random.NextInt64(0, 4738381338321616896);

                //Velocity = Double2d.FromAngle(random.NextDouble()) * g_Soup.MaximumVelocity;

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalPlantIndexes.Add(Index);

                Initialized = true;
            }
            else throw new InvalidOperationException("This animal has already been initialized.");
        }

        public void CollectElement()
        {
            if (Initialized)
            {
                Tile targetTile = g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX];

                if (targetTile.Element > 0)
                {
                    Element += targetTile.Element * g_Soup.PlantElementCollectRate;
                    targetTile.Element -= targetTile.Element * g_Soup.PlantElementCollectRate;

                    if (targetTile.Element < 0) targetTile.Element = 0;

                    Radius = Math.Sqrt(Element / g_Soup.PlantForkCost) / 2d;
                    Mass = Element;
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
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.25d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.25d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.25d, y + 0.75d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.75d, y + 0.75d), 0.356d);
                            Velocity += CalculateCollisionTwoObjects(Position, Radius, new Double2d(x + 0.5d, y + 0.5d), 0.5d);
                        }

                        if (targetTile.LocalPlantPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];
                                if (targetPlant.Exist && targetPlant.Id != Id) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetPlant.Position, targetPlant.Radius, targetPlant.Mass);
                            }
                        }

                        if (targetTile.LocalAnimalPopulation > 0)
                        {
                            for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                            {
                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];
                                if (targetAnimal.Exist) Velocity += CalculateCollisionTwoObjects(Position, Radius, Mass, targetAnimal.Position, targetAnimal.Radius, targetAnimal.Mass);
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

                //Velocity /= Velocity.Length / g_Soup.MaximumVelocity;

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

                if (int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X))) != IntegerizedPositionX || int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y))) != IntegerizedPositionY)
                {
                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalPlantIndexes.Remove(Index);

                    IntegerizedPositionX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(Position.X)));
                    IntegerizedPositionY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(Position.Y)));

                    g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalPlantIndexes.Add(Index);
                }
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public void ApplyDrag()
        {
            if (Initialized)
            {
                Velocity *= 1d - g_Soup.Drag;
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }

        public List<Plant>? CreateOffspring(in Random random)
        {
            if (Initialized)
            {
                if (Element >= g_Soup.PlantForkCost)
                {
                    List<Plant> result = new List<Plant>();
                    int offspringCount = random.Next(g_Soup.PlantForkOffspringCountMin, g_Soup.PlantForkOffspringCountMax + 1);
                    double[] offspringElementAmount = new double[offspringCount];
                    for (int i = 0; i < offspringCount; i++) offspringElementAmount[i] = random.NextDouble() * 0.9d + 0.1d;
                    double offspringElementAmountTotal = 0;
                    for (int i = 0; i < offspringCount; i++) offspringElementAmountTotal += offspringElementAmount[i];
                    for (int i = 0; i < offspringCount; i++) offspringElementAmount[i] = offspringElementAmount[i] / offspringElementAmountTotal * Element;
                    for (int i = 0; i < offspringCount; i++) result.Add(new Plant(Position + Double2d.FromAngle(random.NextDouble()) * 0.1d, offspringElementAmount[i]));

                    OnDisable();
                    return result;
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

                g_Soup.Tiles[IntegerizedPositionY * g_Soup.SizeX + IntegerizedPositionX].LocalPlantIndexes.Remove(Index);
                g_Soup.PlantUnusedIndexes.Add(Index);
            }
            else throw new InvalidOperationException("This animal is not initialized.");
        }
    }
}
