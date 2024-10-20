using Paramecium.Engine;
using System.IO.Packaging;
using static Paramecium.Forms.Renderer.SoupViewDrawShape;
using static Paramecium.Forms.Renderer.WorldPosViewPosConversion;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewOverlayRenderer
    {
        public static void DrawSoupViewOverlay(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel, Point mousePointClient, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            Graphics targetGraphics = Graphics.FromImage(targetBitmap);
            double cameraZoomFactor = double.Pow(2, cameraZoomLevel);

            DrawObjectInformation(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor, selectedObjectType, selectedObjectIndex);
            DrawSelectedObjectInformation(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor, mousePointClient, selectedObjectType, selectedObjectIndex);

            targetGraphics.Dispose();
        }

        public static void DrawSelectedObjectInformation(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor, Point mousePointClient, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

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

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Plant #{RawIdToString(target.Id, 12)}", 0, 0, Color.FromArgb(255, 255, 255));
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

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Animal #{RawIdToString(target.Id, 12)}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Species ID : #{RawIdToString(target.SpeciesId, 6)}", 0, 0, Color.FromArgb(255, 255, 255));
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

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(255, target.ColorRed, target.ColorGreen, target.ColorBlue));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Color : ({target.ColorRed}, {target.ColorGreen}, {target.ColorBlue})", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OffsetY = 0;
                overlayInformationRenderer.OffsetX += 300;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 500, 420, Color.FromArgb(128, 64, 64, 64));

                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Brain Diagram", 0, 0, Color.FromArgb(255, 255, 255));

                for (int i = 0; i < target.Brain.Connections.Count; i++)
                {
                    BrainNodeConnection targetConnection = target.Brain.Connections[i];

                    if (targetConnection.TargetIndex != targetConnection.OriginIndex)
                    {
                        Double2d connectionOriginPos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * targetConnection.OriginIndex) * 180 + new Double2d(250, 220);
                        Double2d connectionTargetPos = Double2d.FromAngle(-0.5 + 1d / target.Brain.Nodes.Count * targetConnection.TargetIndex) * 180 + new Double2d(250, 220);

                        Double2d arrowVector = (connectionTargetPos - connectionOriginPos).Normalized;
                        Double2d arrowStartPos = connectionOriginPos + arrowVector * 7.5d;
                        Double2d arrowEndPos = connectionOriginPos + arrowVector * ((connectionTargetPos - connectionOriginPos).Length - 7.5d);
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

                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Node : {target.Brain.Nodes.Count}", 0, 388, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Connection : {target.Brain.Connections.Count}", 0, 404, Color.FromArgb(255, 255, 255));

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

                        List<BrainNodeConnection> targetNodeIncomingConnections = target.Brain.EnumerateIncomingConnection(i);
                        List<BrainNodeConnection> targetNodeOutgoingConnections = target.Brain.EnumerateOutgoingConnection(i);

                        List<int> targetNodeIncomingConnectionIndexes = target.Brain.EnumerateIncomingConnectionIndex(i);
                        List<int> targetNodeOutgoingConnectionsIndexes = target.Brain.EnumerateOutgoingConnectionIndex(i);

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Incoming :", 15, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        if (targetNodeIncomingConnections.Count == 0)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"No Incoming Connection", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;
                        }
                        for (int j = 0; j < targetNodeIncomingConnections.Count; j++)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Connection #{targetNodeIncomingConnectionIndexes[j]}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Origin Index : {targetNodeIncomingConnections[j].OriginIndex}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Weight : {targetNodeIncomingConnections[j].Weight.ToString("0.000")}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            if (j < targetNodeIncomingConnections.Count - 1)
                            {
                                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                                overlayInformationRenderer.OffsetY += 16;
                            }
                        }

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OffsetY += 16;

                        overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                        overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Outgoing :", 15, 0, Color.FromArgb(255, 255, 255));
                        overlayInformationRenderer.OffsetY += 16;

                        if (targetNodeOutgoingConnections.Count == 0)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"No Outgoing Connection", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;
                        }
                        for (int j = 0; j < targetNodeOutgoingConnections.Count; j++)
                        {
                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Connection #{targetNodeOutgoingConnectionsIndexes[j]}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Target Index : {targetNodeOutgoingConnections[j].TargetIndex}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Weight : {targetNodeOutgoingConnections[j].Weight.ToString("0.000")}", 30, 0, Color.FromArgb(255, 255, 255));
                            overlayInformationRenderer.OffsetY += 16;

                            if (j < targetNodeOutgoingConnections.Count - 1)
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

                /**
                overlayInformationRenderer.OffsetY = 0;
                overlayInformationRenderer.OffsetX += 500;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 460, 420, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Brain I/O Information", 0, 0, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(1));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Bias}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{1d.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.WallAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_WallAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.WallAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.WallProximity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_WallProximity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.WallProximity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PlantAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PlantAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PlantAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PlantProximity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PlantProximity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PlantProximity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.AnimalSameSpeciesAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_AnimalSameSpeciesAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.AnimalSameSpeciesAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.AnimalSameSpeciesProximity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_AnimalSameSpeciesProximity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.AnimalSameSpeciesProximity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.AnimalOtherSpeciesAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_AnimalOtherSpeciesAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.AnimalOtherSpeciesAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.AnimalOtherSpeciesProximity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_AnimalOtherSpeciesProximity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.AnimalOtherSpeciesProximity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.Velocity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Velocity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.Velocity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.AngularVelocity));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_AngularVelocity}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.AngularVelocity.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.Satiety));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Satiety}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.Satiety.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.Damage));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Damage}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.Damage.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneRed));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneRed}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneRed.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneGreen));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneGreen}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneGreen.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneBlue));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneBlue}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneBlue.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneRedAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneRedAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneRedAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneGreenAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneGreenAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneGreenAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.VisionData.PheromoneBlueAvgAngle));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_PheromoneBlueAvgAngle}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.VisionData.PheromoneBlueAvgAngle.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetX += 200;
                overlayInformationRenderer.OffsetY = 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory0));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory0}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory0.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory1));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory1}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory1.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory2));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory2}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory2.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory3));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory3}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory3.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory4));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory4}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory4.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory5));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory5}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory5.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory6));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory6}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory6.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainInput.PrevStepOutput.Memory7));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Input_Memory7}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainInput.PrevStepOutput.Memory7.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetX += 110;
                overlayInformationRenderer.OffsetY = 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Acceleration));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Acceleration}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Acceleration.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Rotation));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Rotation}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Rotation.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Attack));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Attack}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Attack.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.PheromoneRed));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_PheromoneRed}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.PheromoneRed.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.PheromoneGreen));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_PheromoneGreen}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.PheromoneGreen.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.PheromoneBlue));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_PheromoneBlue}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.PheromoneBlue.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory0));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory0}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory0.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory1));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory1}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory1.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory2));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory2}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory2.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory3));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory3}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory3.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory4));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory4}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory4.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory5));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory5}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory5.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory6));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory6}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory6.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));

                overlayInformationRenderer.OffsetY += 20;
                overlayInformationRenderer.OverlayFillEllipse(7, 9, 15, GetColorFromBrainNodeValue(target.BrainOutput.Memory7));
                overlayInformationRenderer.OverlayDrawEllipse(7, 9, 15, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{BrainNodeType.Output_Memory7}", 18, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 8, $"{target.BrainOutput.Memory7.ToString("0.000")}", 18, 10, Color.FromArgb(255, 255, 255));
                **/
            }
        }

        public static void DrawObjectInformation(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();
            if (selectedObjectType != SelectedObjectType.Animal) return;

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

            Animal selectedAnimal = g_Soup.Animals[selectedObjectIndex];

            for (int x = int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

                    if (targetTile.LocalAnimalPopulation > 0)
                    {
                        List<int> LocalAnimalIndexes = new List<int>(targetTile.LocalAnimalIndexes);
                        for (int i = 0; i < LocalAnimalIndexes.Count; i++)
                        {
                            Animal targetAnimal = g_Soup.Animals[LocalAnimalIndexes[i]];

                            if (targetAnimal.Exist && targetAnimal.Index != selectedObjectIndex)
                            {
                                if (selectedAnimal.SpeciesId == targetAnimal.SpeciesId)
                                {
                                    DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius + 0.5d, Color.FromArgb(128, 0, 255, 128));
                                    //DrawArc(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius + 0.5d, -30d, 60d, Color.FromArgb(0, 255, 128));
                                    //DrawArc(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius + 0.5d, 150d, 60d, Color.FromArgb(0, 255, 128));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string RawIdToString(long rawId, int length)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

            string result = string.Empty;

            for (int i = 0; i < length; i++)
            {
                result = chars[(int)(rawId % 36)] + result;
                rawId /= 36;
            }

            return result;
        }

        private static Color GetColorFromBrainNodeValue(double value)
        {
            if (value >= 0)
            {
                return Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(0, 255, 0), double.Min(1d, value));
            }
            else
            {
                return Lerp(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 0, 0), double.Min(1d, -value));
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
