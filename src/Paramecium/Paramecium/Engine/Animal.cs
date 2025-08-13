using Paramecium.Variables;

namespace Paramecium.Engine
{
    // 動物のデータを保存する用のクラス
    public class Animal
    {
        public bool IsAlive { get; set; } = false;                  // セルが生きているかどうか

        public int Index { get; set; } = -1;                        // セルのインデックス
        public long Id { get; set; } = -1;                          // セルのID

        public Double4d SpeciesSignature { get; set; } = new Double4d(1d, 1d, 1d, 1d);      // セルの種族シグネチャ
        public int Generation { get; set; } = 1;                    // セルの世代数
        public long MutationCount { get; set; } = 0;                // セルの累計突然変異数
        public int Age { get; set; } = 0;                           // セルの年齢
        public int OffspringCount { get; set; } = 0;                // 子孫の数

        public Double2d Position { get; set; } = Double2d.Zero;     // セルの位置
        public int TileIndex { get; set; } = 0;                     // セルが属しているタイルのインデックス
        public Double2d Velocity { get; set; } = Double2d.Zero;     // セルの速度
        public double Angle { get; set; } = 0;                      // セルの角度
        public double AngularVelocity { get; set; } = 0;            // セルの角速度
        public double Radius { get; set; } = 0;                     // セルの半径
        public double Element { get; set; } = 0;                    // セルのエレメント量
        public double ReproductionProgress { get; set; } = 0;       // 繁殖の進捗
        public double Mass { get; set; } = 0;                       // セルの質量

        public double ElementLossRate { get; set; } = 0;            // エレメントの消費速度
        public double ReproductionRate { get; set; } = 0;           // 繁殖の進捗速度

        public int Ate { get; set; } = 0;                           // セルが最後に行った植物を食べる試みが成功したかどうか
        public int TimeSinceLastAttacked { get; set; } = 0;         // セルが最後に攻撃されてからの経過時間
        public double LastAttackedAngle { get; set; } = 0;          // セルが最後に攻撃された時の攻撃方向の相対角度
        public int AttackSuccessful { get; set; } = 0;              // セルが最後に行った攻撃が成功したかどうか

        public Brain Brain { get; set; } = new Brain();             // セルのニューラルネット

        public object LockObject = new object();                    // セルのパラメーターを複数スレッドから操作する際の排他制御用オブジェクト

        public Animal() { }
        public Animal(SoupSettings settings, Double2d position, double angle, double element)
        {
            Random rand = new Random();

            IsAlive = true;

            Id = rand.NextInt64(0, 4738381338321616896);

            Position = position;
            Angle = angle;
            Radius = 0.5d;
            Element = element;
            Mass = element;

            TimeSinceLastAttacked = settings.AnimalUnderAttackTime;

            SpeciesSignature = new Double4d(Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6));

