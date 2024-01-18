using Paramecium.Libs;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace Paramecium.Simulation
{
    public class Soup
    {
        public int env_SizeX { get; set; }
        public int env_SizeY { get; set; }

        public double env_WallPerlinNoiseX { get; set; }
        public double env_WallPerlinNoiseY { get; set; }
        public double env_WallPerlinNoiseZ { get; set; }
        public double env_WallPerlinNoiseScale { get; set; }
        public int env_WallPerlinNoiseOctave { get; set; }
        public double env_WallThickness { get; set; }

        public int env_ParticleCountLimit { get; set; }
        public double env_TotalBiomassAmount { get; set; }
        public double BiomassAmount;

        public int sim_InitialAnimalCount { get; set; }

        public int sim_ThreadCountWidth { get; set; }
        public int sim_ThreadCountHeight { get; set; }
        public int sim_ParallelLimit { get; set; }

        public int timeSteps { get; set; }

        public Grid[] TileMap { get; set; }
        public byte[] TileMapByte { get; set; }

        public byte[] GridMapBg { get; set; }
        public byte[] GridMapBgParticle { get; set; }
        public Particle[] Particles { get; set; }
        public List<Particle>[] ParticlesBuffer { get; set; }
        public List<int> UnassignedParticleIds { get; set; }

        public bool IsRunning = false;

        public int PopulationPlant { get; set; }
        public int PopulationAnimal { get; set; }
        public int PopulationTotal { get; set; }

        public bool SoupStop = false;

        public Soup() { }
        public Soup(int env_SizeX, int env_SizeY, double env_WallPerlinNoiseX, double env_WallPerlinNoiseY, double env_WallPerlinNoiseZ, bool WallPerlinNoisePositionRandomize, double env_WallPerlinNoiseScale, int env_WallPerlinNoiseOctave, double env_WallThickness, int env_ParticleCountLimit, double env_TotalBiomassAmount, int sim_InitialAnimalCount)
        {
            this.env_SizeX = env_SizeX;
            this.env_SizeY = env_SizeY;
            TileMap = new Grid[env_SizeX * env_SizeY];

            TileMapByte = new byte[env_SizeX * env_SizeY];
            GridMapBg = new byte[env_SizeX * env_SizeY];
            GridMapBgParticle = new byte[env_SizeX * env_SizeY];

            Random rnd = new Random();

            if (WallPerlinNoisePositionRandomize) this.env_WallPerlinNoiseX = rnd.NextDouble() * 256d;
            else this.env_WallPerlinNoiseX = env_WallPerlinNoiseX;
            if (WallPerlinNoisePositionRandomize) this.env_WallPerlinNoiseY = rnd.NextDouble() * 256d;
            else this.env_WallPerlinNoiseY = env_WallPerlinNoiseY;
            if (WallPerlinNoisePositionRandomize) this.env_WallPerlinNoiseZ = rnd.NextDouble() * 256d;
            else this.env_WallPerlinNoiseZ = env_WallPerlinNoiseZ;
            this.env_WallPerlinNoiseScale = env_WallPerlinNoiseScale;
            this.env_WallPerlinNoiseOctave = env_WallPerlinNoiseOctave;
            this.env_WallThickness = env_WallThickness;

            this.env_ParticleCountLimit = env_ParticleCountLimit;
            this.env_TotalBiomassAmount = env_TotalBiomassAmount;

            this.sim_InitialAnimalCount = sim_InitialAnimalCount;

            sim_ThreadCountWidth = 16;
            sim_ThreadCountHeight = 8;
            sim_ParallelLimit = 1;

            Perlin perlin = new Perlin();
            for (int x = 0; x < env_SizeX; x++)
            {
                for (int y = 0; y < env_SizeY; y++)
                {
                    TileType tileType;
                    if (
                        Math.Abs(perlin.OctavePerlin(
                            this.env_WallPerlinNoiseX + x * env_WallPerlinNoiseScale,
                            this.env_WallPerlinNoiseY + y * env_WallPerlinNoiseScale,
                            this.env_WallPerlinNoiseZ, 
                            env_WallPerlinNoiseOctave, 0.5
                        ) - 0.5) <= env_WallThickness
                    ) tileType = TileType.Wall;
                    else tileType = TileType.None;
                    TileMap[x + y * env_SizeX] = new Grid(x, y, tileType);
                    TileMapByte[x + y * env_SizeX] = (byte)tileType;
                    GridMapBg[x + y * env_SizeX] = (byte)tileType;
                    GridMapBgParticle[x + y * env_SizeX] = (byte)tileType;
                }
            }

            Particles = new Particle[env_ParticleCountLimit];
            ParticlesBuffer = new List<Particle>[sim_ThreadCountWidth * sim_ThreadCountHeight];
            for (int i = 0; i < ParticlesBuffer.Length; i++)
            {
                ParticlesBuffer[i] = new List<Particle>();
            }
            UnassignedParticleIds = new List<int>();
            for (int i = env_ParticleCountLimit - 1; i >= 0; i--)
            {
                UnassignedParticleIds.Add(i);
            }
        }

        public void SoupSetup()
        {
            for (int i = 0; i < sim_InitialAnimalCount; i++)
            {
                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
            }

            double PlacedBiomassAmount = 0;
            Random rnd = new Random();

            while (PlacedBiomassAmount < env_TotalBiomassAmount - 12d)
            {
                Vector2D PlantGeneratePosition = new Vector2D(rnd, 0, 0, env_SizeX, env_SizeY);
                Int2D PlantGenerateGridPosition = Vector2D.ToGridPosition(PlantGeneratePosition);
                while (TileMap[PlantGenerateGridPosition.X + PlantGenerateGridPosition.Y * env_SizeX].Type == TileType.Wall)
                {
                    PlantGeneratePosition = new Vector2D(rnd, 0, 0, env_SizeX, env_SizeY);
                    PlantGenerateGridPosition = Vector2D.ToGridPosition(PlantGeneratePosition);
                }
                double PlantBiomassAmount = rnd.NextDouble() * 8d + 4d;
                ParticlesBuffer[0].Add(new Particle(PlantGeneratePosition, PlantBiomassAmount));
                PlacedBiomassAmount += PlantBiomassAmount;
            }

            {
                Vector2D PlantGeneratePosition = new Vector2D(rnd, 0, 0, env_SizeX, env_SizeY);
                Int2D PlantGenerateGridPosition = Vector2D.ToGridPosition(PlantGeneratePosition);
                while (TileMap[PlantGenerateGridPosition.X + PlantGenerateGridPosition.Y * env_SizeX].Type == TileType.Wall)
                {
                    PlantGeneratePosition = new Vector2D(rnd, 0, 0, env_SizeX, env_SizeY);
                    PlantGenerateGridPosition = Vector2D.ToGridPosition(PlantGeneratePosition);
                }
                double PlantBiomassAmount = env_TotalBiomassAmount - PlacedBiomassAmount;
                ParticlesBuffer[0].Add(new Particle(PlantGeneratePosition, PlantBiomassAmount));
                PlacedBiomassAmount += PlantBiomassAmount;
            }
        }

        public void SoupRun()
        {
            Task.Run(() =>
            {
                {
                    for (int i = 0; i < ParticlesBuffer.Length; i++)
                    {
                        for (int j = 0; j < ParticlesBuffer[i].Count; j++)
                        {
                            ParticlesBuffer[i][j].OnInitialize();
                            Particles[ParticlesBuffer[i][j].Index] = ParticlesBuffer[i][j];
                        }
                        ParticlesBuffer[i].Clear();
                    }

                    int PopulationPlantNext = 0;
                    int PopulationAnimalNext = 0;

                    for (int x = 0; x < env_SizeX; x++)
                    {
                        for (int y = 0; y < env_SizeY; y++)
                        {
                            for (int i = TileMap[x + y * env_SizeX].LocalParticles.Count - 1; i >= 0; i--)
                            {
                                int DisableTargetIndex = TileMap[x + y * env_SizeX].LocalParticles[i];
                                if (!Particles[DisableTargetIndex].IsAlive)
                                {
                                    Particles[DisableTargetIndex].OnDisable();
                                    Particles[DisableTargetIndex] = null;
                                }
                            }

                            if (TileMap[x + y * env_SizeX].Type != TileType.Wall)
                            {
                                GridMapBg[x + y * env_SizeX] = (byte)((int)(Math.Min(TileMap[x + y * env_SizeX].Fertility, 4d) * 8d) + 16);

                                if (TileMap[x + y * env_SizeX].LocalParticles.Count == 0) GridMapBgParticle[x + y * env_SizeX] = (byte)((int)(Math.Min(TileMap[x + y * env_SizeX].Fertility, 4d) * 8d) + 16);
                                else
                                {
                                    for (int j = 0; j < TileMap[x + y * env_SizeX].LocalParticles.Count; j++)
                                    {
                                        if (Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Plant)
                                        {
                                            GridMapBgParticle[x + y * env_SizeX] = 0x02;
                                            PopulationPlantNext++;
                                        }
                                        else if (Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Animal)
                                        {
                                            GridMapBgParticle[x + y * env_SizeX] = 0x03;
                                            PopulationAnimalNext++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    PopulationPlant = PopulationPlantNext;
                    PopulationAnimal = PopulationAnimalNext;
                    PopulationTotal = PopulationPlantNext + PopulationAnimalNext;
                }

                while (true)
                {
                    if (IsRunning)
                    {
                        for (int i = 0; i < ParticlesBuffer.Length; i++)
                        {
                            for (int j = 0; j < ParticlesBuffer[i].Count; j++)
                            {
                                ParticlesBuffer[i][j].OnInitialize();
                                Particles[ParticlesBuffer[i][j].Index] = ParticlesBuffer[i][j];
                            }
                            ParticlesBuffer[i].Clear();
                        }

                        Update(0, 0);
                        Update(0, 1);
                        Update(0, 2);
                        Update(0, 3);

                        Update(1, 0);
                        Update(1, 1);
                        Update(1, 2);
                        Update(1, 3);

                        Update(2, 0);
                        Update(2, 1);
                        Update(2, 2);
                        Update(2, 3);

                        Update(3, 0);
                        Update(3, 1);
                        Update(3, 2);
                        Update(3, 3);

                        int PopulationPlantNext = 0;
                        int PopulationAnimalNext = 0;

                        double BiomassAmountNext = 0;

                        for (int x = 0; x < env_SizeX; x++)
                        {
                            for (int y = 0; y < env_SizeY; y++)
                            {
                                for (int i = TileMap[x + y * env_SizeX].LocalParticles.Count - 1; i >= 0; i--)
                                {
                                    if (!Particles[TileMap[x + y * env_SizeX].LocalParticles[i]].IsAlive) Particles[TileMap[x + y * env_SizeX].LocalParticles[i]].OnDisable();
                                }

                                BiomassAmountNext += TileMap[x + y * env_SizeX].Fertility;

                                if (TileMap[x + y * env_SizeX].Type != TileType.Wall)
                                {
                                    GridMapBg[x + y * env_SizeX] = (byte)((int)(Math.Min(TileMap[x + y * env_SizeX].Fertility, 4d) * 8d) + 16);

                                    if (TileMap[x + y * env_SizeX].LocalParticles.Count == 0) GridMapBgParticle[x + y * env_SizeX] = (byte)((int)(Math.Min(TileMap[x + y * env_SizeX].Fertility, 4d) * 8d) + 16);
                                    else
                                    {
                                        for (int j = 0; j < TileMap[x + y * env_SizeX].LocalParticles.Count; j++)
                                        {
                                            if (Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Plant)
                                            {
                                                GridMapBgParticle[x + y * env_SizeX] = 0x02;
                                                PopulationPlantNext++;
                                            }
                                            else if (Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Animal)
                                            {
                                                GridMapBgParticle[x + y * env_SizeX] = 0x03;
                                                PopulationAnimalNext++;
                                            }
                                            BiomassAmountNext += Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].Satiety;
                                        }
                                    }
                                }
                            }
                        }

                        BiomassAmount = BiomassAmountNext;

                        PopulationPlant = PopulationPlantNext;
                        PopulationAnimal = PopulationAnimalNext;
                        PopulationTotal = PopulationPlantNext + PopulationAnimalNext;

                        if (PopulationAnimal < sim_InitialAnimalCount)
                        {
                            for (int i = PopulationAnimal; i <= sim_InitialAnimalCount; i++)
                            {
                                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
                            }
                        }

                        timeSteps++;
                    }

                    if (SoupStop)
                    {
                        IsRunning = false;
                        break;
                    }
                }
            });
        }

        public void Update(int phase, int quadrant)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = sim_ParallelLimit;

            int RegionSizeWidth = env_SizeX / sim_ThreadCountWidth;
            int RegionSizeHeight = env_SizeY / sim_ThreadCountHeight;
            int ChunkSizeWidth = RegionSizeWidth / 2;
            int ChunkSizeHeight = RegionSizeHeight / 2;

            Parallel.For(0, sim_ThreadCountWidth * sim_ThreadCountHeight, parallelOptions, i =>
            {
                int xStart = RegionSizeWidth * (i % sim_ThreadCountWidth);
                int yStart = RegionSizeHeight * (i / sim_ThreadCountWidth);

                if (quadrant == 1 || quadrant == 3) xStart += ChunkSizeWidth;
                if (quadrant == 2 || quadrant == 3) yStart += ChunkSizeHeight;

                for (int x = xStart; x < xStart + ChunkSizeWidth; x++)
                {
                    for (int y = yStart; y < yStart + ChunkSizeHeight; y++)
                    {
                        for (int j = 0; j < TileMap[x + y * env_SizeX].LocalParticles.Count; j++)
                        {
                            switch (phase)
                            {
                                case 0:
                                    Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].EarlyUpdate();
                                    break;
                                case 1:
                                    Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].MiddleUpdate();
                                    break;
                                case 2:
                                    Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].LateUpdate(i);
                                    break;
                                case 3:
                                    Particles[TileMap[x + y * env_SizeX].LocalParticles[j]].OnStepFinish();
                                    break;
                            }
                        }
                    }
                }
            });
        }
    }
}
