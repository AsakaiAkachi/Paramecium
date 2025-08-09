using Paramecium.Variables;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // スープ全体の情報とシミュレーションの管理を行うためのクラス
    public class Soup
    {
        //////////////////////////
        // 　　　環境設定　　　 //
        //////////////////////////

        public SoupSettings Settings { get; set; } = new SoupSettings();    // スープの設定



        //////////////////////////
        // シミュレーション変数 //
        //////////////////////////

        // 基本情報
        public long ElapsedTimeSteps { get; set; } = 0;                         // スープの経過タイムステップ数
        public long TotalBornCount { get; set; } = 0;                           // 誕生した動物の総数
        public long TotalDieCount { get; set; } = 0;                            // 死滅した動物の総数
        public long LatestGeneration { get; set; } = 0;                         // 最も世代数の進んだ動物の世代数

        // ファイル情報
        public bool Modified { get; set; } = true;                              // スープファイルが読み込まれてから変更されているかどうか
        public bool AutosaveEnabled { get; set; } = false;                      // オートセーブが有効化されているかどうか
        public long AutosaveInterval { get; set; } = 100000;                    // オートセーブの間隔
        public long LastAutoSaveTime { get; set; } = 0;                         // 最後にオートセーブされた時刻

        // ステート管理
        private Task SoupMainThread = Task.CompletedTask;                       // スープのメインスレッド
        [JsonIgnore]
        public SoupState SoupState { get; private set; } = SoupState.Stop;      // スープの状態
        public object SoupStateLockObject = new object();                       // SoupStateを変更する際の排他制御用

        // マルチスレッディング
        [JsonIgnore]
        public int ThreadCount { get; private set; } = 1;                       // スレッド数
        public object ThreadCountLockObject = new object();                     // ThreadCountを変更する際の排他制御用
        public double StepTime = 1;                                             // シミュレーションを1ステップ実行するのにかかった時間

        // エレメント
        public double ElementAmountMultiplier = 1d;                             // エレメント移動時にエレメントを受け取る側の受け取り量に掛ける倍率

        // 個体数
        public int TotalPopulation { get; set; } = 0;                           // スープ内の総個体数
        public int PlantPopulation { get; set; } = 0;                           // スープ内の植物の個体数
        public int AnimalPopulation { get; set; } = 0;                          // スープ内の動物の個体数

        // タイル
        public Tile[] Tiles { get; set; } = Array.Empty<Tile>();                // スープ内のタイルのデータ

        // 植物
        public List<Plant?> Plants { get; set; } = new List<Plant?>();            // スープ内の植物のデータ
        public List<int> PlantUnusedIndexes { get; set; } = new List<int>();    // 植物の未使用状態のインデックス

        // 動物
        public List<Animal?> Animals { get; set; } = new List<Animal?>();         // スープ内の動物のデータ
        public List<int> AnimalUnusedIndexes { get; set; } = new List<int>();   // 動物の未使用状態のインデックス



        public Soup() { }
        public Soup(SoupSettings settings)
        {
            Settings = settings;

            Tiles = new Tile[Settings.Area]; for (int i = 0; i < Tiles.Length; i++) Tiles[i] = new Tile();

            //Random rand = new Random(Settings.InitialSeed);
            Random rand = new Random();

            int defaultTypeTileCount = 0;

            // スープ内の壁を生成する
            if (Settings.WallEnabled)
            {
                Perlin perlin = new Perlin();

                // 壁の生成
                for (int i = 0; i < Tiles.Length; i++)
                {
                    Tile targetTile = Tiles[i];
                    Int2d targetTilePosition = GetTilePositionFromTileIndex(i);

                    if (Math.Abs(perlin.OctavePerlin(Settings.WallNoiseX + targetTilePosition.X * Settings.WallNoiseSamplingInterval, Settings.WallNoiseY + targetTilePosition.Y * Settings.WallNoiseSamplingInterval, Settings.WallNoiseZ, Settings.WallNoiseOctave, 0.5d) - 0.5d) < Settings.WallThickness)
                    {
                        targetTile.Type = TileType.Wall;
                    }
                }

                // 最も大きな連続した壁ではない領域を残して、それ以外を全て壁で埋める (例えば、外部から壁で隔離された小さな領域などがあればそれを壁で埋める)
                bool[] continuousRegionFlag = new bool[Settings.Area];
                int continuousRegionArea = 0;

                for (int i = 0; i < 16; i++)
                {
                    int exploredTileCount = 0;

                    List<int> exploreTiles = new List<int>();
                    bool[] exploredTiles = new bool[Settings.Area];

                    exploreTiles.Add(new Random().Next(0, Settings.Area));
                    for (int j = 0; j < 16; j++)
                    {
                        if (Tiles[exploreTiles[0]].Type == TileType.Wall) exploreTiles[0] = new Random().Next(0, Settings.Area);
                        else break;
                    }

                    if (Tiles[exploreTiles[0]].Type == TileType.Wall) continue;

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

                    if (exploredTileCount > Settings.Area / 2) break;
                }

                for (int i = 0; i < Settings.Area; i++)
                {
                    if (!continuousRegionFlag[i]) Tiles[i].Type = TileType.Wall;
                }

                defaultTypeTileCount = continuousRegionArea;
            }

            // 壁ではないエリアが存在しない場合は何も生成しない
            if (defaultTypeTileCount > 0)
            {
                double unusedElementAmount = Settings.TotalElementAmount;

                // 植物を生成する
                for (int i = 0; i < Settings.InitialPlantPopulation; i++)
                {
                    // 生成する位置をランダムに決定しそこが壁の中であれば生成する位置を再度ランダムに生成する (16回生成しても位置が壁の中であれば処理を打ち切る)
                    Double2d position = new Double2d(rand.NextDouble() * Settings.SizeX, rand.NextDouble() * Settings.SizeY);
                    for (int j = 0; j < 16; j++)
                    {
                        if (Tiles[GetTileIndexFromPosition(position)].Type == TileType.Wall) position = new Double2d(rand.NextDouble() * Settings.SizeX, rand.NextDouble() * Settings.SizeY);
                        else break;
                    }

                    Plant plant = new Plant(settings, position, rand.NextDouble() * Settings.PlantMaximumElementAmount);
                    unusedElementAmount -= plant.Element;

                    AddPlant(plant);

                    TotalPopulation++;
                    PlantPopulation++;
                }

                // 動物を生成する
                for (int i = 0; i < Settings.InitialAnimalPopulation; i++)
                {
                    // 生成する位置をランダムに決定しそこが壁の中であれば生成する位置を再度ランダムに生成する (16回生成しても位置が壁の中であれば処理を打ち切る)
                    Double2d position = new Double2d(rand.NextDouble() * Settings.SizeX, rand.NextDouble() * Settings.SizeY);
                    for (int j = 0; j < 16; j++)
                    {
                        if (Tiles[GetTileIndexFromPosition(position)].Type == TileType.Wall) position = new Double2d(rand.NextDouble() * Settings.SizeX, rand.NextDouble() * Settings.SizeY);
                        else break;
                    }

                    Animal animal = new Animal(settings, position, rand.NextDouble(), Settings.AnimalForkCost);
                    unusedElementAmount -= animal.Element;

                    AddAnimal(animal);

                    TotalPopulation++;
                    AnimalPopulation++;

                    TotalBornCount++;
                }

                // タイルのエレメントを生成する
                for (int i = 0; i < Tiles.Length; i++)
                {
                    if (Tiles[i].Type == TileType.Default) Tiles[i].Element = unusedElementAmount / defaultTypeTileCount;
                }
            }
        }

        public void StartSoupThread()
        {
            if (SoupMainThread.IsCompleted)
            {
                SoupState = SoupState.Pause;

                SoupMainThread = Task.Run(MainLoop);
            }
        }

        private void MainLoop()
        {
            Stopwatch stepTime = Stopwatch.StartNew();

            while (SoupState != SoupState.Stop)
            {
                lock (SoupStateLockObject)
                {
                    if (SoupState == SoupState.Running || SoupState == SoupState.StepRun)
                    {
                        lock (ThreadCountLockObject)
                        {
                            if (SoupState == SoupState.StepRun) stepTime.Restart();

                            ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = ThreadCount };

                            // ElementAmountMultiplierを計算する
                            double currentTotalElementAmount = 0;
                            for (int i = 0; i < Settings.Area; i++)
                            {
                                currentTotalElementAmount += Tiles[i].Element;
                            }
                            for (int i = 0; i < Plants.Count; i++)
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) currentTotalElementAmount += targetPlant.Element;
                            }
                            for (int i = 0; i < Animals.Count; i++)
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) currentTotalElementAmount += targetAnimal.Element;
                            }
                            ElementAmountMultiplier = Settings.TotalElementAmount / currentTotalElementAmount;

                            // タイルのエレメントとフェロモンの流量の計算
                            double[] elementFlowAmount = new double[Settings.Area];
                            double[] pheromoneRedFlowAmount = new double[Settings.Area];
                            double[] pheromoneGreenFlowAmount = new double[Settings.Area];
                            double[] pheromoneBlueFlowAmount = new double[Settings.Area];
                            Parallel.For(0, Settings.Area, parallelOptions, i =>
                            {
                                Int2d tilePos = GetTilePositionFromTileIndex(i);
                                Tile targetTile = Tiles[i];

                                double tileElementAmount = targetTile.Element;
                                double tilepheromoneRedAmount = targetTile.PheromoneRed;
                                double tilepheromoneGreenAmount = targetTile.PheromoneGreen;
                                double tilepheromoneBlueAmount = targetTile.PheromoneBlue;

                                double elementFlowRate = Settings.ElementFlowRate;
                                double pheromoneFlowRate = Settings.PheromoneFlowRate;

                                if (targetTile.Type == TileType.Default)
                                {
                                    for (int x = -1; x <= 1; x++)
                                    {
                                        for (int y = -1; y <= 1; y++)
                                        {
                                            if (Math.Abs(x) + Math.Abs(y) == 1)
                                            {
                                                Int2d targetTilePos = tilePos + new Int2d(x, y);

                                                if (targetTilePos.X >= 0 && targetTilePos.X < Settings.SizeX && targetTilePos.Y >= 0 && targetTilePos.Y < Settings.SizeY)
                                                {
                                                    int targetTileIndex = GetTileIndexFromTilePosition(targetTilePos);
                                                    Tile targetTile2 = Tiles[targetTileIndex];

                                                    if (targetTile2.Type == TileType.Default)
                                                    {
                                                        elementFlowAmount[i] += (targetTile2.Element - tileElementAmount) * elementFlowRate / 4d;

                                                        pheromoneRedFlowAmount[i] += (targetTile2.PheromoneRed - tilepheromoneRedAmount) * pheromoneFlowRate / 4d;
                                                        pheromoneGreenFlowAmount[i] += (targetTile2.PheromoneGreen - tilepheromoneGreenAmount) * pheromoneFlowRate / 4d;
                                                        pheromoneBlueFlowAmount[i] += (targetTile2.PheromoneBlue - tilepheromoneBlueAmount) * pheromoneFlowRate / 4d;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });

                            // 計算したエレメントとフェロモンの流量を適用する
                            Parallel.For(0, Settings.Area, parallelOptions, i =>
                            {
                                Tile targetTile = Tiles[i];

                                if (targetTile.Type == TileType.Default)
                                {
                                    if (elementFlowAmount[i] >= 0) targetTile.Element += elementFlowAmount[i] * ElementAmountMultiplier;
                                    else targetTile.Element += elementFlowAmount[i];

                                    targetTile.PheromoneRed += pheromoneRedFlowAmount[i];
                                    targetTile.PheromoneGreen += pheromoneGreenFlowAmount[i];
                                    targetTile.PheromoneBlue += pheromoneBlueFlowAmount[i];

                                    if (targetTile.PheromoneRed < Settings.MinimumEffectivePheromoneAmount) targetTile.PheromoneRed = 0d;
                                    if (targetTile.PheromoneGreen < Settings.MinimumEffectivePheromoneAmount) targetTile.PheromoneGreen = 0d;
                                    if (targetTile.PheromoneBlue < Settings.MinimumEffectivePheromoneAmount) targetTile.PheromoneBlue = 0d;
                                }
                                else
                                {
                                    targetTile.Element = 0;

                                    targetTile.PheromoneRed = 0d;
                                    targetTile.PheromoneGreen = 0d;
                                    targetTile.PheromoneBlue = 0d;
                                }
                            });

                            // セルの年齢の処理
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) targetPlant.UpdateAge(this, Settings);
                            });
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.UpdateAge(this, Settings);
                            });

                            // 速度に対する抵抗の適用
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) targetPlant.ApplyDrag(this, Settings);
                            });
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.ApplyDrag(this, Settings);
                            });

                            // 植物:エレメントの収集
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) targetPlant.CollectElement(this, Settings);
                            });

                            // 動物:ニューラルネットの更新
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.UpdateBrain(this, Settings);
                            });

                            // 当たり判定の処理
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) targetPlant.UpdateCollision(this, Settings);
                            });
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.UpdateCollision(this, Settings);
                            });

                            // 位置の更新
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) targetPlant.UpdatePosition(this, Settings);
                            });
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.UpdatePosition(this, Settings);
                            });

                            // タイルのフェロモン量を減衰させる
                            Parallel.For(0, Settings.Area, parallelOptions, i =>
                            {
                                Tile targetTile = Tiles[i];

                                if (targetTile.Type == TileType.Default)
                                {
                                    targetTile.PheromoneRed *= 1d - Settings.PheromoneDecayRate;
                                    targetTile.PheromoneGreen *= 1d - Settings.PheromoneDecayRate;
                                    targetTile.PheromoneBlue *= 1d - Settings.PheromoneDecayRate;
                                }
                            });

                            // 動物:エレメントを消費する
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) targetAnimal.LosingElement(this, Settings);
                            });

                            // セルの子孫を生成する
                            List<Plant>?[] plantOffsprings = new List<Plant>[Plants.Count];
                            Parallel.For(0, Plants.Count, parallelOptions, i =>
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null) plantOffsprings[i] = targetPlant.CreateOffspring(this, Settings);
                            });
                            Animal?[] animalOffsprings = new Animal?[Animals.Count];
                            Parallel.For(0, Animals.Count, parallelOptions, i =>
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null) animalOffsprings[i] = targetAnimal.CreateOffspring(this, Settings);
                            });

                            // 死滅したセルの処理
                            for (int i = 0; i < Plants.Count; i++)
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null)
                                {
                                    targetPlant.IsNotAlive(this, Settings);
                                    if (!targetPlant.IsAlive)
                                    {
                                        PlantUnusedIndexes.Add(i);
                                        Plants[i] = null;
                                    }
                                }
                            }
                            for (int i = 0; i < Animals.Count; i++)
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null)
                                {
                                    targetAnimal.IsNotAlive(this, Settings);
                                    if (!targetAnimal.IsAlive)
                                    {
                                        AnimalUnusedIndexes.Add(i);
                                        Animals[i] = null;
                                        TotalDieCount++;
                                    }
                                }
                            }

                            // セルの子孫をスープに追加する
                            for (int i = 0; i < plantOffsprings.Length; i++)
                            {
                                List<Plant>? offsprings = plantOffsprings[i];
                                if (offsprings is not null)
                                {
                                    for (int j = 0; j < offsprings.Count; j++)
                                    {
                                        AddPlant(offsprings[j]);
                                    }
                                }
                            }
                            for (int i = 0; i < animalOffsprings.Length; i++)
                            {
                                Animal? offspring = animalOffsprings[i];
                                if (offspring is not null)
                                {
                                    AddAnimal(offspring);
                                    TotalBornCount++;
                                }
                            }

                            int totalPopulationBuffer = 0;
                            int plantPopulationBuffer = 0;
                            int animalPopulationBuffer = 0;
                            for (int i = 0; i < Plants.Count; i++)
                            {
                                Plant? targetPlant = Plants[i];
                                if (targetPlant is not null)
                                {
                                    if (targetPlant.IsAlive)
                                    {
                                        totalPopulationBuffer++;
                                        plantPopulationBuffer++;
                                    }
                                }
                            }
                            for (int i = 0; i < Animals.Count; i++)
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.IsAlive)
                                    {
                                        totalPopulationBuffer++;
                                        animalPopulationBuffer++;
                                    }
                                }
                            }

                            int latestGenerationBuffer = 0;
                            for (int i = 0; i < Animals.Count; i++)
                            {
                                Animal? targetAnimal = Animals[i];
                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.IsAlive)
                                    {
                                        if (targetAnimal.Generation > latestGenerationBuffer) latestGenerationBuffer = targetAnimal.Generation;
                                    }
                                }
                            }
                            LatestGeneration = latestGenerationBuffer;

                            TotalPopulation = totalPopulationBuffer;
                            PlantPopulation = plantPopulationBuffer;
                            AnimalPopulation = animalPopulationBuffer;

                            if (SoupState == SoupState.StepRun) SoupState = SoupState.Pause;

                            ElapsedTimeSteps++;

                            Modified = true;

                            StepTime = stepTime.Elapsed.TotalSeconds;
                            stepTime.Restart();
                        }
                    }
                }
            }
        }

        public void AddPlant(Plant plant)
        {
            int index = -1;

            if(PlantUnusedIndexes.Count > 0)
            {
                index = PlantUnusedIndexes[PlantUnusedIndexes.Count - 1];
                PlantUnusedIndexes.RemoveAt(PlantUnusedIndexes.Count - 1);

                Plants[index] = plant;
            }
            else
            {
                index = Plants.Count;
                Plants.Add(plant);
            }

            plant.Index = index;

            plant.TileIndex = GetTileIndexFromPosition(plant.Position);

            Tiles[GetTileIndexFromPosition(plant.Position)].PlantIndexes.Add(index);
        }

        public void AddAnimal(Animal animal)
        {
            int index = -1;

            if (AnimalUnusedIndexes.Count > 0)
            {
                index = AnimalUnusedIndexes[AnimalUnusedIndexes.Count - 1];
                AnimalUnusedIndexes.RemoveAt(AnimalUnusedIndexes.Count - 1);

                Animals[index] = animal;
            }
            else
            {
                index = Animals.Count;
                Animals.Add(animal);
            }

            animal.Index = index;

            animal.TileIndex = GetTileIndexFromPosition(animal.Position);

            Tiles[GetTileIndexFromPosition(animal.Position)].AnimalIndexes.Add(index);
        }

        public void SetSoupState(SoupState soupState)
        {
            lock (SoupStateLockObject) SoupState = soupState;
        }

        public void SetThreadCount(int threadCount)
        {
            lock (ThreadCountLockObject) ThreadCount = threadCount;
        }

        public Int2d GetTilePositionFromPosition(Double2d position)
        {
            return new Int2d((int)Math.Max(0, Math.Min(Settings.SizeX - 1, position.X)), (int)Math.Max(0, Math.Min(Settings.SizeY - 1, position.Y)));
        }
        public Int2d GetTilePositionFromTileIndex(int tileIndex)
        {
            return new Int2d(tileIndex % Settings.SizeX, tileIndex / Settings.SizeX);
        }

        public int GetTileIndexFromPosition(Double2d position)
        {
            return (int)Math.Max(0, Math.Min(Settings.SizeX - 1, position.X)) + (int)Math.Max(0, Math.Min(Settings.SizeY - 1, position.Y)) * Settings.SizeX;
        }
        public int GetTileIndexFromTilePosition(Int2d tilePosition)
        {
            return tilePosition.X + tilePosition.Y * Settings.SizeX;
        }

        public Double2d CalculateCollisionTwoObjects(Double2d obj1Pos, double obj1Radius, double obj1Mass, Double2d obj2Pos, double obj2Radius, double obj2Mass)
        {
            double distanceSqr = Double2d.DistanceSquared(obj1Pos, obj2Pos);

            if (distanceSqr < (obj1Radius + obj2Radius) * (obj1Radius + obj2Radius))
            {
                double distance = Double2d.Distance(obj1Pos, obj2Pos);
                return (obj1Pos - obj2Pos).Normalized * (1d - distance / (obj1Radius + obj2Radius)) * Math.Min(1d, obj2Mass / obj1Mass) * Settings.RestitutionCoefficient;
            }
            else return Double2d.Zero;
        }
        public Double2d CalculateCollisionTwoObjects(Double2d obj1Pos, double obj1Radius, Double2d obj2Pos, double obj2Radius)
        {
            double distanceSqr = Double2d.DistanceSquared(obj1Pos, obj2Pos);

            if (distanceSqr < (obj1Radius + obj2Radius) * (obj1Radius + obj2Radius))
            {
                double distance = Double2d.Distance(obj1Pos, obj2Pos);
                return (obj1Pos - obj2Pos).Normalized * (1d - distance / (obj1Radius + obj2Radius)) * Settings.RestitutionCoefficient;
            }
            else return Double2d.Zero;
        }
    }
}
