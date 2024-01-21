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
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public double WallPerlinNoiseX { get; set; }
        public double WallPerlinNoiseY { get; set; }
        public double WallPerlinNoiseZ { get; set; }
        public double WallPerlinNoiseScale { get; set; }
        public int WallPerlinNoiseOctave { get; set; }
        public double WallThickness { get; set; }

        public double TotalBiomassAmount { get; set; }
        public double BiomassAmount;

        public int sim_InitialAnimalCount { get; set; }

        public int sim_ThreadCountWidth { get; set; }
        public int sim_ParallelLimit { get; set; }

        public int timeSteps { get; set; }

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
        public Soup(int env_SizeX, int env_SizeY, double env_WallPerlinNoiseX, double env_WallPerlinNoiseY, double env_WallPerlinNoiseZ, bool WallPerlinNoisePositionRandomize, double env_WallPerlinNoiseScale, int env_WallPerlinNoiseOctave, double env_WallThickness, double env_TotalBiomassAmount, int sim_InitialAnimalCount)
        {
            this.SizeX = env_SizeX;
            this.SizeY = env_SizeY;
            GridMap = new Grid[env_SizeX * env_SizeY];

            GridMapByte = new byte[env_SizeX * env_SizeY];
            GridMapBg = new byte[env_SizeX * env_SizeY];
            GridMapBgParticle = new byte[env_SizeX * env_SizeY];

            Random rnd = new Random();

            if (WallPerlinNoisePositionRandomize) this.WallPerlinNoiseX = rnd.NextDouble() * 256d;
            else this.WallPerlinNoiseX = env_WallPerlinNoiseX;
            if (WallPerlinNoisePositionRandomize) this.WallPerlinNoiseY = rnd.NextDouble() * 256d;
            else this.WallPerlinNoiseY = env_WallPerlinNoiseY;
            if (WallPerlinNoisePositionRandomize) this.WallPerlinNoiseZ = rnd.NextDouble() * 256d;
            else this.WallPerlinNoiseZ = env_WallPerlinNoiseZ;
            this.WallPerlinNoiseScale = env_WallPerlinNoiseScale;
            this.WallPerlinNoiseOctave = env_WallPerlinNoiseOctave;
            this.WallThickness = env_WallThickness;
            this.TotalBiomassAmount = env_TotalBiomassAmount;

            this.sim_InitialAnimalCount = sim_InitialAnimalCount;

            sim_ThreadCountWidth = 16;
            sim_ParallelLimit = 1;

            Perlin perlin = new Perlin();
            for (int x = 0; x < env_SizeX; x++)
            {
                for (int y = 0; y < env_SizeY; y++)
                {
                    TileType tileType;
                    if (
                        Math.Abs(perlin.OctavePerlin(
                            this.WallPerlinNoiseX + x * env_WallPerlinNoiseScale,
                            this.WallPerlinNoiseY + y * env_WallPerlinNoiseScale,
                            this.WallPerlinNoiseZ, 
                            env_WallPerlinNoiseOctave, 0.5
                        ) - 0.5) <= env_WallThickness
                    ) tileType = TileType.Wall;
                    else tileType = TileType.None;
                    GridMap[x + y * env_SizeX] = new Grid(x, y, tileType);
                    GridMapByte[x + y * env_SizeX] = (byte)tileType;
                    GridMapBg[x + y * env_SizeX] = (byte)tileType;
                    GridMapBgParticle[x + y * env_SizeX] = (byte)tileType;
                }
            }

            Particles = new Particle[4];
            ParticlesBuffer = new List<Particle>[sim_ThreadCountWidth];
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
            for (int i = 0; i < sim_InitialAnimalCount; i++)
            {
                ParticlesBuffer[0].Add(new Particle(ParticleType.Animal));
            }

            Random rnd = new Random();

            double GeneratedBiomass = 0;

            while(GeneratedBiomass < TotalBiomassAmount)
            {
                double NewPlantBiomassAmount = Math.Min(rnd.NextDouble() * 15.999, TotalBiomassAmount - GeneratedBiomass);
                Vector2D NewPlantPosition = (new Vector2D(rnd, 0, 0, SizeX, SizeY));
                Int2D NewPlantGridPosition = Vector2D.ToGridPosition(NewPlantPosition);

                if (GridMap[NewPlantGridPosition.X + NewPlantGridPosition.Y * SizeX].Type == TileType.None)
                {
                    ParticlesBuffer[0].Add(new Particle(NewPlantPosition, NewPlantBiomassAmount));
                    GeneratedBiomass += NewPlantBiomassAmount;
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

                        if (SoupState == SoupState.StepRun)
                        {
                            SoupState = SoupState.Pause;
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

            int RegionSizeWidth = SizeX / sim_ThreadCountWidth;
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
                    for (int y = 0; y < SizeY; y++)
                    {
                        if (phase == 4)
                        {
                            BiomassAmountArray[i] += GridMap[x + y * SizeX].Fertility;
                            if (GridMap[x + y * SizeX].Type != TileType.Wall)
                            {
                                //GridMapBg[x + y * env_SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * env_SizeX].Fertility * 8d / (env_TotalBiomassAmount / (env_SizeX * env_SizeY))), 0), 32) + 16);
                                GridMapBg[x + y * SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * SizeX].Fertility * 8d), 0), 32) + 16);
                                GridMapBgParticle[x + y * SizeX] = GridMapBg[x + y * SizeX];
                            }
                            else
                            {
                                GridMap[x + y * SizeX].Fertility = 0;
                                GridMapBg[x + y * SizeX] = 0x01;
                                GridMapBgParticle[x + y * SizeX] = 0x01;
                            }
                        }

                        for (int j = GridMap[x + y * SizeX].LocalParticles.Count - 1; j >= 0; j--)
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
                                    Particles[GridMap[x + y * SizeX].LocalParticles[j]].OnStepFinish();
                                    break;
                                case 4:
                                    if (GridMap[x + y * SizeX].Type != TileType.Wall)
                                    {
                                        if (Particles[GridMap[x + y * SizeX].LocalParticles[j]].Type == ParticleType.Plant)
                                        {
                                            BiomassAmountArray[i] += Particles[GridMap[x + y * SizeX].LocalParticles[j]].Satiety;
                                            if (GridMapBgParticle[x + y * SizeX] != 0x03)
                                            {
                                                GridMapBgParticle[x + y * SizeX] = 0x02;
                                            }
                                            PopulationPlantArray[i]++;
                                            PopulationTotalArray[i]++;
                                        }
                                        if (Particles[GridMap[x + y * SizeX].LocalParticles[j]].Type == ParticleType.Animal)
                                        {
                                            BiomassAmountArray[i] += Particles[GridMap[x + y * SizeX].LocalParticles[j]].Satiety;
                                            GridMapBgParticle[x + y * SizeX] = 0x03;
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

        public void SetSoupState(SoupState soupState)
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
    }

    public enum SoupState
    {
        Stop,
        Pause,
        Running,
        StepRun
    }
}
