using System.Text.Json;

namespace Paramecium.Simulation
{
    public class Soup
    {
        public int Seed { get; set; } = new Random().Next(-2147483648, 2147483647);

        public int SizeX { get; set; } = 512;
        public int SizeY { get; set; } = 256;

        public double WallPerlinNoiseX { get; set; } = 79.14649528369992;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseY { get; set; } = 97.49842312725215;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseZ { get; set; } = 244.70658351525472;// = new Random().NextDouble() * 256d;
        public double WallPerlinNoiseScale { get; set; } = 0.03d;
        public int WallPerlinNoiseOctave { get; set; } = 4;
        public double WallThickness { get; set; } = 0.01d;

        public double TotalBiomassAmount { get; set; } = 262144d;
        public double CurrentBiomassAmount { get; set; }

        public double CellSizeMultiplier { get; set; } = 0.5d;
        public double PlantForkBiomass { get; set; } = 15d;
        public double AnimalForkBiomass { get; set; } = 60d;

        public int PlantBiomassCollectionRange { get; set; } = 3;

        public int InitialAnimalCount { get; set; } = 32;
        public int HatchingTime { get; set; } = 300;
        public double MutationRate { get; set; } = 0.1d;

        public double AnimalColorMutationRange { get; set; } = 0.0875d;
        public double AnimalColorCognateRange { get; set; } = 0.175d;

        public double AnimalElementLosePerStepInPassive { get; set; } = 0.025;
        public double AnimalElementLosePerStepInAccelerating { get; set; } = 0.075d;

        public double PlantSizeMultiplier { get; set; } = 3.872983d;

        public double BiomassAmountMultiplier { get; set; } = 1.0d;

        public int RegionCountWidth { get; set; }
        public int RegionCountHeight { get; set; }
        public int RegionCount { get; set; }
        public int ParallelismLimit { get; set; }

        public long ElapsedTimeStep { get; set; }
        public long TotalBornCount { get; set; }
        public long TotalDieCount { get; set; }
        public object TotalDieCountLockObject = new object();

        public int LatestGeneration { get; set; } = 1;
        public object LatestGenerationLockObject = new object();

        public Grid[] GridMap { get; set; }
        public byte[] GridMapByte { get; set; }

        public byte[] GridMapBg { get; set; }
        public byte[] GridMapBgParticle { get; set; }

        public Plant[] Plants { get; set; }
        public List<Plant>[] PlantBuffer { get; set; }
        public List<int> PlantUnassignedIndexes { get; set; }
        public object PlantUnassignedIndexesLockObject = new object();

        public Animal[] Animals { get; set; }
        public List<Animal>[] AnimalBuffer { get; set; }
        public List<int> AnimalUnassignedIndexes { get; set; }
        public object AnimalUnassignedIndexesLockObject = new object();

        public int PopulationPlant { get; set; }
        public int PopulationAnimal { get; set; }
        public int PopulationTotal { get; set; }

        public SoupState SoupState;
        public object SoupStateLockObject = new object();

        public bool AutoSave { get; set; } = false;
        public int AutoSaveInterval { get; set; } = -1;

        public Soup() { }
        public Soup(
            int seed,
            int sizeX, int sizeY,
            bool EnableWall, double wallPerlinNoiseX, double wallPerlinNoiseY, double wallPerlinNoiseZ,
            double wallPerlinNoiseScale, int wallPerlinNoiseOctave, double wallThickness,
            double totalBiomassAmount,
            double cellSizeMultiplier, double plantForkBiomass, double animalForkBiomass, int plantBiomassCollectionRange,
            int initialAnimalCount, int hatchingTime, 
            double mutationRate, double animalColorMutationRange, double animalColorCognateRange,
            double animalElementLosePerStepInPassive, double animalElementLosePerStepInAccelerating
        )
        {
            Seed = seed;
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

            MutationRate = mutationRate;
            AnimalColorMutationRange = animalColorMutationRange;
            AnimalColorCognateRange = animalColorCognateRange;

            AnimalElementLosePerStepInPassive = animalElementLosePerStepInPassive;
            AnimalElementLosePerStepInAccelerating = animalElementLosePerStepInAccelerating;

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
                    if (EnableWall)
                    {
                        if (
                            Math.Abs(perlin.OctavePerlin(
                                WallPerlinNoiseX + x * WallPerlinNoiseScale,
                                WallPerlinNoiseY + y * WallPerlinNoiseScale,
                                WallPerlinNoiseZ,
                                WallPerlinNoiseOctave, 0.5
                            ) - 0.5) <= WallThickness
                        ) tileType = TileType.Wall;
                        else tileType = TileType.None;
                    }
                    else tileType = TileType.None;
                    GridMap[x + y * SizeX] = new Grid(x, y, tileType);
                    GridMapByte[x + y * SizeX] = (byte)tileType;
                    GridMapBg[x + y * SizeX] = (byte)tileType;
                    GridMapBgParticle[x + y * SizeX] = (byte)tileType;
                }
            }

