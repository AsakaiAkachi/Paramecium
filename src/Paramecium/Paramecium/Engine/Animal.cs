using Paramecium.Variables;
using System.Text.Json.Serialization;

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
        public Double2d VelocityBuffer = Double2d.Zero;
        public double Angle { get; set; } = 0;                      // セルの角度
        public double AngularVelocity { get; set; } = 0;            // セルの角速度
        public double AngularVelocityBuffer = 0;
        public double Radius { get; set; } = 0;                     // セルの半径
        public double Element { get; set; } = 0;                    // セルのエレメント量
        public double ElementBuffer = 0;
        public double ReproductionProgress { get; set; } = 0;       // 繁殖の進捗
        public double Mass { get; set; } = 0;                       // セルの質量

        public double ElementGainLoss { get; set; } = 0;            // エレメントのステップ毎の獲得/喪失量
        public double ElementGainLossBuffer = 0;

        public double ElementCostPerStep { get; set; } = 0;         // エレメントのステップごとの消費速度
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

            Position = position;
            Angle = angle;
            Radius = 0.5d;
            Element = element;
            Mass = element;

            TimeSinceLastAttacked = settings.AnimalUnderAttackTime;

            SpeciesSignature = new Double4d(Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6));

            Brain = Brain.DefaultBrain;
        }
        public Animal(SoupSettings settings, Double2d position, double angle, double element, Animal parent)    // 動物が生殖するとき用のコンストラクター
        {
            Random rand = new Random();

            IsAlive = true;

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

            // 突然変異の処理
            int mutationCount = Brain.TryMutation(settings);
            if (mutationCount > 0)
            {
                MutationCount += mutationCount;
                if (!settings.AnimalMutationDisableSpeciesSigChangeByMutation) SpeciesSignature = new Double4d(Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6), Math.Round(rand.NextDouble(), 6));
            }
        }

        // 現在のステップの処理が始まった際に最初に実行されるメソッド
        public void OnStepStart(Soup soup, SoupSettings settings)
        {
            VelocityBuffer = Velocity;
            AngularVelocityBuffer = AngularVelocity;
            ElementBuffer = Element;
        }
        
        // 年齢等の時間経過で変化するパラメーターの更新処理
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
                VelocityBuffer *= 1d - settings.SoupDrag;
                AngularVelocityBuffer *= 1d - settings.SoupAngularVelocityDrag;

                if (VelocityBuffer.MagnitudeSquared < 0.000001d * 0.000001d) VelocityBuffer = Double2d.Zero;
                if (AngularVelocityBuffer < 0.000001d) AngularVelocityBuffer = 0d;
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
                    Velocity = double.Min(1d, Velocity.Magnitude / settings.SoupMaximumEffectiveVelocity),
                    AngularVelocity = double.Min(1d, AngularVelocity / settings.SoupMaximumEffectiveAngularVelocity),
                    Element = ElementBuffer / settings.AnimalReproductionCost,
                    ReproductionProgress = ReproductionProgress / settings.AnimalReproductionCost,
                    Age = Age / (double)settings.AnimalLifespan,
                    Ate = Ate,
                    Attacked = Math.Ceiling(double.Max(0d, settings.AnimalUnderAttackTime - TimeSinceLastAttacked) / settings.AnimalUnderAttackTime),
                    AttackedAngle = LastAttackedAngle,
                    AttackSuccessful = AttackSuccessful,
                };

                Brain.UpdateBrain(brainInput);

                // ニューラルネットの出力による加速と回転を適用する
                VelocityBuffer += Double2d.FromAngle01(Angle) * double.Max(-1d, double.Min(1d, Brain.Output.Acceleration)) * settings.AnimalMaximumAcceleration;
                AngularVelocityBuffer += double.Max(-1d, double.Min(1d, Brain.Output.Rotation)) * settings.AnimalMaximumAngularAcceleration;
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
                    // 消費するエレメントの量を計算する
                    double elementCost = 0;
                    elementCost += settings.AnimalElementBaseCost;
                    elementCost += settings.AnimalElementAccelerationCost * double.Min(1d, double.Abs(Brain.Output.Acceleration));
                    elementCost += settings.AnimalElementRotationCost * double.Min(1d, double.Abs(Brain.Output.Rotation));
                    if (Brain.Output.Eat > 0 && Brain.Output.Eat > Brain.Output.Attack) elementCost += settings.AnimalElementEatCost;
                    if (Brain.Output.Attack > 0 && Brain.Output.Attack > Brain.Output.Eat) elementCost += settings.AnimalElementAttackCost * double.Max(1d, Math.Sqrt(Brain.Output.Attack));
                    if (Brain.Output.PheromoneRedProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneRedProduction) * settings.AnimalElementPheromoneProductionCost;
                    if (Brain.Output.PheromoneGreenProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneGreenProduction) * settings.AnimalElementPheromoneProductionCost;
                    if (Brain.Output.PheromoneBlueProduction > 0) elementCost += double.Min(1d, Brain.Output.PheromoneBlueProduction) * settings.AnimalElementPheromoneProductionCost;

                    // エレメントを消費する
                    elementCost = double.Min(ElementBuffer, elementCost);
                    ElementBuffer -= elementCost;

                    // ReproductionRateを計算してエレメントをReproductionProgressに移動する
                    double reproductionRate = 0;
                    if (Brain.Output.Reproduction > 0) reproductionRate = double.Min(ElementBuffer, settings.AnimalMaximumReproductionRate * double.Max(0d, double.Min(1d, Brain.Output.Reproduction)));
                    ElementBuffer -= reproductionRate;
                    ReproductionProgress += reproductionRate;

                    // 消費したエレメントはタイルに放出される
                    lock (targetTile.LockObject)
                    {
                        targetTile.ElementBuffer += elementCost * soup.ElementAmountMultiplier;

                        targetTile.PheromoneRedBuffer += double.Max(0d, double.Min(1d, Brain.Output.PheromoneRedProduction)) * settings.AnimalPheromoneProductionRate;
                        targetTile.PheromoneGreenBuffer += double.Max(0d, double.Min(1d, Brain.Output.PheromoneGreenProduction)) * settings.AnimalPheromoneProductionRate;
                        targetTile.PheromoneBlueBuffer += double.Max(0d, double.Min(1d, Brain.Output.PheromoneBlueProduction)) * settings.AnimalPheromoneProductionRate;
                    }

                    Mass = ElementBuffer + ReproductionProgress;

                    ElementCostPerStep = -(elementCost + reproductionRate);
                    ReproductionRate = reproductionRate;
                }
            }
        }

        // 衝突判定の処理
        public void UpdateCollision(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // セルが属しているタイルの位置を取得
                Int2d tilePosition = soup.GetTilePositionFromTileIndex(TileIndex);

                int attackTargetIndex = -1;                             // 攻撃対象のインデックス
                SoupObjectType attackTargetType = SoupObjectType.None;  // 攻撃対象のタイプ
                double attackTargetAngleAbs = 2;                        // 攻撃対象の自身から見た角度の絶対値

                // セルが属しているタイルを中心とした5x5タイルにある動植物に対して衝突判定の処理を行う
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        // 現在処理の対象になっているタイルの位置を取得
                        Int2d targetTilePosition = tilePosition + new Int2d(x, y);

                        // 処理対象のタイルの位置がスープの内かどうかをチェックしてスープの外だったらスキップする
                        if (targetTilePosition.X >= 0 && targetTilePosition.X < settings.SoupSizeX && targetTilePosition.Y >= 0 && targetTilePosition.Y < settings.SoupSizeY)
                        {
                            // 処理対象のタイルのインデックスを取得
                            int targetTileIndex = soup.GetTileIndexFromTilePosition(targetTilePosition);

                            Tile targetTile = soup.Tiles[targetTileIndex];

                            if (targetTile.Type == TileType.Wall)
                            {
                                if (x >= -1 && x <= 1 && y >= -1 && y <= 1)
                                {
                                    // 壁に対する衝突判定の計算
                                    VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.25d), 0.356d);
                                    VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.25d), 0.356d);
                                    VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.75d), 0.356d);
                                    VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.75d), 0.356d);
                                    VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.5d, targetTilePosition.Y + 0.5d), 0.5d);
                                }
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

                                        if (x >= -1 && x <= 1 && y >= -1 && y <= 1) VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);

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

                                            if (x >= -1 && x <= 1 && y >= -1 && y <= 1) VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);

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

                // 植物を食べる処理
                Ate = 0;
                if (attackTargetType == SoupObjectType.Plant)
                {
                    Plant? targetPlant = soup.Plants[attackTargetIndex];

                    if (targetPlant is not null)
                    {
                        // ターゲットになっている植物からエレメントを奪い取る
                        double elementBuffer = 0d;
                        lock (targetPlant.LockObject)
                        {
                            elementBuffer += double.Max(double.Min(double.Min(settings.AnimalPlantIngestionRate, targetPlant.ElementBuffer), settings.AnimalMaximumElementAmount - ElementBuffer), 0d);
                            targetPlant.ElementBuffer -= elementBuffer;
                            targetPlant.ElementGainLossBuffer -= elementBuffer;

                            targetPlant.TimeSinceLastAttacked = 0;

                            Ate = 1;
                        }

                        lock (LockObject)
                        {
                            ElementBuffer += elementBuffer * soup.ElementAmountMultiplier;
                            ElementGainLossBuffer += elementBuffer * soup.ElementAmountMultiplier;
                        }
                    }
                }

                // 動物を攻撃する処理
                AttackSuccessful = 0;
                if (attackTargetType == SoupObjectType.Animal)
                {
                    Animal? targetAnimal = soup.Animals[attackTargetIndex];

                    if (targetAnimal is not null)
                    {
                        // ターゲットになっている動物からエレメントを奪い取る
                        double elementBuffer = 0d;
                        lock (targetAnimal.LockObject)
                        {
                            double fromTargetAngle = Double2d.ToAngle01(Double2d.Rotate01(Position - targetAnimal.Position, -targetAnimal.Angle));

                            targetAnimal.TimeSinceLastAttacked = 0;
                            targetAnimal.LastAttackedAngle = fromTargetAngle;

                            // 実際にエレメントを奪い取るためには「相手がこちらを向いていない」または「自身のNNのAttack出力の値が相手のNNのAttack出力の値より大きい」必要がある
                            if (double.Abs(fromTargetAngle) > 0.125d || Brain.Output.Attack > targetAnimal.Brain.Output.Attack)
                            {
                                elementBuffer += double.Max(double.Min(double.Min(settings.AnimalAnimalIngestionRate, targetAnimal.ElementBuffer), settings.AnimalMaximumElementAmount - ElementBuffer), 0d);
                                targetAnimal.ElementBuffer -= elementBuffer;
                                targetAnimal.ElementGainLossBuffer -= elementBuffer;

                                AttackSuccessful = 1;
                            }
                        }

                        lock (LockObject)
                        {
                            ElementBuffer += elementBuffer * soup.ElementAmountMultiplier;
                            ElementGainLossBuffer += elementBuffer * soup.ElementAmountMultiplier;
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
                Velocity = VelocityBuffer;
                AngularVelocity = AngularVelocityBuffer;

                // effectiveVelocityをMaximumEffectiveVelocity以下に制限する
                Double2d effectiveVelocity = Velocity;
                if (effectiveVelocity.Magnitude > settings.SoupMaximumEffectiveVelocity) effectiveVelocity *= settings.SoupMaximumEffectiveVelocity / effectiveVelocity.Magnitude;

                // 位置を更新する
                Position += effectiveVelocity;

                // 位置がスープの外側だったらスープの中になるようにする
                if (Position.X < Radius)
                {
                    Position = new Double2d(Radius, Position.Y);
                    Velocity = new Double2d(-Velocity.X, Velocity.Y);
                }
                if (Position.X > settings.SoupSizeX - Radius)
                {
                    Position = new Double2d(settings.SoupSizeX - Radius, Position.Y);
                    Velocity = new Double2d(-Velocity.X, Velocity.Y);
                }
                if (Position.Y < Radius)
                {
                    Position = new Double2d(Position.X, Radius);
                    Velocity = new Double2d(Velocity.X, -Velocity.Y);
                }
                if (Position.Y > settings.SoupSizeY - Radius)
                {
                    Position = new Double2d(Position.X, settings.SoupSizeY - Radius);
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

                // effectiveAngularVelocityをMaximumEffectiveAngularVelocity以下に制限する
                double effectiveAngularVelocity = AngularVelocity;
                if (double.Abs(effectiveAngularVelocity) > settings.SoupMaximumEffectiveAngularVelocity) effectiveAngularVelocity *= settings.SoupMaximumEffectiveAngularVelocity / double.Abs(effectiveAngularVelocity);

                // 角度を更新する
                Angle += effectiveAngularVelocity;
                if (Angle > 0.5d) Angle -= 1d;
                if (Angle <= -0.5d) Angle += 1d;
            }
        }

        // 子孫の生成
        public Animal? CreateOffspring(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // ReproductionProgressがAnimalReproductionCost以上であれば子孫を生成する
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
            if (ElementBuffer <= 0) IsAlive = false;
            if (targetTile.Type == TileType.Wall) IsAlive = false;
            if (Age >= settings.AnimalLifespan) IsAlive = false;

            if (!IsAlive)
            {
                // セルが壁に埋まっていなければ持っているエレメントをを全てタイルに放出する
                if (targetTile.Type == TileType.Default) targetTile.ElementBuffer += (double.Max(0d, ElementBuffer) + ReproductionProgress) * soup.ElementAmountMultiplier;
            }
        }

        // 現在のステップの処理が終わる直前に実行されるメソッド
        public void OnStepEnd(Soup soup, SoupSettings settings)
        {
            Element = ElementBuffer;
            ElementGainLoss = ElementGainLossBuffer;
            ElementGainLossBuffer = 0;
        }
    }
}
