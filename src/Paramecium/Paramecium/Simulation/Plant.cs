using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Drawing;
using System.Threading;

namespace Paramecium.Simulation
{
    public class Plant
    {
        public int Index { get; set; }
        public long Id { get; set; }
        public bool IsValid { get; set; }

        public Vector2D Position { get; set; }
        public Int2D IntegerizedPosition { get; set; }
        public Vector2D Velocity { get; set; }

        public double Element { get; set; }
        public double Radius { get; set; }
        public double Mass { get; set; }

        public int Age { get; set; }

        public bool CollisionIsDisabled { get; set; }
        public int ElementCollectionIsDisabled { get; set; }

        private int l_SoupSizeX;
        private int l_SoupSizeY;

        public Plant() { }
        public Plant(Random random, Vector2D position, Vector2D velocity, double element)
        {
            Index = -1;
            Id = random.NextInt64(0, 4738381338321616896);
            IsValid = true;

            Position = position;
            IntegerizedPosition = Vector2D.ToIntegerizedPosition(position);
            Velocity = velocity;

            Element = element;
            Radius = 0.5d * Math.Max(Math.Sqrt(Element) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
            Mass = Math.Pow(Radius, 2);

            Age = -50;
        }

        public void OnInitialize()
        {
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;

            if (g_Soup.PlantUnassignedIndexes.Count == 0)
            {
                for (int i = g_Soup.Plants.Length * 2 - 1; i >= g_Soup.Plants.Length; i--)
                {
                    g_Soup.PlantUnassignedIndexes.Add(i);
                }
                Plant[] ParticlesArrayOld = g_Soup.Plants;
                g_Soup.Plants = new Plant[ParticlesArrayOld.Length * 2];
                ParticlesArrayOld.CopyTo(g_Soup.Plants, 0);
            }
            Index = g_Soup.PlantUnassignedIndexes[g_Soup.PlantUnassignedIndexes.Count - 1];
            g_Soup.PlantUnassignedIndexes.RemoveAt(g_Soup.PlantUnassignedIndexes.Count - 1);
            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlants.Add(Index);
            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlantCount ++;
        }

        public void OnLoaded()
        {
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;
        }

        public void EarlyUpdate()
        {
            if (Age >= 0) Velocity *= 0.9d;

            int PlantBiomassCollectionRange = g_Soup.PlantBiomassCollectionRange;

            if (ElementCollectionIsDisabled < 0)
            {
                Random rnd = new Random();
                int xOffset = 0;
                int yOffset = 0;

                for (int i = 0; i < PlantBiomassCollectionRange * 2; i++)
                {
                    int xOffsetNext = xOffset;
                    int yOffsetNext = yOffset;

                    int shiftDirection = rnd.Next(0, 3);

                    if (shiftDirection == 0)
                    {
                        if (rnd.Next(0, 2) == 0) xOffsetNext++;
                        else xOffsetNext--;
                    }
                    else if (shiftDirection == 1)
                    {
                        if (rnd.Next(0, 2) == 0) yOffsetNext++;
                        else yOffsetNext--;
                    }
                    else { }

                    if (IntegerizedPosition.X + xOffsetNext < 0 || IntegerizedPosition.X + xOffsetNext >= l_SoupSizeX || IntegerizedPosition.Y + yOffsetNext < 0 || IntegerizedPosition.Y + yOffsetNext >= l_SoupSizeY) break;
                    else if (g_Soup.GridMap[(IntegerizedPosition.X + xOffsetNext) + (IntegerizedPosition.Y + yOffsetNext) * l_SoupSizeX].Type == TileType.Wall) break;
                    else if (!(xOffsetNext >= PlantBiomassCollectionRange * -1 && xOffsetNext <= PlantBiomassCollectionRange && yOffsetNext >= PlantBiomassCollectionRange * -1 && yOffsetNext <= PlantBiomassCollectionRange)) break;
                    else
                    {
                        xOffset = xOffsetNext;
                        yOffset = yOffsetNext;
                    }
                }

                Grid targetGrid = g_Soup.GridMap[(IntegerizedPosition.X + xOffset) + (IntegerizedPosition.Y + yOffset) * l_SoupSizeX];
                if (targetGrid.Fertility > 0)
                {
                    Element += Math.Min(0.15d, targetGrid.Fertility) * g_Soup.BiomassAmountMultiplier;
                    targetGrid.Fertility -= Math.Min(0.15d, targetGrid.Fertility);

                    Radius = 0.5d * Math.Max(Math.Sqrt(Element) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);

                    Mass = Math.Pow(Radius, 2);

                    CollisionIsDisabled = false;
                    ElementCollectionIsDisabled = -20;
                }
                else ElementCollectionIsDisabled++;
            }
            else if (ElementCollectionIsDisabled == 0)
            {
                bool reactivation = false;
                for (int x = Math.Max(IntegerizedPosition.X - PlantBiomassCollectionRange, 0); x <= Math.Min(IntegerizedPosition.X + PlantBiomassCollectionRange, l_SoupSizeX - 1); x++)
                {
                    for (int y = Math.Max(IntegerizedPosition.Y - PlantBiomassCollectionRange, 0); y <= Math.Min(IntegerizedPosition.Y + PlantBiomassCollectionRange, l_SoupSizeY - 1); y++)
                    {
                        if (g_Soup.GridMap[x + y * l_SoupSizeX].Fertility > 0)
                        {
                            reactivation = true;
                            ElementCollectionIsDisabled = -100;
                            break;
                        }
                    }
                    if (reactivation) break;
                }
                if (!reactivation) ElementCollectionIsDisabled = 20;
            }
            else ElementCollectionIsDisabled--;
        }

        public void MiddleUpdate()
        {
            if (!CollisionIsDisabled)
            {
                for (int x = Math.Max((int)Math.Floor(Position.X - g_Soup.CellSizeMultiplier), 0); x <= Math.Min((int)Math.Ceiling(Position.X + g_Soup.CellSizeMultiplier), l_SoupSizeX - 1); x++) // 自身に隣接するグリッドに対してforループで壁や生物がいないか調べる
                {
                    for (int y = Math.Max((int)Math.Floor(Position.Y - g_Soup.CellSizeMultiplier), 0); y <= Math.Min((int)Math.Ceiling(Position.Y + g_Soup.CellSizeMultiplier), l_SoupSizeY - 1); y++)
                    {
                        if (g_Soup.GridMap[x + y * l_SoupSizeX].Type == TileType.Wall)
                        {
                            Vector2D WallPosition1 = new Vector2D(x + 0.25, y + 0.25);
                            Vector2D WallPosition2 = new Vector2D(x + 0.75, y + 0.25);
                            Vector2D WallPosition3 = new Vector2D(x + 0.25, y + 0.75);
                            Vector2D WallPosition4 = new Vector2D(x + 0.75, y + 0.75);
                            Vector2D WallPosition5 = new Vector2D(x + 0.5, y + 0.5);
                            double WallDistance1 = Vector2D.Distance(Position, WallPosition1);
                            double WallDistance2 = Vector2D.Distance(Position, WallPosition2);
                            double WallDistance3 = Vector2D.Distance(Position, WallPosition3);
                            double WallDistance4 = Vector2D.Distance(Position, WallPosition4);
                            double WallDistance5 = Vector2D.Distance(Position, WallPosition5);

                            if (WallDistance1 < 0.353553 + Radius)
                            {
                                Velocity += Vector2D.Normalization(Position - WallPosition1) * ((Radius + 0.353553 - WallDistance1) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            }
                            if (WallDistance2 < 0.353553 + Radius)
                            {
                                Velocity += Vector2D.Normalization(Position - WallPosition2) * ((Radius + 0.353553 - WallDistance2) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            }
                            if (WallDistance3 < 0.353553 + Radius)
                            {
                                Velocity += Vector2D.Normalization(Position - WallPosition3) * ((Radius + 0.353553 - WallDistance3) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            }
                            if (WallDistance4 < 0.353553 + Radius)
                            {
                                Velocity += Vector2D.Normalization(Position - WallPosition4) * ((Radius + 0.353553 - WallDistance4) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            }
                            if (WallDistance5 < 0.5 + Radius)
                            {
                                Velocity += Vector2D.Normalization(Position - WallPosition5) * ((Radius + 0.5 - WallDistance5) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            }
                        }
                        if (g_Soup.GridMap[x + y * l_SoupSizeX].LocalPlantCount > 0 && Age > -40)
                        {
                            for (int i = 0; i < g_Soup.GridMap[x + y * l_SoupSizeX].LocalPlantCount; i++)
                            {
                                if (g_Soup.Plants[g_Soup.GridMap[x + y * l_SoupSizeX].LocalPlants[i]].Id != Id)
                                {
                                    Plant target = g_Soup.Plants[g_Soup.GridMap[x + y * l_SoupSizeX].LocalPlants[i]];
                                    Vector2D collidedParticlePosition = target.Position;
                                    double Distance = Vector2D.Distance(Position, collidedParticlePosition);
                                    if (Distance < Radius + target.Radius)
                                    {
                                        target.CollisionIsDisabled = false;
                                        Velocity += Vector2D.Normalization(Position - target.Position) * ((Radius + target.Radius - Distance) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity + target.Velocity), 0.01d) * Math.Min(target.Mass / Mass, 1d) / 2d;
                                        target.Velocity += Vector2D.Normalization(target.Position - Position) * ((target.Radius + Radius - Distance) / (target.Radius * 0.5d)) * Math.Max(Vector2D.Size(target.Velocity + Velocity), 0.01d) * Math.Min(Mass / target.Mass, 1d) / 2d;
                                    }
                                }
                            }
                        }
                        if (g_Soup.GridMap[x + y * l_SoupSizeX].LocalAnimalCount > 0 && Age > -40)
                        {
                            for (int i = 0; i < g_Soup.GridMap[x + y * l_SoupSizeX].LocalAnimalCount; i++)
                            {
                                if (g_Soup.Animals[g_Soup.GridMap[x + y * l_SoupSizeX].LocalAnimals[i]].Id != Id)
                                {
                                    Animal target = g_Soup.Animals[g_Soup.GridMap[x + y * l_SoupSizeX].LocalAnimals[i]];
                                    Vector2D collidedParticlePosition = target.Position;
                                    double Distance = Vector2D.Distance(Position, collidedParticlePosition);
                                    if (Distance < Radius + target.Radius)
                                    {
                                        Velocity += Vector2D.Normalization(Position - target.Position) * ((Radius + target.Radius - Distance) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity + target.Velocity), 0.01d) * Math.Min(target.Mass / Mass, 1d) / 2d;
                                        target.Velocity += Vector2D.Normalization(target.Position - Position) * ((target.Radius + Radius - Distance) / (target.Radius * 0.5d)) * Math.Max(Vector2D.Size(target.Velocity + Velocity), 0.01d) * Math.Min(Mass / target.Mass, 1d) / 2d;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LateUpdate(int threadId)
        {
            if (Element <= 0)
            {
                IsValid = false;
                return;
            }

            if (Element >= g_Soup.PlantForkBiomass)
            {
                Random rnd = new Random();

                while (Element > 0)
                {
                    double SatietySwap = Math.Min(rnd.NextDouble() * g_Soup.PlantForkBiomass / 4, Element);
                    g_Soup.PlantBuffer[threadId].Add(new Plant(rnd, Position + Vector2D.FromAngle(rnd.NextDouble() * 360d) * 0.25d, Vector2D.FromAngle(rnd.NextDouble() * 360d) * 0.1d, SatietySwap * g_Soup.BiomassAmountMultiplier));
                    Element -= SatietySwap;
                }

                IsValid = false;

                return;
            }
            else if (Velocity != Vector2D.Zero || g_Soup.ElapsedTimeStep == 0)
            {
                CollisionIsDisabled = false;

                if (Vector2D.Size(Velocity) > 0.1d)
                {
                    Velocity = Vector2D.Normalization(Velocity) * 0.1d;
                }
                if (Vector2D.Size(Velocity) < 0.0001d && g_Soup.ElapsedTimeStep >= 10)
                {
                    Velocity = Vector2D.Zero;
                    CollisionIsDisabled = true;
                }

                Position += Velocity;
                if (Position.X > l_SoupSizeX)
                {
                    Position = new Vector2D(l_SoupSizeX, Position.Y);
                    Velocity = new Vector2D(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.X < 0)
                {
                    Position = new Vector2D(0, Position.Y);
                    Velocity = new Vector2D(Velocity.X * -1d, Velocity.Y);
                }
                if (Position.Y > l_SoupSizeY)
                {
                    Position = new Vector2D(Position.X, l_SoupSizeY);
                    Velocity = new Vector2D(Velocity.X, Velocity.Y * -1d);
                }
                if (Position.Y < 0)
                {
                    Position = new Vector2D(Position.X, 0);
                    Velocity = new Vector2D(Velocity.X, Velocity.Y * -1d);
                }

                ElementCollectionIsDisabled = -20;
            }
            else CollisionIsDisabled = true;

            Age++;
        }

        public void OnStepFinalize()
        {
            try
            {
                if (IsValid)
                {
                    if (Velocity != Vector2D.Zero)
                    {
                        Int2D NextIntegerizedPosition = Vector2D.ToIntegerizedPosition(Position);

                        if (IntegerizedPosition != NextIntegerizedPosition)
                        {
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlants.Remove(Index);
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlantCount--;
                            IntegerizedPosition = NextIntegerizedPosition;
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlants.Add(Index);
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlantCount++;
                        }
                    }
                }
                else
                {
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += Element * g_Soup.BiomassAmountMultiplier;
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlants.Remove(Index);
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalPlantCount--;
                    lock (g_Soup.PlantUnassignedIndexesLockObject) g_Soup.PlantUnassignedIndexes.Add(Index);
                }
            }
            catch (Exception ex) { ConsoleLog(LogLevel.Failure, ex.ToString()); }
        }
    }
}
