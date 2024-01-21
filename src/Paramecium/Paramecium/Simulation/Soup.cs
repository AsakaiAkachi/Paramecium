using Paramecium.Libs;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Paramecium.Simulation
{
    public class Soup
    {
        public int SizeX { get; set; } = 256;

        public int SizeY { get; set; } = 256;

        public double BiomassTotalAmount { get; set; } = 131072d;

        public double PlantRadiusBase { get; set; } = 0.25d;

        public double PlantDivisionBiomassAmount { get; set; } = 16d;

        public double PlantBiomassCollectionSpeed { get; set; } = 0.1d;

        public int PlantBiomassCollectionRange { get; set; } = 6;

        public double AnimalRadiusBase { get; set; } = 0.25d;

        public double AnimalDivisionBiomassAmountBase { get; set; } = 128d;

        public double AnimalAccelerationForceBase { get; set; } = 0.01d;

        public double AnimalBiomassDecreaseRateBase { get; set; } = 0.0025d;

        public double AnimalBiomassDecreaseRateMovingFactor { get; set; } = 0.0075d;
        public double ParticleDrag { get; set; } = 0.1d;


        public double WallPerlinNoiseX { get; set; } = 0d;

        public double WallPerlinNoiseY { get; set; } = 0d;

        public double WallPerlinNoiseZ { get; set; } = 0d;

        public double WallPerlinNoiseScale { get; set; } = 0.03d;

        public int WallPerlinNoiseOctave { get; set; } = 4;

        public double WallThickness { get; set; } = 0.01d;


        public long TimeStep { get; set; } = 0;

        [JsonIgnore]
        public SoupStatus SoupStatus = SoupStatus.Stop;

        [JsonIgnore]
        public Grid[,] Grids = new Grid[256, 256];

        [JsonPropertyName("Grids")]
        public Grid[] Grids_external
        {
            get
            {
                Grid[] result = new Grid[SizeX * SizeY];
                for (int x = 0; x < SizeX; x++)
                {
                    for (int y = 0; y < SizeY; y++)
                    {
                        result[x + y * SizeX] = Grids[x, y];
                    }
                }
                return result;
            }
            set
            {
                Grid[,] result = new Grid[SizeX, SizeY];
                for (int i = 0; i < value.Length; i++)
                {
                    result[i % SizeX, i / SizeX] = value[i];
                }
            }
        }
        public Particle Particles { get; set; }

        [JsonIgnore]
        public List<int>[] ParticleProcessingQueue;

        [JsonIgnore]
        public List<int>[] ParticleBuffer;


        public Soup() { }

        public void Initialize()
        {
            Perlin perlin = new Perlin();
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    TileType TileType;
                    if (
                        Math.Abs(perlin.OctavePerlin(
                            WallPerlinNoiseX + x * WallPerlinNoiseScale,
                            WallPerlinNoiseY + y * WallPerlinNoiseScale,
                            WallPerlinNoiseZ,
                            WallPerlinNoiseOctave, 0.5
                        ) - 0.5) <= WallThickness
                    ) TileType = TileType.Wall;
                    else TileType = TileType.None;
                    Grids[x, y] = new Grid(x, y, TileType);
                }
            }
        }

        double[] BiomassAmountArray;
        int[] PopulationPlantArray;
        int[] PopulationAnimalArray;
        int[] PopulationTotalArray;

        public void SoupRun()
        {
            BiomassAmountArray = new double[sim_ThreadCountWidth];
            PopulationPlantArray = new int[sim_ThreadCountWidth];
            PopulationAnimalArray = new int[sim_ThreadCountWidth];
            PopulationTotalArray = new int[sim_ThreadCountWidth];

            SoupStatus = SoupStatus.Pause;

            Task.Run(() =>
            {
                {
                    for (int j = 0; j < ParticlesBuffer[0].Count; j++)
                    {
                        ParticlesBuffer[0][j].OnInitialize();
                        Particles[ParticlesBuffer[0][j].Index] = ParticlesBuffer[0][j];
                    }
                    ParticlesBuffer[0].Clear();

                    Update(4, 0);
                    Update(4, 1);

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

                while (SoupState != SoupStatus.Stop)
                {
                    if (SoupState == SoupStatus.Running || SoupState == SoupStatus.StepRun)
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

                        Update(0, 0);
                        Update(0, 1);

                        Update(1, 0);
                        Update(1, 1);

                        Update(2, 0);
                        Update(2, 1);

                        Update(3, 0);
                        Update(3, 1);

                        Update(4, 0);
                        Update(4, 1);

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

                        if (PopulationAnimal < sim_InitialAnimalCount)
                        {
                            for (int i = PopulationAnimal; i <= sim_InitialAnimalCount; i++)
                            {
                                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
                            }
                        }

                        timeSteps++;

                        if (SoupState == SoupStatus.StepRun)
                        {
                            SoupState = SoupStatus.Pause;
                        }
                    }
                    else SoupIsProcessing = false;
                }
            });
        }

        public void Update(int phase, int path)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = sim_ParallelLimit;

            int RegionSizeWidth = env_SizeX / sim_ThreadCountWidth;
            int ChunkSizeWidth = RegionSizeWidth / 2;

            Parallel.For(0, sim_ThreadCountWidth, parallelOptions, i =>
            {
                int xStart = RegionSizeWidth * (i % sim_ThreadCountWidth);

                if (path == 1) xStart += ChunkSizeWidth;

                if (phase == 4 && path == 0)
                {
                    BiomassAmountArray[i] = 0;
                    PopulationPlantArray[i] = 0;
                    PopulationAnimalArray[i] = 0;
                    PopulationTotalArray[i] = 0;
                }

                for (int x = xStart; x < xStart + ChunkSizeWidth; x++)
                {
                    for (int y = 0; y < env_SizeY; y++)
                    {
                        if (phase == 4)
                        {
                            BiomassAmountArray[i] += GridMap[x + y * env_SizeX].Fertility;
                            if (GridMap[x + y * env_SizeX].Type != TileType.Wall)
                            {
                                //GridMapBg[x + y * env_SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * env_SizeX].Fertility * 8d / (env_TotalBiomassAmount / (env_SizeX * env_SizeY))), 0), 32) + 16);
                                GridMapBg[x + y * env_SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * env_SizeX].Fertility * 8d), 0), 32) + 16);
                                GridMapBgParticle[x + y * env_SizeX] = GridMapBg[x + y * env_SizeX];
                            }
                            else
                            {
                                GridMap[x + y * env_SizeX].Fertility = 0;
                                GridMapBg[x + y * env_SizeX] = 0x01;
                                GridMapBgParticle[x + y * env_SizeX] = 0x01;
                            }
                        }

                        for (int j = GridMap[x + y * env_SizeX].LocalParticles.Count - 1; j >= 0; j--)
                        {
                            switch (phase)
                            {
                                case 0:
                                    Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].EarlyUpdate();
                                    break;
                                case 1:
                                    Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].MiddleUpdate();
                                    break;
                                case 2:
                                    Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].LateUpdate(i);
                                    break;
                                case 3:
                                    Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].OnStepFinish();
                                    break;
                                case 4:
                                    if (GridMap[x + y * env_SizeX].Type != TileType.Wall)
                                    {
                                        if (Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Plant)
                                        {
                                            BiomassAmountArray[i] += Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].Satiety;
                                            if (GridMapBgParticle[x + y * env_SizeX] != 0x03)
                                            {
                                                GridMapBgParticle[x + y * env_SizeX] = 0x02;
                                            }
                                            PopulationPlantArray[i]++;
                                            PopulationTotalArray[i]++;
                                        }
                                        if (Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].Type == ParticleType.Animal)
                                        {
                                            BiomassAmountArray[i] += Particles[GridMap[x + y * env_SizeX].LocalParticles[j]].Satiety;
                                            GridMapBgParticle[x + y * env_SizeX] = 0x03;
                                            PopulationAnimalArray[i]++;
                                            PopulationTotalArray[i]++;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            });
        }

        public void SetSoupState(SoupStatus soupState)
        {
            switch (soupState)
            {
                case SoupStatus.Stop:
                    SoupState = SoupStatus.Stop;
                    while (SoupIsProcessing) { }
                    break;
                case SoupStatus.Pause:
                    SoupState = SoupStatus.Pause;
                    while (SoupIsProcessing) { }
                    break;
                case SoupStatus.Running:
                    SoupState = SoupStatus.Running;
                    while (!SoupIsProcessing) { }
                    break;
                case SoupStatus.StepRun:
                    SoupState = SoupStatus.StepRun;
                    while (!SoupIsProcessing) { }
                    break;
            }
        }
    }

    public enum SoupStatus
    {
        Stop,
        Pause,
        Running,
        StepRun
    }
}
