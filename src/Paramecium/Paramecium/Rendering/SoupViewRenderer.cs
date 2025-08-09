using Paramecium.Engine;
using Paramecium.Variables;
using System.Drawing.Imaging;
using System.IO.Packaging;
using System.Runtime.InteropServices;

namespace Paramecium.Rendering
{
    public static partial class SoupViewRenderer
    {
        private static readonly BrainNodeFunction[] _brainNodeAnimalVisionFunctions = new BrainNodeFunction[]   // 動物が視覚を持っているとみなされるためにニューラルネット内が持っている必要があるFunction
        {
            BrainNodeFunction.Input_WallWAvgAngle,
            BrainNodeFunction.Input_WallWAvgProximity,
            BrainNodeFunction.Input_WallWAvgDistance,
            BrainNodeFunction.Input_PlantWAvgAngle,
            BrainNodeFunction.Input_PlantWAvgProximity,
            BrainNodeFunction.Input_PlantWAvgDistance,
            BrainNodeFunction.Input_AnimalWAvgAngle,
            BrainNodeFunction.Input_AnimalWAvgProximity,
            BrainNodeFunction.Input_AnimalWAvgDistance,
            BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff
        };

        private static readonly Double4d _soupBackgroundColor = new Double4d(1, 0, 0, 0.125);                   // スープの背景
        private static readonly Double4d _soupWallColor = new Double4d(1, 0.5, 0.5, 0.5);                       // スープの壁の色
        private static readonly Double4d _soupElementColor1 = new Double4d(1, 0, 0.125, 0);                     // タイルのエレメント量に応じた色(エレメント量がElementPerTileより少ない時用)
        private static readonly Double4d _soupElementColor2 = new Double4d(1, 0, 0.5, 0);                       // タイルのエレメント量に応じた色(エレメント量がElementPerTileより多い時用)
        private static readonly Double4d _pheromoneRedColor = new Double4d(1, 1, 0, 0);                         // 赤フェロモンの色
        private static readonly Double4d _pheromoneGreenColor = new Double4d(1, 0, 1, 0);                       // 緑フェロモンの色
        private static readonly Double4d _pheromoneBlueColor = new Double4d(1, 0, 0, 1);                        // 青フェロモンの色
        private static readonly Double4d _pheromoneRedEffectiveColor = new Double4d(1, 1, 0.5, 0.5);            // 赤フェロモンの有効量の色
        private static readonly Double4d _pheromoneGreenEffectiveColor = new Double4d(1, 0.5, 1, 0.5);          // 緑フェロモンの有効量の色
        private static readonly Double4d _pheromoneBlueEffectiveColor = new Double4d(1, 0.5, 0.5, 1);           // 青フェロモンの有効量の色
        private static readonly Double4d _soupOverviewPlantColor = new Double4d(1, 0, 1, 0);                    // スープのオーバービューの植物の色
        private static readonly Double4d _soupOverviewAnimalColor = new Double4d(1, 1, 1, 1);                   // スープのオーバービューの動物のデフォルトの色
        private static readonly Double4d _plantLowElementColor = new Double4d(1, 0, 0.25, 0);                   // 植物の色(エレメント量が0の時)
        private static readonly Double4d _plantHighElementColor = new Double4d(1, 0, 1, 0);                     // 植物の色(エレメント量がPlantMaximumElementAmountに等しい時)
        private static readonly Double4d _animalEggColor = new Double4d(1, 1, 0.75, 1);                         // 動物の卵の色
        private static readonly Double4d _animalUnderAttackColor = new Double4d(1, 1, 0, 0);                    // 攻撃を受けた動物の色
        private static readonly Double4d _brainDiagramNodeNegativeOutputColor = new Double4d(1, 1, 0, 0);       // Brain Diagramのノードの色(出力がマイナスの時)
        private static readonly Double4d _brainDiagramNodePositiveOutputColor = new Double4d(1, 0, 1, 0);       // Brain Diagramのノードの色(出力がプラスの時)
        private static readonly Double4d _brainDiagramNodeNeutralOutputColor = new Double4d(1, 0, 0, 0);        // Brain Diagramのノードの色(出力が0の時)
        private static readonly Double4d _brainDiagramConnectionWeightNegativeOutputColor = new Double4d(1, 1, 0, 0);       // Brain Diagramの接続の色(重みがマイナスの時)
        private static readonly Double4d _brainDiagramConnectionWeightPositiveOutputColor = new Double4d(1, 0, 1, 0);       // Brain Diagramの接続の色(重みがプラスの時)
        private static readonly Double4d _brainDiagramConnectionWeightNeutralOutputColor = new Double4d(1, 1, 1, 1);        // Brain Diagramの接続の色(重みが0の時)

