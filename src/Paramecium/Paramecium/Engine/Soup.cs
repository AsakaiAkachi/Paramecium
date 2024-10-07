using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Paramecium.Engine
{
    public class Soup
    {
        ///////////////////////////////
        // Soup Environment Settings //
        ///////////////////////////////

        public SoupSettings Settings { get; set; } = new SoupSettings();



        ///////////////////////////////
        // Soup Simulation Variables //
        ///////////////////////////////

        // Basic Information
        public long ElapsedTimeSteps { get; set; } = 0;
        public long TotalBornCount { get; set; } = 0;
        public long TotalDieCount { get; set; } = 0;
        public long LatestGeneration { get; set; } = 0;

        // Soup File Information
        public string FilePath = $@"{g_SoupDefaultFilePath}\{g_SoupDefaultFileName}";
        public bool Modified { get; set; } = true;
        public bool AutosaveEnabled { get; set; } = false;
        public long AutosaveInterval { get; set; } = 100000;

        // Population Information
        public int PopulationPlant { get; set; }
        public int PopulationAnimal { get; set; }

        // Soup State Management
        public bool Initialized { get; set; } = false;
        public Task SoupMainThread = Task.CompletedTask;
        public SoupState SoupState { get; private set; } = SoupState.Pause;
        public object SoupStateLockObject = new object();

        // Multithreading
        public int ThreadCount { get; private set; } = 1;
        public object ThreadCountLockObject = new object();
        public double TimeStepsPerSecond { get; set; }

        // Random Number
        public int CurrentSeed { get; set; }

        // Element
        public double CurrentTotalElementAmount { get; set; }
        public double ElementAmountMultiplier { get; set; }

        // Tile
        public Tile[] Tiles { get; set; } = new Tile[0];

        // Plant
        public List<Plant> Plants { get; set; } = new List<Plant>();
        public List<int> PlantUnusedIndexes { get; set; } = new List<int>();

        // Animal
        public List<Animal> Animals { get; set; } = new List<Animal>();
        public List<int> AnimalUnusedIndexes { get; set; } = new List<int>();



        public Soup() { }

        public Soup(SoupSettings settings)
        {
            Settings = settings;
        }

        public void InitializeSoup()
        {
            if (!Initialized)
            {
                Perlin perlin = new Perlin();

                int soupSizeX = Settings.SizeX;
                int soupSizeY = Settings.SizeY;

                Tiles = new Tile[soupSizeX * soupSizeY];
                int DefaultTypeTileCount = soupSizeX * soupSizeY;

                for (int x = 0; x < soupSizeX; x++)
                {
                    for (int y = 0; y < soupSizeY; y++)
                    {
                        Tiles[y * soupSizeX + x] = new Tile(x, y);
                    }
                }

                if (Settings.WallEnabled)
                {
                    for (int x = 0; x < soupSizeX; x++)
                    {
                        for (int y = 0; y < soupSizeY; y++)
                        {
                            if (Math.Abs(perlin.OctavePerlin(Settings.WallNoiseX + x * Settings.WallNoiseScale, Settings.WallNoiseY + y * Settings.WallNoiseScale, Settings.WallNoiseZ, Settings.WallNoiseOctave, 0.5d) - 0.5d) < Settings.WallThickness)
                            {
                                Tiles[y * soupSizeX + x].Type = TileType.Wall;
                            }
                        }
                    }

                    bool[] continuousRegionFlag = new bool[soupSizeX * soupSizeY];
                    int continuousRegionArea = 0;

                    for (int i = 0; i < 16; i++)
                    {
                        int exploredTileCount = 0;

                        List<int> exploreTiles = new List<int>();
                        bool[] exploredTiles = new bool[soupSizeX * soupSizeY];

                        exploreTiles.Add(new Random().Next(0, soupSizeX * soupSizeY));
                        while (Tiles[exploreTiles[0]].Type == TileType.Wall) exploreTiles[0] = new Random().Next(0, soupSizeX * soupSizeY);

                        while (exploreTiles.Count > 0)
                        {
                            List<int> nextStepExploreTiles = new List<int>();

                            for (int j = 0; j < exploreTiles.Count; j++)
                            {
                                int x = exploreTiles[j] % soupSizeX;
                                int y = exploreTiles[j] / soupSizeX;

                                for (int ix = -1; ix <= 1; ix++)
                                {
                                    for (int iy = -1; iy <= 1; iy++)
                                    {
                                        if (int.Abs(ix + iy) == 1)
                                        {
                                            if (x + ix >= 0 && x + ix < soupSizeX && y + iy >= 0 && y + iy < soupSizeY)
                                            {
                                                if (Tiles[(y + iy) * soupSizeX + (x + ix)].Type == TileType.Default && !exploredTiles[(y + iy) * soupSizeX + (x + ix)])
                                                {
                                                    exploredTiles[(y + iy) * soupSizeX + (x + ix)] = true;
                                                    nextStepExploreTiles.Add((y + iy) * soupSizeX + (x + ix));
                                                    exploredTileCount++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            exploreTiles = nextStepExploreTiles;
                        }

                        if (exploredTileCount > continuousRegionArea)
                        {
                            continuousRegionFlag = exploredTiles;
                            continuousRegionArea = exploredTileCount;
                        }

                        if (exploredTileCount > soupSizeX * soupSizeY / 2) break;
                    }

                    for (int x = 0; x < soupSizeX; x++)
                    {
                        for (int y = 0; y < soupSizeY; y++)
                        {
                            Tile targetTile = Tiles[y * soupSizeX + x];

                            if (!continuousRegionFlag[y * soupSizeX + x])
                            {
                                targetTile.Type = TileType.Wall;
                            }
                        }
                    }

                    DefaultTypeTileCount = continuousRegionArea;
                }

                for (int x = 0; x < soupSizeX; x++)
                {
                    for (int y = 0; y < soupSizeY; y++)
                    {
                        Tile targetTile = Tiles[y * soupSizeX + x];

                        if (targetTile.Type == TileType.Default)
                        {
                            targetTile.Element = (Settings.TotalElementAmount - (Settings.InitialPlantElementAmount * Settings.InitialPlantPopulation + Settings.InitialAnimalElementAmount * Settings.InitialAnimalPopulation)) / DefaultTypeTileCount;
                        }
                    }
                }

                Random random = new Random(Settings.InitialSeed);

                CurrentSeed = random.Next(int.MinValue, int.MaxValue);

                Initialized = true;

                for (int i = 0; i < Settings.InitialPlantPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Plants.Add(new Plant(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), Settings.InitialPlantElementAmount));
                    Plants[Plants.Count - 1].Initialize(Plants.Count - 1, random);

                    PopulationPlant++;
                }

                for (int i = 0; i < Settings.InitialAnimalPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Animals.Add(new Animal(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), random.NextDouble(), Settings.InitialAnimalElementAmount, random));
                    Animals[Animals.Count - 1].Initialize(Animals.Count - 1, random);

                    PopulationAnimal++;
                }
            }
            else throw new InvalidOperationException("This soup has already been initialized.");
        }

        public void StartSoupThread()
        {
            if (SoupMainThread.IsCompleted && Initialized)
            {
                SoupMainThread = Task.Run(MainLoop);
            }
            else if (!Initialized) throw new InvalidOperationException("Soup is not initialized.");
            else if (!SoupMainThread.IsCompleted) throw new InvalidOperationException("Soup Thread have already been started.");
            else throw new InvalidOperationException();
        }

        public void MainLoop()
        {
            while (SoupState != SoupState.Stop)
            {
                lock (SoupStateLockObject)
                {
                    if (SoupState == SoupState.Running || SoupState == SoupState.StepRun)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        ParallelOptions parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = ThreadCount
                        };

                        int soupSizeX = Settings.SizeX;
                        int soupSizeY = Settings.SizeY;

                        CurrentTotalElementAmount = 0d;
                        for (int x = 0; x < soupSizeX; x++)
                        {
                            for (int y = 0; y < soupSizeY; y++)
                            {
                                Tile targetTile = Tiles[y * soupSizeX + x];
                                CurrentTotalElementAmount += targetTile.Element;
                            }
                        }
                        for (int i = 0; i < Plants.Count; i++)
                        {
                            if (Plants[i].Exist)
                            {
                                CurrentTotalElementAmount += Plants[i].Element;
                            }
                        }
                        for (int i = 0; i < Animals.Count; i++)
                        {
                            if (Animals[i].Exist)
                            {
                                CurrentTotalElementAmount += Animals[i].Element;
                            }
                        }
                        ElementAmountMultiplier = Settings.TotalElementAmount / CurrentTotalElementAmount;

                        double[] elementFlowAmount = new double[soupSizeX * soupSizeY];
                        double[] pheromoneRedFlowAmount = new double[soupSizeX * soupSizeY];
                        double[] pheromoneGreenFlowAmount = new double[soupSizeX * soupSizeY];
                        double[] pheromoneBlueFlowAmount = new double[soupSizeX * soupSizeY];
                        //for (int x = 0; x < soupSizeX; x++)
                        Parallel.For(0, soupSizeX, parallelOptions, x =>
                        {
                            for (int y = 0; y < soupSizeY; y++)
                            {
                                Tile targetTile1 = Tiles[y * soupSizeX + x];

                                if (targetTile1.Type == TileType.Default)
                                {
                                    for (int ix = -1; ix <= 1; ix++)
                                    {
                                        for (int iy = -1; iy <= 1; iy++)
                                        {
                                            if (int.Abs(ix) + int.Abs(iy) == 1)
                                            {
                                                if (x + ix >= 0 && x + ix < soupSizeX && y + iy >= 0 && y + iy < soupSizeY)
                                                {
                                                    Tile targetTile2 = Tiles[(y + iy) * soupSizeX + (x + ix)];

                                                    if (targetTile2.Type == TileType.Default)
                                                    {
                                                        elementFlowAmount[y * soupSizeX + x] -= (targetTile1.Element - targetTile2.Element) * Settings.ElementFlowRate;
                                                        if (elementFlowAmount[y * soupSizeX + x] > 0) elementFlowAmount[y * soupSizeX + x] *= ElementAmountMultiplier;
                                                        pheromoneRedFlowAmount[y * soupSizeX + x] -= (targetTile1.PheromoneRed - targetTile2.PheromoneRed) * Settings.PheromoneFlowRate;
                                                        pheromoneGreenFlowAmount[y * soupSizeX + x] -= (targetTile1.PheromoneGreen - targetTile2.PheromoneGreen) * Settings.PheromoneFlowRate;
                                                        pheromoneBlueFlowAmount[y * soupSizeX + x] -= (targetTile1.PheromoneBlue - targetTile2.PheromoneBlue) * Settings.PheromoneFlowRate;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (targetTile1.Type == TileType.Wall)
                                {
                                    targetTile1.Element = 0;
                                    targetTile1.PheromoneRed = 0;
                                    targetTile1.PheromoneGreen = 0;
                                    targetTile1.PheromoneBlue = 0;
                                }
                            }
                        });
                        Parallel.For(0, soupSizeX, parallelOptions, x =>
                        {
                            for (int y = 0; y < soupSizeY; y++)
                            {
                                Tile targetTile = Tiles[y * soupSizeX + x];
                                targetTile.Element += elementFlowAmount[y * soupSizeX + x];
                                targetTile.PheromoneRed += pheromoneRedFlowAmount[y * soupSizeX + x];
                                targetTile.PheromoneGreen += pheromoneGreenFlowAmount[y * soupSizeX + x];
                                targetTile.PheromoneBlue += pheromoneBlueFlowAmount[y * soupSizeX + x];
                                if (targetTile.Element < 0) targetTile.Element = 0;

                                targetTile.PheromoneRed *= 1d - Settings.PheromoneDecayRate;
                                targetTile.PheromoneGreen *= 1d - Settings.PheromoneDecayRate;
                                targetTile.PheromoneBlue *= 1d - Settings.PheromoneDecayRate;
                            }
                        });

                        Parallel.For(0, Plants.Count, parallelOptions, i => { if (Plants[i].Exist) Plants[i].ApplyDrag(); });
                        Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].ApplyDrag(); });

                        for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].CollectElement();

                        Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].UpdateNeuralNet(); });
                        for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) Animals[i].ApplyNeuralNetOutput();

                        Parallel.For(0, Plants.Count, parallelOptions, i => { if (Plants[i].Exist) Plants[i].UpdateCollision(); });
                        Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].UpdateCollision(); });

                        for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].UpdatePosition();
                        for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) Animals[i].UpdatePosition();

                        Random random = new Random(CurrentSeed);

                        List<Plant> PlantOffspringBuffer = new List<Plant>();
                        for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist)
                            {
                                List<Plant>? PlantOffspringLocalBuffer = Plants[i].CreateOffspring(random);

                                if (PlantOffspringLocalBuffer is not null) for (int j = 0; j < PlantOffspringLocalBuffer.Count; j++) PlantOffspringBuffer.Add(PlantOffspringLocalBuffer[j]);
                            }

                        for (int i = 0; i < PlantOffspringBuffer.Count; i++)
                        {
                            if (PlantUnusedIndexes.Count > 0)
                            {
                                Plants[PlantUnusedIndexes[PlantUnusedIndexes.Count - 1]] = PlantOffspringBuffer[i];
                                Plants[PlantUnusedIndexes[PlantUnusedIndexes.Count - 1]].Initialize(PlantUnusedIndexes[PlantUnusedIndexes.Count - 1], random);
                                PlantUnusedIndexes.RemoveAt(PlantUnusedIndexes.Count - 1);
                            }
                            else
                            {
                                Plants.Add(PlantOffspringBuffer[i]);
                                Plants[Plants.Count - 1].Initialize(Plants.Count - 1, random);
                            }
                        }

                        List<Animal> AnimalOffspringBuffer = new List<Animal>();
                        for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist)
                            {
                                Animal? AnimalOffspringLocalBuffer = Animals[i].CreateOffspring(random);

                                if (AnimalOffspringLocalBuffer is not null) AnimalOffspringBuffer.Add(AnimalOffspringLocalBuffer);
                            }

                        for (int i = 0; i < AnimalOffspringBuffer.Count; i++)
                        {
                            if (AnimalUnusedIndexes.Count > 0)
                            {
                                Animals[AnimalUnusedIndexes[AnimalUnusedIndexes.Count - 1]] = AnimalOffspringBuffer[i];
                                Animals[AnimalUnusedIndexes[AnimalUnusedIndexes.Count - 1]].Initialize(AnimalUnusedIndexes[AnimalUnusedIndexes.Count - 1], random);
                                AnimalUnusedIndexes.RemoveAt(AnimalUnusedIndexes.Count - 1);
                            }
                            else
                            {
                                Animals.Add(AnimalOffspringBuffer[i]);
                                Animals[Animals.Count - 1].Initialize(Animals.Count - 1, random);
                            }
                        }

                        for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) LatestGeneration = long.Max(LatestGeneration, Animals[i].Generation);

                        PopulationPlant = 0;
                        PopulationAnimal = 0;
                        for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) PopulationPlant++;
                        for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) PopulationAnimal++;

                        CurrentSeed = random.Next(int.MinValue, int.MaxValue);

                        ElapsedTimeSteps++;
                        Modified = true;

                        if (SoupState == SoupState.StepRun) SoupState = SoupState.Pause;

                        if (ElapsedTimeSteps >= AutosaveInterval && ElapsedTimeSteps % AutosaveInterval == 0 && AutosaveEnabled)
                        {
                            SoupState prevSoupState = SoupState;
                            SoupState = SoupState.Saving;

                            AutosaveEnabled = false;

                            bool PrevModified = Modified;
                            Modified = false;

                            SaveSoup($@"{g_SoupAutosaveFilePath}\{Path.GetFileNameWithoutExtension(FilePath)}_{ElapsedTimeSteps}steps.soup", false);

                            Modified = PrevModified;

                            AutosaveEnabled = true;

                            SoupState = prevSoupState;
                        }

                        sw.Stop();
                        TimeStepsPerSecond *= 0.96d;
                        TimeStepsPerSecond += 1000000d / sw.Elapsed.TotalMicroseconds * 0.04d;
                    }
                }
            }
        }

        public void SaveSoup()
        {
            SaveSoup(FilePath, false);
        }

        public void SaveSoup(string filePath, bool updatePath)
        {
            SoupState prevSoupState = SoupState;
            SetSoupState(SoupState.Saving);

            Modified = false;

            StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8);
            streamWriter.Write(JsonSerializer.Serialize(g_Soup, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals }));
            streamWriter.Close();

            if (updatePath)
            {
                FilePath = filePath;
            }

            SetSoupState(prevSoupState);
        }

        public void SetSoupState(SoupState soupState)
        {
            if (SoupState != soupState)
            {
                lock (SoupStateLockObject)
                {
                    SoupState = soupState;
                }
            }
        }

        public void SetThreadCount(int threadCount)
        {
            if (ThreadCount != threadCount)
            {
                lock (ThreadCountLockObject)
                {
                    ThreadCount = int.Max(1, threadCount);
                }
            }
        }
    }

    public enum SoupState
    {
        Stop,
        Pause,
        Running,
        StepRun,
        Saving
    }
}
