using System.Diagnostics;

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
        public bool Modified { get; set; } = true;

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

        // Tile
        public Tile[] Tiles { get; set; } = new Tile[0];

        // Plant
        public List<Plant> Plants { get; set; } = new List<Plant>();
        public List<int> PlantUnusedIndexes { get; set; } = new List<int>();

        // Animal
        public List<Animal> Animals { get; set; } = new List<Animal>();
        public List<int> AnimalUnusedIndexes { get; set; } = new List<int>();
        
        
        
        // public double GlobalElementAmountMultiplier;



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

                Tiles = new Tile[Settings.SizeX * Settings.SizeY];
                int DefaultTypeTileCount = Settings.SizeX * Settings.SizeY;

                for (int x = 0; x < Settings.SizeX; x++)
                {
                    for (int y = 0; y < Settings.SizeY; y++)
                    {
                        Tiles[y * Settings.SizeX + x] = new Tile(x, y);
                    }
                }

                if (Settings.WallEnabled)
                {
                    for (int x = 0; x < Settings.SizeX; x++)
                    {
                        for (int y = 0; y < Settings.SizeY; y++)
                        {
                            if (Math.Abs(perlin.OctavePerlin(Settings.WallNoiseX + x * Settings.WallNoiseScale, Settings.WallNoiseY + y * Settings.WallNoiseScale, Settings.WallNoiseZ, Settings.WallNoiseOctave, 0.5d) - 0.5d) < Settings.WallThickness)
                            {
                                Tiles[y * Settings.SizeX + x].Type = TileType.Wall;
                            }
                        }
                    }

                    bool[] continuousRegionFlag = new bool[Settings.SizeX * Settings.SizeY];
                    int continuousRegionArea = 0;

                    for (int i = 0; i < 16; i++)
                    {
                        int exploredTileCount = 0;

                        List<int> exploreTiles = new List<int>();
                        bool[] exploredTiles = new bool[Settings.SizeX * Settings.SizeY];

                        exploreTiles.Add(new Random().Next(0, Settings.SizeX * Settings.SizeY));
                        while (Tiles[exploreTiles[0]].Type == TileType.Wall) exploreTiles[0] = new Random().Next(0, Settings.SizeX * Settings.SizeY);

                        while (exploreTiles.Count > 0)
                        {
                            List<int> nextStepExploreTiles = new List<int>();

                            for (int j = 0; j < exploreTiles.Count; j++)
                            {
                                int x = exploreTiles[j] % Settings.SizeX;
                                int y = exploreTiles[j] / Settings.SizeX;

                                for (int ix = -1; ix <= 1; ix++)
                                {
                                    for (int iy = -1; iy <= 1; iy++)
                                    {
                                        if (int.Abs(ix + iy) == 1)
                                        {
                                            if (x + ix >= 0 && x + ix < Settings.SizeX && y + iy >= 0 && y + iy < Settings.SizeY)
                                            {
                                                if (Tiles[(y + iy) * Settings.SizeX + (x + ix)].Type == TileType.Default && !exploredTiles[(y + iy) * Settings.SizeX + (x + ix)])
                                                {
                                                    exploredTiles[(y + iy) * Settings.SizeX + (x + ix)] = true;
                                                    nextStepExploreTiles.Add((y + iy) * Settings.SizeX + (x + ix));
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

                        if (exploredTileCount > Settings.SizeX * Settings.SizeY / 2) break;
                    }

                    for (int x = 0; x < Settings.SizeX; x++)
                    {
                        for (int y = 0; y < Settings.SizeY; y++)
                        {
                            Tile targetTile = Tiles[y * Settings.SizeX + x];

                            if (!continuousRegionFlag[y * Settings.SizeX + x])
                            {
                                targetTile.Type = TileType.Wall;
                            }
                        }
                    }

                    DefaultTypeTileCount = continuousRegionArea;
                }

                for (int x = 0; x < Settings.SizeX; x++)
                {
                    for (int y = 0; y < Settings.SizeY; y++)
                    {
                        Tile targetTile = Tiles[y * Settings.SizeX + x];

                        if (targetTile.Type == TileType.Default)
                        {
                            targetTile.Element = (Settings.TotalElementAmount - (Settings.InitialPlantElementAmount * Settings.InitialPlantPopulation + Settings.InitialAnimalElementAmount * Settings.InitialAnimalPopulation)) / DefaultTypeTileCount;
                        }
                    }
                }

                Random random = new Random(Settings.InitialSeed);

                for (int i = 0; i < Settings.InitialPlantPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Plants.Add(new Plant(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), Settings.InitialPlantElementAmount));
                    Plants[Plants.Count - 1].Initialize(Plants.Count - 1, random);
                }

                for (int i = 0; i < Settings.InitialAnimalPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Animals.Add(new Animal(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), random.NextDouble(), Settings.InitialAnimalElementAmount, random));
                    Animals[Animals.Count - 1].Initialize(Animals.Count - 1, random);
                }

                PopulationPlant = 0;
                PopulationAnimal = 0;
                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) PopulationPlant++;
                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) PopulationAnimal++;

                CurrentSeed = random.Next(int.MinValue, int.MaxValue);

                Initialized = true;
            }
            else throw new InvalidOperationException("This soup has already been initialized.");
        }

        public void StartSoupThread()
        {
            if (SoupMainThread.IsCompleted && Initialized)
            {
                SoupMainThread = Task.Run(() =>
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

                                double[] elementFlowAmount = new double[Settings.SizeX * Settings.SizeY];
                                double[] pheromoneRedFlowAmount = new double[Settings.SizeX * Settings.SizeY];
                                double[] pheromoneGreenFlowAmount = new double[Settings.SizeX * Settings.SizeY];
                                double[] pheromoneBlueFlowAmount = new double[Settings.SizeX * Settings.SizeY];
                                //for (int x = 0; x < Settings.SizeX; x++)
                                Parallel.For(0, Settings.SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < Settings.SizeY; y++)
                                    {
                                        Tile targetTile1 = Tiles[y * Settings.SizeX + x];

                                        if (targetTile1.Type == TileType.Default)
                                        {
                                            for (int ix = -1; ix <= 1; ix++)
                                            {
                                                for (int iy = -1; iy <= 1; iy++)
                                                {
                                                    if (int.Abs(ix) + int.Abs(iy) == 1)
                                                    {
                                                        if (x + ix >= 0 && x + ix < Settings.SizeX && y + iy >= 0 && y + iy < Settings.SizeY)
                                                        {
                                                            Tile targetTile2 = Tiles[(y + iy) * Settings.SizeX + (x + ix)];

                                                            if (targetTile2.Type == TileType.Default)
                                                            {
                                                                elementFlowAmount[y * Settings.SizeX + x] -= (targetTile1.Element - targetTile2.Element) * Settings.ElementFlowRate;
                                                                pheromoneRedFlowAmount[y * Settings.SizeX + x] -= (targetTile1.PheromoneRed - targetTile2.PheromoneRed) * Settings.PheromoneFlowRate;
                                                                pheromoneGreenFlowAmount[y * Settings.SizeX + x] -= (targetTile1.PheromoneGreen - targetTile2.PheromoneGreen) * Settings.PheromoneFlowRate;
                                                                pheromoneBlueFlowAmount[y * Settings.SizeX + x] -= (targetTile1.PheromoneBlue - targetTile2.PheromoneBlue) * Settings.PheromoneFlowRate;
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
                                Parallel.For(0, Settings.SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < Settings.SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * Settings.SizeX + x];
                                        targetTile.Element += elementFlowAmount[y * Settings.SizeX + x];
                                        targetTile.PheromoneRed += pheromoneRedFlowAmount[y * Settings.SizeX + x];
                                        targetTile.PheromoneGreen += pheromoneGreenFlowAmount[y * Settings.SizeX + x];
                                        targetTile.PheromoneBlue += pheromoneBlueFlowAmount[y * Settings.SizeX + x];
                                        if (targetTile.Element < 0) targetTile.Element = 0;

                                        targetTile.PheromoneRed *= 1d - Settings.PheromoneDecayRate;
                                        targetTile.PheromoneGreen *= 1d - Settings.PheromoneDecayRate;
                                        targetTile.PheromoneBlue *= 1d - Settings.PheromoneDecayRate;
                                    }
                                });

                                double CurrentTotalElementAmount = 0d;
                                for(int x = 0; x < Settings.SizeX; x++)
                                {
                                    for (int y = 0; y < Settings.SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * Settings.SizeX + x];
                                        CurrentTotalElementAmount += targetTile.Element;
                                    }
                                }
                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) CurrentTotalElementAmount += Plants[i].Element;
                                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) CurrentTotalElementAmount += Animals[i].Element;

                                double GlobalElementAmountMultiplier = Settings.TotalElementAmount / CurrentTotalElementAmount;
                                GlobalElementAmountMultiplier = Settings.TotalElementAmount / CurrentTotalElementAmount;
                                Parallel.For(0, Settings.SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < Settings.SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * Settings.SizeX + x];
                                        targetTile.Element *= GlobalElementAmountMultiplier;
                                    }
                                });
                                Parallel.For(0, Plants.Count, parallelOptions, i => { Plants[i].Element *= GlobalElementAmountMultiplier; Plants[i].Radius = Math.Sqrt(Plants[i].Element / Settings.PlantForkCost) / 2d; Plants[i].Mass = Plants[i].Element; });
                                Parallel.For(0, Animals.Count, parallelOptions, i => { Animals[i].Element *= GlobalElementAmountMultiplier; Animals[i].Mass = 16d + Animals[i].Element; });

                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].CollectElement();

                                Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].UpdateNeuralNet(); });
                                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) Animals[i].ApplyNeuralNetOutput();

                                Parallel.For(0, Plants.Count, parallelOptions, i => { if (Plants[i].Exist) Plants[i].ApplyDrag(); });
                                Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].ApplyDrag(); });

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

                                sw.Stop();
                                TimeStepsPerSecond *= 0.96d;
                                TimeStepsPerSecond += 1000000d / sw.Elapsed.TotalMicroseconds * 0.04d;
                            }
                        }
                    }
                });
            }
            else if (!Initialized) throw new InvalidOperationException("Soup is not initialized.");
            else if (!SoupMainThread.IsCompleted) throw new InvalidOperationException("Soup Thread have already been started.");
            else throw new InvalidOperationException();
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
        StepRun
    }
}