        private static readonly Pen _soupBorderPen = new Pen(Color.FromArgb(255, 255, 0, 0));                   // スープの端
        private static readonly SolidBrush _soupOutsideBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));    // スープの外側を塗りつぶす用
        private static readonly SolidBrush _soupWallBrush = new SolidBrush((Color)_soupWallColor);              // スープの壁の色
        private static readonly Pen _cellOutlinePen = new Pen(Color.FromArgb(255, 255, 255, 255));              // セルの輪郭
        private static readonly SolidBrush _animalEggBrush = new SolidBrush((Color)_animalEggColor);            // 動物の卵の色
        private static readonly SolidBrush _animalEyeBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));      // 動物の目
        private static readonly Pen _selectedCellPen = new Pen(Color.FromArgb(255, 255, 255, 0));               // 選択中のセル
        private static readonly Pen _selectedCellSameSpeciesPen = new Pen(Color.FromArgb(255, 0, 255, 255));    // 選択中のセルと同種のセル
        private static readonly Pen _rayPointPen = new Pen(Color.FromArgb(255, 255, 255, 0));                   // 
        private static readonly SolidBrush _rayPointBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 0));   // 

        public static Bitmap DrawSoupView(Soup soup, SoupSettings settings, Int2d size, Double2d cameraPosition, int cameraZoomLevel, double unitPerPixel, Int2d mousePosition, SoupObjectType selectedCellType, int selectedCellIndex, long selectedCellId)
        {
            Bitmap result = new Bitmap(size.X, size.Y);

            Graphics graphics = Graphics.FromImage(result);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            DrawSoupOverview(soup, settings, size, graphics, cameraPosition, cameraZoomLevel, unitPerPixel);    // スープのオーバービュー(背景)を描画する

            // 植物と動物を描画する
            if (cameraZoomLevel >= 5)
            {
                // 画面内に入っているタイルの範囲を計算する
                Int2d soupViewStartTilePosition = new Int2d(Math.Max(0, (int)Math.Floor(cameraPosition.X - (size.X / 2d * unitPerPixel)) - 1), Math.Max(0, (int)Math.Floor(cameraPosition.Y - (size.Y / 2d * unitPerPixel)) - 1));
                Int2d soupViewEndTilePosition = new Int2d(Math.Min(settings.SizeX - 1, (int)Math.Ceiling(cameraPosition.X + (size.X / 2d * unitPerPixel)) + 1), Math.Min(settings.SizeY - 1, (int)Math.Ceiling(cameraPosition.Y + (size.Y / 2d * unitPerPixel)) + 1));

                // 植物の描画
                for (int x = soupViewStartTilePosition.X; x <= soupViewEndTilePosition.X; x++)
                {
                    for (int y = soupViewStartTilePosition.Y; y <= soupViewEndTilePosition.Y; y++)
                    {
                        int tileIndex = soup.GetTileIndexFromTilePosition(new Int2d(x, y));
                        Tile targetTile = soup.Tiles[tileIndex];

                        for (int i = 0; i < targetTile.PlantPopulation; i++)
                        {
                            try
                            {
                                int targetPlantIndex = targetTile.PlantIndexes[i];
                                Plant? targetPlant = soup.Plants[targetPlantIndex];

                                if (targetPlant is not null)
                                {
                                    // セルの描画
                                    SolidBrush plantColorBrush = new SolidBrush((Color)Double4d.Lerp(_plantLowElementColor, _plantHighElementColor, targetPlant.Element / settings.PlantMaximumElementAmount));
                                    FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetPlant.Position, targetPlant.Radius, plantColorBrush);
                                    plantColorBrush.Dispose();
                                    DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetPlant.Position, targetPlant.Radius, _cellOutlinePen);
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

                        for (int i = 0; i < targetTile.AnimalPopulation; i++)
                        {
                            try
                            {
                                int targetAnimalIndex = targetTile.AnimalIndexes[i];
                                Animal? targetAnimal = soup.Animals[targetAnimalIndex];

                                if (targetAnimal is not null)
                                {
                                    Double4d targetAnimalSpeciesSignature = targetAnimal.SpeciesSignature;

                                    if (targetAnimal.Age < 0)
                                    {
                                        // 卵の描画
                                        SolidBrush animalColorBrush = new SolidBrush((Color)(new Double4d(1d, targetAnimalSpeciesSignature.Y * 0.9375d, targetAnimalSpeciesSignature.Z * 0.9375d, targetAnimalSpeciesSignature.W * 0.9375d)));

                                        FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius, _animalEggBrush);
                                        FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius * 0.75, animalColorBrush);
                                        DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius, _cellOutlinePen);

                                        animalColorBrush.Dispose();
                                    }
                                    else
                                    {
                                        // セルの描画
                                        SolidBrush animalColorBrush = new SolidBrush((Color)Double4d.Lerp(new Double4d(1d, targetAnimalSpeciesSignature.Y, targetAnimalSpeciesSignature.Z, targetAnimalSpeciesSignature.W), _animalUnderAttackColor, 1d - double.Min(settings.AnimalUnderAttackTime, targetAnimal.TimeSinceLastAttacked) / settings.AnimalUnderAttackTime));
                                        FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius, animalColorBrush);
                                        animalColorBrush.Dispose();
                                        DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius, _cellOutlinePen);

                                        // 目の描画
                                        FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimal.Angle + 0.075d), targetAnimal.Radius * 0.1d, _animalEyeBrush);
                                        FillEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position + Double2d.Rotate01(new Double2d(0.4d, 0d), targetAnimal.Angle - 0.075d), targetAnimal.Radius * 0.1d, _animalEyeBrush);

                                        // 鞭毛の描画
                                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Acceleration))
                                        {
                                            Random animalFlagellumAngleRandom = new Random((int)(targetAnimal.Id % int.MaxValue) ^ (int)(soup.ElapsedTimeSteps % int.MaxValue));
                                            Random animalFlagellumLengthRandom = new Random((int)(targetAnimal.Id % int.MaxValue));
                                            for (int j = 0; j < 3; j++)
                                            {
                                                DrawLine(
                                                    size, graphics, cameraPosition, unitPerPixel,
                                                    targetAnimal.Position + Double2d.FromAngle01(targetAnimal.Angle + 0.5d) * 0.5d,
                                                    targetAnimal.Position + Double2d.FromAngle01(targetAnimal.Angle + 0.5d) * 0.5d + Double2d.FromAngle01(
                                                        targetAnimal.Angle + 0.5d + (animalFlagellumAngleRandom.NextDouble() * 2d - 1d) * 0.05d * double.Min(1d, double.Abs(targetAnimal.Brain.Output.Acceleration)) +      // ニューラルネットのAcceleration出力の値に応じて鞭毛の角度の幅を変える
                                                        double.Max(-1d, double.Min(1d, targetAnimal.Brain.Output.Rotation)) * 0.1d      // ニューラルネットのRotation出力の値に応じて鞭毛の角度を変える
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
                            FillEllipse(size, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.25d, y + 0.25d), 0.356d, _soupWallBrush);
                            FillEllipse(size, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.75d, y + 0.25d), 0.356d, _soupWallBrush);
                            FillEllipse(size, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.25d, y + 0.75d), 0.356d, _soupWallBrush);
                            FillEllipse(size, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.75d, y + 0.75d), 0.356d, _soupWallBrush);
                            FillEllipse(size, graphics, cameraPosition, unitPerPixel, new Double2d(x + 0.5d, y + 0.5d), 0.5d, _soupWallBrush);
                        }
                    }
                }
            }

            // スープの端の線と外側の塗りつぶし
            FillRectangle(size, graphics, cameraPosition, unitPerPixel, new Double2d(-16, -16), new Double2d(settings.SizeX + 16, 0), _soupOutsideBrush);
            FillRectangle(size, graphics, cameraPosition, unitPerPixel, new Double2d(-16, settings.SizeY), new Double2d(settings.SizeX + 16, settings.SizeY + 16), _soupOutsideBrush);
            FillRectangle(size, graphics, cameraPosition, unitPerPixel, new Double2d(-16, -16), new Double2d(0, settings.SizeY + 16), _soupOutsideBrush);
            FillRectangle(size, graphics, cameraPosition, unitPerPixel, new Double2d(settings.SizeX, -16), new Double2d(settings.SizeX + 16, settings.SizeY + 16), _soupOutsideBrush);
            DrawRectangle(size, graphics, cameraPosition, unitPerPixel, Double2d.Zero, new Double2d(settings.SizeX, settings.SizeY), _soupBorderPen);

            // オーバーレイを描画する
            SoupViewOverlayRenderer overlayRenderer = new SoupViewOverlayRenderer();
            overlayRenderer.LineHeight = 16;
            overlayRenderer.ColumnWidth = 300;

            if (selectedCellType == SoupObjectType.Plant) // 植物が選択されている場合
            {
                Plant? targetPlant = soup.Plants[selectedCellIndex];

                if (targetPlant is not null)
                {
                    if (targetPlant.Id == selectedCellId)
                    {
                        // 選択されているセルを強調表示する
                        DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetPlant.Position, targetPlant.Radius + 0.5d, _selectedCellPen);

                        overlayRenderer.OverlayDrawInformation(graphics, $"Plant #{SoupViewOverlayRenderer.StringFromCellId(targetPlant.Id)}");
                        overlayRenderer.OverlayDrawInformation(graphics, $"Generation : {targetPlant.Generation}");
                        overlayRenderer.OverlayDrawInformation(graphics, $"Position : ({targetPlant.Position.X.ToString("0.000")}, {targetPlant.Position.Y.ToString("0.000")})");
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Velocity : ({targetPlant.Velocity.X.ToString("0.000")}, {targetPlant.Velocity.Y.ToString("0.000")}) / {targetPlant.Velocity.Magnitude.ToString("0.000")} u/s", SoupViewOverlayRenderer.OverlayGaugeColor1, targetPlant.Velocity.Magnitude / settings.MaximumEffectiveVelocity);
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Element : {targetPlant.Element.ToString("0.000")} / {settings.PlantMaximumElementAmount.ToString("0.000")}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetPlant.Element / settings.PlantMaximumElementAmount);
                    }
                }
            }
            else if (selectedCellType == SoupObjectType.Animal) // 動物が選択されている場合
            {
                Animal? targetAnimal = soup.Animals[selectedCellIndex];

                if (targetAnimal is not null)
                {
                    if (targetAnimal.Id == selectedCellId)
                    {
                        // 選択されているセルと同種族のセルを強調表示する

                        // 画面内に入っているタイルの範囲を計算する
                        Int2d soupViewStartTilePosition = new Int2d(Math.Max(0, (int)Math.Floor(cameraPosition.X - (size.X / 2d * unitPerPixel)) - 1), Math.Max(0, (int)Math.Ceiling(cameraPosition.Y - (size.Y / 2d * unitPerPixel)) - 1));
                        Int2d soupViewEndTilePosition = new Int2d(Math.Min(settings.SizeX - 1, (int)Math.Floor(cameraPosition.X + (size.X / 2d * unitPerPixel)) + 1), Math.Min(settings.SizeY - 1, (int)Math.Ceiling(cameraPosition.Y + (size.Y / 2d * unitPerPixel)) + 1));

                        for (int x = soupViewStartTilePosition.X; x <= soupViewEndTilePosition.X; x++)
                        {
                            for (int y = soupViewStartTilePosition.Y; y <= soupViewEndTilePosition.Y; y++)
                            {
                                int tileIndex = soup.GetTileIndexFromTilePosition(new Int2d(x, y));
                                Tile targetTile = soup.Tiles[tileIndex];

                                for (int i = 0; i < targetTile.AnimalPopulation; i++)
                                {
                                    try
                                    {
                                        int targetAnimal2Index = targetTile.AnimalIndexes[i];
                                        Animal? targetAnimal2 = soup.Animals[targetAnimal2Index];

                                        if (targetAnimal2 is not null)
                                        {
                                            if (targetAnimal2.Index != selectedCellIndex && targetAnimal2.SpeciesSignature == targetAnimal.SpeciesSignature)
                                            {
                                                DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal2.Position, targetAnimal2.Radius + 0.5d, _selectedCellSameSpeciesPen);
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }

                        // 選択されているセルを強調表示する
                        DrawEllipse(size, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius + 0.5d, _selectedCellPen);

                        overlayRenderer.OverlayDrawInformation(graphics, $"Animal #{SoupViewOverlayRenderer.StringFromCellId(targetAnimal.Id)}");
                        overlayRenderer.OverlayDrawInformation(graphics, $"Generation : {targetAnimal.Generation}");
                        if (targetAnimal.Age >= 0) overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Age : {targetAnimal.Age} / {settings.AnimalLifespan}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Age / (double)settings.AnimalLifespan);
                        else overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Hatching : {settings.AnimalEggHatchingTime + targetAnimal.Age} / {settings.AnimalEggHatchingTime}", SoupViewOverlayRenderer.OverlayGaugeColor1, (settings.AnimalEggHatchingTime + targetAnimal.Age) / (double)settings.AnimalEggHatchingTime);
                        overlayRenderer.OverlayDrawInformation(graphics, $"Offspring : {targetAnimal.OffspringCount}");
                        overlayRenderer.OverlayDrawInformation(graphics, $"Position : ({targetAnimal.Position.X.ToString("0.000")}, {targetAnimal.Position.Y.ToString("0.000")})");
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Velocity : ({targetAnimal.Velocity.X.ToString("0.000")}, {targetAnimal.Velocity.Y.ToString("0.000")}) / {targetAnimal.Velocity.Magnitude.ToString("0.000")} u/step", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Velocity.Magnitude / settings.MaximumEffectiveVelocity);
                        overlayRenderer.OverlayDrawInformation(graphics, $"Angle : {(targetAnimal.Angle + 0.5d).ToString("0.000")}");
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Angular Velocity : {targetAnimal.AngularVelocity.ToString("0.000")} rot/step", SoupViewOverlayRenderer.OverlayGaugeColor1, double.Abs(targetAnimal.AngularVelocity) / settings.MaximumEffectiveAngularVelocity);
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Element : {targetAnimal.Element.ToString("0.000")} / {(settings.AnimalForkCost).ToString("0.000")} ({targetAnimal.ElementLossRate.ToString("0.000")}/step)", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Element / settings.AnimalForkCost);
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Reproduction : {targetAnimal.ReproductionProgress.ToString("0.000")} / {(settings.AnimalForkCost).ToString("0.000")}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.ReproductionProgress / settings.AnimalForkCost);
                        overlayRenderer.OverlayDrawInformationWithGauge(graphics, $"Species Sig. : ({targetAnimal.SpeciesSignature.X.ToString("0.000")}, {targetAnimal.SpeciesSignature.Y.ToString("0.000")}, {targetAnimal.SpeciesSignature.Z.ToString("0.000")}, {targetAnimal.SpeciesSignature.W.ToString("0.000")})", (Color)new Double4d(1d, targetAnimal.SpeciesSignature.Y, targetAnimal.SpeciesSignature.Z, targetAnimal.SpeciesSignature.W), 1d);
                        overlayRenderer.OverlayDrawInformation(graphics, $"Total Mutation : {targetAnimal.MutationCount}");

                        overlayRenderer.NextColumn();

                        // Brain Diagramを描画する
                        overlayRenderer.LineHeight = 420;
                        overlayRenderer.ColumnWidth = 500;
                        overlayRenderer.OverlayFillRectangle(graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
                        overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 12, $"Brain Diagram", SoupViewOverlayRenderer.OverlayTextBrush);

                        overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 12, $"Node : {targetAnimal.Brain.Nodes.Count}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(0, 388));
                        overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 12, $"Connection : {targetAnimal.Brain.Connections.Count}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(0, 404));

                        for (int i = 0; i < targetAnimal.Brain.Connections.Count; i++)
                        {
                            BrainNodeConnection targetConnection = targetAnimal.Brain.Connections[i];

                            Double2d originPos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * targetConnection.OriginIndex) * 180d + new Double2d(250, 220);
                            Double2d targetPos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * targetConnection.TargetIndex) * 180d + new Double2d(250, 220);

                            Double2d arrowVector = (targetPos - originPos).Normalized;
                            Double2d arrowLineVector1 = Double2d.Rotate01(arrowVector, 0.375);
                            Double2d arrowLineVector2 = Double2d.Rotate01(arrowVector, -0.375);
                            originPos += arrowVector * 7;
                            targetPos -= arrowVector * 7;

                            Double4d arrowColor = _brainDiagramConnectionWeightNeutralOutputColor;
                            if (targetConnection.Weight < 0) arrowColor = Double4d.Lerp(arrowColor, _brainDiagramConnectionWeightNegativeOutputColor, double.Min(1d, -targetConnection.Weight));
                            if (targetConnection.Weight > 0) arrowColor = Double4d.Lerp(arrowColor, _brainDiagramConnectionWeightPositiveOutputColor, double.Min(1d, targetConnection.Weight));

                            overlayRenderer.OverlayDrawLine(graphics, (Color)arrowColor, new Int2d((int)originPos.X, (int)originPos.Y), new Int2d((int)targetPos.X, (int)targetPos.Y));
                            overlayRenderer.OverlayDrawLine(graphics, (Color)arrowColor, new Int2d((int)targetPos.X, (int)targetPos.Y), new Int2d((int)(targetPos.X + arrowLineVector1.X * 7), (int)(targetPos.Y + arrowLineVector1.Y * 7)));
                            overlayRenderer.OverlayDrawLine(graphics, (Color)arrowColor, new Int2d((int)targetPos.X, (int)targetPos.Y), new Int2d((int)(targetPos.X + arrowLineVector2.X * 7), (int)(targetPos.Y + arrowLineVector2.Y * 7)));
                        }
                        for (int i = 0; i < targetAnimal.Brain.Nodes.Count; i++)
                        {
                            BrainNode targetNode = targetAnimal.Brain.Nodes[i];

                            Double2d nodePos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * i) * 180d + new Double2d(250, 220);

                            Double4d nodeColor = _brainDiagramNodeNeutralOutputColor;
                            if (targetNode.IsOutput)
                            {
                                if (targetNode.Input < 0) nodeColor = Double4d.Lerp(nodeColor, _brainDiagramNodeNegativeOutputColor, double.Min(1d, -targetNode.Input));
                                if (targetNode.Input > 0) nodeColor = Double4d.Lerp(nodeColor, _brainDiagramNodePositiveOutputColor, double.Min(1d, targetNode.Input));
                            }
                            else
                            {
                                if (targetNode.Output < 0) nodeColor = Double4d.Lerp(nodeColor, _brainDiagramNodeNegativeOutputColor, double.Min(1d, -targetNode.Output));
                                if (targetNode.Output > 0) nodeColor = Double4d.Lerp(nodeColor, _brainDiagramNodePositiveOutputColor, double.Min(1d, targetNode.Output));
                            }
                            overlayRenderer.OverlayFillEllipse(graphics, (Color)nodeColor, new Int2d((int)nodePos.X, (int)nodePos.Y), 7);
                            overlayRenderer.OverlayDrawEllipse(graphics, _cellOutlinePen, new Int2d((int)nodePos.X, (int)nodePos.Y), 7);

                            if (targetNode.Function == BrainNodeFunction.NonOperation) overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 8, $"#{i} Nop", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                            if (targetNode.IsInput) overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 8, $"#{i} Input", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                            if (targetNode.IsHidden) overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 8, $"#{i} Hidden", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                            if (targetNode.IsOutput) overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 8, $"#{i} Output", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));

                        }

                        overlayRenderer.NextColumn();
                        overlayRenderer.LineHeight = 420;
                        overlayRenderer.ColumnWidth = 200;

                        overlayRenderer.OverlayFillRectangle(graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
                        overlayRenderer.LineHeight = 19;

                        overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 12, $"Brain Inputs : ", SoupViewOverlayRenderer.OverlayTextBrush);

                        overlayRenderer.NextLine();
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Bias))
                        {
                            if (targetAnimal.Age >= 0) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_Bias.ToString()}", 1d);
                            else overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_Bias.ToString()}", 0d);
                        }
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Velocity)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_Velocity.ToString()}", targetAnimal.Brain.Input.Velocity);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AngularVelocity)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_AngularVelocity.ToString()}", targetAnimal.Brain.Input.AngularVelocity);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Satiety)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_Satiety.ToString()}", targetAnimal.Brain.Input.Satiety);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Attacked)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_Attacked.ToString()}", targetAnimal.Brain.Input.Attacked);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_WallWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgAngle);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgProximity)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_WallWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgProximity);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgDistance)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_WallWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgDistance);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PlantWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgAngle);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgProximity)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PlantWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgProximity);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgDistance)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PlantWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgDistance);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_AnimalWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgAngle);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgProximity)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_AnimalWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgProximity);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgDistance)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_AnimalWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgDistance);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgSpeciesSigDiff);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneRedConcentration)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneRedConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneRedConcentration);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneRedGradAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneRedGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneRedGradAngle);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneGreenConcentration)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneGreenConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneGreenConcentration);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneGreenGradAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneGreenGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneGreenGradAngle);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneBlueConcentration)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneBlueConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneBlueConcentration);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneBlueGradAngle)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Input_PheromoneBlueGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneBlueGradAngle);

                        overlayRenderer.NextLine();
                        overlayRenderer.OverlayDrawString(graphics, "MS UI Gothic", 12, $"Brain Outputs : ", SoupViewOverlayRenderer.OverlayTextBrush);

                        overlayRenderer.NextLine();
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Acceleration)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_Acceleration.ToString()}", targetAnimal.Brain.Output.Acceleration);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Rotation)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_Rotation.ToString()}", targetAnimal.Brain.Output.Rotation);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Eat)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_Eat.ToString()}", targetAnimal.Brain.Output.Eat);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Attack)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_Attack.ToString()}", targetAnimal.Brain.Output.Attack);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneRedProduction)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_PheromoneRedProduction.ToString()}", targetAnimal.Brain.Output.PheromoneRedProduction);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneGreenProduction)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_PheromoneGreenProduction.ToString()}", targetAnimal.Brain.Output.PheromoneGreenProduction);
                        if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneBlueProduction)) overlayRenderer.OverlayDrawAnimalBrainInOutInfomation(graphics, $"{BrainNodeFunction.Output_PheromoneBlueProduction.ToString()}", targetAnimal.Brain.Output.PheromoneBlueProduction);

                        overlayRenderer.ResetOffset();
                        overlayRenderer.LineHeight = 16;
                        overlayRenderer.ColumnWidth = 300;
                        overlayRenderer.NextColumn();
                        overlayRenderer.LineHeight = 420;
                        overlayRenderer.ColumnWidth = 500;

                        // マウスホバーしたノードの詳細情報を表示する
                        for (int i = 0; i < targetAnimal.Brain.Nodes.Count; i++)
                        {
                            BrainNode targetNode = targetAnimal.Brain.Nodes[i];

                            Double2d nodePos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * i) * 180d + new Double2d(250, 220);

                            if (Double2d.Distance(nodePos, new Double2d(mousePosition.X - overlayRenderer.Offset.X, mousePosition.Y - overlayRenderer.Offset.Y)) < 7)
                            {
                                int offset = 0;

                                overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"Node #{i}", nodePos, ref offset);
                                overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"Function : {targetNode.Function.ToString()}", nodePos, ref offset);
                                if (targetNode.IsHidden || targetNode.IsOutput)  overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"Input : {targetNode.Input.ToString("0.000")}", nodePos, ref offset);
                                if (targetNode.IsInput || targetNode.IsHidden) overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"Output : {targetNode.Output.ToString("0.000")}", nodePos, ref offset);
                                overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, string.Empty, nodePos, ref offset);
                                overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"Connections :", nodePos, ref offset);

                                if (targetNode.IsHidden || targetNode.IsOutput)
                                {
                                    overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"   Incoming :", nodePos, ref offset);

                                    int incomingConnectionCount = 0;
                                    for (int j = 0; j < targetAnimal.Brain.Connections.Count; j++)
                                    {
                                        BrainNodeConnection targetConnection = targetAnimal.Brain.Connections[j];

                                        if (targetConnection.TargetIndex == i)
                                        {
                                            if (incomingConnectionCount > 0) overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, string.Empty, nodePos, ref offset);

                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Connections #{j}", nodePos, ref offset);
                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Origin Index : {targetConnection.OriginIndex}", nodePos, ref offset);
                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Weight : {targetConnection.Weight.ToString("0.000")}", nodePos, ref offset);

                                            incomingConnectionCount++;
                                        }
                                    }
                                    if (incomingConnectionCount == 0)  overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      No Connection", nodePos, ref offset);
                                }

                                if (targetNode.IsHidden) overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, string.Empty, nodePos, ref offset);

                                if (targetNode.IsInput || targetNode.IsHidden)
                                {
                                    overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"   Outgoing :", nodePos, ref offset);

                                    int outgoingConnectionCount = 0;
                                    for (int j = 0; j < targetAnimal.Brain.Connections.Count; j++)
                                    {
                                        BrainNodeConnection targetConnection = targetAnimal.Brain.Connections[j];

                                        if (targetConnection.OriginIndex == i)
                                        {
                                            if (outgoingConnectionCount > 0) overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, string.Empty, nodePos, ref offset);

                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Connections #{j}", nodePos, ref offset);
                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Target Index : {targetConnection.TargetIndex}", nodePos, ref offset);
                                            overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      Weight : {targetConnection.Weight.ToString("0.000")}", nodePos, ref offset);

                                            outgoingConnectionCount++;
                                        }
                                    }
                                    if (outgoingConnectionCount == 0) overlayRenderer.OverlayDrawAnimalBrainNodeInfomation(graphics, $"      No Connection", nodePos, ref offset);
                                }
                            }
                        }
                    }
                }
            }
            else // 何も選択されていない場合(マウスホバーされたタイルの情報を表示する)
            {
                Double2d mousePositionInSoup = new Double2d((mousePosition.X - size.X / 2d) * unitPerPixel + cameraPosition.X, (mousePosition.Y - size.Y / 2d) * unitPerPixel + cameraPosition.Y);
                if (mousePositionInSoup.X >= 0 && mousePositionInSoup.X <= settings.SizeX && mousePositionInSoup.Y >= 0 && mousePositionInSoup.Y <= settings.SizeX)
                {
                    Int2d mouseTilePosition = soup.GetTilePositionFromPosition(mousePositionInSoup);
                    int mouseTileIndex = soup.GetTileIndexFromPosition(mousePositionInSoup);

                    Tile targetTile = soup.Tiles[mouseTileIndex];

                    overlayRenderer.OverlayDrawInformation(graphics, $"Tile #{mouseTileIndex}");
                    overlayRenderer.OverlayDrawInformation(graphics, $"Position : ({mouseTilePosition.X}, {mouseTilePosition.Y})");
                    overlayRenderer.OverlayDrawInformation(graphics, $"Type : {targetTile.Type.ToString()}");
                    overlayRenderer.OverlayDrawInformationWith2Gauges(graphics, $"Element : {targetTile.Element.ToString("0.000")}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetTile.Element / settings.ElementPerTile, SoupViewOverlayRenderer.OverlayGaugeColor2, (targetTile.Element - settings.ElementPerTile) / settings.ElementPerTile / 3d);
                    overlayRenderer.OverlayDrawInformationWith2Gauges(graphics, $"Red Pheromone : {targetTile.PheromoneRed.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneRedEffectiveColor, Math.Sqrt(targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneRedColor, targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount);
                    overlayRenderer.OverlayDrawInformationWith2Gauges(graphics, $"Green Pheromone : {targetTile.PheromoneGreen.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneGreenEffectiveColor, Math.Sqrt(targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneGreenColor, targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount);
                    overlayRenderer.OverlayDrawInformationWith2Gauges(graphics, $"Blue Pheromone : {targetTile.PheromoneBlue.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneBlueEffectiveColor, Math.Sqrt(targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneBlueColor, targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount);
                }
            }

            graphics.Dispose();

            return result;
        }

        // スープのオーバービュー(背景)の描画
        private static void DrawSoupOverview(Soup soup, SoupSettings settings, Int2d size, Graphics graphics, Double2d cameraPosition, int cameraZoomLevel, double unitPerPixel)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((-cameraPosition.X) / unitPerPixel + size.X / 2d - Math.Floor(0.5d / unitPerPixel)), (int)Math.Floor((-cameraPosition.Y) / unitPerPixel + size.Y / 2d - Math.Floor(0.5d / unitPerPixel)));
            Int2d drawSize = new Int2d((int)Math.Ceiling(settings.SizeX / unitPerPixel + (1d / unitPerPixel)), (int)Math.Ceiling(settings.SizeY / unitPerPixel + (1d / unitPerPixel)));

            Double4d[] overviewColor = new Double4d[settings.Area];
            uint[] overviewData = new uint[(settings.SizeX + 1) * (settings.SizeY + 1)];

            // ピクセルの色を計算する
            for (int i = 0; i < overviewColor.Length; i++)
            {
                Tile targetTile = soup.Tiles[i];

                if (targetTile.Element <= settings.ElementPerTile) overviewColor[i] = Double4d.Lerp(_soupBackgroundColor, _soupElementColor1, targetTile.Element / settings.ElementPerTile);
                else overviewColor[i] = Double4d.Lerp(_soupElementColor1, _soupElementColor2, double.Min(1d, (targetTile.Element - settings.ElementPerTile) / settings.ElementPerTile / 3d));

                overviewColor[i] = new Double4d(
                    1d,
                    double.Lerp(overviewColor[i].Y, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount)))),
                    double.Lerp(overviewColor[i].Z, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount)))),
                    double.Lerp(overviewColor[i].W, 1d, double.Max(0d, double.Min(1d, Math.Sqrt(targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount))))
                );

                // cameraZoomLevelが4以下の場合、タイルに植物か動物がいればそれに応じた色でピクセルの色を上書きする
                if (cameraZoomLevel <= 4)
                {
                    if (targetTile.PlantPopulation > 0)
                    {
                        try
                        {
                            Plant? targetPlant = soup.Plants[targetTile.PlantIndexes[targetTile.PlantPopulation - 1]];

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
                            Animal? targetAnimal = soup.Animals[targetTile.AnimalIndexes[targetTile.AnimalPopulation - 1]];

                            if (targetAnimal is not null)
                            {
                                //if (targetAnimal.Age < 0) overviewColor[i] = _animalEggColor;
                                //else
                                //{
                                //    Double4d targetAnimalSpeciesSignature = targetAnimal.SpeciesSignature;
                                //    overviewColor[i] = new Double4d(1d, targetAnimalSpeciesSignature.Y, targetAnimalSpeciesSignature.Z, targetAnimalSpeciesSignature.W);
                                //}
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
                Int2d pixelPosition = new Int2d(i % (settings.SizeX + 1), i / (settings.SizeY + 1));
                if (pixelPosition.X > 0 && pixelPosition.Y > 0)
                {
                    overviewData[i] = (uint)overviewColor[(pixelPosition.X - 1) + (pixelPosition.Y - 1) * settings.SizeX];
                }
            }

            // uint型配列をbyte型配列としてBitmapの色情報に書き込み
            Bitmap overviewImage = new Bitmap(settings.SizeX + 1, settings.SizeY + 1);
            BitmapData overviewImageData = overviewImage.LockBits(new Rectangle(0, 0, overviewImage.Width, overviewImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(MemoryMarshal.AsBytes<uint>(overviewData).ToArray(), 0, overviewImageData.Scan0, overviewData.Length * 4);
            overviewImage.UnlockBits(overviewImageData);

            // オーバービューを描画する
            graphics.DrawImage(overviewImage, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);

            // Bitmapを破棄
            overviewImage.Dispose();
        }

        // 線の描画
        private static void DrawLine(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d endPixelPosition = new Int2d((int)Math.Floor((endPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((endPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));

            graphics.DrawLine(pen, startPixelPosition.X, startPixelPosition.Y, endPixelPosition.X, endPixelPosition.Y);
        }

        // 長方形の描画
        private static void DrawRectangle(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling((endPosition.X - startPosition.X) / unitPerPixel), (int)Math.Ceiling((endPosition.Y - startPosition.Y) / unitPerPixel));

            graphics.DrawRectangle(pen, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
        private static void FillRectangle(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d startPosition, Double2d endPosition, SolidBrush brush)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((startPosition.X - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((startPosition.Y - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling((endPosition.X - startPosition.X) / unitPerPixel), (int)Math.Ceiling((endPosition.Y - startPosition.Y) / unitPerPixel));

            graphics.FillRectangle(brush, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }

        // 円の描画
        private static void DrawEllipse(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d centerPosition, double radius, Pen pen)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((centerPosition.X - radius - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((centerPosition.Y - radius - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling(radius * 2d / unitPerPixel), (int)Math.Ceiling(radius * 2d / unitPerPixel));

            graphics.DrawEllipse(pen, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
        private static void FillEllipse(Int2d size, Graphics graphics, Double2d cameraPosition, double unitPerPixel, Double2d centerPosition, double radius, SolidBrush brush)
        {
            Int2d startPixelPosition = new Int2d((int)Math.Floor((centerPosition.X - radius - cameraPosition.X) / unitPerPixel + size.X / 2d), (int)Math.Floor((centerPosition.Y - radius - cameraPosition.Y) / unitPerPixel + size.Y / 2d));
            Int2d drawSize = new Int2d((int)Math.Ceiling(radius * 2d / unitPerPixel), (int)Math.Ceiling(radius * 2d / unitPerPixel));

            graphics.FillEllipse(brush, startPixelPosition.X, startPixelPosition.Y, drawSize.X, drawSize.Y);
        }
    }
}