            /*
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
            */
            Plants = new Plant[4];
            PlantBuffer = new List<Plant>[RegionCount];
            for (int i = 0; i < PlantBuffer.Length; i++)
            {
                PlantBuffer[i] = new List<Plant>();
            }
            PlantUnassignedIndexes = new List<int>();
            for (int i = 4 - 1; i >= 0; i--)
            {
                PlantUnassignedIndexes.Add(i);
            }

            Animals = new Animal[4];
            AnimalBuffer = new List<Animal>[RegionCount];
            for (int i = 0; i < PlantBuffer.Length; i++)
            {
                AnimalBuffer[i] = new List<Animal>();
            }
            AnimalUnassignedIndexes = new List<int>();
            for (int i = 4 - 1; i >= 0; i--)
            {
                AnimalUnassignedIndexes.Add(i);
            }

            SoupState = SoupState.Stop;
        }

        public void SoupSetup()
        {
            Random random = new Random(Seed);

            while (CurrentBiomassAmount < TotalBiomassAmount - (AnimalForkBiomass * InitialAnimalCount))
            {
                double NewPlantBiomassAmount = Math.Min(random.NextDouble() * (PlantForkBiomass * 0.8d + 0.1d), TotalBiomassAmount - (AnimalForkBiomass * InitialAnimalCount) - CurrentBiomassAmount);
                Vector2D NewPlantPosition = (new Vector2D(random, 0, 0, SizeX, SizeY));
                Int2D NewPlantGridPosition = Vector2D.ToIntegerizedPosition(NewPlantPosition);

                if (GridMap[NewPlantGridPosition.X + NewPlantGridPosition.Y * SizeX].Type == TileType.None)
                {
                    PlantBuffer[0].Add(new Plant(random, NewPlantPosition, Vector2D.Zero, NewPlantBiomassAmount));
                    CurrentBiomassAmount += NewPlantBiomassAmount;
                }
            }

            for (int i = 0; i < InitialAnimalCount; i++)
            {
                AnimalBuffer[0].Add(new Animal(random, new Vector2D(random, 0, 0, SizeX, SizeY), Vector2D.Zero, random.NextDouble() * 360d, AnimalForkBiomass));
            }
        }

        public void SoupLoaded()
        {
            for (int i = 0; i < Plants.Length; i++)
            {
                if (Plants[i] is not null)
                {
                    if (Plants[i].IsValid)
                    {
                        Plants[i].OnLoaded();
                    }
                }
            }
            for (int i = 0; i < Animals.Length; i++)
            {
                if (Animals[i] is not null)
                {
                    if (Animals[i].IsValid)
                    {
                        Animals[i].OnLoaded();
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
                    for (int j = 0; j < PlantBuffer[0].Count; j++)
                    {
                        PlantBuffer[0][j].Age = 0;
                        PlantBuffer[0][j].OnInitialize();
                        Plants[PlantBuffer[0][j].Index] = PlantBuffer[0][j];
                    }
                    PlantBuffer[0].Clear();
                    for (int j = 0; j < AnimalBuffer[0].Count; j++)
                    {
                        //AnimalBuffer[0][j].Age = 0;
                        AnimalBuffer[0][j].OnInitialize();
                        Animals[AnimalBuffer[0][j].Index] = AnimalBuffer[0][j];
                        TotalBornCount++;
                    }
                    AnimalBuffer[0].Clear();

                    Update(4);

                    CurrentBiomassAmount = 0;
                    PopulationPlant = 0;
                    PopulationAnimal = 0;
                    PopulationTotal = 0;
                    for (int i = 0; i < BiomassAmountArray.Length; i++)
                    {
                        CurrentBiomassAmount += BiomassAmountArray[i];
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
                    lock (SoupStateLockObject)
                    {
                        if (SoupState == SoupState.Running || SoupState == SoupState.StepRun)
                        {
                            for (int i = 0; i < PlantBuffer.Length; i++)
                            {
                                for (int j = 0; j < PlantBuffer[i].Count; j++)
                                {
                                    PlantBuffer[i][j].OnInitialize();
                                    Plants[PlantBuffer[i][j].Index] = PlantBuffer[i][j];
                                }
                                PlantBuffer[i].Clear();
                            }
                            for (int i = 0; i < AnimalBuffer.Length; i++)
                            {
                                for (int j = 0; j < AnimalBuffer[i].Count; j++)
                                {
                                    AnimalBuffer[i][j].OnInitialize();
                                    Animals[AnimalBuffer[i][j].Index] = AnimalBuffer[i][j];
                                    TotalBornCount++;
                                }
                                AnimalBuffer[i].Clear();
                            }

                            BiomassAmountMultiplier = Math.Min(Math.Max(1d + (TotalBiomassAmount / CurrentBiomassAmount - 1d) * 10d, 0d), 2d);

                            Update(0);
                            Update(1);
                            Update(2);
                            Update(3);
                            Update(4);

                            CurrentBiomassAmount = 0;
                            PopulationPlant = 0;
                            PopulationAnimal = 0;
                            PopulationTotal = 0;
                            for (int i = 0; i < BiomassAmountArray.Length; i++)
                            {
                                CurrentBiomassAmount += BiomassAmountArray[i];
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

                            ElapsedTimeStep++;

                            if (AutoSave && ElapsedTimeStep % AutoSaveInterval == 0 && ElapsedTimeStep != 0)
                            {
                                EventLog.PushEventLog($"Saving soup...");

                                string jsonString = JsonSerializer.Serialize(g_Soup, new JsonSerializerOptions() { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals });

                                StreamWriter sw1 = new StreamWriter($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves\{Path.GetFileNameWithoutExtension(g_FormMain.FilePath)}_autosave{ElapsedTimeStep}.soup", false);
                                sw1.Write(jsonString);
                                sw1.Dispose();

                                StreamWriter sw2 = new StreamWriter($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\autosaves\_LatestAutosave.soup", false);
                                sw2.Write(jsonString);
                                sw2.Dispose();

                                jsonString = string.Empty;

                                GC.Collect();

                                EventLog.PushEventLog($"Soup has been saved.");
                            }

                            if (SoupState == SoupState.StepRun)
                            {
                                SoupState = SoupState.Pause;
                            }
                        }
                    }
                }
            });
        }

        public void Update(int phase)
        {
            try
            {
                ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = ParallelismLimit };

                //int RegionSizeWidth = SizeX / RegionCountWidth;
                //int ChunkSizeWidth = RegionSizeWidth / 2;
                int RegionSizeWidth = 32;
                int RegionSizeHeight = 32;
                int ChunkSizeWidth = 16;
                int ChunkSizeHeight = 16;

                for (int path = 0; path < 4; path++)
                {
                    Random random = new Random((int)((Seed + ElapsedTimeStep * 4 + phase) % 2147483647));
                    int pathRoute = random.Next(0, 3 + 1);

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

                        if (pathRoute == 0)
                        {
                            for (int x = xStart; x < xStart + ChunkSizeWidth; x++)
                            {
                                for (int y = yStart; y < yStart + ChunkSizeHeight; y++)
                                {
                                    GridUpdate(i, phase, x, y);
                                }
                            }
                        }
                        else if (pathRoute == 1)
                        {
                            for (int x = xStart; x < xStart + ChunkSizeWidth; x++)
                            {
                                for (int y = yStart + ChunkSizeHeight - 1; y >= yStart; y--)
                                {
                                    GridUpdate(i, phase, x, y);
                                }
                            }
                        }
                        else if (pathRoute == 2)
                        {
                            for (int x = xStart + ChunkSizeWidth - 1; x >= xStart; x--)
                            {
                                for (int y = yStart; y < yStart + ChunkSizeHeight; y++)
                                {
                                    GridUpdate(i, phase, x, y);
                                }
                            }
                        }
                        else if (pathRoute == 3)
                        {
                            for (int x = xStart + ChunkSizeWidth - 1; x >= xStart; x--)
                            {
                                for (int y = yStart + ChunkSizeHeight - 1; y >= yStart; y--)
                                {
                                    GridUpdate(i, phase, x, y);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex) { ConsoleLog(LogLevel.Failure, ex.ToString()); }
        }

        public void GridUpdate(int thread, int phase, int x, int y)
        {
            try
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
                        BiomassAmountArray[thread] += GridMap[x + y * SizeX].Fertility;
                        GridMapBg[x + y * SizeX] = (byte)((int)Math.Min(Math.Max(Math.Round(GridMap[x + y * SizeX].Fertility / (TotalBiomassAmount / (SizeX * SizeY)) * 32d), 0), 32) + 16);
                        GridMapBgParticle[x + y * SizeX] = GridMapBg[x + y * SizeX];
                    }
                    else
                    {
                        GridMapBg[x + y * SizeX] = 0x10;
                        GridMapBgParticle[x + y * SizeX] = 0x10;
                    }

                    GridMap[x + y * SizeX].LocalPlantCount = GridMap[x + y * SizeX].LocalPlants.Count;
                    GridMap[x + y * SizeX].LocalAnimalCount = GridMap[x + y * SizeX].LocalAnimals.Count;
                }

                if (GridMap[x + y * SizeX].LocalPlantCount > 0)
                {
                    for (int j = GridMap[x + y * SizeX].LocalPlantCount - 1; j >= 0; j--)
                    {
                        Plant Target = Plants[GridMap[x + y * SizeX].LocalPlants[j]];

                        switch (phase)
                        {
                            case 0:
                                Target.EarlyUpdate();
                                break;
                            case 1:
                                Target.MiddleUpdate();
                                break;
                            case 2:
                                Target.LateUpdate(thread);
                                break;
                            case 3:
                                Target.OnStepFinalize();
                                break;
                            case 4:
                                BiomassAmountArray[thread] += Target.Element;
                                //PopulationTotalArray[thread]++;
                                //PopulationPlantArray[thread]++;
                                break;
                        }
                    }

                    if (phase == 4)
                    {
                        GridMapBgParticle[x + y * SizeX] = 0x02;

                        PopulationTotalArray[thread] += GridMap[x + y * SizeX].LocalPlantCount;
                        PopulationPlantArray[thread] += GridMap[x + y * SizeX].LocalPlantCount;
                    }
                }

                if (GridMap[x + y * SizeX].LocalAnimalCount > 0)
                {
                    for (int j = GridMap[x + y * SizeX].LocalAnimalCount - 1; j >= 0; j--)
                    {
                        Animal Target = Animals[GridMap[x + y * SizeX].LocalAnimals[j]];

                        switch (phase)
                        {
                            case 0:
                                Target.EarlyUpdate();
                                break;
                            case 1:
                                Target.MiddleUpdate();
                                break;
                            case 2:
                                Target.LateUpdate(thread);
                                break;
                            case 3:
                                Target.OnStepFinalize();
                                break;
                            case 4:
                                BiomassAmountArray[thread] += Target.Element;
                                break;
                        }
                    }

                    if (phase == 4)
                    {
                        GridMapBgParticle[x + y * SizeX] = 0x03;

                        PopulationTotalArray[thread] += GridMap[x + y * SizeX].LocalAnimalCount;
                        PopulationAnimalArray[thread] += GridMap[x + y * SizeX].LocalAnimalCount;
                    }
                }
            }
            catch (Exception ex) { ConsoleLog(LogLevel.Failure, ex.ToString()); }
        }

        public void SetSoupState(SoupState soupState)
        {
            switch (soupState)
            {
                case SoupState.Stop:
                    lock(SoupStateLockObject) SoupState = SoupState.Stop;
                    break;
                case SoupState.Pause:
                    lock (SoupStateLockObject) SoupState = SoupState.Pause;
                    break;
                case SoupState.ResourceLock:
                    lock (SoupStateLockObject) SoupState = SoupState.ResourceLock;
                    break;
                case SoupState.Running:
                    lock (SoupStateLockObject) SoupState = SoupState.Running;
                    break;
                case SoupState.StepRun:
                    lock (SoupStateLockObject) SoupState = SoupState.StepRun;
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
