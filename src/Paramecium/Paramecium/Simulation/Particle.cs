using Paramecium.Libs;
using System;
using System.CodeDom;
using System.Drawing;
using System.Runtime;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Paramecium.Simulation
{
    public class Particle
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public ParticleType Type { get; set; }
        public bool IsAlive { get; set; }

        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; }

        public double Angle { get; set; }

        public Int2D GridPosition { get; set; }

        public double Radius { get; set; }
        public ColorInt3 Color { get; set; }

        public double Satiety { get; set; }

        public bool InCollision { get; set; }

        public int RaceId { get; set; }
        public Gene Genes { get; set; }

        public int Generation { get; set; }
        public int Age { get; set; }

        public int TargetIndex { get; set; } = -1;
        public int TargetId { get; set; } = -1;

        const double CellSize = 0.5;

        public Particle()
        {
            Type = ParticleType.Plant;
            IsAlive = false;
            Position = new Vector2D();
            Velocity = new Vector2D();
            GridPosition = new Int2D();
            Radius = 0;
            Color = new ColorInt3(0, 0, 0);
            Satiety = 0;
            InCollision = false;
        }

        public Particle(ParticleType type)
        {
            Type = type;
            IsAlive = true;
            Random rnd = new Random();
            Position = new Vector2D(rnd, 0, 0, Global.g_Soup.SizeX, Global.g_Soup.SizeY);
            GridPosition = Vector2D.ToGridPosition(Position);
            Velocity = new Vector2D();

            switch (type)
            {
                case ParticleType.Plant:
                    Satiety = rnd.NextDouble() * 8d + 4d;
                    Radius = Math.Max(Math.Sqrt(Satiety) / 8d * CellSize, 0.025d);
                    Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
                    break;
                case ParticleType.Animal:
                    Genes = new Gene(rnd);
                    Angle = rnd.NextDouble() * 360d;
                    Radius = 0.5 * CellSize;
                    Satiety = 64d;
                    Color = new ColorInt3(Genes.GeneColorRed, Genes.GeneColorGreen, Genes.GeneColorBlue);
                    RaceId = rnd.Next(0, 2147483647);
                    Generation = 1;
                    Age = 300;
                    break;
            }
        }
        public Particle(Vector2D position, double satiety)
        {
            Type = ParticleType.Plant;
            IsAlive = true;
            Random rnd = new Random();
            Position = position;
            GridPosition = Vector2D.ToGridPosition(Position);
            Velocity = new Vector2D();

            Satiety = satiety;
            Radius = Math.Max(Math.Sqrt(Satiety) / 8d * CellSize, 0.025d);
            Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
        }
        public Particle(Particle parent, double satiety)
        {
            Random rnd = new Random();

            Type = parent.Type;
            IsAlive = true;
            Position = new Vector2D();
            Velocity = new Vector2D();
            Color = new ColorInt3();

            Satiety = satiety;

            if (parent.Type == ParticleType.Plant)
            {
                Position = parent.Position + Vector2D.FromAngle(rnd.NextDouble() * 360d) * 0.1d;
                Velocity = new Vector2D(rnd, -0.1d, -0.1d, 0.1d, 0.1d);
                Radius = Math.Max(Math.Sqrt(Satiety) / 8d * CellSize, 0.025d);
                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
                Age = -50;
            }
            else if (parent.Type == ParticleType.Animal)
            {
                Genes = new Gene(parent.Genes);
                Angle = parent.Angle + 180;
                Position = parent.Position + Vector2D.FromAngle(Angle) * parent.Radius * 0.75d;
                Radius = 0.25 * CellSize;
                RaceId = parent.RaceId;
                Color = new ColorInt3(255, 127, 255);
                Generation = parent.Generation + 1;
                Age = -300;
            }
            GridPosition = Vector2D.ToGridPosition(Position);
        }

        public void OnInitialize()
        {
            if (g_Soup.UnassignedParticleIds.Count == 0)
            {
                for (int i = g_Soup.Particles.Length * 2 - 1; i >= g_Soup.Particles.Length; i--)
                {
                    g_Soup.UnassignedParticleIds.Add(i);
                }
                Particle[] ParticlesArrayOld = g_Soup.Particles;
                g_Soup.Particles = new Particle[ParticlesArrayOld.Length * 2];
                ParticlesArrayOld.CopyTo(g_Soup.Particles, 0);
            }
            Index = g_Soup.UnassignedParticleIds[g_Soup.UnassignedParticleIds.Count - 1];
            g_Soup.UnassignedParticleIds.RemoveAt(g_Soup.UnassignedParticleIds.Count - 1);
            g_Soup.GridMap[GridPosition.X + GridPosition.Y * g_Soup.SizeX].LocalParticles.Add(Index);

            Id = new Random().Next(100000000, 1000000000);
        }

        public void EarlyUpdate()
        {
            if (Type == ParticleType.Animal || (Type == ParticleType.Plant && Age >= 0))
            {
                Velocity *= 0.9d;
            }
        }
        public void MiddleUpdate(int threadId)
        {
            int local_env_SizeX = Global.g_Soup.SizeX;
            int local_env_SizeY = Global.g_Soup.SizeY;

            switch (Type)
            {
                case ParticleType.Plant:
                    Random rnd = new Random();
                    int xOffset = 0;
                    int yOffset = 0;

                    for (int i = 0; i < 6; i++)
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

                        if (GridPosition.X + xOffsetNext < 0 || GridPosition.X + xOffsetNext >= local_env_SizeX || GridPosition.Y + yOffsetNext < 0 || GridPosition.Y + yOffsetNext >= local_env_SizeY) { }
                        else if (Global.g_Soup.GridMap[(GridPosition.X + xOffsetNext) + (GridPosition.Y + yOffsetNext) * local_env_SizeX].Type == TileType.Wall) { }
                        else if (xOffsetNext >= -3 && xOffsetNext <= 3 && yOffsetNext >= -3 && yOffsetNext <= 3)
                        {
                            xOffset = xOffsetNext;
                            yOffset = yOffsetNext;
                        }
                    }

                    Grid TargetGrid = Global.g_Soup.GridMap[(GridPosition.X + xOffset) + (GridPosition.Y + yOffset) * local_env_SizeX];
                    Satiety += Math.Min(0.16d, TargetGrid.Fertility);
                    TargetGrid.Fertility -= Math.Min(0.16d, TargetGrid.Fertility);

                    /**
                    if (!InCollision)
                    {
                        for (int x = Math.Max(GridPosition.X - 3, 0); x <= Math.Min(GridPosition.X + 3, local_env_SizeX - 1); x++)
                        {
                            for (int y = Math.Max(GridPosition.Y - 3, 0); y <= Math.Min(GridPosition.Y + 3, local_env_SizeY - 1); y++)
                            {
                                Grid TargetGrid = Variables.SoupInstance.TileMap[x + y * local_env_SizeX];
                                Satiety += TargetGrid.Fertility * 0.005d;
                                TargetGrid.Fertility *= 0.995d;
                            }
                        }
                    }
                    **/
                    break;
                case ParticleType.Animal:
                    if (Age > 0)
                    {
                        bool breakFlag = false;

                        int RaycastIteration = 30;
                        double RaycastRange = 10;

                        double RaycastResultWall = 0d;
                        double RaycastResultPlant = 0d;

                        Particle? Target = null;
                        double TargetDistance = RaycastRange / CellSize;

                        if (TargetIndex != -1 && TargetId != -1)
                        {
                            if (Global.g_Soup.Particles[TargetIndex] is not null)
                            {
                                if (Global.g_Soup.Particles[TargetIndex].IsAlive)
                                {
                                    if (Global.g_Soup.Particles[TargetIndex].Id == TargetId)
                                    {
                                        if (Vector2D.Size(Global.g_Soup.Particles[TargetIndex].Position - Position) < RaycastRange)
                                        {
                                            Target = Global.g_Soup.Particles[TargetIndex];
                                        }
                                        else
                                        {
                                            TargetIndex = -1;
                                            TargetId = -1;
                                        }
                                    }
                                    else
                                    {
                                        TargetIndex = -1;
                                        TargetId = -1;
                                    }
                                }
                                else
                                {
                                    TargetIndex = -1;
                                    TargetId = -1;
                                }
                            }
                            else
                            {
                                TargetIndex = -1;
                                TargetId = -1;
                            }
                        }

                        if (Target is not null)
                        {
                            Vector2D RaycastVector = Vector2D.Normalization(Target.Position - Position) * (RaycastRange / RaycastIteration);
                            Vector2D RaycastScannerVector = Vector2D.Rotate(Vector2D.Normalization(Target.Position - Position), 90) * (RaycastRange / RaycastIteration * 1.414213);

                            for (int j = 1; j <= RaycastIteration; j++)
                            {
                                if (
                                    Position.X + RaycastVector.X < 0 || Position.X + RaycastVector.X >= local_env_SizeX ||
                                    Position.Y + RaycastVector.Y < 0 || Position.Y + RaycastVector.Y >= local_env_SizeY
                                )
                                {
                                    Target = null;
                                    TargetIndex = -1;
                                    TargetId = -1;
                                    break;
                                }
                                else
                                {
                                    Int2D TargetGrid0 = Vector2D.ToGridPosition(Position + RaycastVector * j);
                                    Int2D TargetGrid1 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector * -1d);
                                    Int2D TargetGrid2 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector);

                                    if (Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].Type == TileType.Wall)
                                    {
                                        Target = null;
                                        TargetIndex = -1;
                                        TargetId = -1;
                                        break;
                                    }
                                    else
                                    {
                                        int LocalParticlesCount = Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles.Count;
                                        if (LocalParticlesCount > 0)
                                        {
                                            List<int> LocalParticles = Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles;
                                            for (int k = 0; k < LocalParticlesCount; k++)
                                            {
                                                if (LocalParticles[k] != Index)
                                                {
                                                    Particle TargetParticle = Global.g_Soup.Particles[LocalParticles[k]];

                                                    if (Global.g_Soup.Particles[LocalParticles[k]] == Target)
                                                    {
                                                        breakFlag = true;
                                                    }
                                                }
                                            }
                                        }

                                        if (breakFlag) break;

                                        if (Global.g_Soup.GridMap[TargetGrid1.X + TargetGrid1.Y * local_env_SizeX].Type == TileType.Wall)
                                        {
                                            if (!breakFlag)
                                            {
                                                Target = null;
                                                TargetIndex = -1;
                                                TargetId = -1;
                                            }
                                            break;
                                        }
                                        if (Global.g_Soup.GridMap[TargetGrid2.X + TargetGrid2.Y * local_env_SizeX].Type == TileType.Wall)
                                        {
                                            if (!breakFlag)
                                            {
                                                Target = null;
                                                TargetIndex = -1;
                                                TargetId = -1;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = -10; i <= 10; i++)
                            {
                                Vector2D RaycastVector = Vector2D.FromAngle(Angle + (i * 6)) * (RaycastRange / RaycastIteration);
                                Vector2D RaycastScannerVector = Vector2D.FromAngle(Angle + (i * 6) + 90) * (RaycastRange / RaycastIteration * 1.414213);

                                for (int j = 1; j <= RaycastIteration; j++)
                                {
                                    if (
                                        Position.X + RaycastVector.X < 0 || Position.X + RaycastVector.X >= local_env_SizeX ||
                                        Position.Y + RaycastVector.Y < 0 || Position.Y + RaycastVector.Y >= local_env_SizeY
                                    )
                                    {
                                        RaycastResultWall += 1d * ((RaycastIteration - j) / (double)RaycastIteration);
                                        break;
                                    }
                                    else
                                    {
                                        Int2D TargetGrid0 = Vector2D.ToGridPosition(Position + RaycastVector * j);
                                        Int2D TargetGrid1 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector * -1d);
                                        Int2D TargetGrid2 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector);

                                        if (Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].Type == TileType.Wall)
                                        {
                                            RaycastResultWall += 1d * ((RaycastIteration - j) / (double)RaycastIteration);
                                            break;
                                        }
                                        else
                                        {
                                            int LocalParticlesCount = Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles.Count;
                                            if (LocalParticlesCount > 0)
                                            {
                                                List<int> LocalParticles = Global.g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles;
                                                for (int k = 0; k < LocalParticlesCount; k++)
                                                {
                                                    if (LocalParticles[k] != Index)
                                                    {
                                                        Particle TargetParticle = Global.g_Soup.Particles[LocalParticles[k]];

                                                        bool IsValidTarget = false;

                                                        double TargetDistanceTemp = Vector2D.Size(TargetParticle.Position - Position) / (Math.Max(TargetParticle.Radius, 0.001d) / CellSize);
                                                        if (TargetParticle.Type == ParticleType.Plant)
                                                        {
                                                            TargetDistanceTemp /= Math.Max(1d - (Genes.GeneDiet + 1d) / 2d, 0.001d);
                                                            IsValidTarget = true;
                                                        }
                                                        else if (TargetParticle.Type == ParticleType.Animal)
                                                        {
                                                            TargetDistanceTemp /= Math.Max((Genes.GeneDiet + 1d) / 2d, 0.001d);
                                                            if (Math.Sqrt(Math.Pow(Color.Red - TargetParticle.Color.Red, 2) + Math.Pow(Color.Green - TargetParticle.Color.Green, 2) + Math.Pow(Color.Blue - TargetParticle.Color.Blue, 2)) > 16d && TargetParticle.Age >= 0) IsValidTarget = true;
                                                        }

                                                        if (TargetDistanceTemp < TargetDistance && IsValidTarget)
                                                        {
                                                            Target = TargetParticle;
                                                            TargetDistance = TargetDistanceTemp;
                                                            TargetIndex = LocalParticles[k];
                                                            TargetId = TargetParticle.Id;
                                                        }
                                                    }
                                                    breakFlag = true;
                                                }
                                            }

                                            if (breakFlag) break;

                                            if (Global.g_Soup.GridMap[TargetGrid1.X + TargetGrid1.Y * local_env_SizeX].Type == TileType.Wall)
                                            {
                                                break;
                                            }
                                            if (Global.g_Soup.GridMap[TargetGrid2.X + TargetGrid2.Y * local_env_SizeX].Type == TileType.Wall)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        /**
                        if (RaycastResultPlant > 0)
                        {
                            Angle += RaycastResultPlant;
                            Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.25d;
                        }
                        else
                        {
                            Angle += RaycastResultWall;
                            Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.5d;
                        }
                        **/

                        if (Target is not null)
                        {
                            Angle += Math.Min(Math.Max(Vector2D.ToAngle(Target.Position - Position) - Angle, -12), 12);
                            //Velocity += Vector2D.Normalization(Target.Position - Position) * (0.5d + Vector2D.Size(Target.Position - Position) / RaycastRange * 0.5d) * 0.01d;
                            Velocity += Vector2D.FromAngle(Angle) * 0.01d * (0.5d + Vector2D.Size(Target.Position - Position) / RaycastRange * 0.5d);
                        }
                        else
                        {
                            Angle += Math.Sin(((Age % 628) + (Index % 628)) * 0.01d) * 3d;
                            Velocity += Vector2D.FromAngle(Angle) * 0.005d;
                        }

                        //Global.g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_env_SizeX].Fertility += Math.Min(0.075d * Vector2D.Size(Velocity), Satiety);
                        //Satiety -= Math.Min(0.075d * Vector2D.Size(Velocity), Satiety);
                    }
                    break;
            }

            InCollision = false;
            for (int x = Math.Max((int)Math.Floor(Position.X - CellSize), 0); x <= Math.Min((int)Math.Ceiling(Position.X + CellSize), local_env_SizeX - 1); x++)
            {
                for (int y = Math.Max((int)Math.Floor(Position.Y - CellSize), 0); y <= Math.Min((int)Math.Ceiling(Position.Y + CellSize), local_env_SizeY - 1); y++)
                {
                    if (Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].Type == TileType.Wall)
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
                            InCollision = true;
                        }
                        if (WallDistance2 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition2) * ((Radius + 0.353553 - WallDistance2) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            InCollision = true;
                        }
                        if (WallDistance3 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition3) * ((Radius + 0.353553 - WallDistance3) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            InCollision = true;
                        }
                        if (WallDistance4 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition4) * ((Radius + 0.353553 - WallDistance4) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            InCollision = true;
                        }
                        if (WallDistance5 < 0.5 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition5) * ((Radius + 0.5 - WallDistance5) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                            InCollision = true;
                        }
                    }

                    int LocalParticlesCount = Global.g_Soup.GridMap[x + y * local_env_SizeX].LocalParticles.Count;
                    if (LocalParticlesCount >= 1)
                    {
                        List<int> TargetIds = Global.g_Soup.GridMap[x + y * local_env_SizeX].LocalParticles;
                        for (int i = 0; i < LocalParticlesCount; i++)
                        {
                            if (TargetIds[i] != Index)
                            {
                                Particle Target = Global.g_Soup.Particles[TargetIds[i]];
                                Vector2D collidedParticlePosition = Target.Position;
                                double Distance = Vector2D.Distance(Position, collidedParticlePosition);
                                if (Distance < Radius + Target.Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - Target.Position) * ((Radius + Target.Radius - Distance) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity + Target.Velocity), 0.01d);
                                    InCollision = true;
                                    Target.InCollision = true;

                                    if (Type == ParticleType.Animal)
                                    {
                                        if (Target.Index == TargetIndex)
                                        {

                                            if (Target.Type == ParticleType.Plant)
                                            {
                                                Satiety += Math.Min(1d, Target.Satiety) * (1d - (Genes.GeneDiet + 1d) / 2d);
                                                g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * g_Soup.SizeX].Fertility += Math.Min(1d, Target.Satiety) * ((Genes.GeneDiet + 1d) / 2d);
                                            }
                                            else if (Target.Type == ParticleType.Animal)
                                            {
                                                Satiety += Math.Min(1d, Target.Satiety) * ((Genes.GeneDiet + 1d) / 2d);
                                                g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * g_Soup.SizeX].Fertility += Math.Min(1d, Target.Satiety) * (1d - (Genes.GeneDiet + 1d) / 2d);
                                            }
                                            Target.Satiety -= Math.Min(1d, Target.Satiety);

                                            if (Target.Satiety <= 0)
                                            {
                                                Target.IsAlive = false;

                                                //if (new Random().NextDouble() < 0.1d)
                                                //{
                                                //    g_Soup.ParticlesBuffer[threadId].Add(new Particle(Position + (Vector2D.FromAngle(Angle + 180) * CellSize * 0.45d), 0));
                                                //}
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
            if (Type == ParticleType.Plant)
            {
                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
                Radius = Math.Max(Math.Sqrt(Satiety) / 8d * CellSize, 0.025d);

                if (Satiety >= 16)
                {
                    Random rnd = new Random();

                    while (Satiety > 4)
                    {
                        double SatietySwap = rnd.NextDouble() * Math.Min(6, Satiety);
                        Global.g_Soup.ParticlesBuffer[threadId].Add(new Particle(this, SatietySwap));
                        Satiety -= SatietySwap;
                    }
                    Global.g_Soup.ParticlesBuffer[threadId].Add(new Particle(this, Satiety));
                    Satiety = 0;

                    IsAlive = false;

                    return;
                }
            }
            if (Type == ParticleType.Animal)
            {
                if (Age > 0)
                {
                    if (Age <= 300) Radius = 0.25 * CellSize + 0.25 * CellSize * (Math.Min(300, Age) / 300d);

                    if (Satiety >= 128)
                    {
                        Random rnd = new Random();

                        Global.g_Soup.ParticlesBuffer[threadId].Add(new Particle(this, 64));
                        Satiety -= 64;
                    }

                    Global.g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * Global.g_Soup.SizeX].Fertility += Math.Min(0.025d, Satiety) + Math.Min(0.075d * Vector2D.Size(Velocity), Satiety);
                    Satiety -= Math.Min(0.025d, Satiety) + Math.Min(0.075d * Vector2D.Size(Velocity), Satiety); ;

                    if (Satiety <= 0) IsAlive = false;
                }

                if (Age == 0)
                {
                    //Radius = 0.5 * CellSize;
                    Color = new ColorInt3(Genes.GeneColorRed, Genes.GeneColorGreen, Genes.GeneColorBlue);
                }
            }

            Age++;

            if (Vector2D.Size(Velocity) > 0.1d)
            {
                Velocity = Vector2D.Normalization(Velocity) * 0.1d;
            }

            Position += Velocity;
            if (Position.X > Global.g_Soup.SizeX)
            {
                Position.X = Global.g_Soup.SizeX;
                Velocity.X *= -1d;
            }
            if (Position.X < 0)
            {
                Position.X = 0;
                Velocity.X *= -1d;
            }
            if (Position.Y > Global.g_Soup.SizeY)
            {
                Position.Y = Global.g_Soup.SizeY;
                Velocity.Y *= -1d;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
                Velocity.Y *= -1d;
            }

            if (Angle < 0d) Angle += 360d;
            if (Angle >= 360d) Angle -= 360d;
        }

        public void OnStepFinish()
        {
            Int2D NextGridPosition = Vector2D.ToGridPosition(Position);

            if (Global.g_Soup.GridMap[NextGridPosition.X + NextGridPosition.Y * Global.g_Soup.SizeX].Type == TileType.Wall)
            {
                IsAlive = false;
            }

            if (IsAlive)
            {
                if (GridPosition != NextGridPosition)
                {
                    Global.g_Soup.GridMap[GridPosition.X + GridPosition.Y * Global.g_Soup.SizeX].LocalParticles.Remove(Index);
                    GridPosition = NextGridPosition;
                    Global.g_Soup.GridMap[GridPosition.X + GridPosition.Y * Global.g_Soup.SizeX].LocalParticles.Add(Index);
                }
            }

            if (!IsAlive)
            {
                Global.g_Soup.GridMap[GridPosition.X + GridPosition.Y * Global.g_Soup.SizeX].Fertility += Satiety;
                Global.g_Soup.GridMap[GridPosition.X + GridPosition.Y * Global.g_Soup.SizeX].LocalParticles.Remove(Index);
                Global.g_Soup.UnassignedParticleIds.Add(Index);
                return;
            }
        }
    }

    public class Gene
    {
        public int GeneColorRed { get; set; }
        public int GeneColorGreen { get; set; }
        public int GeneColorBlue { get; set; }
        public double GeneDiet { get; set; }

        public Gene()
        {

        }

        public Gene(Random random)
        {
            Random rnd = new Random();

            GeneColorRed = rnd.Next(0, 256);
            GeneColorGreen = rnd.Next(0, 256);
            GeneColorBlue = rnd.Next(0, 256);
            GeneDiet = -1d;
        }

        public Gene(Gene parentGenes)
        {
            Random rnd = new Random();

            GeneColorRed = parentGenes.GeneColorRed;
            GeneColorGreen = parentGenes.GeneColorGreen;
            GeneColorBlue = parentGenes.GeneColorBlue;
            GeneDiet = Math.Min(Math.Max(GeneDiet + (rnd.NextDouble() * 0.2d - 0.1d), -1d), 1d);

            if (rnd.NextDouble() < 0.02d)
            {
                GeneColorRed = rnd.Next(0, 256);
                GeneColorGreen = rnd.Next(0, 256);
                GeneColorBlue = rnd.Next(0, 256);
                GeneDiet = rnd.NextDouble() * 2d - 1d;
            }
            else
            {
                int MutatingColor = rnd.Next(0, 3 + 1);
                switch (MutatingColor)
                {
                    case 0:
                        GeneColorRed = Math.Min(Math.Max(GeneColorRed + rnd.Next(-1, 1 + 1), 0), 255);
                        break;
                    case 1:
                        GeneColorGreen = Math.Min(Math.Max(GeneColorGreen + rnd.Next(-1, 1 + 1), 0), 255);
                        break;
                    case 2:
                        GeneColorBlue = Math.Min(Math.Max(GeneColorBlue + rnd.Next(-1, 1 + 1), 0), 255);
                        break;
                }
            }
        }

        public static double TryMutation(double targetGene)
        {
            Random rnd = new Random();

            if (rnd.NextDouble() < 0.1d)
            {
                if (rnd.NextDouble() < 0.1d)
                {
                    return rnd.NextDouble() * 2d;
                }
                else
                {
                    return Math.Max(Math.Min(targetGene + (rnd.NextDouble() * 0.2d - 0.1d), 2d), 0d);
                }
            }
            else return targetGene;
        }
        public static double TryMutationSize(double targetGene)
        {
            Random rnd = new Random();

            if (rnd.NextDouble() < 0.1d)
            {
                if (rnd.NextDouble() < 0.1d)
                {
                    return rnd.NextDouble() * 1.25d + 0.25d;
                }
                else
                {
                    return Math.Max(Math.Min(targetGene + (rnd.NextDouble() * 0.2d - 0.1d), 1.5d), 0.25d);
                }
            }
            else return targetGene;
        }
        public static ColorInt3 TryMutationColor(ColorInt3 targetGene)
        {
            Random rnd = new Random();

            if (rnd.NextDouble() < 0.01d)
            {
                return new ColorInt3(rnd);
            }
            else return targetGene;
        }
    }

    public enum ParticleType
    {
        Plant,
        Animal
    }
}
