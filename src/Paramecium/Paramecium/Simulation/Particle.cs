using Paramecium.Libs;
using System;
using System.CodeDom;
using System.Drawing;
using System.Runtime;
using System.Security.Policy;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Paramecium.Simulation
{
    public class Particle
    {
        public int Index { get; set; }
        public long Id { get; set; }
        public ParticleType Type { get; set; }
        public bool IsAlive { get; set; }

        public bool IsGrabbed { get; set; }

        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; }

        public double Angle { get; set; }

        public Int2D GridPosition { get; set; }

        public double Radius { get; set; }
        public ColorInt3 Color { get; set; }

        public double Biomass { get; set; }
        public double BiomassStockpiled { get; set; }

        public double Mass { get; set; }

        public bool InCollision { get; set; }
        public bool CollisionDisabled { get; set; }

        public int NextGridBiomassCheck { get; set; }

        public Gene Genes { get; set; }
        public double HealthMax { get; set; }
        public double PlantPriority { get; set; }
        public double AnimalPriority { get; set; }

        public double Health { get; set; }
        public int TimeSinceLastDamage { get; set; }

        public int Generation { get; set; }
        public int Age { get; set; }
        public int OffspringCount { get; set; }

        public int TargetIndex { get; set; } = -1;
        public long TargetId { get; set; } = -1;

        public double RandomWalkTargetAngle { get; set; }

        //public double AgingFactor { get; set; } = 1;

        private int local_SizeX;
        private int local_SizeY;


        public Particle()
        {
            //local_SizeX = g_Soup.SizeX;
            //local_SizeY = g_Soup.SizeY;

            Type = ParticleType.Plant;
            IsAlive = false;
            Position = new Vector2D();
            Velocity = new Vector2D();
            GridPosition = new Int2D();
            Radius = 0;
            Color = new ColorInt3(0, 0, 0);
            Biomass = 0;
            InCollision = false;
        }

        public Particle(ParticleType type)
        {
            local_SizeX = g_Soup.SizeX;
            local_SizeY = g_Soup.SizeY;

            Type = type;
            IsAlive = true;
            Random rnd = new Random();
            Position = new Vector2D(rnd, 0, 0, local_SizeX, local_SizeY);
            GridPosition = Vector2D.ToGridPosition(Position);
            Velocity = new Vector2D();

            switch (type)
            {
                case ParticleType.Plant:
                    Biomass = rnd.NextDouble() * 8d + 4d;
                    Radius = 0.5d * Math.Max(Math.Sqrt(Biomass) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
                    Color = new ColorInt3(0, 63 + (int)(192 / 16 * Biomass), 0);
                    break;
                case ParticleType.Animal:
                    Genes = new Gene(rnd);
                    Angle = rnd.NextDouble() * 360d;
                    Radius = 0.5 * g_Soup.CellSizeMultiplier;
                    Biomass = 64d;
                    Color = new ColorInt3(Genes.GeneColorRed, Genes.GeneColorGreen, Genes.GeneColorBlue);
                    Health = Genes.GeneHealth * 10d;
                    Generation = 1;
                    Age = g_Soup.HatchingTime;
                    Mass = Math.Pow(Radius, 2);
                    break;
            }
        }
        public Particle(Vector2D position, double satiety)
        {
            local_SizeX = g_Soup.SizeX;
            local_SizeY = g_Soup.SizeY;

            Type = ParticleType.Plant;
            IsAlive = true;
            Random rnd = new Random();
            Position = position;
            GridPosition = Vector2D.ToGridPosition(Position);
            Velocity = new Vector2D();

            Biomass = satiety;
            Radius = 0.5d * Math.Max(Math.Sqrt(Biomass) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
            Color = new ColorInt3(0, 63 + (int)(192 / 16 * Biomass), 0);
        }
        public Particle(Particle parent, double satiety)
        {
            local_SizeX = g_Soup.SizeX;
            local_SizeY = g_Soup.SizeY;

            Random rnd = new Random();

            Type = parent.Type;
            IsAlive = true;
            Position = new Vector2D();
            Velocity = new Vector2D();
            Color = new ColorInt3();

            Biomass = satiety;

            if (parent.Type == ParticleType.Plant)
            {
                Position = parent.Position + Vector2D.FromAngle(rnd.NextDouble() * 360d) * 0.2d * g_Soup.CellSizeMultiplier;
                //Velocity = Vector2D.FromAngle(rnd.NextDouble() * 360d) * rnd.NextDouble() * 0.1d;
                Radius = 0.5d * Math.Max(Math.Sqrt(Biomass) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Biomass), 0);
                Age = -50;
            }
            else if (parent.Type == ParticleType.Animal)
            {
                Genes = new Gene(parent.Genes);
                Angle = parent.Angle + 180;
                Position = parent.Position + Vector2D.FromAngle(Angle) * parent.Radius * 0.75d;
                Radius = 0.25 * g_Soup.CellSizeMultiplier;
                Color = new ColorInt3(255, 127, 255);
                Health = Genes.GeneHealth * 10d;
                Generation = parent.Generation + 1;
                Age = g_Soup.HatchingTime * -1;
                Mass = Math.Pow(Radius, 2);
            }
            GridPosition = Vector2D.ToGridPosition(Position);
        }

        public void OnInitialize()
        {
            local_SizeX = g_Soup.SizeX;
            local_SizeY = g_Soup.SizeY;

            if (g_Soup.UnassignedParticleIds.Count == 0)
            {
                for (int i = g_Soup.Particles.Length * 2 - 1; i >= g_Soup.Particles.Length; i--)
                {
                    g_Soup.UnassignedParticleIds.Add(i);
                }
                Particle[] ParticlesArrayOld = g_Soup.Particles;
                g_Soup.Particles = new Particle[ParticlesArrayOld.Length * 2];
                ParticlesArrayOld.CopyTo(g_Soup.Particles, 0);
            }
            Index = g_Soup.UnassignedParticleIds[g_Soup.UnassignedParticleIds.Count - 1];
            g_Soup.UnassignedParticleIds.RemoveAt(g_Soup.UnassignedParticleIds.Count - 1);
            g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticles.Add(Index);
            g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticleCount++;

            Id = new Random().NextInt64(0, 4738381338321616896);

            RandomWalkTargetAngle = new Random().NextDouble() * 360d;

            if (Type == ParticleType.Plant)
            {
                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Biomass), 0);
                Radius = Math.Max(Math.Sqrt(Biomass) / 8d * g_Soup.CellSizeMultiplier, 0.025d);

                Mass = Math.Pow(Radius, 2);

                NextGridBiomassCheck = -100;

                g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalPlantCount++;
            }
            else
            {
                HealthMax = Genes.GeneHealth * 10d;
                PlantPriority = Math.Max(1d - (Genes.GeneDiet + 1d) / 2d * 1.5d, 0);
                AnimalPriority = (Genes.GeneDiet + 1d) / 2d;

                g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalAnimalCount++;
            }
        }

        public void OnLoaded()
        {
            local_SizeX = g_Soup.SizeX;
            local_SizeY = g_Soup.SizeY;
        }

        public void EarlyUpdate()
        {
            if (!IsGrabbed)
            {
                // 速度に抗力を適用する
                if (Type == ParticleType.Animal || (Type == ParticleType.Plant && Age >= 0))
                {
                    Velocity *= 0.9d;
                }

                if (InCollision)
                {
                    CollisionDisabled = false;
                }

                switch (Type)
                {
                    // 植物がタイルからバイオマスを吸収する処理
                    case ParticleType.Plant:
                        int PlantBiomassCollectionRange = g_Soup.PlantBiomassCollectionRange;

                        if (NextGridBiomassCheck < 0)
                        {
                            Random rnd = new Random();
                            int xOffset = 0;
                            int yOffset = 0;

                            for (int i = 0; i < PlantBiomassCollectionRange * 2; i++)
                            {
                                int xOffsetNext = xOffset;
                                int yOffsetNext = yOffset;

                                int shiftDirection = rnd.Next(0, 3);

                                if (shiftDirection == 0)
                                {
                                    if (rnd.Next(0, 2) == 0) xOffsetNext++;
                                    else xOffsetNext--;
                                }
                                else if (shiftDirection == 1)
                                {
                                    if (rnd.Next(0, 2) == 0) yOffsetNext++;
                                    else yOffsetNext--;
                                }
                                else { }

                                if (GridPosition.X + xOffsetNext < 0 || GridPosition.X + xOffsetNext >= local_SizeX || GridPosition.Y + yOffsetNext < 0 || GridPosition.Y + yOffsetNext >= local_SizeY) break;
                                else if (g_Soup.GridMap[(GridPosition.X + xOffsetNext) + (GridPosition.Y + yOffsetNext) * local_SizeX].Type == TileType.Wall) break;
                                else if (!(xOffsetNext >= PlantBiomassCollectionRange * -1 && xOffsetNext <= PlantBiomassCollectionRange && yOffsetNext >= PlantBiomassCollectionRange * -1 && yOffsetNext <= PlantBiomassCollectionRange)) break;
                                else
                                {
                                    xOffset = xOffsetNext;
                                    yOffset = yOffsetNext;
                                }
                            }

                            Grid TargetGrid = g_Soup.GridMap[(GridPosition.X + xOffset) + (GridPosition.Y + yOffset) * local_SizeX]; // バイオマスの吸収元となるグリッドを取得
                            if (TargetGrid.Fertility > 0)
                            {
                                // 吸収元グリッドからバイオマスを吸収
                                Biomass += Math.Min(0.15d, TargetGrid.Fertility);
                                TargetGrid.Fertility -= Math.Min(0.15d, TargetGrid.Fertility);

                                // 色と半径を再計算
                                Color = new ColorInt3(0, 63 + (int)(192 / 16 * Biomass), 0);
                                Radius = 0.5d * Math.Max(Math.Sqrt(Biomass) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);

                                // 質量を再計算
                                Mass = Math.Pow(Radius, 2);

                                CollisionDisabled = false;
                                NextGridBiomassCheck = -20;
                            }
                            else NextGridBiomassCheck++;
                        }
                        else if (NextGridBiomassCheck == 0)
                        {
                            // 自身から一定範囲内にバイオマスを含むグリッドがあるかどうかを調べる
                            // なければ20ステップの間バイオマスの収集の処理を一切行わず、20ステップ経過したらまた範囲内にバイオマスを含むグリッドがあるか調べる
                            // 範囲内にバイオマスを含むグリッドがあればバイオマスの吸収処理を再開する

                            bool reactivation = false;
                            for (int x = Math.Max(GridPosition.X - PlantBiomassCollectionRange, 0); x <= Math.Min(GridPosition.X + PlantBiomassCollectionRange, local_SizeX - 1); x++)
                            {
                                for (int y = Math.Max(GridPosition.Y - PlantBiomassCollectionRange, 0); y <= Math.Min(GridPosition.Y + PlantBiomassCollectionRange, local_SizeY - 1); y++)
                                {
                                    if (g_Soup.GridMap[x + y * local_SizeX].Fertility > 0)
                                    {
                                        reactivation = true;
                                        NextGridBiomassCheck = -100;
                                        break;
                                    }
                                }
                                if (reactivation) break;
                            }
                            if (!reactivation) NextGridBiomassCheck = 20;
                        }
                        else NextGridBiomassCheck--;



                        /**
                        if (!InCollision)
                        {
                            for (int x = Math.Max(GridPosition.X - 3, 0); x <= Math.Min(GridPosition.X + 3, local_env_SizeX - 1); x++)
                            {
                                for (int y = Math.Max(GridPosition.Y - 3, 0); y <= Math.Min(GridPosition.Y + 3, local_env_SizeY - 1); y++)
                                {
                                    Grid TargetGrid = Variables.SoupInstance.TileMap[x + y * local_env_SizeX];
                                    Satiety += TargetGrid.Fertility * 0.005d;
                                    TargetGrid.Fertility *= 0.995d;
                                }
                            }
                        }
                        **/
                        break;
                    case ParticleType.Animal:
                        if (Age > 0)
                        {
                            bool breakFlag = false;

                            int RaycastIteration = 20;
                            double RaycastRange = 10;

                            double RaycastResultWall = 0d;
                            double RaycastResultPlant = 0d;

                            Particle? Target = null;
                            double TargetDistance = RaycastRange;

                            // ターゲットが存在しているか調べる
                            if (TargetIndex != -1 && TargetId != -1)
                            {
                                if (g_Soup.Particles[TargetIndex] is not null)
                                {
                                    if (g_Soup.Particles[TargetIndex].IsAlive)
                                    {
                                        if (g_Soup.Particles[TargetIndex].Id == TargetId)
                                        {
                                            if (Vector2D.Size(g_Soup.Particles[TargetIndex].Position - Position) < RaycastRange)
                                            {
                                                Target = g_Soup.Particles[TargetIndex];
                                            }
                                            else
                                            {
                                                TargetIndex = -1;
                                                TargetId = -1;
                                            }
                                        }
                                        else
                                        {
                                            TargetIndex = -1;
                                            TargetId = -1;
                                        }
                                    }
                                    else
                                    {
                                        TargetIndex = -1;
                                        TargetId = -1;
                                    }
                                }
                                else
                                {
                                    TargetIndex = -1;
                                    TargetId = -1;
                                }
                            }

                            if (Target is not null) // ターゲットが存在している場合
                            {
                                // レイキャストのベクトルを設定する
                                Vector2D RaycastVector = Vector2D.Normalization(Target.Position - Position) * (RaycastRange / RaycastIteration);
                                Vector2D RaycastScannerVector = Vector2D.Rotate(Vector2D.Normalization(Target.Position - Position), 90) * (RaycastRange / RaycastIteration * 1.414213);

                                // 自身からターゲットまでの間のレイキャストを実行し、壁がないか確認する
                                // 壁があった場合はターゲットをリセットする
                                for (int j = 0; j <= RaycastIteration; j++)
                                {
                                    if (
                                        Position.X + RaycastVector.X < 0 || Position.X + RaycastVector.X >= local_SizeX ||
                                        Position.Y + RaycastVector.Y < 0 || Position.Y + RaycastVector.Y >= local_SizeY
                                    )
                                    {
                                        TargetDistance = RaycastRange;
                                        Target = null;
                                        TargetIndex = -1;
                                        TargetId = -1;
                                        break;
                                    }
                                    else
                                    {
                                        Int2D TargetGrid0 = Vector2D.ToGridPosition(Position + RaycastVector * j);
                                        Int2D TargetGrid1 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector * -1d);
                                        Int2D TargetGrid2 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector);

                                        if (g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_SizeX].Type == TileType.Wall)
                                        {
                                            Target = null;
                                            TargetIndex = -1;
                                            TargetId = -1;
                                            break;
                                        }
                                        else
                                        {
                                            int LocalParticlesCount = g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_SizeX].LocalParticleCount;
                                            if (LocalParticlesCount > 0)
                                            {
                                                if (Target.GridPosition == TargetGrid0)
                                                {
                                                    break;
                                                }
                                            }

                                            if (g_Soup.GridMap[TargetGrid1.X + TargetGrid1.Y * local_SizeX].Type == TileType.Wall)
                                            {
                                                Target = null;
                                                TargetIndex = -1;
                                                TargetId = -1;
                                                break;
                                            }
                                            if (g_Soup.GridMap[TargetGrid2.X + TargetGrid2.Y * local_SizeX].Type == TileType.Wall)
                                            {
                                                Target = null;
                                                TargetIndex = -1;
                                                TargetId = -1;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else // ターゲットが存在していなかった場合
                            {
                                // -90°～+90°までの範囲でレイキャストを実行する
                                for (int i = -15; i <= 15; i++)
                                {
                                    // レイキャストのベクトルを設定する
                                    Vector2D RaycastVector = Vector2D.FromAngle(Angle + (i * 6)) * (RaycastRange / RaycastIteration);
                                    Vector2D RaycastScannerVector = Vector2D.Rotate(RaycastVector, 90) * 1.414213;

                                    // 壁または生物に当たるまでレイキャストを実行する
                                    for (int j = 0; j <= RaycastIteration; j++)
                                    {
                                        if (
                                            Position.X + RaycastVector.X < 0 || Position.X + RaycastVector.X >= local_SizeX ||
                                            Position.Y + RaycastVector.Y < 0 || Position.Y + RaycastVector.Y >= local_SizeY
                                        ) // スープの端に当たった場合
                                        {
                                            RaycastResultWall += 1d * ((RaycastIteration - j) / (double)RaycastIteration);
                                            break;
                                        }
                                        else
                                        {
                                            Int2D TargetGrid0 = Vector2D.ToGridPosition(Position + RaycastVector * j);
                                            Int2D TargetGrid1 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector * -1d);
                                            Int2D TargetGrid2 = Vector2D.ToGridPosition(Position + RaycastVector * j + RaycastScannerVector);

                                            if (g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_SizeX].Type == TileType.Wall) // 壁に当たった場合
                                            {
                                                RaycastResultWall += 1d * ((RaycastIteration - j) / (double)RaycastIteration);
                                                break; // その角度のレイキャストを終了する
                                            }
                                            else
                                            {
                                                int LocalParticlesCount = g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_SizeX].LocalParticleCount;
                                                if (LocalParticlesCount > 0) // 生物に当たった場合
                                                {
                                                    List<int> LocalParticles = g_Soup.GridMap[TargetGrid0.X + TargetGrid0.Y * local_SizeX].LocalParticles;
                                                    for (int k = 0; k < LocalParticlesCount; k++)
                                                    {
                                                        Particle TargetParticle = g_Soup.Particles[LocalParticles[k]];

                                                        if (TargetParticle.Index == Index) { } // 自分自身が検出された場合は何もしない
                                                        else
                                                        {
                                                            bool IsValidTarget = false;

                                                            double TargetDistanceTemp = Vector2D.Size(TargetParticle.Position - Position) / (Math.Max(TargetParticle.Radius * 2d, 0.001d) / g_Soup.CellSizeMultiplier); // ターゲットまでの補正距離を求める

                                                            // 食性に応じてターゲットまでの距離にさらに補正をかける
                                                            if (TargetParticle.Type == ParticleType.Plant)
                                                            {
                                                                TargetDistanceTemp /= Math.Max(PlantPriority, 0.001d);
                                                                IsValidTarget = true;
                                                            }
                                                            else if (TargetParticle.Type == ParticleType.Animal)
                                                            {
                                                                TargetDistanceTemp /= Math.Max(AnimalPriority, 0.001d);
                                                                if (Math.Sqrt(Math.Pow(Color.Red - TargetParticle.Color.Red, 2) + Math.Pow(Color.Green - TargetParticle.Color.Green, 2) + Math.Pow(Color.Blue - TargetParticle.Color.Blue, 2)) > 16d && TargetParticle.Age >= 1) IsValidTarget = true;
                                                            }

                                                            // 補正後のターゲットまでの距離がすでにターゲット候補になっている生物より近い場合はターゲット候補を変更する
                                                            if (TargetDistanceTemp < TargetDistance && IsValidTarget)
                                                            {
                                                                Target = TargetParticle;
                                                                TargetDistance = TargetDistanceTemp;
                                                                TargetIndex = LocalParticles[k];
                                                                TargetId = TargetParticle.Id;
                                                            }
                                                            breakFlag = true;
                                                        }
                                                    }
                                                }

                                                if (breakFlag) break;

                                                // スキャン中のグリッドに隣接するグリッドが壁である場合はレイキャストを終了する
                                                if (g_Soup.GridMap[TargetGrid1.X + TargetGrid1.Y * local_SizeX].Type == TileType.Wall)
                                                {
                                                    break;
                                                }
                                                if (g_Soup.GridMap[TargetGrid2.X + TargetGrid2.Y * local_SizeX].Type == TileType.Wall)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            /**
                            if (RaycastResultPlant > 0)
                            {
                                Angle += RaycastResultPlant;
                                Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.25d;
                            }
                            else
                            {
                                Angle += RaycastResultWall;
                                Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.5d;
                            }
                            **/

                            if (Target is not null) // ターゲットが存在する場合
                            {
                                // ターゲットに向かって移動する
                                double TargetAngle = Vector2D.ToAngle(Target.Position - Position);
                                if (TargetAngle > 180d + Angle) TargetAngle -= 360d;
                                if (TargetAngle < -180d + Angle) TargetAngle += 360d;
                                //Angle += Math.Min(Math.Max(TargetAngle - Angle, -12 / AgingFactor), 12 / AgingFactor);
                                Angle += Math.Min(Math.Max(TargetAngle - Angle, -12), 12);

                                //Velocity += Vector2D.Normalization(Target.Position - Position) * (0.5d + Vector2D.Size(Target.Position - Position) / RaycastRange * 0.5d) * 0.01d;
                                //if (Target.Type == ParticleType.Plant) Velocity += Vector2D.FromAngle(Angle) * 0.01d * (0.2d + Vector2D.Size(Target.Position - Position) / RaycastRange * 0.8d) / AgingFactor;
                                //else if (Target.Type == ParticleType.Animal) Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.5d / AgingFactor;
                                if (Target.Type == ParticleType.Plant) Velocity += Vector2D.FromAngle(Angle) * 0.01d * (0.2d + Vector2D.Size(Target.Position - Position) / RaycastRange * 0.8d);
                                else if (Target.Type == ParticleType.Animal) Velocity += Vector2D.FromAngle(Angle) * 0.01d * 0.5d;
                            }
                            else // ターゲットが存在しない場合
                            {
                                // ランダムに方向を設定してそこにランダムな時間移動する
                                if (new Random().NextDouble() < 0.005d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                if (RandomWalkTargetAngle > 180d + Angle) RandomWalkTargetAngle -= 360d;
                                if (RandomWalkTargetAngle < -180d + Angle) RandomWalkTargetAngle += 360d;

                                //Angle += Math.Min(Math.Max(RandomWalkTargetAngle - Angle, -12 / AgingFactor), 12 / AgingFactor);
                                //Velocity += Vector2D.FromAngle(Angle) * 0.005d / AgingFactor;
                                Angle += Math.Min(Math.Max(RandomWalkTargetAngle - Angle, -12), 12);
                                Velocity += Vector2D.FromAngle(Angle) * 0.005d;
                            }

                            //g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_env_SizeX].Fertility += Math.Min(0.075d * Vector2D.Size(Velocity), Satiety);
                            //Satiety -= Math.Min(0.075d * Vector2D.Size(Velocity), Satiety);
                        }
                        break;
                }
            }
        }

        public void MiddleUpdate(int threadId)
        {
            if (!IsGrabbed)
            {
                if (!CollisionDisabled) // 当たり判定処理が無効化されていない場合のみ当たり判定処理を実行する
                {
                    InCollision = false;
                    for (int x = Math.Max((int)Math.Floor(Position.X - g_Soup.CellSizeMultiplier), 0); x <= Math.Min((int)Math.Ceiling(Position.X + g_Soup.CellSizeMultiplier), local_SizeX - 1); x++) // 自身に隣接するグリッドに対してforループで壁や生物がいないか調べる
                    {
                        for (int y = Math.Max((int)Math.Floor(Position.Y - g_Soup.CellSizeMultiplier), 0); y <= Math.Min((int)Math.Ceiling(Position.Y + g_Soup.CellSizeMultiplier), local_SizeY - 1); y++)
                        {
                            if (g_Soup.GridMap[x + y * local_SizeX].Type == TileType.Wall) // グリッドが壁であった場合
                            {
                                // 壁を構成する各当たり判定をチェックする

                                Vector2D WallPosition1 = new Vector2D(x + 0.25, y + 0.25);
                                Vector2D WallPosition2 = new Vector2D(x + 0.75, y + 0.25);
                                Vector2D WallPosition3 = new Vector2D(x + 0.25, y + 0.75);
                                Vector2D WallPosition4 = new Vector2D(x + 0.75, y + 0.75);
                                Vector2D WallPosition5 = new Vector2D(x + 0.5, y + 0.5);
                                double WallDistance1 = Vector2D.Distance(Position, WallPosition1);
                                double WallDistance2 = Vector2D.Distance(Position, WallPosition2);
                                double WallDistance3 = Vector2D.Distance(Position, WallPosition3);
                                double WallDistance4 = Vector2D.Distance(Position, WallPosition4);
                                double WallDistance5 = Vector2D.Distance(Position, WallPosition5);

                                if (WallDistance1 < 0.353553 + Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - WallPosition1) * ((Radius + 0.353553 - WallDistance1) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                                    InCollision = true;
                                    if (Type == ParticleType.Animal)
                                    {
                                        if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                    }
                                }
                                if (WallDistance2 < 0.353553 + Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - WallPosition2) * ((Radius + 0.353553 - WallDistance2) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                                    InCollision = true;
                                    if (Type == ParticleType.Animal)
                                    {
                                        if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                    }
                                }
                                if (WallDistance3 < 0.353553 + Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - WallPosition3) * ((Radius + 0.353553 - WallDistance3) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                                    InCollision = true;
                                    if (Type == ParticleType.Animal)
                                    {
                                        if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                    }
                                }
                                if (WallDistance4 < 0.353553 + Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - WallPosition4) * ((Radius + 0.353553 - WallDistance4) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                                    InCollision = true;
                                    if (Type == ParticleType.Animal)
                                    {
                                        if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                    }
                                }
                                if (WallDistance5 < 0.5 + Radius)
                                {
                                    Velocity += Vector2D.Normalization(Position - WallPosition5) * ((Radius + 0.5 - WallDistance5) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity), 0.01d);
                                    InCollision = true;
                                    if (Type == ParticleType.Animal)
                                    {
                                        if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                                    }
                                }

                            }
                            else // グリッドが壁でなく、かつ生物がいた場合
                            {
                                int LocalParticlesCount = g_Soup.GridMap[x + y * local_SizeX].LocalParticleCount;
                                if (LocalParticlesCount >= 1)
                                {
                                    List<int> TargetIds = g_Soup.GridMap[x + y * local_SizeX].LocalParticles;
                                    for (int i = 0; i < LocalParticlesCount; i++)
                                    {
                                        if (TargetIds[i] != Index)
                                        {
                                            Particle Target = g_Soup.Particles[TargetIds[i]];
                                            Vector2D collidedParticlePosition = Target.Position;
                                            double Distance = Vector2D.Distance(Position, collidedParticlePosition);
                                            if (Distance < Radius + Target.Radius) // 対象との距離が「自身の半径+対象の半径」未満である場合
                                            {
                                                Velocity += Vector2D.Normalization(Position - Target.Position) * ((Radius + Target.Radius - Distance) / (Radius * 0.5d)) * Math.Max(Vector2D.Size(Velocity + Target.Velocity), 0.01d) * Math.Min(Target.Mass / Mass, 1d);
                                                InCollision = true;
                                                Target.CollisionDisabled = false;
                                                Target.InCollision = true;

                                                if (Type == ParticleType.Animal) // 自身が動物の場合
                                                {
                                                    if (Target.Index == TargetIndex) // 対象がターゲットにしている生物の場合
                                                    {
                                                        if (Target.Type == ParticleType.Plant) // ターゲットが植物の場合
                                                        {
                                                            // 対象のバイオマスを削り取り、そのうち食性に応じた一定割合を自身のバイオマスにする
                                                            // 残りはグリッドのバイオマスになる
                                                            Biomass += Math.Min(PlantPriority * (0.5d + 0.375d * Genes.GeneStrength), Target.Biomass);
                                                            g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += Math.Min((1d - PlantPriority) * (0.5d + 0.375d * Genes.GeneStrength), Target.Biomass);
                                                            Target.Biomass -= Math.Min(0.5d + 0.375d * Genes.GeneStrength, Target.Biomass);
                                                            Target.Radius = 0.5d * Math.Max(Math.Sqrt(Target.Biomass) / g_Soup.PlantSizeMultiplier * g_Soup.CellSizeMultiplier, 0.01d);
                                                            Target.Mass = Math.Pow(Target.Radius, 2);
                                                        }
                                                        else if (Target.Type == ParticleType.Animal) // ターゲットが動物の場合
                                                        {
                                                            // 対象に攻撃を行い、対象のHPが0になったら対象のバイオマスのうち食性に応じた一定割合を自身のバイオマスにする
                                                            // 残りはグリッドのバイオマスになる
                                                            Target.TargetIndex = Index;
                                                            Target.TargetId = Id;

                                                            //Target.Velocity += Vector2D.Normalization(Target.Position - Position) * 0.02d;
                                                            Target.TimeSinceLastDamage = 100;

                                                            //Target.Health -= Math.Max((Genes.GeneStrength / AgingFactor) * 0.25d * Math.Min((Genes.GeneStrength / AgingFactor) / Math.Max(Target.Genes.GeneHardness, 0.01d), 1d) * Math.Min((Genes.GeneAgility / AgingFactor) / Math.Max((Target.Genes.GeneAgility / Target.AgingFactor), 0.01d), 1d), 0.1d) * (new Random().NextDouble() * 2d);


                                                            if (new Random().NextDouble() < 0.1d * Math.Pow(2d, Genes.GeneAgility - Target.Genes.GeneAgility))
                                                            {
                                                                Target.Health -= Math.Max(Genes.GeneStrength * (AnimalPriority * 0.5d + 0.5d) * (Genes.GeneStrength - Target.Genes.GeneHardness) * (new Random().NextDouble() * 2d), 0.1d);
                                                            }

                                                            if (Target.Health <= 0d)
                                                            {
                                                                Biomass += (Target.Biomass + Target.BiomassStockpiled) * AnimalPriority;
                                                                g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += (Target.Biomass + Target.BiomassStockpiled) * (1d - AnimalPriority);
                                                                Target.Biomass = 0;
                                                                Target.Health = 0;
                                                            }

                                                            //Satiety += Math.Min(0.1d, Target.Satiety) * ((Genes.GeneDiet + 1d) / 2d);
                                                            //g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += Math.Min(0.1d, Target.Satiety) * (1d - (Genes.GeneDiet + 1d) / 2d);
                                                        }

                                                        // 対象のバイオマスが0になったら対象は死亡する
                                                        if (Target.Biomass <= 0)
                                                        {
                                                            Target.IsAlive = false;

                                                            RandomWalkTargetAngle = Angle;

                                                            //if (new Random().NextDouble() < 0.1d)
                                                            //{
                                                            //    g_Soup.ParticlesBuffer[threadId].Add(new Particle(Position + (Vector2D.FromAngle(Angle + 180) * g_Soup.CellSizeMultiplier * 0.45d), 0));
                                                            //}
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
                }
            }
        }

        #region LateUpdate
        public void LateUpdate(int threadId)
        {
            if (!IsGrabbed)
            {
                if (Type == ParticleType.Plant)
                {
                    if (Biomass >= g_Soup.PlantForkBiomass)
                    {
                        Random rnd = new Random();

                        while (Biomass > 0)
                        {
                            double SatietySwap = Math.Min(rnd.NextDouble() * g_Soup.PlantForkBiomass / 4, Biomass);
                            g_Soup.ParticlesBuffer[threadId].Add(new Particle(this, SatietySwap));
                            Biomass -= SatietySwap;
                        }

                        IsAlive = false;

                        return;
                    }
                }
                if (Type == ParticleType.Animal)
                {
                    if (Age > 0)
                    {
                        if (Age <= g_Soup.HatchingTime) // ふ化後の大きさが徐々に大きくなっていく際の処理
                        {
                            Radius = 0.25 * g_Soup.CellSizeMultiplier + 0.25 * g_Soup.CellSizeMultiplier * (Math.Min(g_Soup.HatchingTime, Age) / (double)g_Soup.HatchingTime);
                            Mass = Math.Pow(Radius, 2);
                        }

                        if (Biomass > Genes.ForkCost)
                        {
                            BiomassStockpiled += Biomass - Genes.ForkCost;
                            Biomass = Genes.ForkCost;
                        }
                        if (BiomassStockpiled >= Genes.ForkCost) // 分裂時の処理
                        {
                            Random rnd = new Random();

                            g_Soup.ParticlesBuffer[threadId].Add(new Particle(this, Genes.ForkCost));
                            BiomassStockpiled -= Genes.ForkCost;

                            OffspringCount++;
                        }

                        // 最後にダメージを受けてから100ステップ以上たっていれば毎ステップ0.1ずつ体力を回復する
                        if (TimeSinceLastDamage > 0) TimeSinceLastDamage--;
                        else if (TimeSinceLastDamage == 0 && Health < HealthMax && Biomass > 0d)
                        {
                            g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += Math.Min(0.1, Biomass);
                            Biomass -= Math.Min(0.1d, Biomass);
                            Health = Math.Min(Health + 0.1d, HealthMax);
                        }
                        else if (TimeSinceLastDamage == 0 && Health >= HealthMax) TimeSinceLastDamage = -1;

                        if (Health > HealthMax) Health = HealthMax;

                        //g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += Math.Min(0.05d + (0.15d * Vector2D.Size(Velocity) * AgingFactor), Satiety);
                        //Satiety -= Math.Min(0.05d + (0.15d * Vector2D.Size(Velocity) * AgingFactor), Satiety);
                        g_Soup.GridMap[(GridPosition.X) + (GridPosition.Y) * local_SizeX].Fertility += Math.Min(0.05d + (0.15d * Vector2D.Size(Velocity)), Biomass);
                        Biomass -= Math.Min(0.05d + (0.15d * Vector2D.Size(Velocity)), Biomass);

                        //if (Age > g_Soup.AgingBeginsAge) AgingFactor = 1d + (double)Math.Max((Age - g_Soup.AgingBeginsAge) / (double)g_Soup.AgingAbilityDeclineBaseTime, 0);

                        if (Biomass <= 0)
                        {
                            if (BiomassStockpiled > 0)
                            {
                                Biomass += Math.Min(Genes.ForkCost / 10d, BiomassStockpiled);
                                BiomassStockpiled -= Math.Min(Genes.ForkCost / 10d, BiomassStockpiled);
                            }
                            else if (Health > 0)
                            {
                                Health -= 0.1d;
                            }
                            else IsAlive = false;
                        }
                    }

                    if (Age == 0)
                    {
                        //Radius = 0.5 * g_Soup.CellSizeMultiplier;
                        Color = new ColorInt3(Genes.GeneColorRed, Genes.GeneColorGreen, Genes.GeneColorBlue);
                    }
                }

                Age++;

                // 速度が0でなければ速度に応じて移動させる
                if (Velocity != Vector2D.Zero || g_Soup.ElapsedTimeStep == 0)
                {
                    for (int x = Math.Max((int)Math.Floor(Position.X - g_Soup.CellSizeMultiplier), 0); x <= Math.Min((int)Math.Ceiling(Position.X + g_Soup.CellSizeMultiplier), local_SizeX - 1); x++)
                    {
                        for (int y = Math.Max((int)Math.Floor(Position.Y - g_Soup.CellSizeMultiplier), 0); y <= Math.Min((int)Math.Ceiling(Position.Y + g_Soup.CellSizeMultiplier), local_SizeY - 1); y++)
                        {
                            if (g_Soup.GridMap[x + y * local_SizeX].LocalParticleCount > 0)
                            {
                                for (int i = 0; i < g_Soup.GridMap[x + y * local_SizeX].LocalParticleCount; i++)
                                {
                                    g_Soup.Particles[g_Soup.GridMap[x + y * local_SizeX].LocalParticles[i]].CollisionDisabled = false;
                                }
                            }
                        }
                    }

                    if (Vector2D.Size(Velocity) > 0.1d)
                    {
                        Velocity = Vector2D.Normalization(Velocity) * 0.1d;
                    }
                    if (Vector2D.Size(Velocity) < 0.001d)
                    {
                        Velocity = Vector2D.Zero;
                    }

                    Position += Velocity;
                    if (Position.X > local_SizeX)
                    {
                        Position.X = local_SizeX;
                        Velocity.X *= -1d;
                        if (Type == ParticleType.Animal)
                        {
                            if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                        }
                    }
                    if (Position.X < 0)
                    {
                        Position.X = 0;
                        Velocity.X *= -1d;
                        if (Type == ParticleType.Animal)
                        {
                            if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                        }
                    }
                    if (Position.Y > local_SizeY)
                    {
                        Position.Y = local_SizeY;
                        Velocity.Y *= -1d;
                        if (Type == ParticleType.Animal)
                        {
                            if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                        }
                    }
                    if (Position.Y < 0)
                    {
                        Position.Y = 0;
                        Velocity.Y *= -1d;
                        if (Type == ParticleType.Animal)
                        {
                            if (new Random().NextDouble() < 0.1d) RandomWalkTargetAngle = new Random().NextDouble() * 360d;
                        }
                    }

                    if (Angle < 0d) Angle += 360d;
                    if (Angle >= 360d) Angle -= 360d;

                    NextGridBiomassCheck = -100;
                }
                else
                {
                    CollisionDisabled = true;
                }
            }
        }
        #endregion

        public void OnStepFinish(int threadId)
        {
            Int2D NextGridPosition = Vector2D.ToGridPosition(Position);

            if (g_Soup.GridMap[NextGridPosition.X + NextGridPosition.Y * local_SizeX].Type == TileType.Wall && !IsGrabbed)
            {
                IsAlive = false;
            }

            if (IsAlive)
            {
                if (GridPosition != NextGridPosition)
                {
                    g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticles.Remove(Index);
                    g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticleCount--;
                    if (Type == ParticleType.Plant) g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalPlantCount--;
                    else g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalAnimalCount--;
                    GridPosition = NextGridPosition;
                    g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticles.Add(Index);
                    g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticleCount++;
                    if (Type == ParticleType.Plant) g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalPlantCount++;
                    else g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalAnimalCount++;
                }
            }

            if (!IsAlive)
            {
                g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].Fertility += Biomass;
                g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticles.Remove(Index);
                g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalParticleCount--;
                if (Type == ParticleType.Plant) g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalPlantCount--;
                else g_Soup.GridMap[GridPosition.X + GridPosition.Y * local_SizeX].LocalAnimalCount--;
                g_Soup.UnassignedParticleIds.Add(Index);
            }
        }
    }

    public class Gene
    {
        public long RaceId { get; set; }

        public int GeneColorRed { get; set; }
        public int GeneColorGreen { get; set; }
        public int GeneColorBlue { get; set; }
        public double GeneDiet { get; set; }
        public double GeneHealth { get; set; }
        public double GeneStrength { get; set; }
        public double GeneHardness { get; set; }
        public double GeneAgility { get; set; }
        public double ActionGeneUnassignedPoint { get; set; }

        public double ForkCost { get; set; }

        public Gene()
        {

        }

        public Gene(Random random)
        {
            Random rnd = new Random();

            RaceId = rnd.NextInt64(0, 2176782336);

            GeneColorRed = rnd.Next(0, 256);
            GeneColorGreen = rnd.Next(0, 256);
            GeneColorBlue = rnd.Next(0, 256);
            GeneDiet = -1d;
            GeneHealth = 0.5d;
            GeneStrength = 0d;
            GeneHardness = 0d;
            GeneAgility = 0d;
            ActionGeneUnassignedPoint = 3.5d;
            ForkCost = g_Soup.AnimalForkBiomass;
        }

        public Gene(Gene parentGenes)
        {
            Random rnd = new Random();

            RaceId = parentGenes.RaceId;

            GeneColorRed = parentGenes.GeneColorRed;
            GeneColorGreen = parentGenes.GeneColorGreen;
            GeneColorBlue = parentGenes.GeneColorBlue;

            GeneDiet = Math.Min(Math.Max(parentGenes.GeneDiet + (rnd.NextDouble() * 0.2d - 0.1d), -1d), 1d);

            GeneHealth = Math.Min(Math.Max(parentGenes.GeneHealth + (rnd.NextDouble() * 0.2d - 0.1d), 0d), 4d);
            GeneStrength = Math.Min(Math.Max(parentGenes.GeneStrength + (rnd.NextDouble() * 0.2d - 0.1d), 0d), 4d);
            GeneHardness = Math.Min(Math.Max(parentGenes.GeneHardness + (rnd.NextDouble() * 0.2d - 0.1d), 0d), 4d);
            GeneAgility = Math.Min(Math.Max(parentGenes.GeneAgility + (rnd.NextDouble() * 0.2d - 0.1d), 0d), 4d);
            ActionGeneUnassignedPoint = Math.Min(Math.Max(parentGenes.ActionGeneUnassignedPoint + (rnd.NextDouble() * 0.2d - 0.1d), 0d), 4d);
            double ActionGenesValueSum = GeneHealth + GeneStrength + GeneHardness + GeneAgility + ActionGeneUnassignedPoint;
            GeneHealth /= ActionGenesValueSum / 4d;
            GeneStrength /= ActionGenesValueSum / 4d;
            GeneHardness /= ActionGenesValueSum / 4d;
            GeneAgility /= ActionGenesValueSum / 4d;
            ActionGeneUnassignedPoint /= ActionGenesValueSum / 4d;

            if (rnd.NextDouble() < g_Soup.MutationRate)
            {
                RaceId = rnd.NextInt64(0, 2176782336);

                GeneColorRed = rnd.Next(0, 256);
                GeneColorGreen = rnd.Next(0, 256);
                GeneColorBlue = rnd.Next(0, 256);
                GeneDiet = rnd.NextDouble() * 2d - 1d;

                GeneHealth = Math.Min(Math.Max(parentGenes.GeneHealth + (rnd.NextDouble() * 2d - 1d), 0d), 4d);
                GeneStrength = Math.Min(Math.Max(parentGenes.GeneStrength + (rnd.NextDouble() * 2d - 1d), 0d), 4d);
                GeneHardness = Math.Min(Math.Max(parentGenes.GeneHardness + (rnd.NextDouble() * 2d - 1d), 0d), 4d);
                GeneAgility = Math.Min(Math.Max(parentGenes.GeneAgility + (rnd.NextDouble() * 2d - 1d), 0d), 4d);
                ActionGeneUnassignedPoint = Math.Min(Math.Max(parentGenes.ActionGeneUnassignedPoint + (rnd.NextDouble() * 2d - 1d), 0d), 4d);
                double ActionGenesValueSumMutation = GeneHealth + GeneStrength + GeneHardness + GeneAgility + ActionGeneUnassignedPoint;
                GeneHealth /= ActionGenesValueSumMutation / 4d;
                GeneStrength /= ActionGenesValueSumMutation / 4d;
                GeneHardness /= ActionGenesValueSumMutation / 4d;
                GeneAgility /= ActionGenesValueSumMutation / 4d;
                ActionGeneUnassignedPoint /= ActionGenesValueSumMutation / 4d;
            }
            else
            {
                int MutatingColor = rnd.Next(0, 3 + 1);
                switch (MutatingColor)
                {
                    case 0:
                        GeneColorRed = Math.Min(Math.Max(parentGenes.GeneColorRed + rnd.Next(-4, 4 + 1), 0), 255);
                        break;
                    case 1:
                        GeneColorGreen = Math.Min(Math.Max(parentGenes.GeneColorGreen + rnd.Next(-4, 4 + 1), 0), 255);
                        break;
                    case 2:
                        GeneColorBlue = Math.Min(Math.Max(parentGenes.GeneColorBlue + rnd.Next(-4, 4 + 1), 0), 255);
                        break;
                }
            }

            //ForkCost = 30d + (GeneHealth * 15d) + (GeneStrength * 15d) + (GeneHardness * 15d) + (GeneAgility * 15d);
            ForkCost = g_Soup.AnimalForkBiomass;
            //ForkCost = 45d + (GeneHealth * 15d) + (GeneStrength * 20d) + (GeneHardness * 20d) + (GeneAgility * 20d);
        }
    }

    public class ParticleData
    {
        public int Index;
        public long Id;
        public ParticleType Type;

        public Vector2D Position;
        public Vector2D Velocity;
        public double Angle;

        public double Radius;
        public ColorInt3 Color;

        public int Age;

        public ParticleData(Particle particle)
        {
            Index = particle.Index;
            Id = particle.Id;
            Type = particle.Type;
            Position = new Vector2D(particle.Position);
            Velocity = new Vector2D(particle.Velocity);
            Angle = particle.Angle;
            Radius = particle.Radius;
            Color = new ColorInt3(particle.Color);
            Age = particle.Age;
        }
    }

    public enum ParticleType
    {
        Plant,
        Animal
    }
}
