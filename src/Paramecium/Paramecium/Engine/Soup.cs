using System.Diagnostics;

namespace Paramecium.Engine
{
    public class Soup
    {
        ///////////////////////////////
        // Soup Environment Settings //
        ///////////////////////////////

        // Seed Settings
        public int InitialSeed { get; set; } = 0;

        // Soup Size Settings
        public int SizeX { get; set; } = 512;
        public int SizeY { get; set; } = 256;

        // Wall Settings
        public bool WallEnabled { get; set; } = true;
        public double WallNoiseX { get; set; } = 0d;
        public double WallNoiseY { get; set; } = 0d;
        public double WallNoiseZ { get; set; } = 0d;
        public double WallNoiseScale { get; set; } = 0.03d;
        public int WallNoiseOctave { get; set; } = 4;
        public double WallThickness { get; set; } = 0.008d;

        // Element Settings
        public double TotalElementAmount { get; set; } = 65536d;
        public double ElementFlowRate { get; set; } = 0.04d;

        // Physical Settings
        public double Drag { get; set; } = 0.01d;
        public double AngularVelocityDrag { get; set; } = 0.01d;
        public double MaximumVelocity { get; set; } = 0.1d;
        public double RestitutionCoefficient { get; set; } = 1d;

        // Plant Settings
        public int InitialPlantPopulation { get; set; } = 256;
        public double InitialPlantElementAmount { get; set; } = 4d;
        public double PlantForkCost { get; set; } = 16d;
        public int PlantForkOffspringCountMin { get; set; } = 4;
        public int PlantForkOffspringCountMax { get; set; } = 8;
        public double PlantElementCollectRate { get; set; } = 0.04d;

        // Animal Basic Settings
        public int InitialAnimalPopulation { get; set; } = 1024;
        public double InitialAnimalElementAmount { get; set; } = 32d;
        public double AnimalForkCost { get; set; } = 64d;

        // Animal Mutation Settings
        public double AnimalMutationRate { get; set; } = 0.2d;
        public double AnimalNeuralNetWeightMutationRate { get; set; } = 0.01d;
        public double AnimalSpeciesIdMutationRate { get; set; } = 0.1d;



        ///////////////////////////////
        // Soup Simulation Variables //
        ///////////////////////////////

        // Basic Information
        public long ElapsedTimeSteps { get; set; } = 0;
        public long TotalBornCount { get; set; } = 0;
        public long TotalDieCount { get; set; } = 0;
        public long LatestGeneration { get; set; } = 0;

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



        public Soup() { }

        public void InitializeSoup()
        {
            if (!Initialized)
            {
                Perlin perlin = new Perlin();

                Tiles = new Tile[SizeX * SizeY];
                int DefaultTypeTileCount = SizeX * SizeY;
                for (int x = 0; x < SizeX; x++)
                {
                    for (int y = 0; y < SizeY; y++)
                    {
                        Tiles[y * SizeX + x] = new Tile(x, y);

                        if (WallEnabled)
                        {
                            if (Math.Abs(perlin.OctavePerlin(WallNoiseX + x * WallNoiseScale, WallNoiseY + y * WallNoiseScale, WallNoiseZ, WallNoiseOctave, 0.5d) - 0.5d) < WallThickness)
                            {
                                Tiles[y * SizeX + x].Type = TileType.Wall;
                                DefaultTypeTileCount--;
                            }
                        }
                    }
                }

                for (int x = 0; x < SizeX; x++)
                {
                    for (int y = 0; y < SizeY; y++)
                    {
                        Tile targetTile = Tiles[y * SizeX + x];

                        if (targetTile.Type == TileType.Default)
                        {
                            targetTile.Element = (TotalElementAmount - (InitialPlantElementAmount * InitialPlantPopulation + InitialAnimalElementAmount * InitialAnimalPopulation)) / DefaultTypeTileCount;
                        }
                    }
                }

                Random random = new Random();

                for (int i = 0; i < InitialPlantPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Plants.Add(new Plant(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), InitialPlantElementAmount));
                    Plants[Plants.Count - 1].Initialize(Plants.Count - 1, random);
                }

                for (int i = 0; i < InitialAnimalPopulation; i++)
                {
                    int targetTileIndex = random.Next(0, Tiles.Length);
                    while (Tiles[targetTileIndex].Type == TileType.Wall) targetTileIndex = random.Next(0, Tiles.Length);

                    Tile targetTile = Tiles[targetTileIndex];

                    Animals.Add(new Animal(new Double2d(targetTile.PositionX + random.NextDouble(), targetTile.PositionY + random.NextDouble()), random.NextDouble(), InitialAnimalElementAmount, random));
                    Animals[Animals.Count - 1].Initialize(Animals.Count - 1, random);
                }

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

                                double[] elementFlowAmount = new double[SizeX * SizeY];
                                //for (int x = 0; x < SizeX; x++)
                                Parallel.For(0, SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < SizeY; y++)
                                    {
                                        Tile targetTile1 = Tiles[y * SizeX + x];

                                        if (targetTile1.Type == TileType.Default)
                                        {
                                            for (int ix = -1; ix <= 1; ix++)
                                            {
                                                for (int iy = -1; iy <= 1; iy++)
                                                {
                                                    if (int.Abs(ix) + int.Abs(iy) == 1)
                                                    {
                                                        if (x + ix >= 0 && x + ix < SizeX && y + iy >= 0 && y + iy < SizeY)
                                                        {
                                                            Tile targetTile2 = Tiles[(y + iy) * SizeX + (x + ix)];

                                                            if (targetTile2.Type == TileType.Default)
                                                            {
                                                                elementFlowAmount[y * SizeX + x] -= (targetTile1.Element - targetTile2.Element) * ElementFlowRate;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                });
                                Parallel.For(0, SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * SizeX + x];
                                        targetTile.Element += elementFlowAmount[y * SizeX + x];
                                        if (targetTile.Element < 0) targetTile.Element = 0;
                                    }
                                });

                                double CurrentTotalElementAmount = 0d;
                                for(int x = 0; x < SizeX; x++)
                                {
                                    for (int y = 0; y < SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * SizeX + x];
                                        CurrentTotalElementAmount += targetTile.Element;
                                    }
                                }
                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) CurrentTotalElementAmount += Plants[i].Element;
                                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) CurrentTotalElementAmount += Animals[i].Element;