            Brain = Brain.DefaultBrain;
        }
        public Animal(SoupSettings settings, Double2d position, double angle, double element, Animal parent)
        {
            Random rand = new Random();

            IsAlive = true;

            Id = rand.NextInt64(0, 4738381338321616896);

            Generation = parent.Generation + 1;
            Age = -settings.AnimalEggHatchingTime;

            Position = position;
            Angle = angle;
            Radius = 0.5d * settings.AnimalEggRadiusRatio;
            Element = element;
            Mass = element;

            TimeSinceLastAttacked = settings.AnimalUnderAttackTime;

            SpeciesSignature = parent.SpeciesSignature;
            MutationCount = parent.MutationCount;

            Brain = new Brain(parent.Brain);

            if (rand.NextDouble() < settings.AnimalMutationRate)
            {
                for (int i = 0; i < settings.AnimalMaximumMutationCount; i++)
                {
                    bool mutationSuccessful = Brain.Mutate(settings);
                    if (mutationSuccessful)
                    {
                        MutationCount++;

                        if (!settings.AnimalDisableSpeciesSigChangeByMutation) SpeciesSignature = new Double4d(Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6));
                    }
                    if (rand.NextDouble() > settings.AnimalMutationCountFactor) break;
                }
            }
        }

        public void UpdateAge(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                Age += 1;

                if (Age < 0)
                {
                    Radius = double.Lerp(0.5d, 0.5d * settings.AnimalEggRadiusRatio, -Age / (double)settings.AnimalEggHatchingTime);
                }
                else Radius = 0.5d;

                TimeSinceLastAttacked++;
                if (TimeSinceLastAttacked >= settings.AnimalUnderAttackTime) LastAttackedAngle = 0;
            }
        }

        // 速度に対する抵抗を適用する
        public void ApplyDrag(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                Velocity *= 1d - settings.Drag;
                AngularVelocity *= 1d - settings.AngularVelocityDrag;
            }
        }

        // ニューラルネットの更新処理
        public void UpdateBrain(Soup soup, SoupSettings settings)
        {
            if (IsAlive && Age >= 0)
            {
                AnimalVisionOutput animalVisionOutput = AnimalVision.Observe(soup, settings, Position, Angle, Index, SpeciesSignature, 25, 32, 8d, 0.5d);
                BrainInput brainInput = new BrainInput()
                {
                    VisionData = animalVisionOutput,
                    Velocity = double.Min(1d, Velocity.Magnitude / settings.MaximumEffectiveVelocity),
                    AngularVelocity = double.Min(1d, AngularVelocity / settings.MaximumEffectiveAngularVelocity),
                    Element = Element / settings.AnimalReproductionCost,
                    ReproductionProgress = ReproductionProgress / settings.AnimalReproductionCost,
                    Age = Age / (double)settings.AnimalLifespan,
                    Ate = Ate,
                    Attacked = Math.Ceiling(double.Max(0d, settings.AnimalUnderAttackTime - TimeSinceLastAttacked) / settings.AnimalUnderAttackTime),
                    AttackedAngle = LastAttackedAngle,
                    AttackSuccessful = AttackSuccessful,
                };

                Brain.UpdateBrain(brainInput);

                Velocity += Double2d.FromAngle01(Angle) * double.Max(-1d, double.Min(1d, Brain.Output.Acceleration)) * settings.AnimalMaximumAcceleration;
                AngularVelocity += double.Max(-1d, double.Min(1d, Brain.Output.Rotation)) * settings.AnimalMaximumAngularAcceleration;
            }
        }

        // 衝突判定の処理
        public void UpdateCollision(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // セルが属しているタイルの位置を取得
                Int2d tilePosition = soup.GetTilePositionFromTileIndex(TileIndex);

                int attackTargetIndex = -1;
                SoupObjectType attackTargetType = SoupObjectType.None;
                double attackTargetAngleAbs = 2;

                // セルが属しているタイルを中心とした5x5タイルにある動植物に対して衝突判定の処理を行う
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        // 現在処理の対象になっているタイルの位置を取得
                        Int2d targetTilePosition = tilePosition + new Int2d(x, y);

                        // 処理対象のタイルの位置がスープの内かどうかをチェックしてスープの外だったらスキップする
                        if (targetTilePosition.X >= 0 && targetTilePosition.X < settings.SizeX && targetTilePosition.Y >= 0 && targetTilePosition.Y < settings.SizeY)
                        {
                            // 処理対象のタイルのインデックスを取得
                            int targetTileIndex = soup.GetTileIndexFromTilePosition(targetTilePosition);

                            Tile targetTile = soup.Tiles[targetTileIndex];

                            if (targetTile.Type == TileType.Wall)
                            {
                                // 壁に対する衝突判定の計算
                                Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.25d), 0.356d);
                                Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.25d), 0.356d);
                                Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.75d), 0.356d);
                                Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.75d), 0.356d);
                                Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.5d, targetTilePosition.Y + 0.5d), 0.5d);
                            }
                            else
                            {
                                // 植物に対する衝突判定の計算
                                for (int i = 0; i < soup.Tiles[targetTileIndex].PlantPopulation; i++)
                                {
                                    // 衝突判定の処理の相手になる植物を取得
                                    int targetIndex = targetTile.PlantIndexes[i];

                                    // 衝突判定の処理の本体
                                    Plant? targetPlant = soup.Plants[targetIndex];
                                    if (targetPlant is not null)
                                    {
                                        double targetRadius = targetPlant.Radius;
                                        Double2d targetPosition = targetPlant.Position;
                                        double targetMass = targetPlant.Mass;

                                        Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);

                                        // ニューラルネットのEat出力の値が0より大きくかつEat出力の値がAttack出力の値より大きい場合、正面にある植物をターゲットとしてマークする
                                        if (Age >= 0 && Brain.Output.Eat > 0 && Brain.Output.Eat > Brain.Output.Attack)
                                        {
                                            double targetAngleAbs = double.Abs(Double2d.ToAngle01(Double2d.Rotate01(targetPosition - Position, -Angle)));

                                            if (targetAngleAbs <= 0.125d && Double2d.DistanceSquared(Position, targetPosition) < (Radius + targetRadius + 0.25d) * (Radius + targetRadius + 0.25d))
                                            {
                                                if (targetAngleAbs < attackTargetAngleAbs)
                                                {
                                                    attackTargetIndex = targetIndex;
                                                    attackTargetType = SoupObjectType.Plant;
                                                    attackTargetAngleAbs = targetAngleAbs;
                                                }
                                            }
                                        }
                                    }
                                }

                                // 動物に対する衝突判定の計算
                                for (int i = 0; i < soup.Tiles[targetTileIndex].AnimalPopulation; i++)
                                {
                                    // 衝突判定の処理の相手になる植物を取得
                                    int targetIndex = targetTile.AnimalIndexes[i];

                                    // 自分自身に対しては衝突判定の処理は行わない
                                    if (targetIndex != Index)
                                    {
                                        // 衝突判定の処理の本体
                                        Animal? targetAnimal = soup.Animals[targetIndex];
                                        if (targetAnimal is not null)
                                        {
                                            double targetRadius = targetAnimal.Radius;
                                            Double2d targetPosition = targetAnimal.Position;
                                            double targetMass = targetAnimal.Mass;

                                            double distanceSqr = Double2d.DistanceSquared(Position, targetPosition);

                                            Velocity += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);

                                            // ニューラルネットのAttack出力の値が0より大きくかつAttack出力の値がEat出力の値より大きい場合、正面にある動物をターゲットとしてマークする
                                            if (Age >= 0 && Brain.Output.Attack > 0 && Brain.Output.Attack > Brain.Output.Eat && (SpeciesSignature != targetAnimal.SpeciesSignature || !settings.AnimalDisableSameSpeciesAttack) && targetAnimal.Age >= 0)
                                            {
                                                double targetAngleAbs = double.Abs(Double2d.ToAngle01(Double2d.Rotate01(targetPosition - Position, -Angle)));

                                                if (targetAngleAbs <= 0.125d && distanceSqr < (Radius + targetRadius + 0.25d) * (Radius + targetRadius + 0.25d))
                                                {
                                                    if (targetAngleAbs < attackTargetAngleAbs)
                                                    {
                                                        attackTargetIndex = targetIndex;
                                                        attackTargetType = SoupObjectType.Animal;
                                                        attackTargetAngleAbs = targetAngleAbs;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Ate = 0;

                if (attackTargetType == SoupObjectType.Plant)
                {
                    Plant? targetPlant = soup.Plants[attackTargetIndex];

                    if (targetPlant is not null)
                    {
                        double elementBuffer = 0d;
                        lock (targetPlant.LockObject)
                        {
                            elementBuffer += double.Max(double.Min(double.Min(settings.AnimalPlantIngestionRate, targetPlant.Element), settings.AnimalMaximumElementAmount - Element), 0d);
                            targetPlant.Element -= elementBuffer;

                            targetPlant.TimeSinceLastAttacked = 0;

                            Ate = 1;
                        }

                        lock (LockObject)
                        {
                            Element += elementBuffer * soup.ElementAmountMultiplier;
                        }
                    }
                }

                AttackSuccessful = 0;

                if (attackTargetType == SoupObjectType.Animal)
                {
                    Animal? targetAnimal = soup.Animals[attackTargetIndex];

                    if (targetAnimal is not null)
                    {
                        double elementBuffer = 0d;
                        lock (targetAnimal.LockObject)
                        {
                            double fromTargetAngle = Double2d.ToAngle01(Double2d.Rotate01(Position - targetAnimal.Position, -targetAnimal.Angle));

                            targetAnimal.TimeSinceLastAttacked = 0;
                            targetAnimal.LastAttackedAngle = fromTargetAngle;

                            // 実際にエレメントを奪い取るためには「相手がこちらを向いていない」または「自身のNNのAttack出力の値が相手のNNのAttack出力の値より大きい」必要がある
                            if (double.Abs(fromTargetAngle) > 0.125d || Brain.Output.Attack > targetAnimal.Brain.Output.Attack)
                            {
                                elementBuffer += double.Max(double.Min(double.Min(settings.AnimalAnimalIngestionRate, targetAnimal.Element), settings.AnimalMaximumElementAmount - Element), 0d);
                                targetAnimal.Element -= elementBuffer;

                                AttackSuccessful = 1;
                            }
                        }

                        lock (LockObject)
                        {
                            Element += elementBuffer * soup.ElementAmountMultiplier;
                        }
                    }
                }
            }
        }

        // 位置の更新
        public void UpdatePosition(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // effectiveVelocityをMaximumEffectiveVelocity以下に制限する
                Double2d effectiveVelocity = Velocity;
                if (effectiveVelocity.Magnitude > settings.MaximumEffectiveVelocity) effectiveVelocity *= settings.MaximumEffectiveVelocity / effectiveVelocity.Magnitude;

                // 位置を更新する
                Position += effectiveVelocity;

                // 位置がスープの外側だったらスープの中になるようにする
                if (Position.X < Radius)
                {
                    Position = new Double2d(Radius, Position.Y);
                    Velocity = new Double2d(-Velocity.X, Velocity.Y);
                }
                if (Position.X > settings.SizeX - Radius)
                {
                    Position = new Double2d(settings.SizeX - Radius, Position.Y);
                    Velocity = new Double2d(-Velocity.X, Velocity.Y);
                }
                if (Position.Y < Radius)
                {
                    Position = new Double2d(Position.X, Radius);
                    Velocity = new Double2d(Velocity.X, -Velocity.Y);
                }
                if (Position.Y > settings.SizeY - Radius)
                {
                    Position = new Double2d(Position.X, settings.SizeY - Radius);
                    Velocity = new Double2d(Velocity.X, -Velocity.Y);
                }

                // 位置を更新した後の自身が属しているタイルのインデックスを取得する
                int currentTileIndex = soup.GetTileIndexFromPosition(Position);

                // 位置を更新する前とした後で属するタイルのインデックスが変わっていたらタイル側のインデックス情報を更新する
                if (TileIndex != currentTileIndex)
                {
                    lock (soup.Tiles[TileIndex].LockObject)
                    {
                        soup.Tiles[TileIndex].AnimalIndexes.Remove(Index);
                    }
                    lock (soup.Tiles[currentTileIndex].LockObject)
                    {
                        soup.Tiles[currentTileIndex].AnimalIndexes.Add(Index);
                    }

                    TileIndex = currentTileIndex;
                }

                double effectiveAngularVelocity = AngularVelocity;
                if (double.Abs(effectiveAngularVelocity) > settings.MaximumEffectiveAngularVelocity) effectiveAngularVelocity *= settings.MaximumEffectiveAngularVelocity / double.Abs(effectiveAngularVelocity);
                Angle += effectiveAngularVelocity;
                if (Angle > 0.5d) Angle -= 1d;
                if (Angle <= -0.5d) Angle += 1d;
            }
        }

        // エレメントの消費とフェロモンの生産
        public void LosingElement(Soup soup, SoupSettings settings)
        {
            if (IsAlive && Age >= 0)
            {
                Tile targetTile = soup.Tiles[TileIndex];

                lock (targetTile.LockObject)
                {
                    double elementCost = 0;
                    elementCost += settings.AnimalElementBaseCost;
                    elementCost += settings.AnimalElementAccelerationCost * double.Min(1d, double.Abs(Brain.Output.Acceleration));
                    elementCost += settings.AnimalElementRotationCost * double.Min(1d, double.Abs(Brain.Output.Rotation));
                    if (Brain.Output.Eat > 0 && Brain.Output.Eat > Brain.Output.Attack) elementCost += settings.AnimalElementEatCost;
                    if (Brain.Output.Attack > 0 && Brain.Output.Attack > Brain.Output.Eat) elementCost += settings.AnimalElementAttackCost * double.Max(1d, Math.Sqrt(Brain.Output.Attack));
                    if (Brain.Output.PheromoneRedProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneRedProduction) * settings.AnimalElementPheromoneProductionCost;
                    if (Brain.Output.PheromoneGreenProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneGreenProduction) * settings.AnimalElementPheromoneProductionCost;
                    if (Brain.Output.PheromoneBlueProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneBlueProduction) * settings.AnimalElementPheromoneProductionCost;

                    elementCost = double.Min(Element, elementCost);
                    Element -= elementCost;

                    double reproductionRate = 0;
                    if (Brain.Output.Reproduction > 0) reproductionRate = double.Min(Element, settings.AnimalMaximumReproductionRate * double.Max(0d, double.Min(1d, Brain.Output.Reproduction)));
                    Element -= reproductionRate;
                    ReproductionProgress += reproductionRate;

                    lock (targetTile.LockObject)
                    {
                        targetTile.Element += elementCost * soup.ElementAmountMultiplier;

                        targetTile.PheromoneRed += double.Max(0d, double.Min(1d, Brain.Output.PheromoneRedProduction)) * settings.AnimalPheromoneProductionRate;
                        targetTile.PheromoneGreen += double.Max(0d, double.Min(1d, Brain.Output.PheromoneGreenProduction)) * settings.AnimalPheromoneProductionRate;
                        targetTile.PheromoneBlue += double.Max(0d, double.Min(1d, Brain.Output.PheromoneBlueProduction)) * settings.AnimalPheromoneProductionRate;
                    }

                    Mass = Element + ReproductionProgress;

                    ElementLossRate = -(elementCost + reproductionRate);
                    ReproductionRate = reproductionRate;
                }
            }
        }

        // 子孫の生成
        public Animal? CreateOffspring(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // エレメントの量がPlantMaximumElementAmount以上であれば子孫を生成して自身は死滅する
                if (ReproductionProgress >= settings.AnimalReproductionCost)
                {
                    Random rand = new Random();

                    Animal result = new Animal(settings, Position + Double2d.FromAngle01(Angle + 0.5d) * 0.25d, Angle + 0.5d, settings.AnimalReproductionCost, this);

                    ReproductionProgress -= settings.AnimalReproductionCost;

                    OffspringCount++;

                    return result;
                }
            }
            return null;
        }

        // セルが生きていない時の処理
        public void IsNotAlive(Soup soup, SoupSettings settings)
        {
            Tile targetTile = soup.Tiles[TileIndex];

            // エレメント量が0以下であるかセルが壁の中に埋まっているならそのセルは死んでいるものとして扱う
            if (Element <= 0) IsAlive = false;
            if (targetTile.Type == TileType.Wall) IsAlive = false;
            if (Age >= settings.AnimalLifespan) IsAlive = false;

            if (!IsAlive)
            {
                // タイル側のインデックス情報から自身のインデックスを削除し、タイルが壁でなければ残っているエレメントをタイルに追加する
                targetTile.AnimalIndexes.Remove(Index);
                if (targetTile.Type == TileType.Default) targetTile.Element += (double.Max(0d, Element) + ReproductionProgress) * soup.ElementAmountMultiplier;
            }
        }
    }
}
