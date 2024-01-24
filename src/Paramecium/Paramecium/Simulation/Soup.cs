using Paramecium.Libs;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace Paramecium.Simulation
{
    public class Soup
    {
        public int SizeX { get; set; } = 512;
        public int SizeY { get; set; } = 256;

        public double WallPerlinNoiseX { get; set; } = 79.14649528369992;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseY { get; set; } = 97.49842312725215;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseZ { get; set; } = 244.70658351525472;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseScale { get; set; } = 0.03d;
        public int WallPerlinNoiseOctave { get; set; } = 4;
        public double WallThickness { get; set; } = 0.01d;

        public double TotalBiomassAmount { get; set; } = 262144d;
        public double BiomassAmount { get; set; }

        public double CellSizeMultiplier { get; set; } = 0.5d;
        public double PlantForkBiomass { get; set; } = 15d;
        public double AnimalForkBiomass { get; set; } = 60d;

        public int PlantBiomassCollectionRange { get; set; } = 3;

        public int InitialAnimalCount { get; set; } = 32;
        public int HatchingTime { get; set; } = 3000;
        //public int AgingBeginsAge { get; set; } = 6000;
        //public int AgingAbilityDeclineBaseTime { get; set; } = 1500;

        public double PlantSizeMultiplier { get; set; } = 3.872983d;

        public int RegionCountWidth { get; set; }
        public int RegionCountHeight { get; set; }
        public int RegionCount { get; set; }
        public int ParallelismLimit { get; set; }

        public int ElapsedTimeStep { get; set; }

        public Grid[] GridMap { get; set; }
        public byte[] GridMapByte { get; set; }

        public byte[] GridMapBg { get; set; }
        public byte[] GridMapBgParticle { get; set; }
        public Particle[] Particles { get; set; }
        public List<Particle>[] ParticlesBuffer { get; set; }
        public List<int> UnassignedParticleIds { get; set; }

        public int PopulationPlant { get; set; }
        public int PopulationAnimal { get; set; }
        public int PopulationTotal { get; set; }

        public SoupState SoupState;
        public bool SoupIsProcessing;

        public Soup() { }
        public Soup(
            int sizeX, int sizeY,
            double wallPerlinNoiseX, double wallPerlinNoiseY, double wallPerlinNoiseZ,
            double wallPerlinNoiseScale, int wallPerlinNoiseOctave, double wallThickness,
            double totalBiomassAmount,
            double cellSizeMultiplier, double plantForkBiomass, double animalForkBiomass, int plantBiomassCollectionRange,
            int initialAnimalCount, int hatchingTime
        )
        {
            SizeX = sizeX; SizeY = sizeY;
            WallPerlinNoiseX = wallPerlinNoiseX; WallPerlinNoiseY = wallPerlinNoiseY; WallPerlinNoiseZ = wallPerlinNoiseZ;
            WallPerlinNoiseScale = wallPerlinNoiseScale; WallPerlinNoiseOctave = wallPerlinNoiseOctave; WallThickness = wallThickness;
            TotalBiomassAmount = totalBiomassAmount;
            CellSizeMultiplier = cellSizeMultiplier; PlantForkBiomass = plantForkBiomass; AnimalForkBiomass = animalForkBiomass; PlantBiomassCollectionRange = plantBiomassCollectionRange;
            InitialAnimalCount = initialAnimalCount; HatchingTime = hatchingTime;

            PlantSizeMultiplier = Math.Sqrt(PlantForkBiomass);

            GridMap = new Grid[SizeX * SizeY];

            GridMapByte = new byte[SizeX * SizeY];
            GridMapBg = new byte[SizeX * SizeY];
            GridMapBgParticle = new byte[SizeX * SizeY];

            RegionCountWidth = (int)Math.Ceiling(SizeX / 32d);
            RegionCountHeight = (int)Math.Ceiling(SizeY / 32d);
            RegionCount = RegionCountWidth * RegionCountHeight;
            ParallelismLimit = 1;

            Perlin perlin = new Perlin();
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    TileType tileType;
                    if (
                        Math.Abs(perlin.OctavePerlin(
                            WallPerlinNoiseX + x * WallPerlinNoiseScale,
                            WallPerlinNoiseY + y * WallPerlinNoiseScale,
                            WallPerlinNoiseZ, 
                            WallPerlinNoiseOctave, 0.5
                        ) - 0.5) <= WallThickness
                    ) tileType = TileType.Wall;
                    else tileType = TileType.None;
                    GridMap[x + y * SizeX] = new Grid(x, y, tileType);
                    GridMapByte[x + y * SizeX] = (byte)tileType;
                    GridMapBg[x + y * SizeX] = (byte)tileType;
                    GridMapBgParticle[x + y * SizeX] = (byte)tileType;
                }
            }

            Particles = new Particle[4];
            ParticlesBuffer = new List<Particle>[RegionCount];
            for (int i = 0; i < ParticlesBuffer.Length; i++)
            {
                ParticlesBuffer[i] = new List<Particle>();
            }
            UnassignedParticleIds = new List<int>();
            for (int i = 4 - 1; i >= 0; i--)
            {
                UnassignedParticleIds.Add(i);
            }

            SoupState = SoupState.Stop;
            SoupIsProcessing = false;
        }

        public void SoupSetup()
        {
            for (int i = 0; i < InitialAnimalCount; i++)
            {
                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
            }

            Random rnd = new Random();

            while (BiomassAmount < TotalBiomassAmount - (AnimalForkBiomass * InitialAnimalCount))
            {
                double NewPlantBiomassAmount = Math.Min(rnd.NextDouble() * (PlantForkBiomass * 0.8d + 0.1d), TotalBiomassAmount - (AnimalForkBiomass * InitialAnimalCount) - BiomassAmount);
                Vector2D NewPlantPosition = (new Vector2D(rnd, 0, 0, SizeX, SizeY));
                Int2D NewPlantGridPosition = Vector2D.ToGridPosition(NewPlantPosition);

                if (GridMap[NewPlantGridPosition.X + NewPlantGridPosition.Y * SizeX].Type == TileType.None)
                {
                    ParticlesBuffer[0].Add(new Particle(NewPlantPosition, NewPlantBiomassAmount));
                    BiomassAmount += NewPlantBiomassAmount;
                }
            }
        }

        public void SoupLoaded()
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i] is not null)
                {
                    if (Particles[i].IsAlive)
                    {
                        Particles[i].OnLoaded();
                    }
                }
            }
        }

        double[] BiomassAmountArray;
        int[] PopulationPlantArray;
        int[] PopulationAnimalArray;
        int[] PopulationTotalArray;

        public void SoupRun()
        {
            BiomassAmountArray = new double[RegionCount];
            PopulationPlantArray = new int[RegionCount];
            PopulationAnimalArray = new int[RegionCount];
            PopulationTotalArray = new int[RegionCount];

            SoupState = SoupState.Pause;

            Task.Run(() =>
            {
                {
                    for (int j = 0; j < ParticlesBuffer[0].Count; j++)
                    {
                        ParticlesBuffer[0][j].OnInitialize();
                        Particles[ParticlesBuffer[0][j].Index] = ParticlesBuffer[0][j];
                    }
                    ParticlesBuffer[0].Clear();

                    Update(4);

                    BiomassAmount = 0;
                    PopulationPlant = 0;
                    PopulationAnimal = 0;
                    PopulationTotal = 0;
                    for (int i = 0; i < BiomassAmountArray.Length; i++)
                    {
                        BiomassAmount += BiomassAmountArray[i];
                    }
                    for (int i = 0; i < PopulationPlantArray.Length; i++)
                    {
                        PopulationPlant += PopulationPlantArray[i];
                    }
                    for (int i = 0; i < PopulationPlantArray.Length; i++)
                    {
                        PopulationAnimal += PopulationAnimalArray[i];
                    }
                    for (int i = 0; i < PopulationPlantArray.Length; i++)
                    {
                        PopulationTotal += PopulationTotalArray[i];
                    }

                    /**

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
                    **/
                }

                while (SoupState != SoupState.Stop)
                {
                    if (SoupState == SoupState.Running || SoupState == SoupState.StepRun)
                    {
                        SoupIsProcessing = true;
                        for (int i = 0; i < ParticlesBuffer.Length; i++)
                        {
                            for (int j = 0; j < ParticlesBuffer[i].Count; j++)
                            {
                                ParticlesBuffer[i][j].OnInitialize();
                                Particles[ParticlesBuffer[i][j].Index] = ParticlesBuffer[i][j];
                            }
                            ParticlesBuffer[i].Clear();
                        }

                        Update(0);
                        Update(1);
                        Update(2);
                        Update(3);
                        Update(4);

                        /**
                        int PopulationPlantNext = 0;
                        int PopulationAnimalNext = 0;

                        double BiomassAmountNext = 0;

                        for (int x = 0; x < env_SizeX; x++)
                        {
                            for (int y = 0; y < env_SizeY; y++)
                            {
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
                        **/

                        BiomassAmount = 0;
                        PopulationPlant = 0;
                        PopulationAnimal = 0;
                        PopulationTotal = 0;
                        for (int i = 0; i < BiomassAmountArray.Length; i++)
                        {
                            BiomassAmount += BiomassAmountArray[i];
                        }
                        for (int i = 0; i < PopulationPlantArray.Length; i++)
                        {
                            PopulationPlant += PopulationPlantArray[i];
                        }
                        for (int i = 0; i < PopulationPlantArray.Length; i++)
                        {
                            PopulationAnimal += PopulationAnimalArray[i];
                        }
                        for (int i = 0; i < PopulationPlantArray.Length; i++)
                        {
                            PopulationTotal += PopulationTotalArray[i];
                        }

                        if (PopulationAnimal < InitialAnimalCount)
                        {
                            for (int i = PopulationAnimal; i <= InitialAnimalCount; i++)
                            {
                                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
                            }
                        }

                        ElapsedTimeStep++;

                        if (SoupState == SoupState.StepRun)
                        {
                            SoupState = SoupState.Pause;
                        }
                    }
                    else SoupIsProcessing = false;
                }
            });
        }

        public void Update(int phase)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = ParallelismLimit;

            //int RegionSizeWidth = SizeX / RegionCountWidth;
            //int ChunkSizeWidth = RegionSizeWidth / 2;
            int RegionSizeWidth = 32;
            int RegionSizeHeight = 32;
            int ChunkSizeWidth = 16;
            int ChunkSizeHeight = 16;

            for (int path = 0; path < 4; path++)
            {
                Parallel.For(0, RegionCount, parallelOptions, i =>
                {
                    int xStart = RegionSizeWidth * (i % RegionCountWidth);
                    int yStart = RegionSizeHeight * (i / RegionCountWidth);

                    if (path == 1 || path == 3) xStart += ChunkSizeWidth;
                    if (path == 2 || path == 3) yStart += ChunkSizeHeight;

                    if (phase == 4 && path == 0)
                    {
                        BiomassAmountArray[i] = 0;
                        PopulationPlantArray[i] = 0;
                        PopulationAnimalArray[i] = 0;
                        PopulationTotalArray[i] = 0;
                    }

                    for (int x = xStart; x < xStart + ChunkSizeWidth; x++)
                    {
                        for (int y = yStart; y < yStart + ChunkSizeHeight; y++)
                        {
                            if (phase == 4)
                            {
                                if (GridMap[x + y * SizeX].Type == TileType.Wall)
                                {
                                    GridMap[x + y * SizeX].Fertility = 0;
                                    GridMapBg[x + y * SizeX] = 0x01;
                                    GridMapBgParticle[x + y * SizeX] = 0x01;
                                }
                                else if (GridMap[x + y * SizeX].Fertility > 0d)
                                {
                                    GridMap[x + y * SizeX].Fertility /= (BiomassAmount / TotalBiomassAmount);
                                    BiomassAmountArray[i] += GridMap[x + y * SizeX].Fertility;
                                    GridMapBg[x + y * SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * SizeX].Fertility * 8d / (TotalBiomassAmount / (SizeX * SizeY))), 0), 32) + 16);
                                    GridMapBgParticle[x + y * SizeX] = GridMapBg[x + y * SizeX];
                                }
                                else
                                {
                                    GridMapBg[x + y * SizeX] = 0x10;
                                    GridMapBgParticle[x + y * SizeX] = 0x10;
                                }
                            }

                            if (GridMap[x + y * SizeX].LocalParticleCount > 0)
                            {
                                for (int j = 0; j < GridMap[x + y * SizeX].LocalParticleCount; j++)
                                {
                                    switch (phase)
                                    {
                                        case 0:
                                            Particles[GridMap[x + y * SizeX].LocalParticles[j]].EarlyUpdate();
                                            break;
                                        case 1:
                                            Particles[GridMap[x + y * SizeX].LocalParticles[j]].MiddleUpdate(i);
                                            break;
                                        case 2:
                                            Particles[GridMap[x + y * SizeX].LocalParticles[j]].LateUpdate(i);
                                            break;
                                        case 3:
                                            Particles[GridMap[x + y * SizeX].LocalParticles[j]].OnStepFinish(i);
                                            break;
                                        case 4:
                                            if (GridMap[x + y * SizeX].Type != TileType.Wall)
                                            {
                                                Particles[GridMap[x + y * SizeX].LocalParticles[j]].Biomass /= (BiomassAmount / TotalBiomassAmount);
                                                if (GridMap[x + y * SizeX].LocalPlantCount > 0)
                                                {
                                                    if (Particles[GridMap[x + y * SizeX].LocalParticles[j]].Type == ParticleType.Plant)
                                                    {
                                                        BiomassAmountArray[i] += Particles[GridMap[x + y * SizeX].LocalParticles[j]].Biomass;
                                                        if (GridMapBgParticle[x + y * SizeX] != 0x03)
                                                        {
                                                            GridMapBgParticle[x + y * SizeX] = 0x02;
                                                        }
                                                        PopulationPlantArray[i]++;
                                                        PopulationTotalArray[i]++;
                                                    }
                                                }
                                                if (GridMap[x + y * SizeX].LocalAnimalCount > 0)
                                                {
                                                    if (Particles[GridMap[x + y * SizeX].LocalParticles[j]].Type == ParticleType.Animal)
                                                    {
                                                        BiomassAmountArray[i] += Particles[GridMap[x + y * SizeX].LocalParticles[j]].Biomass;
                                                        GridMapBgParticle[x + y * SizeX] = 0x03;
                                                        PopulationAnimalArray[i]++;
                                                        PopulationTotalArray[i]++;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        public async void SetSoupState(SoupState soupState)
        {
            switch (soupState)
            {
                case SoupState.Stop:
                    SoupState = SoupState.Stop;
                    while (SoupIsProcessing) { }
                    break;
                case SoupState.Pause:
                    SoupState = SoupState.Pause;
                    while (SoupIsProcessing) { }
                    break;
                case SoupState.ResourceLock:
                    SoupState = SoupState.ResourceLock;
                    while (SoupIsProcessing) { }
                    break;
                case SoupState.Running:
                    SoupState = SoupState.Running;
                    while (!SoupIsProcessing) { }
                    break;
                case SoupState.StepRun:
                    SoupState = SoupState.StepRun;
                    while (!SoupIsProcessing) { }
                    break;
            }
        }

        /**
        public List<ParticleData> GetParticleDatas(int startX, int startY, int endX, int endY)
        {
            SoupState prevSoupState = SoupState;
            SetSoupState(SoupState.ResourceLock);

            List<ParticleData> result = new List<ParticleData>();

            for (int x = Math.Max(startX, 0); x < Math.Min(endX, SizeX - 1); x++)
            {
                for (int y = Math.Max(startY, 0); y < Math.Min(endY, SizeY - 1); y++)
                {
                    if (GridMap[x + y * SizeX].LocalParticleCount > 0)
                    {
                        for (int i = 0; i < GridMap[x + y * SizeX].LocalParticleCount; i++)
                        {
                            result.Add(new ParticleData(Particles[GridMap[x + y * SizeX].LocalParticles[i]]));
                        }
                    }
                }
            }

            SetSoupState(prevSoupState);

            return result;
        }
        **/
    }

    public enum SoupState
    {
        Stop,
        Pause,
        ResourceLock,
        Running,
        StepRun,
    }
}