                                //Console.WriteLine($"{CurrentTotalElementAmount} / {TotalElementAmount}");
                                double GlobalElementAmountMultiplier = TotalElementAmount / CurrentTotalElementAmount;
                                Parallel.For(0, SizeX, parallelOptions, x =>
                                {
                                    for (int y = 0; y < SizeY; y++)
                                    {
                                        Tile targetTile = Tiles[y * SizeX + x];
                                        targetTile.Element *= GlobalElementAmountMultiplier;
                                    }
                                });
                                Parallel.For(0, Plants.Count, parallelOptions, i => { Plants[i].Element *= GlobalElementAmountMultiplier; });
                                Parallel.For(0, Animals.Count, parallelOptions, i => { Animals[i].Element *= GlobalElementAmountMultiplier; });

                                Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].ApplyDrag(); });

                                Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].UpdateNeuralNet(); });
                                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) Animals[i].ApplyNeuralNetOutput();

                                //for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].ApplyDrag();
                                Parallel.For(0, Plants.Count, parallelOptions, i => { if (Plants[i].Exist) Plants[i].ApplyDrag(); });

                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].CollectElement();

                                Parallel.For(0, Plants.Count, parallelOptions, i => { if (Plants[i].Exist) Plants[i].UpdateCollision(); });
                                Parallel.For(0, Animals.Count, parallelOptions, i => { if (Animals[i].Exist) Animals[i].UpdateCollision(); });

                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist) Plants[i].UpdatePosition();
                                for (int i = 0; i < Animals.Count; i++) if (Animals[i].Exist) Animals[i].UpdatePosition();

                                List<Plant> PlantOffspringBuffer = new List<Plant>();
                                for (int i = 0; i < Plants.Count; i++) if (Plants[i].Exist)
                                    {
                                        List<Plant>? PlantOffspringLocalBuffer = Plants[i].CreateOffspring(new Random());

                                        if (PlantOffspringLocalBuffer is not null) for (int j = 0; j < PlantOffspringLocalBuffer.Count; j++) PlantOffspringBuffer.Add(PlantOffspringLocalBuffer[j]);
                                    }

                                for (int i = 0; i < PlantOffspringBuffer.Count; i++)
                                {
                                    if (PlantUnusedIndexes.Count > 0)
                                    {
                                        Plants[PlantUnusedIndexes[PlantUnusedIndexes.Count - 1]] = PlantOffspringBuffer[i];
                                        Plants[PlantUnusedIndexes[PlantUnusedIndexes.Count - 1]].Initialize(PlantUnusedIndexes[PlantUnusedIndexes.Count - 1], new Random());
                                        PlantUnusedIndexes.RemoveAt(PlantUnusedIndexes.Count - 1);
                                    }
                                    else
                                    {
                                        Plants.Add(PlantOffspringBuffer[i]);
                                        Plants[Plants.Count - 1].Initialize(Plants.Count - 1, new Random());
                                    }
                                }

                                ElapsedTimeSteps++;

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
