using Paramecium.Engine;
using static Paramecium.Forms.Renderer.SoupViewDrawShape;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewOverlayRenderer
    {
        public static void DrawSoupViewOverlay(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel, Point mousePointClient, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            Graphics targetGraphics = Graphics.FromImage(targetBitmap);
            double cameraZoomFactor = double.Pow(2, cameraZoomLevel);

            DrawSelectedObjectInformation(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor, mousePointClient, selectedObjectType, selectedObjectIndex);

            targetGraphics.Dispose();
        }

        public static void DrawSelectedObjectInformation(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor, Point mousePointClient, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            OverlayInformationRenderer overlayInformationRenderer = new OverlayInformationRenderer(targetGraphics);

            /**
            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"CurrentTotalElementAmount : {g_Soup.CurrentTotalElementAmount}", 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetY += 16;

            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"ElementAmountMultiplier : {g_Soup.ElementAmountMultiplier}", 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetY += 16;
            **/

            if (selectedObjectType == SelectedObjectType.Tile)
            {
                Tile target = g_Soup.Tiles[selectedObjectIndex];

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Tile #{selectedObjectIndex}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Position : ({target.PositionX}, {target.PositionY})", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Type : {target.Type}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.Element / (g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY)))), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Max(0d, double.Min(1d, target.Element / (g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY)) - 1d))), 16, Color.FromArgb(255, 0, 192, 0));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Element : {target.Element.ToString("0.000")} elm", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.PheromoneRed)), 16, Color.FromArgb(255, 192, 0, 0));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Red Pheromone : {target.PheromoneRed.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.PheromoneGreen)), 16, Color.FromArgb(255, 0, 192, 0));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Green Pheromone : {target.PheromoneGreen.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.PheromoneBlue)), 16, Color.FromArgb(255, 0, 0, 192));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Blue Pheromone : {target.PheromoneBlue.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;
            }
            if (selectedObjectType == SelectedObjectType.Plant)
            {
                Plant target = g_Soup.Plants[selectedObjectIndex];

                DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, target.Position, target.Radius + 0.5d, Color.FromArgb(255, 255, 0));

                string targetIdString = String.Empty;
                long targetId = target.Id;
                string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
                for (int i = 0; i < 12; i++)
                {
                    targetIdString = chars[(int)(targetId % 36)] + targetIdString;
                    targetId /= 36;
                }

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Plant #{targetIdString}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Position : ({target.Position.X.ToString("0.000")}, {target.Position.Y.ToString("0.000")})", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * ((double)target.Velocity.Length / g_Soup.Settings.MaximumVelocity)), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Velocity : ({target.Velocity.X.ToString("0.000")}, {target.Velocity.Y.ToString("0.000")}) / {target.Velocity.Length.ToString("0.000")} u/s", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.Element / g_Soup.Settings.PlantForkCost)), 16, Color.FromArgb(255, 0, 192, 0));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Element : {target.Element.ToString("0.000")} elm", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;
            }
            if (selectedObjectType == SelectedObjectType.Animal)
            {
                Animal target = g_Soup.Animals[selectedObjectIndex];

                DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, target.Position, target.Radius + 0.5d, Color.FromArgb(255, 255, 0));

                string targetIdString = String.Empty;
                long targetId = target.Id;
                string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
                for (int i = 0; i < 12; i++)
                {
                    targetIdString = chars[(int)(targetId % 36)] + targetIdString;
                    targetId /= 36;
                }

                string targetSpecieIdString = String.Empty;
                long targetSpecieId = target.SpeciesId;
                for (int i = 0; i < 6; i++)
                {
                    targetSpecieIdString = chars[(int)(targetSpecieId % 36)] + targetSpecieIdString;
                    targetSpecieId /= 36;
                }

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Animal #{targetIdString}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Species ID : #{targetSpecieIdString}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Generation : {target.Generation}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Offspring : {target.OffspringCount}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * ((double)target.Age / g_Soup.Settings.AnimalMaximumAge)), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Age : {target.Age}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Position : ({target.Position.X.ToString("0.000")}, {target.Position.Y.ToString("0.000")})", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * ((double)target.Velocity.Length / g_Soup.Settings.MaximumVelocity)), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Velocity : ({target.Velocity.X.ToString("0.000")}, {target.Velocity.Y.ToString("0.000")}) / {target.Velocity.Length.ToString("0.000")} u/s", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Angle : {target.Angle.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * (double.Abs(target.AngularVelocity) / g_Soup.Settings.MaximumAngularVelocity)), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Angular Velocity : {target.AngularVelocity.ToString("0.000")} rot/s", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Min(1d, target.Element / g_Soup.Settings.AnimalForkCost)), 16, Color.FromArgb(255, 128, 128, 128));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * double.Max(0d, double.Min(1d, target.Element / g_Soup.Settings.AnimalForkCost - 1d))), 16, Color.FromArgb(255, 0, 192, 0));
                overlayInformationRenderer.OverlayFillRectangle(0, 12, (int)(300 * double.Min(1d, target.CurrentStepElementCost / (g_Soup.Settings.AnimalElementBaseCost + g_Soup.Settings.AnimalElementAccelerationCost + g_Soup.Settings.AnimalElementRotationCost + g_Soup.Settings.AnimalElementAttackCost + (g_Soup.Settings.AnimalElementPheromoneProductionCost * 3d)))), 16, Color.FromArgb(255, 192, 192, 192));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Element : {target.Element.ToString("0.000")} elm (-{target.CurrentStepElementCost.ToString("0.000")} elm/step)", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                /**
                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(255, 192, 0, 0));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * (1d - target.Diet)), 16, Color.FromArgb(255, 0, 192, 0));
                if (target.Diet <= 0.1d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Fully Herbivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.333d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Herbivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.5d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Omnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.9d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Carnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Fully Carnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;
                **/

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(255, target.ColorRed, target.ColorGreen, target.ColorBlue));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Color : ({target.ColorRed}, {target.ColorGreen}, {target.ColorBlue})", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OffsetY = 0;
                overlayInformationRenderer.OffsetX += 300;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 500, 420, Color.FromArgb(128, 64, 64, 64));

                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Brain Diagram", 0, 0, Color.FromArgb(255, 255, 255));

                for (int i = 0; i < target.Brain.Nodes.Count; i++)
                {
                    BrainNode targetNode = target.Brain.Nodes[i];

                    Double2d nodePos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * i) * 180 + new Double2d(250, 220);

                    for (int j = 0; j < targetNode.Connections.Count; j++)
                    {
                        BrainNodeConnection targetConnection = targetNode.Connections[j];

                        if (targetConnection.TargetIndex != i)
                        {
                            Double2d connectionTargetPos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * targetConnection.TargetIndex) * 180 + new Double2d(250, 220);

                            Double2d arrowVector = (connectionTargetPos - nodePos).Normalized;
                            Double2d arrowStartPos = nodePos + arrowVector * 7.5d;
                            Double2d arrowEndPos = nodePos + arrowVector * ((connectionTargetPos - nodePos).Length - 7.5d);
                            Double2d arrowLineVector1 = Double2d.Rotate(arrowVector, 0.375) * 7.5d;
                            Double2d arrowLineVector2 = Double2d.Rotate(arrowVector, -0.375) * 7.5d;

                            Color arrowColor;
                            if (targetConnection.Weight >= 0)
                            {
                                arrowColor = Lerp(Color.FromArgb(255, 255, 255), Color.FromArgb(0, 255, 0), double.Min(1d, targetConnection.Weight));
                            }
                            else
                            {
                                arrowColor = Lerp(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 0, 0), double.Min(1d, -targetConnection.Weight));
                            }

                            overlayInformationRenderer.OverlayDrawLine((int)(arrowStartPos.X), (int)(arrowStartPos.Y), (int)(arrowEndPos.X), (int)(arrowEndPos.Y), arrowColor);
                            overlayInformationRenderer.OverlayDrawLine((int)(arrowEndPos.X), (int)(arrowEndPos.Y), (int)(arrowEndPos.X + arrowLineVector1.X), (int)(arrowEndPos.Y + arrowLineVector1.Y), arrowColor);
                            overlayInformationRenderer.OverlayDrawLine((int)(arrowEndPos.X), (int)(arrowEndPos.Y), (int)(arrowEndPos.X + arrowLineVector2.X), (int)(arrowEndPos.Y + arrowLineVector2.Y), arrowColor);
                        }
                    }
                }

                for (int i = 0; i < target.Brain.Nodes.Count; i++)
                {
                    Double2d nodePos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * i) * 180 + new Double2d(250, 220);
                    BrainNode targetNode = target.Brain.Nodes[i];
                    Color nodeColor;

                    if (!BrainNode.BrainNodeTypeIsOutput(targetNode.Type))
                    {
                        if (targetNode.Output >= 0)
                        {
                            nodeColor = Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(0, 255, 0), double.Min(1d, targetNode.Output));
                        }
                        else
                        {
                            nodeColor = Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 0, 0), double.Min(1d, -targetNode.Output));
                        }
                    }
                    else if (BrainNode.BrainNodeTypeIsOutput(targetNode.Type))
                    {
                        if (targetNode.Input >= 0)
                        {
                            nodeColor = Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(0, 255, 0), double.Min(1d, targetNode.Input));
                        }
                        else
                        {
                            nodeColor = Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 0, 0), double.Min(1d, -targetNode.Input));
                        }
                    }
                    else
                    {
                        nodeColor = Color.FromArgb(0, 0, 0);
                    }

                    overlayInformationRenderer.OverlayFillEllipse((int)(nodePos.X), (int)(nodePos.Y), 15, nodeColor);
                    overlayInformationRenderer.OverlayDrawEllipse((int)(nodePos.X), (int)(nodePos.Y), 15, Color.FromArgb(255, 255, 255));

                    if (BrainNode.BrainNodeTypeIsInput(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Input", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (BrainNode.BrainNodeTypeIsHidden(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Hidden", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (BrainNode.BrainNodeTypeIsOutput(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Output", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (targetNode.Type == BrainNodeType.NonOperation)
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Nop", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                }

                for (int i = 0; i < target.Brain.Nodes.Count; i++)
                {
                    Double2d nodePos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * i) * 180 + new Double2d(250, 220);
                    BrainNode targetNode = target.Brain.Nodes[i];

                    if (BrainNode.BrainNodeTypeIsInput(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Input", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (BrainNode.BrainNodeTypeIsHidden(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Hidden", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (BrainNode.BrainNodeTypeIsOutput(targetNode.Type))
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Output", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                    else if (targetNode.Type == BrainNodeType.NonOperation)
                    {
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"#{i} Nop", (int)(nodePos.X + 7.5), (int)(nodePos.Y + 7.5), Color.FromArgb(255, 255, 255));
                    }
                }

                for (int i = 0; i < target.Brain.Nodes.Count; i++)
                {
                    Double2d nodePos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * i) * 180 + new Double2d(250, 220);

                    if (mousePointClient.X > nodePos.X + overlayInformationRenderer.OffsetX - 7.5d && mousePointClient.X < nodePos.X + overlayInformationRenderer.OffsetX + 7.5d && mousePointClient.Y > nodePos.Y + overlayInformationRenderer.OffsetY - 7.5d && mousePointClient.Y < nodePos.Y + overlayInformationRenderer.OffsetY + 7.5)
                    {
                        BrainNode targetNode = target.Brain.Nodes[i];

                        int PrevOffsetX = overlayInformationRenderer.OffsetX;
                        int PrevOffsetY = overlayInformationRenderer.OffsetY;
                        overlayInformationRenderer.OffsetX += (int)(nodePos.X + 7.5);
                        overlayInformationRenderer.OffsetY += (int)(nodePos.Y + 7.5);

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Node #{i}", 0, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Type : {targetNode.Type}", 0, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Input : {targetNode.Input.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Output : {targetNode.Output.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Connections : ", 0, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        if (targetNode.Connections.Count == 0)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"No Connection", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;
                        }
                        for (int j = 0; j < targetNode.Connections.Count; j++)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Connection #{j}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Target Index : {targetNode.Connections[j].TargetIndex}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Weight : {targetNode.Connections[j].Weight.ToString("0.000")}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            if (j < targetNode.Connections.Count - 1)
                            {
                                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                                overlayInformationRenderer.OffsetY += 16;
                            }
                        }

                        overlayInformationRenderer.OffsetX = PrevOffsetX;
                        overlayInformationRenderer.OffsetY = PrevOffsetY;

                        break;
                    }
                }
            }
        }

        public enum SelectedObjectType
        {
            None,
            Tile,
            Plant,
            Animal
        }
    }
}
