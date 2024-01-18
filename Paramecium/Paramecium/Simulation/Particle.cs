using Paramecium.Libs;
using System;
using System.CodeDom;
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
            Position = new Vector2D(rnd, 0, 0, Variables.SoupInstance.env_SizeX, Variables.SoupInstance.env_SizeY);
            Velocity = new Vector2D();
            GridPosition = Vector2D.ToGridPosition(Position);

            switch (type)
            {
                case ParticleType.Plant:
                    Satiety = 0;
                    Radius = (0.1d + Math.Log2(Math.Max(Satiety, 1)) * 0.1d);
                    Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
                    break;
                case ParticleType.Animal:
                    Genes = new Gene(rnd);
                    Angle = rnd.NextDouble() * 360d;
                    Satiety = 64;
                    Radius = Genes.gene_Size;
                    Color = Genes.gene_Color;
                    RaceId = rnd.Next(0, 2147483647);
                    Generation = 1;
                    Age = 0;
                    break;
            }
        }
        public Particle(Vector2D position, double satiety)
        {
            Type = ParticleType.Plant;
            IsAlive = true;
            Random rnd = new Random();
            Position = position;
            Velocity = new Vector2D();
            GridPosition = Vector2D.ToGridPosition(Position);

            Satiety = satiety;
            Radius = (0.1d + Math.Log2(Math.Max(Satiety, 1)) * 0.1d);
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
                Position = parent.Position + new Vector2D(rnd, -0.5, -0.5, 0.5, 0.5);
                Velocity = new Vector2D(rnd, -0.5d, -0.5d, 0.5d, 0.5d);
                Radius = (0.1d + Math.Log2(Math.Max(Satiety, 1)) * 0.1d);
                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Satiety), 0);
            }
            else if (parent.Type == ParticleType.Animal)
            {
                Genes = new Gene(parent.Genes);
                Angle = parent.Angle + 180;
                Position = parent.Position + Vector2D.FromAngle(Angle) * Genes.gene_Size;
                Radius = 0.25d;
                RaceId = parent.RaceId;
                Color = new ColorInt3(255, 127, 255);
                Generation = parent.Generation + 1;
                Age = -300;
            }
            GridPosition = Vector2D.ToGridPosition(Position);
        }

        public void OnInitialize()
        {
            Index = Variables.SoupInstance.UnassignedParticleIds[Variables.SoupInstance.UnassignedParticleIds.Count - 1];
            Variables.SoupInstance.UnassignedParticleIds.RemoveAt(Variables.SoupInstance.UnassignedParticleIds.Count - 1);
            Variables.SoupInstance.TileMap[GridPosition.X + GridPosition.Y * Variables.SoupInstance.env_SizeX].LocalParticles.Add(Index);

            Id = new Random().Next(100000000, 1000000000);
        }

        public void EarlyUpdate()
        {
            Velocity *= 0.9d;
        }
        public void MiddleUpdate()
        {
            int local_env_SizeX = Variables.SoupInstance.env_SizeX;
            int local_env_SizeY = Variables.SoupInstance.env_SizeY;

            switch (Type)
            {
                case ParticleType.Plant:
                    if (!InCollision)
                    {
                        for (int x = Math.Max(GridPosition.X - 3, 0); x <= Math.Min(GridPosition.X + 3, local_env_SizeX - 1); x++)
                        {
                            for (int y = Math.Max(GridPosition.Y - 3, 0); y <= Math.Min(GridPosition.Y + 3, local_env_SizeY - 1); y++)
                            {
                                Grid TargetGrid = Variables.SoupInstance.TileMap[x + y * local_env_SizeX];
                                Satiety += TargetGrid.Fertility * 0.001d;
                                TargetGrid.Fertility *= 0.999d;
                            }
                        }
                    }
                    break;
                case ParticleType.Animal:
                    if (Age > 0)
                    {

                        Random rnd = new Random(Index);

                        bool breakFlag = false;

                        int RaycastIteration = 100;
                        double RaycastRange = 10d;

                        double InputWallAngle = 0;
                        double InputWallDistance = RaycastRange;
                        double InputPlantAngle = 0;
                        double InputPlantDistance = RaycastRange;
                        double InputAnimalSameSpeciesAngle = 0;
                        double InputAnimalSameSpeciesDistance = RaycastRange;
                        double InputAnimalDifferentSpeciesAngle = 0;
                        double InputAnimalDifferentSpeciesDistance = RaycastRange;

                        double distance = RaycastRange;

                        for (int i = -6; i <= 6; i++)
                        {
                            Vector2D RaycastVector = Vector2D.FromAngle(Angle + (i * 10)) * (RaycastRange / RaycastIteration);
                            Vector2D RaycastScannerVector = Vector2D.FromAngle(Angle + (i * 10) + 90) * (RaycastRange / RaycastIteration * 1.414213);

                            for (int j = 1; j <= RaycastIteration; j++)
                            {
                                if (
                                    Position.X + RaycastVector.X < 0 || Position.X + RaycastVector.X >= local_env_SizeX ||
                                    Position.Y + RaycastVector.Y < 0 || Position.Y + RaycastVector.Y >= local_env_SizeY
                                )
                                {
                                    InputWallAngle += (1.0d * (i / 6d) + Genes.gene_InputWallAngleAddend * 0.1d) * ((RaycastIteration - j) / (double)RaycastIteration) * 0.1d;
                                    if (RaycastRange / RaycastIteration * j < InputWallDistance) InputWallDistance = RaycastRange / RaycastIteration * j;
                                    break;
                                }
                                else
                                {
                                    Int2D TargetGrid0 = Vector2D.ToGridPosition(Position + RaycastVector * j);
                                    Int2D TargetGrid1 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector * -1d);
                                    Int2D TargetGrid2 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector);

                                    if (Variables.SoupInstance.TileMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].Type == TileType.Wall)
                                    {
                                        InputWallAngle += (1.0d * (i / 6d) + Genes.gene_InputWallAngleAddend * 0.1d) * ((RaycastIteration - j) / (double)RaycastIteration) * 0.1d;
                                        if (RaycastRange / RaycastIteration * j < InputWallDistance) InputWallDistance = RaycastRange / RaycastIteration * j;
                                        break;
                                    }
                                    else
                                    {
                                        int LocalParticlesCount = Variables.SoupInstance.TileMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles.Count;
                                        if (LocalParticlesCount > 0)
                                        {
                                            List<int> LocalParticles = Variables.SoupInstance.TileMap[TargetGrid0.X + TargetGrid0.Y * local_env_SizeX].LocalParticles;
                                            for (int k = 0; k < LocalParticlesCount; k++)
                                            {
                                                if (LocalParticles[k] != Index)
                                                {
                                                    Particle TargetParticle = Variables.SoupInstance.Particles[LocalParticles[k]];

                                                    if (TargetParticle.Type == ParticleType.Plant)
                                                    {
                                                        InputPlantAngle += 1.0d * (i / 6d) * ((RaycastIteration - j) / (double)RaycastIteration);
                                                        if (RaycastRange / RaycastIteration * j < InputPlantDistance) InputPlantDistance = j * (RaycastRange / RaycastIteration);
                                                    }
                                                    else if (TargetParticle.Type == ParticleType.Plant)
                                                    {
                                                        if (RaceId == TargetParticle.RaceId)
                                                        {
                                                            InputAnimalSameSpeciesAngle += 1.0d * (i / 6d) * ((RaycastIteration - j) / (double)RaycastIteration);
                                                            if (RaycastRange / RaycastIteration * j < InputAnimalSameSpeciesDistance) InputAnimalSameSpeciesDistance = j * (RaycastRange / RaycastIteration);
                                                        }
                                                        else
                                                        {
                                                            InputAnimalDifferentSpeciesAngle += 1.0d * (i / 6d) * ((RaycastIteration - j) / (double)RaycastIteration);
                                                            if (RaycastRange / RaycastIteration * j < InputAnimalDifferentSpeciesDistance) InputAnimalDifferentSpeciesDistance = j * (RaycastRange / RaycastIteration);
                                                        }
                                                    }
                                                }
                                                breakFlag = true;
                                            }
                                        }

                                        if (breakFlag) break;

                                        if (Variables.SoupInstance.TileMap[TargetGrid1.X + TargetGrid1.Y * local_env_SizeX].Type == TileType.Wall)
                                        {
                                            break;
                                        }
                                        if (Variables.SoupInstance.TileMap[TargetGrid2.X + TargetGrid2.Y * local_env_SizeX].Type == TileType.Wall)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        Angle += InputWallAngle * Genes.gene_InputWallAngleMultiplier + InputPlantAngle * Genes.gene_InputPlantAngleMultiplier + InputAnimalSameSpeciesAngle * Genes.gene_InputAnimalSameSpeciesAngleMultiplier + InputAnimalDifferentSpeciesAngle * Genes.gene_InputAnimalDifferentSpeciesAngleMultiplier;
                        Velocity += Vector2D.FromAngle(Angle) * 0.01d * (Math.Max(distance, 1d) / RaycastRange);

                        Variables.SoupInstance.TileMap[(GridPosition.X) + (GridPosition.Y) * local_env_SizeX].Fertility += Math.Min(0.075d * (Math.Max(distance, 1d) / RaycastRange), Satiety);
                        Satiety -= Math.Min(0.075d * (Math.Max(distance, 1d) / RaycastRange), Satiety);
                    }
                    break;
            }

            InCollision = false;
            for (int x = Math.Max(GridPosition.X - 1, 0); x <= Math.Min(GridPosition.X + 1, local_env_SizeX - 1); x++)
            {
                for (int y = Math.Max(GridPosition.Y - 1, 0); y <= Math.Min(GridPosition.Y + 1, local_env_SizeY - 1); y++)
                {
                    if (Variables.SoupInstance.TileMap[x + y * Variables.SoupInstance.env_SizeX].Type == TileType.Wall)
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
                            Velocity += Vector2D.Normalization(Position - WallPosition1) * (Radius + 0.353553 - WallDistance1) * Math.Min((Vector2D.Size(Velocity) + 0.05d), 0.2d);
                            InCollision = true;
                        }
                        if (WallDistance2 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition2) * (Radius + 0.353553 - WallDistance2) * Math.Min((Vector2D.Size(Velocity) + 0.05d), 0.2d);
                            InCollision = true;
                        }
                        if (WallDistance3 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition3) * (Radius + 0.353553 - WallDistance3) * Math.Min((Vector2D.Size(Velocity) + 0.05d), 0.2d);
                            InCollision = true;
                        }
                        if (WallDistance4 < 0.353553 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition4) * (Radius + 0.353553 - WallDistance4) * Math.Min((Vector2D.Size(Velocity) + 0.05d), 0.2d);
                            InCollision = true;
                        }
                        if (WallDistance5 < 0.5 + Radius)
                        {
                            Velocity += Vector2D.Normalization(Position - WallPosition5) * (Radius + 0.5 - WallDistance5) * Math.Min((Vector2D.Size(Velocity) + 0.05d), 0.2d);
                            InCollision = true;
                        }
                    }

                    int LocalParticlesCount = Variables.SoupInstance.TileMap[x + y * local_env_SizeX].LocalParticles.Count;
                    if (LocalParticlesCount >= 1)
                    {
                        List<int> TargetIds = Variables.SoupInstance.TileMap[x + y * local_env_SizeX].LocalParticles;
                        for (int i = 0; i < LocalParticlesCount; i++)
                        {
                            if (TargetIds[i] != Index)
                            {
                                Particle Target = Variables.SoupInstance.Particles[TargetIds[i]];
                                Vector2D collidedParticlePosition = Target.Position;
                                double Distance = Vector2D.Distance(Position, collidedParticlePosition);
                                if (Distance < Radius + Target.Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - Target.Position) * Math.Min(Radius + Target.Radius - Distance, 0.1d) * Math.Min(Vector2D.Size(Velocity + Target.Velocity), 0.2d);
                                    InCollision = true;
                                    Target.InCollision = true;

                                    if (Type == ParticleType.Animal && Target.Type == ParticleType.Plant)
                                    {
                                        Satiety += Math.Min(1d, Target.Satiety);
                                        Target.Satiety -= Math.Min(1d, Target.Satiety);

                                        if (Target.Satiety <= 0)
                                        {
                                            Target.IsAlive = false;
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
                Radius = (0.1d + Math.Log2(Math.Max(Satiety, 1)) * 0.1d);

                if (Satiety >= 16)
                {
                    Random rnd = new Random();

                    while (Satiety > 4)
                    {
                        double SatietySwap = rnd.NextDouble() * Math.Min(6, Satiety);
                        Variables.SoupInstance.ParticlesBuffer[threadId].Add(new Particle(this, SatietySwap));
                        Satiety -= SatietySwap;
                    }
                    Variables.SoupInstance.ParticlesBuffer[threadId].Add(new Particle(this, Satiety));

                    IsAlive = false;

                    return;
                }
            }
            if (Type == ParticleType.Animal)
            {
                if (Age > 0)
                {
                    if (Satiety >= 128)
                    {
                        Random rnd = new Random();

                        Variables.SoupInstance.ParticlesBuffer[threadId].Add(new Particle(this, 64));
                        Satiety -= 64;
                    }

                    Variables.SoupInstance.TileMap[(GridPosition.X) + (GridPosition.Y) * Variables.SoupInstance.env_SizeX].Fertility += Math.Min(0.025d, Satiety);
                    Satiety -= Math.Min(0.025d, Satiety);

                    if (Satiety <= 0) IsAlive = false;
                }

                if (Age == 0)
                {
                    Radius = Genes.gene_Size;
                    Color = Genes.gene_Color;
                }

                Age++;
            }

            Position += Velocity;
            if (Position.X > Variables.SoupInstance.env_SizeX)
            {
                Position.X = Variables.SoupInstance.env_SizeX;
                Velocity.X *= -1d;
            }
            if (Position.X < 0)
            {
                Position.X = 0;
                Velocity.X *= -1d;
            }
            if (Position.Y > Variables.SoupInstance.env_SizeY)
            {
                Position.Y = Variables.SoupInstance.env_SizeY;
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
            if (GridPosition != NextGridPosition)
            {
                Variables.SoupInstance.TileMap[GridPosition.X + GridPosition.Y * Variables.SoupInstance.env_SizeX].LocalParticles.Remove(Index);
                GridPosition = NextGridPosition;
                Variables.SoupInstance.TileMap[GridPosition.X + GridPosition.Y * Variables.SoupInstance.env_SizeX].LocalParticles.Add(Index);
            }
        }

        public void OnDisable()
        {
            Variables.SoupInstance.TileMap[GridPosition.X + GridPosition.Y * Variables.SoupInstance.env_SizeX].LocalParticles.Remove(Index);
            Variables.SoupInstance.UnassignedParticleIds.Add(Index);
        }
    }

    public class Gene
    {
        public double gene_Size { get; set; }
        public ColorInt3 gene_Color { get; set; }
        public double gene_InputWallAngleMultiplier { get; set; }
        public double gene_InputWallAngleAddend { get; set; }
        public double gene_InputWallDistanceMultiplier { get; set; }
        public double gene_InputPlantAngleMultiplier { get; set; }
        public double gene_InputPlantDistanceMultiplier { get; set; }
        public double gene_InputAnimalSameSpeciesAngleMultiplier { get; set; }
        public double gene_InputAnimalSameSpeciesDistanceMultiplier { get; set; }
        public double gene_InputAnimalDifferentSpeciesAngleMultiplier { get; set; }
        public double gene_InputAnimalDifferentSpeciesDistanceMultiplier { get; set; }

        public Gene()
        {

        }

        public Gene(Random random)
        {
            gene_Size = 0.5d;
            gene_Color = new ColorInt3(random);
            gene_InputWallAngleMultiplier = random.NextDouble() * 2d;
            gene_InputWallAngleAddend = random.NextDouble() * 2d - 1d;
            gene_InputWallDistanceMultiplier = random.NextDouble() * 2d;
            gene_InputPlantAngleMultiplier = random.NextDouble() * 2d;
            gene_InputPlantDistanceMultiplier = random.NextDouble() * 2d;
            gene_InputAnimalSameSpeciesAngleMultiplier = random.NextDouble() * 2d;
            gene_InputAnimalSameSpeciesDistanceMultiplier = random.NextDouble() * 2d;
            gene_InputAnimalDifferentSpeciesAngleMultiplier = random.NextDouble() * 2d;
            gene_InputAnimalDifferentSpeciesDistanceMultiplier = random.NextDouble() * 2d;
        }

        public Gene(Gene parentGenes)
        {
            //gene_Size = TryMutationSize(parentGenes.gene_Size);
            gene_Size = 0.5d;
            gene_Color = parentGenes.gene_Color;
            gene_InputWallAngleMultiplier = TryMutation(parentGenes.gene_InputWallAngleMultiplier);
            gene_InputWallAngleAddend = TryMutation(parentGenes.gene_InputWallAngleAddend);
            gene_InputWallDistanceMultiplier = TryMutation(parentGenes.gene_InputWallDistanceMultiplier);
            gene_InputPlantAngleMultiplier = TryMutation(parentGenes.gene_InputPlantAngleMultiplier);
            gene_InputPlantDistanceMultiplier = TryMutation(parentGenes.gene_InputPlantDistanceMultiplier);
            gene_InputAnimalSameSpeciesAngleMultiplier = TryMutation(parentGenes.gene_InputPlantDistanceMultiplier);
            gene_InputAnimalSameSpeciesDistanceMultiplier = TryMutation(parentGenes.gene_InputPlantDistanceMultiplier);
            gene_InputAnimalDifferentSpeciesAngleMultiplier = TryMutation(parentGenes.gene_InputPlantDistanceMultiplier);
            gene_InputAnimalDifferentSpeciesDistanceMultiplier = TryMutation(parentGenes.gene_InputPlantDistanceMultiplier);
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
