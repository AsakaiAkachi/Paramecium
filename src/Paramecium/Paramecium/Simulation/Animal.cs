﻿using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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
        public double Radius { get; set; }
        public double Mass { get; set; }
        public int RegenerationCooldown { get; set; }

        public int Age { get; set; }
        public int Generation { get; set; }
        public int OffspringCount { get; set; }
        //public double CumulativeMutationRate { get; set; }
        public List<long> ParentGenealogicalTree { get; set; } = new List<long>();

        public long RaceId { get; set; }
        public int RaceIndex { get; set; }

        public double GeneColorRed { get; set; }
        public double GeneColorGreen { get; set; }
        public double GeneColorBlue { get; set; }

        public double[] Brain { get; set; }

        public int[] BrainInput { get; set; }
        public double BrainOutputAcceleration { get; set; }
        public double BrainOutputRotation { get; set; }
        public double BrainOutputAttack { get; set; }

        private int l_Seed;
        private int l_SoupSizeX;
        private int l_SoupSizeY;
        private double l_AnimalColorCognateRange;

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
            Radius = 0.25d;
            Mass = Math.Pow(Radius, 2);

            RaceId = random.NextInt64(0, 2176782336);

            //GeneColorRed = random.Next(0, 255 + 1);
            //GeneColorGreen = random.Next(0, 255 + 1);
            //GeneColorBlue = random.Next(0, 255 + 1);
            GeneColorRed = random.NextDouble();
            GeneColorGreen = random.NextDouble();
            GeneColorBlue = random.NextDouble();

            Age = g_Soup.HatchingTime;
            Generation = 1;
            //CumulativeMutationRate = 0;

            Brain = new double[(29 * 10 * 4 * 3) + (9 * 4 * 4 * 3)];
            for (int i = 0; i < Brain.Length; i++)
            {
                Brain[i] = random.NextDouble() * 2d - 1d;
            }
            BrainInput = new int[(29 * 10) + (9 * 4)];
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
            Radius = 0.125d;
            Mass = Math.Pow(Radius, 2);

            Age = g_Soup.HatchingTime * -1;
            Generation = parent.Generation + 1;
            //CumulativeMutationRate = parent.CumulativeMutationRate;
            ParentGenealogicalTree = new List<long>(parent.ParentGenealogicalTree);
            ParentGenealogicalTree.Insert(0, parent.Id);
            if (ParentGenealogicalTree.Count > 100)
            {
                ParentGenealogicalTree.RemoveAt(ParentGenealogicalTree.Count - 1);
            }

            Brain = new double[(29 * 10 * 4 * 3) + (9 * 4 * 4 * 3)];
            for (int i = 0; i < Brain.Length; i++)
            {
                if (random.NextDouble() < g_Soup.MutationRate)
                {
                    Brain[i] = random.NextDouble() * 2d - 1d;
                    //CumulativeMutationRate += Math.Abs(parent.Brain[i] - Brain[i]) / 2d / Brain.Length;
                }
                else Brain[i] = parent.Brain[i];
            }
            BrainInput = new int[(29 * 10) + (9 * 4)];

            //if (CumulativeMutationRate >= g_Soup.MutationRate * 10d)
            if (false && random.NextDouble() < g_Soup.MutationRate)
            {
                RaceId = random.NextInt64(0, 2176782336);

                GeneColorRed = random.Next(0, 255 + 1);
                GeneColorGreen = random.Next(0, 255 + 1);
                GeneColorBlue = random.Next(0, 255 + 1);

                //CumulativeMutationRate = 0d;

                //EventLog.PushEventLog($"新種族「{LongToBase36(RaceId, 6)}」が誕生しました。(位置 : ({Position.X:0.000}, {Position.Y:0.000}))");
            }
            else
            {
                RaceId = parent.RaceId;

                GeneColorRed = parent.GeneColorRed;
                GeneColorGreen = parent.GeneColorGreen;
                GeneColorBlue = parent.GeneColorBlue;

                int ColorMutation = random.Next(0, 2 + 1);

                if (ColorMutation == 0) GeneColorRed = Math.Min(Math.Max(parent.GeneColorRed + (random.NextDouble() * g_Soup.AnimalColorMutationRange * 2d - g_Soup.AnimalColorMutationRange), 0d), 1d);
                else if (ColorMutation == 1) GeneColorGreen = Math.Min(Math.Max(parent.GeneColorGreen + (random.NextDouble() * g_Soup.AnimalColorMutationRange * 2d - g_Soup.AnimalColorMutationRange), 0d), 1d);
                else if (ColorMutation == 2) GeneColorBlue = Math.Min(Math.Max(parent.GeneColorBlue + (random.NextDouble() * g_Soup.AnimalColorMutationRange * 2d - g_Soup.AnimalColorMutationRange), 0d), 1d);
            }
        }
        public Animal(Animal original, Vector2D position)
        {
            Index = -1;
            Id = new Random(g_Soup.Seed).NextInt64(0, 4738381338321616896);
            IsValid = original.IsValid;

            Position = position;
            IntegerizedPosition = Vector2D.ToIntegerizedPosition(position);
            Velocity = new Vector2D();
            Angle = original.Angle;

            Element = original.Element;
            Radius = original.Radius;
            Mass = original.Mass;

            Age = original.Age;
            Generation = 1;
            ParentGenealogicalTree = original.ParentGenealogicalTree;

            Brain = original.Brain;
            BrainInput = original.BrainInput;

            RaceId = original.RaceId;
            GeneColorRed = original.GeneColorRed;
            GeneColorGreen = original.GeneColorGreen;
            GeneColorBlue = original.GeneColorBlue;
        }

        public void OnInitialize()
        {
            l_Seed = g_Soup.Seed;
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;
            l_AnimalColorCognateRange = g_Soup.AnimalColorCognateRange;

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

            if (Generation > g_Soup.LatestGeneration)
            {
                g_Soup.LatestGeneration = Generation;
            }
        }

        public void OnLoaded()
        {
            l_Seed = g_Soup.Seed;
            l_SoupSizeX = g_Soup.SizeX;
            l_SoupSizeY = g_Soup.SizeY;
            l_AnimalColorCognateRange = g_Soup.AnimalColorCognateRange;
        }

        public void EarlyUpdate()
        {
            Velocity *= 0.9d;

            if (Age >= 0)
            {
                BrainOutputAcceleration = 0;
                BrainOutputRotation = 0;
                BrainOutputAttack = 0;

                for (int i = -14; i <= 14; i++)
                {
                    Vector2D rayVector = Vector2D.FromAngle(Angle + i * 6.4285714286);
                    Vector2D rayWallVector = Vector2D.FromAngle(Angle + i * 6.4285714286 + 90) * 0.25d * 1.414213;

                    bool j_break = false;

                    for (int j = 0; j <= 9; j++)
                    {
                        if (j_break)
                        {
                            BrainInput[(i + 14) * 10 + j] = 0;
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
                                        BrainInput[(i + 14) * 10 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12];
                                        BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }

                                    Int2D TargetGrid = Vector2D.ToIntegerizedPosition(TargetPosition);
                                    BrainInput[(i + 14) * 10 + j] = 0;
                                    if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[(i + 14) * 10 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12];
                                        BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    else if (j > 1)
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            BrainInput[(i + 14) * 10 + j] = 2;
                                            BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 1 * 3];
                                            BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 1 * 3 + 1];
                                            BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 1 * 3 + 2];
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    //if (target.RaceId == RaceId)
                                                    if (Math.Sqrt(Math.Pow(GeneColorRed - target.GeneColorRed, 2) + Math.Pow(GeneColorGreen - target.GeneColorGreen, 2) + Math.Pow(GeneColorBlue - target.GeneColorBlue, 2)) < l_AnimalColorCognateRange)
                                                    {
                                                        BrainInput[(i + 14) * 10 + j] = 3;
                                                        BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 2 * 3];
                                                        BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 2 * 3 + 1];
                                                        BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 2 * 3 + 2];
                                                    }
                                                    else
                                                    {
                                                        BrainInput[(i + 14) * 10 + j] = 4;
                                                        BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 3 * 3];
                                                        BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 3 * 3 + 1];
                                                        BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 3 * 3 + 2];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount; l++)
                                            {
                                                Plant target = g_Soup.Plants[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlants[l]];

                                                double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - (Angle + i * 6.4285714286);
                                                if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                                if (TargetRelativeAngle >= 360 - 45 / (j + 1) || TargetRelativeAngle <= 45 / (j + 1))
                                                {
                                                    BrainInput[(i + 14) * 10 + j] = 2;
                                                    BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 1 * 3];
                                                    BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 1 * 3 + 1];
                                                    BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 1 * 3 + 2];
                                                }
                                            }
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - (Angle + i * 6.4285714286);
                                                    if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                                    if (TargetRelativeAngle >= 360 - 45 / (j + 1) || TargetRelativeAngle <= 45 / (j + 1))
                                                    {
                                                        //if (target.RaceId == RaceId)
                                                        if (Math.Sqrt(Math.Pow(GeneColorRed - target.GeneColorRed, 2) + Math.Pow(GeneColorGreen - target.GeneColorGreen, 2) + Math.Pow(GeneColorBlue - target.GeneColorBlue, 2)) < l_AnimalColorCognateRange)
                                                        {
                                                            BrainInput[(i + 14) * 10 + j] = 3;
                                                            BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 2 * 3];
                                                            BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 2 * 3 + 1];
                                                            BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 2 * 3 + 2];
                                                        }
                                                        else
                                                        {
                                                            BrainInput[(i + 14) * 10 + j] = 4;
                                                            BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12 + 3 * 3];
                                                            BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 3 * 3 + 1];
                                                            BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 3 * 3 + 2];
                                                        }
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
                                        BrainInput[(i + 14) * 10 + j] = 1;
                                        BrainOutputAcceleration += Brain[(i + 14) * 120 + j * 12];
                                        BrainOutputRotation += Brain[(i + 14) * 120 + j * 12 + 1];
                                        BrainOutputAttack += Brain[(i + 14) * 120 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    if (j < 9)
                                    {
                                        Int2D TargetGridWall1 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector * -1);
                                        if (g_Soup.GridMap[TargetGridWall1.X + TargetGridWall1.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[(i + 14) * 10 + j] = 1;
                                            BrainOutputAcceleration += Brain[(i + 14) * 120 + (j + 1) * 12];
                                            BrainOutputRotation += Brain[(i + 14) * 120 + (j + 1) * 12 + 1];
                                            BrainOutputAttack += Brain[(i + 14) * 120 + (j + 1) * 12 + 2];
                                            j_break = true;
                                            break;
                                        }
                                        Int2D TargetGridWall2 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector);
                                        if (g_Soup.GridMap[TargetGridWall2.X + TargetGridWall2.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[(i + 14) * 10 + j] = 1;
                                            BrainOutputAcceleration += Brain[(i + 14) * 120 + (j + 1) * 12];
                                            BrainOutputRotation += Brain[(i + 14) * 120 + (j + 1) * 12 + 1];
                                            BrainOutputAttack += Brain[(i + 14) * 120 + (j + 1) * 12 + 2];
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
                    Vector2D rayVector = Vector2D.FromAngle(Angle + i * 22.5 + 180);
                    Vector2D rayWallVector = Vector2D.FromAngle(Angle + i * 22.5 + 180 + 90) * 0.25d * 1.414213;

                    bool j_break = false;

                    for (int j = 0; j <= 3; j++)
                    {
                        if (j_break)
                        {
                            BrainInput[290 + (i + 4) * 4 + j] = 0;
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
                                        BrainInput[290 + (i + 4) * 4 + j] = 1;
                                        BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12];
                                        BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 1];
                                        BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }

                                    Int2D TargetGrid = Vector2D.ToIntegerizedPosition(TargetPosition);
                                    BrainInput[290 + (i + 4) * 4 + j] = 0;
                                    if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].Type == TileType.Wall)
                                    {
                                        BrainInput[290 + (i + 4) * 4 + j] = 1;
                                        BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12];
                                        BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 1];
                                        BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    else if (j > 1)
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            BrainInput[290 + (i + 4) * 4 + j] = 2;
                                            BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3];
                                            BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3 + 1];
                                            BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3 + 2];
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    //if (target.RaceId == RaceId)
                                                    if (Math.Sqrt(Math.Pow(GeneColorRed - target.GeneColorRed, 2) + Math.Pow(GeneColorGreen - target.GeneColorGreen, 2) + Math.Pow(GeneColorBlue - target.GeneColorBlue, 2)) < l_AnimalColorCognateRange)
                                                    {
                                                        BrainInput[290 + (i + 4) * 4 + j] = 3;
                                                        BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3];
                                                        BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3 + 1];
                                                        BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3 + 2];
                                                    }
                                                    else
                                                    {
                                                        BrainInput[290 + (i + 4) * 4 + j] = 4;
                                                        BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3];
                                                        BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3 + 1];
                                                        BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3 + 2];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlantCount; l++)
                                            {
                                                Plant target = g_Soup.Plants[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalPlants[l]];

                                                double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - (Angle + i * 22.5 + 180);
                                                if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                                if (TargetRelativeAngle >= 360 - 45 / (j + 1) || TargetRelativeAngle <= 45 / (j + 1))
                                                {
                                                    BrainInput[290 + (i + 4) * 4 + j] = 2;
                                                    BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3];
                                                    BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3 + 1];
                                                    BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 1 * 3 + 2];
                                                }
                                            }
                                        }
                                        if (g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount > 0)
                                        {
                                            for (int l = 0; l < g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimalCount; l++)
                                            {
                                                Animal target = g_Soup.Animals[g_Soup.GridMap[TargetGrid.X + TargetGrid.Y * l_SoupSizeX].LocalAnimals[l]];
                                                if (target.Id != Id)
                                                {
                                                    double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - (Angle + i * 22.5 + 180);
                                                    if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                                    if (TargetRelativeAngle >= 360 - 45 / (j + 1) || TargetRelativeAngle <= 45 / (j + 1))
                                                    {
                                                        //if (target.RaceId == RaceId)
                                                        if (Math.Sqrt(Math.Pow(GeneColorRed - target.GeneColorRed, 2) + Math.Pow(GeneColorGreen - target.GeneColorGreen, 2) + Math.Pow(GeneColorBlue - target.GeneColorBlue, 2)) < l_AnimalColorCognateRange)
                                                        {
                                                            BrainInput[290 + (i + 4) * 4 + j] = 3;
                                                            BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3];
                                                            BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3 + 1];
                                                            BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 2 * 3 + 2];
                                                        }
                                                        else
                                                        {
                                                            BrainInput[290 + (i + 4) * 4 + j] = 4;
                                                            BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3];
                                                            BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3 + 1];
                                                            BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 3 * 3 + 2];
                                                        }
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
                                        BrainInput[290 + (i + 4) * 4 + j] = 1;
                                        BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + j * 12];
                                        BrainOutputRotation += Brain[3480 + (i + 4) * 36 + j * 12 + 1];
                                        BrainOutputAttack += Brain[3480 + (i + 4) * 36 + j * 12 + 2];
                                        j_break = true;
                                        break;
                                    }
                                    if (j < 3)
                                    {
                                        Int2D TargetGridWall1 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector * -1);
                                        if (g_Soup.GridMap[TargetGridWall1.X + TargetGridWall1.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[290 + (i + 4) * 4 + j] = 1;
                                            BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + (j + 1) * 12];
                                            BrainOutputRotation += Brain[3480 + (i + 4) * 36 + (j + 1) * 12 + 1];
                                            BrainOutputAttack += Brain[3480 + (i + 4) * 36 + (j + 1) * 12 + 2];
                                            j_break = true;
                                            break;
                                        }
                                        Int2D TargetGridWall2 = Vector2D.ToIntegerizedPosition(Position + rayVector * j + (rayVector * k * 0.25) + rayWallVector);
                                        if (g_Soup.GridMap[TargetGridWall2.X + TargetGridWall2.Y * l_SoupSizeX].Type == TileType.Wall)
                                        {
                                            BrainInput[290 + (i + 4) * 4 + j] = 1;
                                            BrainOutputAcceleration += Brain[3480 + (i + 4) * 36 + (j + 1) * 12];
                                            BrainOutputRotation += Brain[3480 + (i + 4) * 36 + (j + 1) * 12 + 1];
                                            BrainOutputAttack += Brain[3480 + (i + 4) * 36 + (j + 1) * 12 + 2];
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

                g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += Math.Min(Math.Min(Math.Abs(BrainOutputAcceleration), 10d) / 10d * g_Soup.AnimalElementLosePerStepInAccelerating, Element) * g_Soup.BiomassAmountMultiplier;
                Element -= Math.Min(Math.Min(Math.Abs(BrainOutputAcceleration), 10d) / 10d * g_Soup.AnimalElementLosePerStepInAccelerating, Element);
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
                                        double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - Angle;
                                        if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                        if (TargetRelativeAngle >= 360 - 60 || TargetRelativeAngle <= 60)
                                        {
                                            Element += Math.Min(1d, target.Element) * g_Soup.BiomassAmountMultiplier;
                                            target.Element -= Math.Min(1d, target.Element);
                                            target.ElementCollectionIsDisabled = -20;
                                            target.Radius = 0.5d * Math.Max(Math.Sqrt(target.Element) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
                                            target.Mass = Math.Pow(Radius, 2);
                                        }
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

                                    if (BrainOutputAttack > 0)
                                    {
                                        if (BrainOutputAttack > target.BrainOutputAttack && target.Age >= 0)
                                        {
                                            double TargetRelativeAngle = Vector2D.ToAngle(target.Position - Position) - Angle;
                                            if (TargetRelativeAngle < 0) TargetRelativeAngle += 360;
                                            if (TargetRelativeAngle >= 360 - 60 || TargetRelativeAngle <= 60)
                                            {
                                                Element += Math.Min(Math.Min(BrainOutputAttack - target.BrainOutputAttack, 5d), target.Element) * g_Soup.BiomassAmountMultiplier;
                                                target.Element -= Math.Min(Math.Min(BrainOutputAttack - target.BrainOutputAttack, 5d), target.Element);
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

        public void LateUpdate(int threadId)
        {
            if (double.IsInfinity(Position.X) || double.IsNaN(Position.X))
            {
                Position = new Vector2D(0, Position.Y);
            }
            if (double.IsInfinity(Position.Y) || double.IsNaN(Position.Y))
            {
                Position = new Vector2D(Position.X, 0);
            }

            if (double.IsInfinity(Velocity.X) || double.IsNaN(Velocity.X))
            {
                Velocity = new Vector2D(0, Velocity.Y);
            }
            if (double.IsInfinity(Velocity.Y) || double.IsNaN(Velocity.Y))
            {
                Velocity = new Vector2D(Velocity.X, 0);
            }

            if (Age >= 0)
            {
                g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX].Fertility += Math.Min(g_Soup.AnimalElementLosePerStepInPassive, Element) * g_Soup.BiomassAmountMultiplier;
                Element -= Math.Min(g_Soup.AnimalElementLosePerStepInPassive, Element);
            }

            if (Element <= 0)
            {
                IsValid = false;
                return;
            }

            if (Element >= g_Soup.AnimalForkBiomass * 2d)
            {
                g_Soup.AnimalBuffer[threadId].Add(new Animal(new Random((int)((l_Seed + Id + OffspringCount + Age) % 2147483647)), this));
                Element -= g_Soup.AnimalForkBiomass;
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
            if (Position.X > l_SoupSizeX - Radius)
            {
                Position = new Vector2D(l_SoupSizeX - Radius, Position.Y);
                Velocity = new Vector2D(Velocity.X * -1d, Velocity.Y);
            }
            if (Position.X < 0 + Radius)
            {
                Position = new Vector2D(0 + Radius, Position.Y);
                Velocity = new Vector2D(Velocity.X * -1d, Velocity.Y);
            }
            if (Position.Y > l_SoupSizeY - Radius)
            {
                Position = new Vector2D(Position.X, l_SoupSizeY - Radius);
                Velocity = new Vector2D(Velocity.X, Velocity.Y * -1d);
            }
            if (Position.Y < 0 + Radius)
            {
                Position = new Vector2D(Position.X, 0 + Radius);
                Velocity = new Vector2D(Velocity.X, Velocity.Y * -1d);
            }

            if (Angle < 0d) Angle += 360d;
            if (Angle >= 360d) Angle -= 360d;

            Age++;
        }

        public void OnStepFinalize()
        {
            Int2D NextIntegerizedPosition = Vector2D.ToIntegerizedPosition(Position);
            Grid currentGrid = g_Soup.GridMap[IntegerizedPosition.X + IntegerizedPosition.Y * l_SoupSizeX];
            Grid nextGrid = g_Soup.GridMap[NextIntegerizedPosition.X + NextIntegerizedPosition.Y * l_SoupSizeX];

            if (double.IsInfinity(Velocity.X) || double.IsNaN(Velocity.X) || double.IsInfinity(Velocity.Y) || double.IsNaN(Velocity.Y) ||
                double.IsInfinity(Position.X) || double.IsNaN(Position.X) || double.IsInfinity(Position.Y) || double.IsNaN(Position.Y))
            {
                currentGrid.LocalAnimals.Remove(Index);
                //currentGrid.LocalAnimalCount--;
                currentGrid.LocalAnimalCount = currentGrid.LocalAnimals.Count;
                lock (g_Soup.AnimalUnassignedIndexesLockObject) g_Soup.AnimalUnassignedIndexes.Add(Index);
                lock (g_Soup.TotalDieCountLockObject) g_Soup.TotalDieCount++;
                g_Soup.Animals[Index] = null;

                return;
            }

            if (currentGrid.Type == TileType.Wall || nextGrid.Type == TileType.Wall) IsValid = false;

            if (IsValid)
            {
                if (Velocity != Vector2D.Zero)
                {
                    if (IntegerizedPosition != NextIntegerizedPosition)
                    {
                        currentGrid.LocalAnimals.Remove(Index);
                        //prevGrid.LocalAnimalCount--;
                        currentGrid.LocalAnimalCount = currentGrid.LocalAnimals.Count;

                        IntegerizedPosition = NextIntegerizedPosition;

                        nextGrid.LocalAnimals.Add(Index);
                        //nextGrid.LocalAnimalCount++;
                        nextGrid.LocalAnimalCount = nextGrid.LocalAnimals.Count;
                    }
                }
            }
            else
            {
                currentGrid.Fertility += Element * g_Soup.BiomassAmountMultiplier;
                currentGrid.LocalAnimals.Remove(Index);
                //currentGrid.LocalAnimalCount--;
                currentGrid.LocalAnimalCount = currentGrid.LocalAnimals.Count;
                lock (g_Soup.AnimalUnassignedIndexesLockObject) g_Soup.AnimalUnassignedIndexes.Add(Index);
                lock (g_Soup.TotalDieCountLockObject) g_Soup.TotalDieCount++;
                g_Soup.Animals[Index] = null;
            }
        }
    }
}