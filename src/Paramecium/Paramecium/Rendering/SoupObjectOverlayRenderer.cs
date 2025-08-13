using Paramecium.Engine;
using Paramecium.Variables;
using System.Drawing;

namespace Paramecium.Rendering
{
    public static class SoupObjectOverlayRenderer
    {
        private static readonly Double4d _pheromoneRedColor = new Double4d(1, 1, 0, 0);                                     // 赤フェロモンの色
        private static readonly Double4d _pheromoneGreenColor = new Double4d(1, 0, 1, 0);                                   // 緑フェロモンの色
        private static readonly Double4d _pheromoneBlueColor = new Double4d(1, 0, 0, 1);                                    // 青フェロモンの色
        private static readonly Double4d _pheromoneRedEffectiveColor = new Double4d(1, 1, 0.5, 0.5);                        // 赤フェロモンの有効量の色
        private static readonly Double4d _pheromoneGreenEffectiveColor = new Double4d(1, 0.5, 1, 0.5);                      // 緑フェロモンの有効量の色
        private static readonly Double4d _pheromoneBlueEffectiveColor = new Double4d(1, 0.5, 0.5, 1);                       // 青フェロモンの有効量の色
        private static readonly Double4d _brainDiagramNodeNegativeOutputColor = new Double4d(1, 1, 0, 0);                   // Brain Diagramのノードの色(出力がマイナスの時)
        private static readonly Double4d _brainDiagramNodePositiveOutputColor = new Double4d(1, 0, 1, 0);                   // Brain Diagramのノードの色(出力がプラスの時)
        private static readonly Double4d _brainDiagramNodeNeutralOutputColor = new Double4d(1, 0, 0, 0);                    // Brain Diagramのノードの色(出力が0の時)
        private static readonly Double4d _brainDiagramConnectionNegativeWeightColor = new Double4d(1, 1, 0, 0);             // Brain Diagramの接続の色(重みがマイナスの時)
        private static readonly Double4d _brainDiagramConnectionPositiveWeightColor = new Double4d(1, 0, 1, 0);             // Brain Diagramの接続の色(重みがプラスの時)
        private static readonly Double4d _brainDiagramConnectionNeutralWeightColor = new Double4d(1, 1, 1, 1);              // Brain Diagramの接続の色(重みが0の時)

        private static readonly Pen _selectedCellPen = new Pen(Color.FromArgb(255, 255, 255, 0));                           // 選択中のセル
        private static readonly Pen _selectedCellSameSpeciesPen = new Pen(Color.FromArgb(191, 0, 255, 255));                // 選択中のセルと同種のセル
        private static readonly Pen _nodeOutlineAndConnectionPen = new Pen(Color.FromArgb(255, 255, 255, 255));             // ノードの輪郭

