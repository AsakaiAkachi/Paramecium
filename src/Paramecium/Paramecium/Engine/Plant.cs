using Paramecium.Variables;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // 植物のデータを保存する用のクラス
    public class Plant
    {
        public bool IsAlive { get; set; } = false;                  // セルが生きているかどうか

        public int Index { get; set; } = -1;                        // セルのインデックス
        public long Id { get; set; } = -1;                          // セルのID

        public long Generation { get; set; } = 1;                   // セルの世代数
        public int Age { get; set; } = 0;                           // セルの年齢

        public Double2d Position { get; set; } = Double2d.Zero;     // セルの位置
        public int TileIndex { get; set; } = 0;                     // セルが属しているタイルのインデックス
        public Double2d Velocity { get; set; } = Double2d.Zero;     // セルの速度
        private Double2d VelocityBuffer = Double2d.Zero;
        public double Radius { get; set; } = 0;                     // セルの半径
        public double Element { get; set; } = 0;                    // セルのエレメント量
        public double ElementBuffer = 0;
        public double Mass { get; set; } = 0;                       // セルの質量

        public double ElementGainLoss { get; set; } = 0;            // エレメントのステップ毎の獲得/喪失量
        public double ElementGainLossBuffer = 0;

        public int TimeSinceLastAttacked { get; set; } = 0;         // セルが最後に攻撃されてからの経過時間

        public object LockObject = new object();                    // セルのパラメーターを複数スレッドから操作する際の排他制御用オブジェクト

        public Plant() { }
        public Plant(SoupSettings settings, Double2d position, double element)
        {
            Random rand = new Random();

            IsAlive = true;

            Position = position;
            Radius = CalcRadius(settings, element);
            Element = element;
            Mass = element;

            TimeSinceLastAttacked = settings.PlantUnderAttackTime;
        }
        public Plant(SoupSettings settings, Double2d position, double element, Plant parent)    // 植物が分裂するとき用のコンストラクター
        {
            Random rand = new Random();

            IsAlive = true;

            Generation = parent.Generation + 1;

            Position = position;
            Radius = CalcRadius(settings, element);
            Element = element;
            Mass = element;

            TimeSinceLastAttacked = settings.PlantUnderAttackTime;
        }

        // 現在のステップの処理が始まった際に最初に実行されるメソッド
        public void OnStepStart(Soup soup, SoupSettings settings)
        {
            VelocityBuffer = Velocity;
            ElementBuffer = Element;
        }

        // 年齢等の時間経過で変化するパラメーターの更新処理
        public void UpdateAge(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                Age += 1;

                TimeSinceLastAttacked++;
            }
        }

        // 速度に対する抵抗を適用する
        public void ApplyDrag(Soup soup, SoupSettings settings)
        {
            if (IsAlive && Age >= settings.PlantSpreadingTime)
            {
                VelocityBuffer *= 1d - settings.SoupDrag;

                if (VelocityBuffer.MagnitudeSquared < 0.000001d * 0.000001d) VelocityBuffer = Double2d.Zero;
            }
        }

        // 衝突判定の処理
        public void UpdateCollision(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // セルが属しているタイルの位置を取得
                Int2d tilePosition = soup.GetTilePositionFromTileIndex(TileIndex);

                double WallRestitutionCoefficientMultiplier = 1d;
                if (Velocity.MagnitudeSquared > settings.SoupMaximumEffectiveVelocity * settings.SoupMaximumEffectiveVelocity) WallRestitutionCoefficientMultiplier = double.Max(1d, Velocity.Magnitude / settings.SoupMaximumEffectiveVelocity);

                // セルが属しているタイルを中心とした3x3タイルにある植物に対して衝突判定の処理を行う
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
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
                                // 壁に対する衝突判定の計算
                                VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.25d), 0.356d) * WallRestitutionCoefficientMultiplier;
                                VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.25d), 0.356d) * WallRestitutionCoefficientMultiplier;
                                VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.25d, targetTilePosition.Y + 0.75d), 0.356d) * WallRestitutionCoefficientMultiplier;
                                VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.75d, targetTilePosition.Y + 0.75d), 0.356d) * WallRestitutionCoefficientMultiplier;
                                VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, new Double2d(targetTilePosition.X + 0.5d, targetTilePosition.Y + 0.5d), 0.5d) * WallRestitutionCoefficientMultiplier;
                            }
                            else
                            {
                                // 植物に対する衝突判定の計算
                                for (int i = 0; i < soup.Tiles[targetTileIndex].PlantPopulation; i++)
                                {
                                    // 衝突判定の処理の相手になる植物を取得
                                    int targetIndex = targetTile.PlantIndexes[i];

                                    // 自分自身に対しては衝突判定の処理は行わない
                                    if (targetIndex != Index)
                                    {
                                        // 衝突判定の処理の本体
                                        Plant? targetPlant = soup.Plants[targetIndex];
                                        if (targetPlant is not null)
                                        {
                                            double targetRadius = targetPlant.Radius;
                                            Double2d targetPosition = targetPlant.Position;
                                            double targetMass = targetPlant.Mass;

                                            VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);
                                        }
                                    }
                                }

                                // 動物に対する衝突判定の計算
                                for (int i = 0; i < soup.Tiles[targetTileIndex].AnimalPopulation; i++)
                                {
                                    // 衝突判定の処理の相手になる動物を取得
                                    int targetIndex = targetTile.AnimalIndexes[i];

                                    // 衝突判定の処理の本体
                                    Animal? targetAnimal = soup.Animals[targetIndex];
                                    if (targetAnimal is not null)
                                    {
                                        double targetRadius = targetAnimal.Radius;
                                        Double2d targetPosition = targetAnimal.Position;
                                        double targetMass = targetAnimal.Mass;

                                        VelocityBuffer += soup.CalculateCollisionTwoObjects(Position, Radius, Mass, targetPosition, targetRadius, targetMass);
                                    }
                                }
                            }
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
                        soup.Tiles[TileIndex].PlantIndexes.Remove(Index);
                    }
                    lock (soup.Tiles[currentTileIndex].LockObject)
                    {
                        soup.Tiles[currentTileIndex].PlantIndexes.Add(Index);
                    }

                    TileIndex = currentTileIndex;
                }
            }
        }

        // タイルのエレメントの収集
        public void CollectElement(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                if (TimeSinceLastAttacked >= settings.PlantUnderAttackTime)
                {
                    Tile targetTile = soup.Tiles[TileIndex];

                    double elementBuffer = 0;
                    lock (targetTile.LockObject)
                    {
                        elementBuffer = targetTile.ElementBuffer * settings.PlantElementCollectRate;
                        targetTile.ElementBuffer -= elementBuffer;

                        if (targetTile.ElementBuffer < 0) targetTile.ElementBuffer = 0;
                    }

                    ElementBuffer += elementBuffer * soup.ElementAmountMultiplier;
                    ElementGainLossBuffer += elementBuffer * soup.ElementAmountMultiplier;
                }

                Radius = CalcRadius(settings, ElementBuffer);
                Mass = ElementBuffer;
            }
        }

        // 子孫の生成
        public List<Plant>? CreateOffspring(Soup soup, SoupSettings settings)
        {
            if (IsAlive)
            {
                // エレメントの量がPlantMaximumElementAmount以上であれば子孫を生成して自身は死滅する
                if (ElementBuffer >= settings.PlantMaximumElementAmount)
                {
                    Random rand = new Random();

                    List<Plant> result = new List<Plant>();
                    int divisionCount = rand.Next(settings.PlantDivisionCountMin, settings.PlantDivisionCountMax + 1);

                    double[] offspringElementAmount = new double[divisionCount];
                    double offspringElementAmountTotal = 0d;

                    for (int i = 0; i < divisionCount; i++)
                    {
                        offspringElementAmount[i] = rand.NextDouble();
                        offspringElementAmountTotal += offspringElementAmount[i];
                    }
                    for (int i = 0; i < divisionCount; i++) offspringElementAmount[i] *= (1d / offspringElementAmountTotal) * ElementBuffer;

                    for (int i = 0; i < divisionCount; i++) result.Add(new Plant(settings, Position + Double2d.FromAngle01(rand.NextDouble()) * Radius * 0.1d, offspringElementAmount[i], this));

                    ElementBuffer = 0;
                    IsAlive = false;

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

            if (!IsAlive)
            {
                // セルが壁に埋まっていなければ持っているエレメントをを全てタイルに放出する
                if (targetTile.Type == TileType.Default) targetTile.ElementBuffer += double.Max(0d, ElementBuffer) * soup.ElementAmountMultiplier;
            }
        }

        // 現在のステップの処理が終わる直前に実行されるメソッド
        public void OnStepEnd(Soup soup, SoupSettings settings)
        {
            Element = ElementBuffer;
            ElementGainLoss = ElementGainLossBuffer;
            ElementGainLossBuffer = 0;
        }

        // 半径の計算
        public static double CalcRadius(SoupSettings settings, double element)
        {
            return Math.Min(0.5d, Math.Sqrt(element / settings.PlantMaximumElementAmount) * 0.5d);
        }
    }
}
