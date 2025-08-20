using Paramecium.Engine;
using Paramecium.Variables;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Paramecium.Rendering
{
    // SoupViewを描画するクラス
    public static partial class SoupViewRenderer
    {
        private static readonly Double4d _soupBackgroundColor = new Double4d(1, 0, 0, 0.125);                               // スープの背景
        private static readonly Double4d _soupWallColor = new Double4d(1, 0.5, 0.5, 0.5);                                   // スープの壁の色
        private static readonly Double4d _soupElementColor1 = new Double4d(1, 0, 0.125, 0);                                 // タイルのエレメント量に応じた色(エレメント量がElementPerTileより少ない時用)
        private static readonly Double4d _soupElementColor2 = new Double4d(1, 0, 0.5, 0);                                   // タイルのエレメント量に応じた色(エレメント量がElementPerTileより多い時用)
        private static readonly Double4d _soupOverviewPlantColor = new Double4d(1, 0, 1, 0);                                // スープのオーバービューの植物の色
        private static readonly Double4d _soupOverviewAnimalColor = new Double4d(1, 1, 1, 1);                               // スープのオーバービューの動物のデフォルトの色
        private static readonly Double4d _cellOutlineColor = new Double4d(1, 1, 1, 1);                                      // セルの輪郭線の色
        private static readonly Double4d _plantLowElementColor = new Double4d(1, 0, 0.25, 0);                               // 植物の色(エレメント量が0の時)
        private static readonly Double4d _plantHighElementColor = new Double4d(1, 0, 1, 0);                                 // 植物の色(エレメント量がPlantMaximumElementAmountに等しい時)
        private static readonly Double4d _animalEyeColor = new Double4d(1, 0, 0, 0);                                        // 動物の目の色
        private static readonly Double4d _animalEyeOutlineColor = new Double4d(1, 0.5, 0.5, 0.5);                           // 動物の目の輪郭の色(色が非常に暗い動物用)
        private static readonly Double4d _animalEggColor = new Double4d(1, 1, 0.75, 1);                                     // 動物の卵の色
        private static readonly Double4d _animalUnderAttackColor = new Double4d(1, 1, 0, 0);                                // 攻撃を受けた動物の色
        private static readonly Double4d _animalUnderAttackColor2 = new Double4d(1, 0, 0, 0);                               // 攻撃を受けた動物の色 (色が_animalUnderAttackColorに近い動物用)

        private static readonly Pen _soupBorderPen = new Pen(Color.FromArgb(255, 255, 0, 0));                               // スープの端
        private static readonly SolidBrush _soupOutsideBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));                // スープの外側を塗りつぶす用
        private static readonly SolidBrush _soupWallBrush = new SolidBrush((Color)_soupWallColor);                          // スープの壁の色
        private static readonly Pen _cellOutlinePen = new Pen((Color)_cellOutlineColor);                                    // セルの輪郭
        private static readonly SolidBrush _animalEggBrush = new SolidBrush((Color)_animalEggColor);                        // 動物の卵の色
        private static readonly SolidBrush _animalEyeBrush = new SolidBrush((Color)_animalEyeColor);                        // 動物の目の色
        private static readonly Pen _animalEyeOutlinePen = new Pen((Color)_animalEyeOutlineColor);                          // 動物の目の輪郭の色(色が非常に暗い動物用)

        public static void DrawSoupView(Bitmap soupViewImage, Soup soup, SoupSettings settings, Double2d cameraPosition, int cameraZoomLevel, double unitPerPixel, Int2d mousePosition, SoupObjectPointer selectedObjectPointer)
        {
            Graphics graphics = Graphics.FromImage(soupViewImage);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Int2d imageSize = new Int2d(soupViewImage.Width, soupViewImage.Height);

            DrawSoupOverview(soup, settings, imageSize, graphics, cameraPosition, cameraZoomLevel, unitPerPixel);    // スープのオーバービュー(背景)を描画する

            // 植物と動物を描画する
            if (cameraZoomLevel >= 5)
            {
                // 画面内に入っているタイルの範囲を計算する
                Int2d soupViewStartTilePosition = new Int2d(Math.Max(0, (int)Math.Floor(cameraPosition.X - (imageSize.X / 2d * unitPerPixel)) - 1), Math.Max(0, (int)Math.Floor(cameraPosition.Y - (imageSize.Y / 2d * unitPerPixel)) - 1));
                Int2d soupViewEndTilePosition = new Int2d(Math.Min(settings.SoupSizeX - 1, (int)Math.Ceiling(cameraPosition.X + (imageSize.X / 2d * unitPerPixel)) + 1), Math.Min(settings.SoupSizeY - 1, (int)Math.Ceiling(cameraPosition.Y + (imageSize.Y / 2d * unitPerPixel)) + 1));

                // 植物の描画
                for (int x = soupViewStartTilePosition.X; x <= soupViewEndTilePosition.X; x++)
                {
                    for (int y = soupViewStartTilePosition.Y; y <= soupViewEndTilePosition.Y; y++)
                    {
                        int tileIndex = soup.GetTileIndexFromTilePosition(new Int2d(x, y));
                        Tile targetTile = soup.Tiles[tileIndex];

                        List<int> targetTilePlantIndexes = new List<int>(targetTile.PlantIndexes);

                        for (int i = 0; i < targetTilePlantIndexes.Count; i++)
                        {
                            try
                            {
                                int targetPlantIndex = targetTilePlantIndexes[i];
                                Plant? targetPlant = soup.Plants[targetPlantIndex];

                                if (targetPlant is not null)
                                {
                                    // セルの描画
                                    Double2d targetPlantPosition = targetPlant.Position;
                                    double targetPlantRadius = targetPlant.Radius;

                                    SolidBrush plantColorBrush = new SolidBrush((Color)Double4d.Lerp(_plantLowElementColor, _plantHighElementColor, targetPlant.Element / settings.PlantMaximumElementAmount));

                                    FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetPlantPosition, targetPlantRadius, plantColorBrush);
                                    plantColorBrush.Dispose();
                                    DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetPlantPosition, targetPlantRadius, _cellOutlinePen);
                                }
                            }
                            catch { }
                        }
                    }
                }

                // 動物の描画
                for (int x = soupViewStartTilePosition.X; x <= soupViewEndTilePosition.X; x++)
                {
                    for (int y = soupViewStartTilePosition.Y; y <= soupViewEndTilePosition.Y; y++)
                    {
                        int tileIndex = soup.GetTileIndexFromTilePosition(new Int2d(x, y));
                        Tile targetTile = soup.Tiles[tileIndex];

                        List<int> targetTileAnimalIndexes = new List<int>(targetTile.AnimalIndexes);

                        for (int i = 0; i < targetTileAnimalIndexes.Count; i++)
                        {
                            try
                            {
                                int targetAnimalIndex = targetTileAnimalIndexes[i];
                                Animal? targetAnimal = soup.Animals[targetAnimalIndex];

                                if (targetAnimal is not null)
                                {
                                    Double2d targetAnimalPosition = targetAnimal.Position;
                                    double targetAnimalAngle = targetAnimal.Angle;
                                    double targetAnimalRadius = targetAnimal.Radius;

                                    double targetAnimalBrainOutputAcceleration = targetAnimal.Brain.Output.Acceleration;
                                    double targetAnimalBrainOutputRotation = targetAnimal.Brain.Output.Rotation;

                                    Double4d targetAnimalSpeciesSignature = targetAnimal.SpeciesSignature;
                                    Double4d targetAnimalColor = new Double4d(1d, targetAnimalSpeciesSignature.Y, targetAnimalSpeciesSignature.Z, targetAnimalSpeciesSignature.W);

                                    if (targetAnimal.Age < 0)
                                    {
                                        // 卵の描画
                                        SolidBrush animalColorBrush = new SolidBrush((Color)(new Double4d(1d, targetAnimalSpeciesSignature.Y * 0.9375d, targetAnimalSpeciesSignature.Z * 0.9375d, targetAnimalSpeciesSignature.W * 0.9375d)));

                                        FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition, targetAnimalRadius, _animalEggBrush);
                                        FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition, targetAnimalRadius * 0.75, animalColorBrush);
                                        DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition, targetAnimalRadius, _cellOutlinePen);

                                        animalColorBrush.Dispose();
                                    }
                                    else
                                    {
                                        SolidBrush animalColorBrush;

                                        // 色の決定
                                        if (settings.AnimalUnderAttackTime > 0)
                                        {
                                            if (Double4d.DistanceSquared(targetAnimalColor, _animalUnderAttackColor) < 0.333333d * 0.333333d) animalColorBrush = new SolidBrush((Color)Double4d.Lerp(targetAnimalColor, _animalUnderAttackColor2, (1d - double.Min(settings.AnimalUnderAttackTime, targetAnimal.TimeSinceLastAttacked) / settings.AnimalUnderAttackTime) * 0.75d));
                                            else animalColorBrush = new SolidBrush((Color)Double4d.Lerp(targetAnimalColor, _animalUnderAttackColor, (1d - double.Min(settings.AnimalUnderAttackTime, targetAnimal.TimeSinceLastAttacked) / settings.AnimalUnderAttackTime) * 0.75d));
                                        }
                                        else animalColorBrush = new SolidBrush((Color)targetAnimalColor);

                                        // セルの描画
                                        FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition, targetAnimalRadius, animalColorBrush);
                                        animalColorBrush.Dispose();
                                        DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition, targetAnimalRadius, _cellOutlinePen);

                                        // 目の描画
                                        FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimalAngle + 0.075d), targetAnimalRadius * 0.1d, _animalEyeBrush);
                                        FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimalAngle - 0.075d), targetAnimalRadius * 0.1d, _animalEyeBrush);
                                        if (Double4d.DistanceSquared(targetAnimalColor, new Double4d(1d, 0d, 0d, 0d)) < 0.125d * 0.125d)
                                        {
                                            DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimalAngle + 0.075d), targetAnimalRadius * 0.1d, _animalEyeOutlinePen);
                                            DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimalPosition + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimalAngle - 0.075d), targetAnimalRadius * 0.1d, _animalEyeOutlinePen);
                                        }

                                        // 鞭毛の描画
                                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Acceleration))
                                        {
                                            Random animalFlagellumAngleRandom = new Random((int)(targetAnimal.Id % int.MaxValue) ^ (int)(soup.ElapsedTimeSteps % int.MaxValue));
                                            Random animalFlagellumLengthRandom = new Random((int)(targetAnimal.Id % int.MaxValue));
                                            for (int j = 0; j < 3; j++)
                                            {
                                                DrawLine(
                                                    imageSize, graphics, cameraPosition, unitPerPixel,
                                                    targetAnimalPosition + Double2d.FromAngle01(targetAnimalAngle + 0.5d) * 0.5d,
                                                    targetAnimalPosition + Double2d.FromAngle01(targetAnimalAngle + 0.5d) * 0.5d + Double2d.FromAngle01(
                                                        targetAnimalAngle + 0.5d + (animalFlagellumAngleRandom.NextDouble() * 2d - 1d) * 0.05d * Math.Sqrt(double.Min(1d, double.Abs(targetAnimalBrainOutputAcceleration))) +       // ニューラルネットのAcceleration出力の値に応じて鞭毛の角度の幅を変える
                                                        Math.Sqrt(double.Min(1d, double.Abs(targetAnimalBrainOutputRotation))) * -double.Sign(targetAnimalBrainOutputRotation) * 0.1d       // ニューラルネットのRotation出力の値に応じて鞭毛の角度を変える
                                                    ) * (0.5d + ((animalFlagellumLengthRandom.NextDouble() * 2d - 1d) * 0.1d)),         // 鞭毛の長さをランダム化する
                                                    _cellOutlinePen
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }

                // 壁の描画
                for (int x = soupViewStartTilePosition.X; x <= soupViewEndTilePosition.X; x++)
                {
                    for (int y = soupViewStartTilePosition.Y; y <= soupViewEndTilePosition.Y; y++)
                    {
                        int tileIndex = soup.GetTileIndexFromTilePosition(new Int2d(x, y));
                        Tile targetTile = soup.Tiles[tileIndex];

                        if (targetTile.Type == TileType.Wall)
                        {
                            FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.25d, y + 0.25d), 0.356d, _soupWallBrush);
                            FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.75d, y + 0.25d), 0.356d, _soupWallBrush);
                            FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.25d, y + 0.75d), 0.356d, _soupWallBrush);
                            FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.75d, y + 0.75d), 0.356d, _soupWallBrush);
                            FillEllipse(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.5d, y + 0.5d), 0.5d, _soupWallBrush);
                        }
                    }
                }
            }

            // スープの端の線と外側の塗りつぶし
            FillRectangle(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(-16, -16), new Double2d(settings.SoupSizeX + 16, 0), _soupOutsideBrush);
            FillRectangle(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(-16, settings.SoupSizeY), new Double2d(settings.SoupSizeX + 16, settings.SoupSizeY + 16), _soupOutsideBrush);
            FillRectangle(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(-16, -16), new Double2d(0, settings.SoupSizeY + 16), _soupOutsideBrush);
            FillRectangle(imageSize, graphics, cameraPosition, unitPerPixel, new Double2d(settings.SoupSizeX, -16), new Double2d(settings.SoupSizeX + 16, settings.SoupSizeY + 16), _soupOutsideBrush);
            DrawRectangle(imageSize, graphics, cameraPosition, unitPerPixel, Double2d.Zero, new Double2d(settings.SoupSizeX, settings.SoupSizeY), _soupBorderPen);


            graphics.Dispose();
        }

        // スープのオーバービュー(背景)の描画
        private static void DrawSoupOverview(Soup soup, SoupSettings settings, Int2d size, Graphics graphics, Double2d cameraPosition, int cameraZoomLevel, double unitPerPixel)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((-cameraPosition.X) / unitPerPixel + size.X / 2d - Math.Floor(0.5d / unitPerPixel)), (int)Math.Floor((-cameraPosition.Y) / unitPerPixel + size.Y / 2d - Math.Floor(0.5d / unitPerPixel)));
            Int2d drawSize = new Int2d((int)Math.Ceiling(settings.SoupSizeX / unitPerPixel + (1d / unitPerPixel)), (int)Math.Ceiling(settings.SoupSizeY / unitPerPixel + (1d / unitPerPixel)));

            Double4d[] overviewColor = new Double4d[settings.SoupArea];
            uint[] overviewData = new uint[(settings.SoupSizeX + 1) * (settings.SoupSizeY + 1)];

            // ピクセルの色を計算する
            for (int i = 0; i < overviewColor.Length; i++)
            {
                Tile targetTile = soup.Tiles[i];

                if (targetTile.Element <= settings.SoupElementPerTile) overviewColor[i] = Double4d.Lerp(_soupBackgroundColor, _soupElementColor1, targetTile.Element / settings.SoupElementPerTile);
                else overviewColor[i] = Double4d.Lerp(_soupElementColor1, _soupElementColor2, double.Min(1d, (targetTile.Element - settings.SoupElementPerTile) / settings.SoupElementPerTile / 3d));

                overviewColor[i] = new Double4d(
                    1d,
                    double.Lerp(overviewColor[i].Y, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneRed / settings.SoupMaximumEffectivePheromoneAmount)))),
                    double.Lerp(overviewColor[i].Z, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneGreen / settings.SoupMaximumEffectivePheromoneAmount)))),
                    double.Lerp(overviewColor[i].W, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneBlue / settings.SoupMaximumEffectivePheromoneAmount))))
                );

                // cameraZoomLevelが4以下の場合、タイルに植物か動物がいればそれに応じた色でピクセルの色を上書きする
                if (cameraZoomLevel <= 4)
                {
                    if (targetTile.PlantPopulation > 0)
                    {
                        try
                        {
                            List<int> targetTilePlantIndexes = new List<int>(targetTile.PlantIndexes);

                            Plant? targetPlant = soup.Plants[targetTilePlantIndexes[targetTilePlantIndexes.Count - 1]];

                            if (targetPlant is not null) overviewColor[i] = _soupOverviewPlantColor;
                        }
                        catch
                        {
                            overviewColor[i] = _soupOverviewPlantColor;
                        }
                    }
                    if (targetTile.AnimalPopulation > 0)
                    {
                        try
                        {
                            List<int> targetTileAnimalIndexes = new List<int>(targetTile.AnimalIndexes);

                            Animal? targetAnimal = soup.Animals[targetTileAnimalIndexes[targetTileAnimalIndexes.Count - 1]];

                            if (targetAnimal is not null)
                            {
                                Double4d targetAnimalSpeciesSignature = targetAnimal.SpeciesSignature;
                                overviewColor[i] = new Double4d(1d, targetAnimalSpeciesSignature.Y, targetAnimalSpeciesSignature.Z, targetAnimalSpeciesSignature.W);
                            }
                        }
                        catch
                        {
                            overviewColor[i] = _soupOverviewAnimalColor;
                        }
                    }
                }

                if (targetTile.Type == TileType.Wall)
                {
                    overviewColor[i] = _soupWallColor;
                }
            }

            // 計算した色をuint型配列に格納
            for (int i = 0; i < overviewData.Length; i++)
            {
                Int2d pixelPosition = new Int2d(i % (settings.SoupSizeX + 1), i / (settings.SoupSizeX + 1));
                if (pixelPosition.X > 0 && pixelPosition.Y > 0)
                {
                    overviewData[i] = (uint)overviewColor[(pixelPosition.X - 1) + (pixelPosition.Y - 1) * settings.SoupSizeX];
                }
            }

            // uint型配列をbyte型配列としてBitmapの色情報に書き込み
            Bitmap overviewImage = new Bitmap(settings.SoupSizeX + 1, settings.SoupSizeY + 1);
            BitmapData overviewImageData = overviewImage.LockBits(new Rectangle(0, 0, overviewImage.Width, overviewImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(MemoryMarshal.AsBytes<uint>(overviewData).ToArray(), 0, overviewImageData.Scan0, overviewData.Length * 4);
            overviewImage.UnlockBits(overviewImageData);

            // オーバービューを描画する
            graphics.DrawImage(overviewImage, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);

            // Bitmapを破棄
            overviewImage.Dispose();
        }

        // 線の描画
        public static void DrawLine(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d endPixelPosition = new Int2d((int)Math.Floor((endPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((endPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));

            graphics.DrawLine(pen, startPixelPosition.X, startPixelPosition.Y, endPixelPosition.X, endPixelPosition.Y);
        }

        // 長方形の描画
        public static void DrawRectangle(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling((endPosition.X - startPosition.X) / unitPerPixel), (int)Math.Ceiling((endPosition.Y - startPosition.Y) / unitPerPixel));

            graphics.DrawRectangle(pen, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
        public static void FillRectangle(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, SolidBrush brush)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling((endPosition.X - startPosition.X) / unitPerPixel), (int)Math.Ceiling((endPosition.Y - startPosition.Y) / unitPerPixel));

            graphics.FillRectangle(brush, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }

        // 円の描画
        public static void DrawEllipse(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d centerPosition, double radius, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((centerPosition.X - radius - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((centerPosition.Y - radius - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling(radius * 2d / unitPerPixel), (int)Math.Ceiling(radius * 2d / unitPerPixel));

            graphics.DrawEllipse(pen, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
        public static void FillEllipse(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d centerPosition, double radius, SolidBrush brush)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((centerPosition.X - radius - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((centerPosition.Y - radius - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling(radius * 2d / unitPerPixel), (int)Math.Ceiling(radius * 2d / unitPerPixel));

            graphics.FillEllipse(brush, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
    }
}
