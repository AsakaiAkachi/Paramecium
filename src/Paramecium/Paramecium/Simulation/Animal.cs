using System.Text.Json.Serialization;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Paramecium.Simulation
{
    public class Animal
    {
        public int Index { get; set; }
        public long Id { get; set; }
        public bool IsValid { get; set; }

        public Vector2D Position { get; set; }
        public Int2D IntegerizedPosition { get; set; }
        public Vector2D Velocity { get; set; }
        public double Angle { get; set; }

        public double Element { get; set; }
        public double OffspringProgress { get; set; }
        public double Radius { get; set; }
        public double Mass { get; set; }

        public int Age { get; set; }
        public int Generation { get; set; }
        public int OffspringCount { get; set; }

        public long RaceId { get; set; }

        public int GeneColorRed { get; set; }
        public int GeneColorGreen { get; set; }
        public int GeneColorBlue { get; set; }

        public double[] Brain { get; set; }

        public int[] BrainInput { get; set; }
        public double BrainOutputAcceleration { get; set; }
        public double BrainOutputRotation { get; set; }
        public double BrainOutputAttack { get; set; }

        private int l_SoupSizeX;
        private int l_SoupSizeY;

        public Animal() { }
        public Animal(Random random, Vector2D position, Vector2D velocity, double angle, double element)
        {
            Index = -1;
            Id = random.NextInt64(0, 4738381338321616896);
            IsValid = true;

            Position = position;
            IntegerizedPosition = Vector2D.ToIntegerizedPosition(position);
            Velocity = velocity;
            Angle = angle;

            Element = element;
            OffspringProgress = 0;
            Radius = 0.25d;
            Mass = Math.Pow(Radius, 2);

            RaceId = random.NextInt64(0, 2176782336);

            GeneColorRed = random.Next(0, 255 + 1);
            GeneColorGreen = random.Next(0, 255 + 1);
            GeneColorBlue = random.Next(0, 255 + 1);

            Age = g_Soup.HatchingTime;
            Generation = 1;

            Brain = new double[(33 * 15 * 4 * 3) + (9 * 5 * 4 * 3)]; // 33 rays x 15 scan x 4 type x 3 output + 9 rays x 5 scan x 4 type x 3 output
            for (int i = 0; i < Brain.Length; i++)
            {
                Brain[i] = random.NextDouble() * 2d - 1d;
            }
            BrainInput = new int[(33 * 15) + (9 * 5)];
        }
        public Animal(Random random, Animal parent)
        {
            Index = -1;
            Id = random.NextInt64(0, 4738381338321616896);
            IsValid = true;

            Position = parent.Position;
            IntegerizedPosition = Vector2D.ToIntegerizedPosition(parent.Position);
            Velocity = new Vector2D();
            Angle = parent.Angle + 180;

            Element = g_Soup.AnimalForkBiomass;
            OffspringProgress = 0;
            Radius = 0.125d;
            Mass = Math.Pow(Radius, 2);

            if (random.NextDouble() < 0.01d)
            {
                RaceId = random.NextInt64(0, 2176782336);

                GeneColorRed = random.Next(0, 255 + 1);
                GeneColorGreen = random.Next(0, 255 + 1);
                GeneColorBlue = random.Next(0, 255 + 1);

                EventLog.PushEventLog($"新種族「{LongToBase36(RaceId, 6)}」が誕生しました。(位置 : ({Position.X:0.000}, {Position.Y:0.000}))");
            }
            else
            {
                RaceId = parent.RaceId;

                GeneColorRed = parent.GeneColorRed;
                GeneColorGreen = parent.GeneColorGreen;
                GeneColorBlue = parent.GeneColorBlue;
            }

            Age = g_Soup.HatchingTime * -1;
            Generation = parent.Generation + 1;

            Brain = new double[(33 * 15 * 4 * 3) + (9 * 5 * 4 * 3)];
            for (int i = 0; i < Brain.Length; i++)
            {
                if (random.NextDouble() < g_Soup.MutationRate) Brain[i] = random.NextDouble() * 2d - 1d;
                else Brain[i] = parent.Brain[i];
            }
            BrainInput = new int[(33 * 15) + (9 * 5)];
        }

        public void OnInitialize()
        {
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;

            if (g_Soup.AnimalUnassignedIndexes.Count == 0)
            {
                for (int i = g_Soup.Animals.Length * 2 - 1; i >= g_Soup.Animals.Length; i--)
                {
                    g_Soup.AnimalUnassignedIndexes.Add(i);
                }
                Animal[] ParticlesArrayOld = g_Soup.Animals;
                g_Soup.Animals = new Animal[ParticlesArrayOld.Length * 2];
                ParticlesArrayOld.CopyTo(g_Soup.Animals, 0);
            }
            Index = g_Soup.AnimalUnassignedIndexes[g_Soup.AnimalUnassignedIndexes.Count - 1];
            g_Soup.AnimalUnassignedIndexes.RemoveAt(g_Soup.AnimalUnassignedIndexes.Count - 1);
            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimals.Add(Index);
            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimalCount++;
        }

        public void OnLoaded()
        {
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;
        }

        public void EarlyUpdate()
        {
            if (double.IsInfinity(Angle)) ConsoleLog(LogLevel.Warning, "Angle is positive or negative infinity!");
            if (double.IsInfinity(Element)) ConsoleLog(LogLevel.Warning, "Element is positive or negative infinity!");
            if (double.IsInfinity(OffspringProgress)) ConsoleLog(LogLevel.Warning, "OffspringProgress is positive or negative infinity!");
            if (double.IsInfinity(Radius)) ConsoleLog(LogLevel.Warning, "Radius is positive or negative infinity!");
            if (double.IsInfinity(Mass)) ConsoleLog(LogLevel.Warning, "Mass is positive or negative infinity!");
            if (double.IsInfinity(BrainOutputAcceleration)) ConsoleLog(LogLevel.Warning, "BrainOutputAcceleration is positive or negative infinity!");
            if (double.IsInfinity(BrainOutputRotation)) ConsoleLog(LogLevel.Warning, "BrainOutputRotation is positive or negative infinity!");
            if (double.IsInfinity(BrainOutputAttack)) ConsoleLog(LogLevel.Warning, "BrainOutputAttack is positive or negative infinity!");

            Velocity *= 0.9d;

            if (Age >= 0)
            {
                BrainOutputAcceleration = 0;
                BrainOutputRotation = 0;
                BrainOutputAttack = 0;

                for (int i = -16; i <= 16; i++)
                {
                    Vector2D rayVector = Vector2D.FromAngle(Angle + i * 5.625) * 0.666;
                    Vector2D rayWallVector = Vector2D.FromAngle(Angle + i * 5.625 + 90) * 0.666 * 0.25d * 1.414213;

                    bool j_break = false;

                    for (int j = 0; j < 15; j++)
                    {
                        if (j_break)
                        {
                            BrainInput[(i + 16) * 15 + j] = 0;
                        }
                        else
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (k == 0)
                                {
                                    Vector2D TargetPosition = Position + rayVector * j;
                                    if (TargetPosition.X < 0 || TargetPosition.X > l_SoupSizeX || TargetPosition.Y < 0 || TargetPosition.Y > l_SoupSizeY)
                                    {
                                        BrainInput[(i + 16) * 15 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12];
                                        BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }

                                    Int2D TargetGrid = Vector2D.ToIntegerizedPosition(TargetPosition);
                                    BrainInput[(i + 16) * 15 + j] = 0;
                                    if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[(i + 16) * 15 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12];
                                        BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            BrainInput[(i + 16) * 15 + j] = 2;
                                            BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12 + 1 * 3];
                                            BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 1 * 3 + 1];
                                            BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 1 * 3 + 2];
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    if (target.RaceId == RaceId)
                                                    {
                                                        BrainInput[(i + 16) * 15 + j] = 3;
                                                        BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12 + 2 * 3];
                                                        BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 2 * 3 + 1];
                                                        BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 2 * 3 + 2];
                                                    }
                                                    else
                                                    {
                                                        BrainInput[(i + 16) * 15 + j] = 4;
                                                        BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12 + 3 * 3];
                                                        BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 3 * 3 + 1];
                                                        BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 3 * 3 + 2];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Int2D TargetGridWall0 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25));
                                    if (g_Soup.GridMap[TargetGridWall0.X + TargetGridWall0.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[(i + 16) * 15 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 16) * 180 + j * 12];
                                        BrainOutputRotation += Brain[(i + 16) * 180 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 16) * 180 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    if (j < 14)
                                    {
                                        Int2D TargetGridWall1 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector * -1);
                                        if (g_Soup.GridMap[TargetGridWall1.X + TargetGridWall1.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[(i + 16) * 15 + (j + 1)] = 1;
                                            BrainOutputAcceleration += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3];
                                            BrainOutputRotation += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3 + 1];
                                            BrainOutputAttack += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3 + 2];
                                            j_break = true;
                                            break;
                                        }
                                        Int2D TargetGridWall2 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector);
                                        if (g_Soup.GridMap[TargetGridWall2.X + TargetGridWall2.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[(i + 16) * 15 + (j + 1)] = 1;
                                            BrainOutputAcceleration += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3];
                                            BrainOutputRotation += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3 + 1];
                                            BrainOutputAttack += Brain[(i + 16) * 180 + (j + 1) * 12 + 2 * 3 + 2];
                                            j_break = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = -4; i <= 4; i++)
                {
                    Vector2D rayVector = Vector2D.FromAngle(Angle + i * 19.375 + 180) * 0.666;
                    Vector2D rayWallVector = Vector2D.FromAngle(Angle + i * 19.375 + 180) * 0.666 * 0.25d * 1.414213;

                    bool j_break = false;

                    for (int j = 0; j < 5; j++)
                    {
                        if (j_break)
                        {
                            BrainInput[495 + (i + 4) * 5 + j] = 0;
                        }
                        else
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (k == 0)
                                {
                                    Vector2D TargetPosition = Position + rayVector * j;
                                    if (TargetPosition.X < 0 || TargetPosition.X > l_SoupSizeX || TargetPosition.Y < 0 || TargetPosition.Y > l_SoupSizeY)
                                    {
                                        BrainInput[495 + (i + 4) * 5 + j] = 1;
                                        BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12];
                                        BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 1];
                                        BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }

                                    Int2D TargetGrid = Vector2D.ToIntegerizedPosition(TargetPosition);
                                    BrainInput[495 + (i + 4) * 5 + j] = 0;
                                    if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[495 + (i + 4) * 5 + j] = 1;
                                        BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12];
                                        BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 1];
                                        BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            BrainInput[495 + (i + 4) * 5 + j] = 2;
                                            BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12 + 1 * 3];
                                            BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 1 * 3 + 1];
                                            BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 1 * 3 + 2];
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    if (target.RaceId == RaceId)
                                                    {
                                                        BrainInput[495 + (i + 4) * 5 + j] = 3;
                                                        BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12 + 2 * 3];
                                                        BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 2 * 3 + 1];
                                                        BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 2 * 3 + 2];
                                                    }
                                                    else
                                                    {
                                                        BrainInput[495 + (i + 4) * 5 + j] = 4;
                                                        BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12 + 3 * 3];
                                                        BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 3 * 3 + 1];
                                                        BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 3 * 3 + 2];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Int2D TargetGridWall0 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25));
                                    if (g_Soup.GridMap[TargetGridWall0.X + TargetGridWall0.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[495 + (i + 4) * 5 + j] = 1;
                                        BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + j * 12];
                                        BrainOutputRotation += Brain[5940 + (i + 4) * 48 + j * 12 + 1];
                                        BrainOutputAttack += Brain[5940 + (i + 4) * 48 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    if (j < 4)
                                    {
                                        Int2D TargetGridWall1 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector * -1);
                                        if (g_Soup.GridMap[TargetGridWall1.X + TargetGridWall1.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[495 + (i + 4) * 5 + (j + 1)] = 1;
                                            BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3];
                                            BrainOutputRotation += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3 + 1];
                                            BrainOutputAttack += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3 + 2];
                                            j_break = true;
                                            break;
                                        }
                                        Int2D TargetGridWall2 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector);
                                        if (g_Soup.GridMap[TargetGridWall2.X + TargetGridWall2.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[495 + (i + 4) * 5 + (j + 1)] = 1;
                                            BrainOutputAcceleration += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3];
                                            BrainOutputRotation += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3 + 1];
                                            BrainOutputAttack += Brain[5940 + (i + 4) * 48 + (j + 1) * 12 + 2 * 3 + 2];
                                            j_break = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Angle += Math.Min(Math.Max(BrainOutputRotation, -10), 10) * 1.8d;
                Velocity += Vector2D.FromAngle(Angle) * Math.Min(Math.Max(BrainOutputAcceleration, -10d), 10d) * 0.001d;

                g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += Math.Min(Math.Min(Math.Abs(BrainOutputAcceleration), 10d) / 10d * 0.0375d, Element) * g_Soup.BiomassAmountMultiplier;
                Element -= Math.Min(Math.Min(Math.Abs(BrainOutputAcceleration), 10d) / 10d * 0.0375d, Element);
            }
        }

        public void MiddleUpdate()
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
                    if (g_Soup.GridMap[x + y * l_SoupSizeX].LocalPlantCount > 0)
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

                                    if (BrainOutputAttack > 0)
                                    {
                                        Element += Math.Min(1.5d, target.Element) * g_Soup.BiomassAmountMultiplier;
                                        target.Element -= Math.Min(1.5d, target.Element);
                                    }
                                }
                            }
                        }
                    }
                    if (g_Soup.GridMap[x + y * l_SoupSizeX].LocalAnimalCount > 0)
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

                                    if (BrainOutputAttack > 0 && BrainOutputAttack > target.BrainOutputAttack && target.Age >= 0)
                                    {
                                        Element += Math.Min(5d, target.Element) * g_Soup.BiomassAmountMultiplier;
                                        target.Element -= Math.Min(5d, target.Element);
                                        if (target.Element <= 0)
                                        {
                                            Element += target.OffspringProgress * g_Soup.BiomassAmountMultiplier;
                                            target.OffspringProgress = 0;
                                        }
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
            if (Age >= 0)
            {
                g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += Math.Min(0.0125d, Element) * g_Soup.BiomassAmountMultiplier;
                Element -= Math.Min(0.0125d, Element);
            }

            if (Element <= 0)
            {
                IsValid = false;
                return;
            }

            if (Element >= g_Soup.AnimalForkBiomass)
            {
                OffspringProgress += (Element - g_Soup.AnimalForkBiomass) * g_Soup.BiomassAmountMultiplier;
                Element -= Element - g_Soup.AnimalForkBiomass;
            }
            if (OffspringProgress >= g_Soup.AnimalForkBiomass)
            {
                g_Soup.AnimalBuffer[threadId].Add(new Animal(new Random(), this));
                OffspringProgress -= g_Soup.AnimalForkBiomass;
                OffspringCount++;
            }

            if (Age >= 0 && Age <= g_Soup.HatchingTime)
            {
                Radius = 0.25 * g_Soup.CellSizeMultiplier + 0.25 * g_Soup.CellSizeMultiplier * (Math.Min(g_Soup.HatchingTime, Age) / (double)g_Soup.HatchingTime);
                Mass = Math.Pow(Radius, 2);
            }

            if (Vector2D.Size(Velocity) > 0.1d)
            {
                Velocity = Vector2D.Normalization(Velocity) * 0.1d;
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
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimals.Remove(Index);
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimalCount--;
                            IntegerizedPosition = NextIntegerizedPosition;
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimals.Add(Index);
                            g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimalCount++;
                        }
                    }
                }
                else
                {
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += (Element + OffspringProgress) * g_Soup.BiomassAmountMultiplier;
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimals.Remove(Index);
                    g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].LocalAnimalCount--;
                    lock (g_Soup.AnimalUnassignedIndexesLockObject) g_Soup.AnimalUnassignedIndexes.Add(Index);
                    g_Soup.Animals[Index] = null;
                }
            }
            catch (Exception ex) { ConsoleLog(LogLevel.Failure, ex.ToString()); }
        }
    }
}