        public static void DrawSoupObjectOverlayRenderer(Bitmap soupViewImage, Soup soup, SoupSettings settings, Double2d cameraPosition, int cameraZoomLevel, double unitPerPixel, Int2d mousePosition, SoupObjectPointer selectedObjectPointer, OverlayToggles overlayToggles)
        {
            if ((overlayToggles & OverlayToggles.AllOverlays) != OverlayToggles.AllOverlays) return;

            Int2d imageSize = new Int2d(soupViewImage.Width, soupViewImage.Height);

            SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo = new SoupViewOverlayRenderer.OverlayDrawInfo(SoupViewOverlayRenderer.OverlayOrigin.UpperLeft, Int2d.Zero, imageSize, new Int2d(325, 16));
            SoupViewOverlayRenderer.OverlayDrawInfo overlay2ndLayerDrawInfo = new SoupViewOverlayRenderer.OverlayDrawInfo(SoupViewOverlayRenderer.OverlayOrigin.UpperLeft, Int2d.Zero, imageSize, new Int2d(325, 16));

            Graphics graphics = Graphics.FromImage(soupViewImage);

            if (selectedObjectPointer.ObjectType == SoupObjectType.Plant) // 植物が選択されている場合
            {
                if ((overlayToggles & OverlayToggles.SelectedObject) == OverlayToggles.SelectedObject)
                {
                    Plant? targetPlant = soup.Plants[selectedObjectPointer.ObjectIndex];

                    if (targetPlant is not null)
                    {
                        if (targetPlant.Id == selectedObjectPointer.ObjectId)
                        {
                            SoupViewRenderer.DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetPlant.Position, targetPlant.Radius + 0.5d, _selectedCellPen);

                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Plant #{Soup.StringFromCellId(targetPlant.Id)}");
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Generation : {targetPlant.Generation}");
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Position : ({targetPlant.Position.X.ToString("0.000")}, {targetPlant.Position.Y.ToString("0.000")})");
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Velocity : ({targetPlant.Velocity.X.ToString("0.000")}, {targetPlant.Velocity.Y.ToString("0.000")}) / {targetPlant.Velocity.Magnitude.ToString("0.000")} u/s", SoupViewOverlayRenderer.OverlayGaugeColor1, targetPlant.Velocity.Magnitude / settings.MaximumEffectiveVelocity);
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Element : {targetPlant.Element.ToString("0.000")} / {settings.PlantMaximumElementAmount.ToString("0.000")}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetPlant.Element / settings.PlantMaximumElementAmount);
                        }
                    }
                }
            }
            else if (selectedObjectPointer.ObjectType == SoupObjectType.Animal) // 動物が選択されている場合
            {
                Animal? targetAnimal = soup.Animals[selectedObjectPointer.ObjectIndex];

                if (targetAnimal is not null)
                {
                    if (targetAnimal.Id == selectedObjectPointer.ObjectId)
                    {
                        if ((overlayToggles & OverlayToggles.SelectedObject) == OverlayToggles.SelectedObject)
                        {
                            // 選択されているセルと同種族のセルを強調表示する

                            // 画面内に入っているタイルの範囲を計算する
                            Int2d soupViewStartTilePosition = new Int2d(Math.Max(0, (int)Math.Floor(cameraPosition.X - (imageSize.X / 2d * unitPerPixel)) - 1), Math.Max(0, (int)Math.Ceiling(cameraPosition.Y - (imageSize.Y / 2d * unitPerPixel)) - 1));
                            Int2d soupViewEndTilePosition = new Int2d(Math.Min(settings.SizeX - 1, (int)Math.Floor(cameraPosition.X + (imageSize.X / 2d * unitPerPixel)) + 1), Math.Min(settings.SizeY - 1, (int)Math.Ceiling(cameraPosition.Y + (imageSize.Y / 2d * unitPerPixel)) + 1));

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
                                                if (targetAnimal2.Index != selectedObjectPointer.ObjectIndex && targetAnimal2.SpeciesSignature == targetAnimal.SpeciesSignature)
                                                {
                                                    SoupViewRenderer.DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimal2.Position, targetAnimal2.Radius + 0.5d, _selectedCellSameSpeciesPen);
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }

                            // 選択されているセルを強調表示する
                            SoupViewRenderer.DrawEllipse(imageSize, graphics, cameraPosition, unitPerPixel, targetAnimal.Position, targetAnimal.Radius + 0.5d, _selectedCellPen);

                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Animal #{Soup.StringFromCellId(targetAnimal.Id)}");
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Species Sig. : ({targetAnimal.SpeciesSignature.X.ToString("0.000")}, {targetAnimal.SpeciesSignature.Y.ToString("0.000")}, {targetAnimal.SpeciesSignature.Z.ToString("0.000")}, {targetAnimal.SpeciesSignature.W.ToString("0.000")})", (Color)new Double4d(1d, targetAnimal.SpeciesSignature.Y, targetAnimal.SpeciesSignature.Z, targetAnimal.SpeciesSignature.W), 1d);
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Total Mutation : {targetAnimal.MutationCount}");
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Generation : {targetAnimal.Generation}");
                            if (targetAnimal.Age >= 0) SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Age : {targetAnimal.Age} / {settings.AnimalLifespan}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Age / (double)settings.AnimalLifespan);
                            else SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Hatching : {settings.AnimalEggHatchingTime + targetAnimal.Age} / {settings.AnimalEggHatchingTime}", SoupViewOverlayRenderer.OverlayGaugeColor1, (settings.AnimalEggHatchingTime + targetAnimal.Age) / (double)settings.AnimalEggHatchingTime);
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Offspring : {targetAnimal.OffspringCount}");
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Position : ({targetAnimal.Position.X.ToString("0.000")}, {targetAnimal.Position.Y.ToString("0.000")})");
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Velocity : ({targetAnimal.Velocity.X.ToString("0.000")}, {targetAnimal.Velocity.Y.ToString("0.000")}) / {targetAnimal.Velocity.Magnitude.ToString("0.000")} u/step", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Velocity.Magnitude / settings.MaximumEffectiveVelocity);
                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Angle : {(targetAnimal.Angle + 0.5d).ToString("0.000")}");
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Angular Velocity : {targetAnimal.AngularVelocity.ToString("+0.000;-0.000;0.000")} rot/step", SoupViewOverlayRenderer.OverlayGaugeColor1, double.Abs(targetAnimal.AngularVelocity) / settings.MaximumEffectiveAngularVelocity);
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Element : {targetAnimal.Element.ToString("0.000")} / {(settings.AnimalMaximumElementAmount).ToString("0.000")} ({targetAnimal.ElementLossRate.ToString("+0.000;-0.000;0.000")}/step)", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.Element / settings.AnimalReproductionCost);
                            SoupViewOverlayRenderer.OverlayDrawInformationWithGauge(overlayDrawInfo, graphics, $"Reproduction : {targetAnimal.ReproductionProgress.ToString("0.000")} / {(settings.AnimalReproductionCost).ToString("0.000")} ({targetAnimal.ReproductionRate.ToString("+0.000;-0.000;0.000")}/step)", SoupViewOverlayRenderer.OverlayGaugeColor1, targetAnimal.ReproductionProgress / settings.AnimalReproductionCost);

                            overlayDrawInfo.NextColumn();
                        }

                        if ((overlayToggles & OverlayToggles.AnimalBrainDiagram) == OverlayToggles.AnimalBrainDiagram)
                        {
                            // Brain Diagramを描画する

                            overlay2ndLayerDrawInfo = new SoupViewOverlayRenderer.OverlayDrawInfo(SoupViewOverlayRenderer.OverlayOrigin.UpperLeft, overlayDrawInfo.ItemOffset, imageSize, new Int2d(330, 16));

                            overlayDrawInfo.ItemSize = new Int2d(500, 420);

                            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
                            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, $"Brain Diagram", SoupViewOverlayRenderer.OverlayTextBrush);

                            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, $"Node : {targetAnimal.Brain.Nodes.Count}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(0, 388));
                            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, $"Connection : {targetAnimal.Brain.Connections.Count}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(0, 404));

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

                                Double4d arrowColor = _brainDiagramConnectionNeutralWeightColor;
                                if (targetConnection.Weight < 0) arrowColor = Double4d.Lerp(arrowColor, _brainDiagramConnectionNegativeWeightColor, double.Min(1d, -targetConnection.Weight));
                                if (targetConnection.Weight > 0) arrowColor = Double4d.Lerp(arrowColor, _brainDiagramConnectionPositiveWeightColor, double.Min(1d, targetConnection.Weight));

                                SoupViewOverlayRenderer.OverlayDrawLine(overlayDrawInfo, graphics, (Color)arrowColor, new Int2d((int)originPos.X, (int)originPos.Y), new Int2d((int)targetPos.X, (int)targetPos.Y));
                                SoupViewOverlayRenderer.OverlayDrawLine(overlayDrawInfo, graphics, (Color)arrowColor, new Int2d((int)targetPos.X, (int)targetPos.Y), new Int2d((int)(targetPos.X + arrowLineVector1.X * 7), (int)(targetPos.Y + arrowLineVector1.Y * 7)));
                                SoupViewOverlayRenderer.OverlayDrawLine(overlayDrawInfo, graphics, (Color)arrowColor, new Int2d((int)targetPos.X, (int)targetPos.Y), new Int2d((int)(targetPos.X + arrowLineVector2.X * 7), (int)(targetPos.Y + arrowLineVector2.Y * 7)));
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
                                SoupViewOverlayRenderer.OverlayFillEllipse(overlayDrawInfo, graphics, (Color)nodeColor, new Int2d((int)nodePos.X, (int)nodePos.Y), 7);
                                SoupViewOverlayRenderer.OverlayDrawEllipse(overlayDrawInfo, graphics, _nodeOutlineAndConnectionPen, new Int2d((int)nodePos.X, (int)nodePos.Y), 7);
                            }
                            for (int i = 0; i < targetAnimal.Brain.Nodes.Count; i++)
                            {
                                BrainNode targetNode = targetAnimal.Brain.Nodes[i];

                                Double2d nodePos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * i) * 180d + new Double2d(250, 220);

                                if (targetNode.Function == BrainNodeFunction.NonOperation) SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"#{i} Nop", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                                if (targetNode.IsInput) SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"#{i} Input", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                                if (targetNode.IsHidden) SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"#{i} Hidden", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                                if (targetNode.IsOutput) SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"#{i} Output", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d((int)(nodePos.X + 7), (int)(nodePos.Y + 7)));
                            }

                            overlayDrawInfo.NextColumn();
                        }

                        if ((overlayToggles & OverlayToggles.AnimalBrainInputOutput) == OverlayToggles.AnimalBrainInputOutput)
                        {
                            overlayDrawInfo.ItemSize = new Int2d(200, 420);

                            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
                            overlayDrawInfo.ItemSize = new Int2d(overlayDrawInfo.ItemSize.X, 19);

                            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, $"Brain Inputs : ", SoupViewOverlayRenderer.OverlayTextBrush);

                            overlayDrawInfo.NextLine();
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Bias))
                            {
                                if (targetAnimal.Age >= 0) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Bias.ToString()}", 1d, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                                else SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Bias.ToString()}", 0d, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            }
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Velocity)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Velocity.ToString()}", targetAnimal.Brain.Input.Velocity, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AngularVelocity)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AngularVelocity.ToString()}", targetAnimal.Brain.Input.AngularVelocity, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Element)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Element.ToString()}", targetAnimal.Brain.Input.Element, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_ReproductionProgress)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_ReproductionProgress.ToString()}", targetAnimal.Brain.Input.ReproductionProgress, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Age)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Age.ToString()}", targetAnimal.Brain.Input.Age, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Ate)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Ate.ToString()}", targetAnimal.Brain.Input.Ate, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_Attacked)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_Attacked.ToString()}", targetAnimal.Brain.Input.Attacked, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AttackdAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AttackdAngle.ToString()}", targetAnimal.Brain.Input.AttackedAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AttackSuccessful)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AttackSuccessful.ToString()}", targetAnimal.Brain.Input.AttackSuccessful, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_WallWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgProximity)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_WallWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgProximity, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_WallWAvgDistance)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_WallWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.WallWAvgDistance, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PlantWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgProximity)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PlantWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgProximity, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PlantWAvgDistance)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PlantWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.PlantWAvgDistance, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AnimalWAvgAngle.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgProximity)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AnimalWAvgProximity.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgProximity, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgDistance)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AnimalWAvgDistance.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgDistance, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_AnimalWAvgSpeciesSigDiff.ToString()}", targetAnimal.Brain.Input.VisionData.AnimalWAvgSpeciesSigDiff, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneRedConcentration)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneRedConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneRedConcentration, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneRedGradAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneRedGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneRedGradAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneGreenConcentration)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneGreenConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneGreenConcentration, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneGreenGradAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneGreenGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneGreenGradAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneBlueConcentration)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneBlueConcentration.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneBlueConcentration, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Input_PheromoneBlueGradAngle)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Input_PheromoneBlueGradAngle.ToString()}", targetAnimal.Brain.Input.VisionData.PheromoneBlueGradAngle, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);

                            overlayDrawInfo.NextLine(); ;
                            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, $"Brain Outputs : ", SoupViewOverlayRenderer.OverlayTextBrush);

                            overlayDrawInfo.NextLine(); ;
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Acceleration)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_Acceleration.ToString()}", targetAnimal.Brain.Output.Acceleration, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Rotation)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_Rotation.ToString()}", targetAnimal.Brain.Output.Rotation, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Eat)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_Eat.ToString()}", targetAnimal.Brain.Output.Eat, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Attack)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_Attack.ToString()}", targetAnimal.Brain.Output.Attack, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_Reproduction)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_Reproduction.ToString()}", targetAnimal.Brain.Output.Reproduction, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneRedProduction)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_PheromoneRedProduction.ToString()}", targetAnimal.Brain.Output.PheromoneRedProduction, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneGreenProduction)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_PheromoneGreenProduction.ToString()}", targetAnimal.Brain.Output.PheromoneGreenProduction, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                            if (targetAnimal.Brain.ContainNodeFunction(BrainNodeFunction.Output_PheromoneBlueProduction)) SoupViewOverlayRenderer.OverlayDrawAnimalBrainInOutInfomation(overlayDrawInfo, graphics, $"{BrainNodeFunction.Output_PheromoneBlueProduction.ToString()}", targetAnimal.Brain.Output.PheromoneBlueProduction, _brainDiagramNodeNeutralOutputColor, _brainDiagramNodeNegativeOutputColor, _brainDiagramNodePositiveOutputColor, _nodeOutlineAndConnectionPen);
                        }

                        if ((overlayToggles & OverlayToggles.AnimalBrainDiagram) == OverlayToggles.AnimalBrainDiagram)
                        {
                            // マウスホバーしたノードの詳細情報を表示する
                            for (int i = targetAnimal.Brain.Nodes.Count - 1; i >= 0; i--)
                            {
                                BrainNode targetNode = targetAnimal.Brain.Nodes[i];

                                Double2d nodePos = Double2d.FromAngle01(-0.5d + 1d / targetAnimal.Brain.Nodes.Count * i) * 180d + new Double2d(250, 220);

                                if (Double2d.DistanceSquared(nodePos, new Double2d(mousePosition.X - overlay2ndLayerDrawInfo.OriginOffset.X, mousePosition.Y - overlay2ndLayerDrawInfo.OriginOffset.Y)) < 7d * 7d)
                                {
                                    overlay2ndLayerDrawInfo.ItemOffset = new Int2d((int)nodePos.X + 7, (int)nodePos.Y + 7);

                                    SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"Node #{i}");
                                    SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"Function : {targetNode.Function.ToString()}");
                                    if (targetNode.IsHidden || targetNode.IsOutput) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"Input : {targetNode.Input.ToString("0.000")}");
                                    if (targetNode.IsInput || targetNode.IsHidden) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"Output : {targetNode.Output.ToString("0.000")}");
                                    SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, string.Empty);
                                    SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"Connections :");

                                    if (targetNode.IsHidden || targetNode.IsOutput)
                                    {
                                        SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"   Incoming :");

                                        int incomingConnectionCount = 0;
                                        for (int j = 0; j < targetAnimal.Brain.Connections.Count; j++)
                                        {
                                            BrainNodeConnection targetConnection = targetAnimal.Brain.Connections[j];

                                            if (targetConnection.TargetIndex == i)
                                            {
                                                if (incomingConnectionCount > 0) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, string.Empty);

                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Connections #{j}");
                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Origin Index : {targetConnection.OriginIndex}");
                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Weight : {targetConnection.Weight.ToString("0.000")}");

                                                incomingConnectionCount++;
                                            }
                                        }
                                        if (incomingConnectionCount == 0) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      No Connection");
                                    }

                                    if (targetNode.IsHidden) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, string.Empty);

                                    if (targetNode.IsInput || targetNode.IsHidden)
                                    {
                                        SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"   Outgoing :");

                                        int outgoingConnectionCount = 0;
                                        for (int j = 0; j < targetAnimal.Brain.Connections.Count; j++)
                                        {
                                            BrainNodeConnection targetConnection = targetAnimal.Brain.Connections[j];

                                            if (targetConnection.OriginIndex == i)
                                            {
                                                if (outgoingConnectionCount > 0) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, string.Empty);

                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Connections #{j}");
                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Target Index : {targetConnection.TargetIndex}");
                                                SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      Weight : {targetConnection.Weight.ToString("0.000")}");

                                                outgoingConnectionCount++;
                                            }
                                        }
                                        if (outgoingConnectionCount == 0) SoupViewOverlayRenderer.OverlayDrawInformation(overlay2ndLayerDrawInfo, graphics, $"      No Connection");
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (selectedObjectPointer.ObjectType == SoupObjectType.Tile)
            {
                Tile? targetTile = (Tile?)selectedObjectPointer.GetSoupObject();

                if (targetTile is not null)
                {
                    SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Tile #{targetTile.Index}");
                    SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Position : ({targetTile.Position.X}, {targetTile.Position.Y})");
                    SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, $"Type : {targetTile.Type.ToString()}");
                    SoupViewOverlayRenderer.OverlayDrawInformationWith2Gauges(overlayDrawInfo, graphics, $"Element : {targetTile.Element.ToString("0.000")}", SoupViewOverlayRenderer.OverlayGaugeColor1, targetTile.Element / settings.ElementPerTile, SoupViewOverlayRenderer.OverlayGaugeColor2, (targetTile.Element - settings.ElementPerTile) / settings.ElementPerTile / 3d);
                    SoupViewOverlayRenderer.OverlayDrawInformationWith2Gauges(overlayDrawInfo, graphics, $"Red Pheromone : {targetTile.PheromoneRed.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneRedEffectiveColor, Math.Sqrt(targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneRedColor, targetTile.PheromoneRed / settings.MaximumEffectivePheromoneAmount);
                    SoupViewOverlayRenderer.OverlayDrawInformationWith2Gauges(overlayDrawInfo, graphics, $"Green Pheromone : {targetTile.PheromoneGreen.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneGreenEffectiveColor, Math.Sqrt(targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneGreenColor, targetTile.PheromoneGreen / settings.MaximumEffectivePheromoneAmount);
                    SoupViewOverlayRenderer.OverlayDrawInformationWith2Gauges(overlayDrawInfo, graphics, $"Blue Pheromone : {targetTile.PheromoneBlue.ToString("0.000")} (eff. : {Math.Sqrt(targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount).ToString("0.000")})", (Color)_pheromoneBlueEffectiveColor, Math.Sqrt(targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount), (Color)_pheromoneBlueColor, targetTile.PheromoneBlue / settings.MaximumEffectivePheromoneAmount);
                }
            }

            graphics.Dispose();
        }

        public static void OverlayDrawInformation(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text)
        {
            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, SoupViewOverlayRenderer.OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }

        public static void OverlayDrawInformationWithGauge(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, SolidBrush gaugeBrush, double gaugeValue)
        {
            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gaugeValue, gaugeBrush);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, SoupViewOverlayRenderer.OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }
        public static void OverlayDrawInformationWithGauge(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, Color gaugeColor, double gaugeValue)
        {
            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gaugeValue, gaugeColor);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, SoupViewOverlayRenderer.OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }

        public static void OverlayDrawInformationWith2Gauges(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, SolidBrush gauge1Brush, double gauge1Value, SolidBrush gauge2Brush, double gauge2Value)
        {
            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gauge1Value, gauge1Brush);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gauge2Value, gauge2Brush);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, SoupViewOverlayRenderer.OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }
        public static void OverlayDrawInformationWith2Gauges(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, Color gauge1Color, double gauge1Value, Color gauge2Color, double gauge2Value)
        {
            SoupViewOverlayRenderer.OverlayFillRectangle(overlayDrawInfo, graphics, SoupViewOverlayRenderer.OverlayBackgroundBrush);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gauge1Value, gauge1Color);
            SoupViewOverlayRenderer.OverlayDrawGauge(overlayDrawInfo, graphics, gauge2Value, gauge2Color);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, SoupViewOverlayRenderer.OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }

        public static void OverlayDrawAnimalBrainInOutInfomation(SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo, Graphics graphics, string nodeName, double value, Double4d neutralOutputColor, Double4d negativeOutputColor, Double4d positiveOutputColor, Pen nodeOutlinePen)
        {
            Double4d color = neutralOutputColor;
            if (value < 0) color = Double4d.Lerp(color, negativeOutputColor, double.Min(1d, -value));
            if (value > 0) color = Double4d.Lerp(color, positiveOutputColor, double.Min(1d, value));

            SoupViewOverlayRenderer.OverlayFillEllipse(overlayDrawInfo, graphics, (Color)color, new Int2d(8, 8), 7);
            SoupViewOverlayRenderer.OverlayDrawEllipse(overlayDrawInfo, graphics, nodeOutlinePen, new Int2d(8, 8), 7);
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"{nodeName}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(18, 0));
            SoupViewOverlayRenderer.OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 8, $"{value.ToString("0.000")}", SoupViewOverlayRenderer.OverlayTextBrush, new Int2d(18, 9));
            overlayDrawInfo.NextLine();
        }
    }
}
