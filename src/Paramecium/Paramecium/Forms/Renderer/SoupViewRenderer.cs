using Paramecium.Engine;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Paramecium.Forms.Renderer.SoupViewDrawShape;
using static Paramecium.Forms.Renderer.WorldPosViewPosConversion;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewRenderer
    {
        public static void DrawSoupView(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            Graphics targetGraphics = Graphics.FromImage(targetBitmap);
            double cameraZoomFactor = double.Pow(2, cameraZoomLevel);

            DrawSoupBackground(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor);
            if (cameraZoomLevel >= 4)
            {
                DrawSoupPlant(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor);
                DrawSoupAnimal(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor);
                DrawSoupWall(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor);
            }
            DrawSoupBorder(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor);

            targetGraphics.Dispose();
        }

        public static void DrawSoupBackground(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            Bitmap backgroundCanvas = new Bitmap(g_Soup.Settings.SizeX, g_Soup.Settings.SizeY, PixelFormat.Format32bppRgb);

            BitmapData backgroundCanvasData = backgroundCanvas.LockBits(
                new Rectangle(0, 0, g_Soup.Settings.SizeX, g_Soup.Settings.SizeY),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb
            );

            byte[] backgroundCanvasByte = new byte[backgroundCanvas.Width * backgroundCanvas.Height * 4];

            for (int x = 0; x < g_Soup.Settings.SizeX; x++)
            {
                for (int y = 0; y < g_Soup.Settings.SizeY; y++)
                {
                    Color pixelColor = Color.FromArgb(0, 0, 0);

                    Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                    if (targetTile.Type == TileType.Default)
                    {
                        if (targetTile.Element <= 0) pixelColor = Color.FromArgb(0, 0, 32);
                        else if (targetTile.Element <= g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY)) pixelColor = Lerp(Color.FromArgb(0, 0, 32), Color.FromArgb(0, 64, 0), targetTile.Element / (g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY)));
                        else if (targetTile.Element <= g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY) * 2d) pixelColor = Lerp(Color.FromArgb(0, 64, 0), Color.FromArgb(0, 128, 0), (targetTile.Element - (g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY))) / (g_Soup.Settings.TotalElementAmount / (g_Soup.Settings.SizeX * g_Soup.Settings.SizeY)));
                        else pixelColor = Color.FromArgb(0, 128, 0);

                        pixelColor = Color.FromArgb(
                            int.Max(0, int.Min(255, pixelColor.R + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneRed)) * (255 - pixelColor.R)))),
                            int.Max(0, int.Min(255, pixelColor.G + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneGreen)) * (255 - pixelColor.G)))),
                            int.Max(0, int.Min(255, pixelColor.B + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneBlue)) * (255 - pixelColor.B))))
                        );

                        if (cameraZoomLevel < 4)
                        {
                            if (targetTile.LocalPlantPopulation > 0) pixelColor = Color.FromArgb(0, 255, 0);
                            if (targetTile.LocalAnimalPopulation > 0) pixelColor = Color.FromArgb(255, 255, 255);
                        }
                    }
                    else if (g_Soup.Tiles[y * g_Soup.Settings.SizeX + x].Type == TileType.Wall)
                    {
                        pixelColor = Color.FromArgb(128, 128, 128);
                    }

                    backgroundCanvasByte[(y * g_Soup.Settings.SizeX + x) * 4] = (byte)int.Max(0, int.Min(255, pixelColor.B));
                    backgroundCanvasByte[(y * g_Soup.Settings.SizeX + x) * 4 + 1] = (byte)int.Max(0, int.Min(255, pixelColor.G));
                    backgroundCanvasByte[(y * g_Soup.Settings.SizeX + x) * 4 + 2] = (byte)int.Max(0, int.Min(255, pixelColor.R));
                    backgroundCanvasByte[(y * g_Soup.Settings.SizeX + x) * 4 + 3] = (byte)int.Max(0, int.Min(255, pixelColor.A));
                }
            }

            Marshal.Copy(backgroundCanvasByte, 0, backgroundCanvasData.Scan0, backgroundCanvasByte.Length);

            backgroundCanvas.UnlockBits(backgroundCanvasData);

            Bitmap backgroundCanvasDrawImageBuffer = new Bitmap(g_Soup.Settings.SizeX + 1, g_Soup.Settings.SizeY + 1);

            Graphics backgroundCanvasBufferGraphics = Graphics.FromImage(backgroundCanvasDrawImageBuffer);
            backgroundCanvasBufferGraphics.DrawImage(backgroundCanvas, 1, 1, backgroundCanvas.Width, backgroundCanvas.Height);
            backgroundCanvasBufferGraphics.Dispose();

            targetGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            targetGraphics.DrawImage(
            backgroundCanvasDrawImageBuffer,
                (float)(WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - Math.Floor(0.5d * cameraZoomFactor)),
                (float)(WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - Math.Floor(0.5d * cameraZoomFactor)),
                (float)(backgroundCanvasDrawImageBuffer.Width * cameraZoomFactor),
                (float)(backgroundCanvasDrawImageBuffer.Height * cameraZoomFactor)
            );

            backgroundCanvas.Dispose();
            backgroundCanvasDrawImageBuffer.Dispose();
        }

        public static void DrawSoupBorder(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, -1d), new Double2d(g_Soup.Settings.SizeX + 1, 0), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, g_Soup.Settings.SizeY), new Double2d(g_Soup.Settings.SizeX + 1, g_Soup.Settings.SizeY + 1), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, -1d), new Double2d(0, g_Soup.Settings.SizeY + 1), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(g_Soup.Settings.SizeX, -1d), new Double2d(g_Soup.Settings.SizeX + 1, g_Soup.Settings.SizeY + 1), Color.FromArgb(0, 0, 0));

            DrawRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(0d, 0d), new Double2d(g_Soup.Settings.SizeX, g_Soup.Settings.SizeY), Color.FromArgb(255, 0, 0));
        }

        public static void DrawSoupWall(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            for (int x = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                    if (targetTile.Type == TileType.Wall)
                    {
                        FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(x + 0.25d, y + 0.25d), 0.356d, Color.FromArgb(128, 128, 128));
                        FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(x + 0.75d, y + 0.25d), 0.356d, Color.FromArgb(128, 128, 128));
                        FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(x + 0.25d, y + 0.75d), 0.356d, Color.FromArgb(128, 128, 128));
                        FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(x + 0.75d, y + 0.75d), 0.356d, Color.FromArgb(128, 128, 128));
                    }
                }
            }
        }

        public static void DrawSoupPlant(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            for (int x = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                    if (targetTile.LocalPlantPopulation > 0)
                    {
                        List<int> LocalPlantIndexes = new List<int>(targetTile.LocalPlantIndexes);
                        for (int i = 0; i < LocalPlantIndexes.Count; i++)
                        {
                            Plant targetPlant = g_Soup.Plants[LocalPlantIndexes[i]];

                            if (targetPlant.Exist)
                            {
                                FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetPlant.Position, targetPlant.Radius, Lerp(Color.FromArgb(0, 64, 0), Color.FromArgb(0, 255, 0), targetPlant.Element / g_Soup.Settings.PlantForkCost));
                                DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetPlant.Position, targetPlant.Radius, Color.FromArgb(255, 255, 255));
                            }
                        }
                    }
                }
            }
        }

        public static void DrawSoupAnimal(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            for (int x = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                    if (targetTile.LocalAnimalPopulation > 0)
                    {
                        List<int> LocalAnimalIndexes = new List<int>(targetTile.LocalAnimalIndexes);
                        for (int i = 0; i < LocalAnimalIndexes.Count; i++)
                        {
                            Animal targetAnimal = g_Soup.Animals[LocalAnimalIndexes[i]];

                            if (targetAnimal.Exist)
                            {
                                FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius, Color.FromArgb(targetAnimal.ColorRed, targetAnimal.ColorGreen, targetAnimal.ColorBlue));
                                DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius, Color.FromArgb(255, 255, 255));
                                //DrawLine(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d, targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.75d, Color.FromArgb(255, 255, 255));

                                if (cameraZoomLevel >= 5)
                                {
                                    Random tailAngleRandom = new Random((int)((targetAnimal.Id % int.MaxValue + g_Soup.ElapsedTimeSteps % int.MaxValue) % int.MaxValue));

                                    for (int j = 0; j < 3; j++)
                                    {
                                        DrawLine(
                                            targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor,
                                            targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d,
                                            targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d + Double2d.FromAngle(targetAnimal.Angle + 0.5d + (tailAngleRandom.NextDouble() * 2d - 1d) * 0.05d * targetAnimal.BrainOutput.Acceleration + double.Max(-1d, double.Min(1d, -targetAnimal.BrainOutput.Rotation)) * 0.15d) * 0.5d,
                                            Color.FromArgb(255, 255, 255)
                                        );
                                    }

                                    FillEllipse(
                                        targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor,
                                        targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.075d) * 0.4d,
                                        0.05,
                                        Color.FromArgb(0, 0, 0)
                                    );
                                    FillEllipse(
                                        targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor,
                                        targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle - 0.075d) * 0.4d,
                                        0.05,
                                        Color.FromArgb(0, 0, 0)
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static class SoupViewOverlayRenderer
    {
        public static void DrawSoupOverlayView(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel, Point mousePointClient, SelectedObjectType selectedObjectType, int selectedObjectIndex)
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

            if (selectedObjectType == SelectedObjectType.Tile)
            {
                Tile target = g_Soup.Tiles[selectedObjectIndex];

                OverlayInformationRenderer overlayInformationRenderer = new OverlayInformationRenderer(targetGraphics);

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

                OverlayInformationRenderer overlayInformationRenderer = new OverlayInformationRenderer(targetGraphics);

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

                OverlayInformationRenderer overlayInformationRenderer = new OverlayInformationRenderer(targetGraphics);

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

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(255, 192, 0, 0));
                overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)(300 * (1d - target.Diet)), 16, Color.FromArgb(255, 0, 192, 0));
                if (target.Diet <= 0.1d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Fully Herbivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.333d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Herbivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.5d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Omnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else if (target.Diet <= 0.9d) overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Carnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                else overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"Diet : {target.Diet.ToString("0.000")} (Fully Carnivorous)", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

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

                /**
                overlayInformationRenderer.OffsetX += 400;
                overlayInformationRenderer.OffsetY = 0;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"WallAvgAngle : {target.BrainInput.VisionData.WallAvgAngle.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"WallProximity : {target.BrainInput.VisionData.WallProximity.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"PlantAvgAngle : {target.BrainInput.VisionData.PlantAvgAngle.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"PlantProximity : {target.BrainInput.VisionData.PlantProximity.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"AnimalSameSpeciesAvgAngle : {target.BrainInput.VisionData.AnimalSameSpeciesAvgAngle.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"AnimalSameSpeciesProximity : {target.BrainInput.VisionData.AnimalSameSpeciesProximity.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"AnimalOtherSpeciesAvgAngle : {target.BrainInput.VisionData.AnimalOtherSpeciesAvgAngle.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;

                overlayInformationRenderer.OverlayFillRectangle(0, 0, 300, 16, Color.FromArgb(128, 64, 64, 64));
                overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, $"AnimalOtherSpeciesProximity : {target.BrainInput.VisionData.AnimalOtherSpeciesProximity.ToString("0.000")}", 0, 0, Color.FromArgb(255, 255, 255));
                overlayInformationRenderer.OffsetY += 16;
                **/
            }
        }

        public class OverlayInformationRenderer
        {
            public int OffsetX;
            public int OffsetY;

            Graphics Graphics;

            public OverlayInformationRenderer(Graphics graphics)
            {
                Graphics = graphics;
            }

            public void OverlayDrawRectangle(int startX, int startY, int endX, int endY, Color color)
            {
                SoupViewOverlayRenderer.OverlayDrawRectangle(Graphics, startX + OffsetX, startY + OffsetY, endX + OffsetX, endY + OffsetY, color);
            }
            public void OverlayFillRectangle(int startX, int startY, int endX, int endY, Color color)
            {
                SoupViewOverlayRenderer.OverlayFillRectangle(Graphics, startX + OffsetX, startY + OffsetY, endX + OffsetX, endY + OffsetY, color);
            }

            public void OverlayDrawEllipse(int centerX, int centerY, int diameter, Color color)
            {
                SoupViewOverlayRenderer.OverlayDrawEllipse(Graphics, centerX + OffsetX, centerY + OffsetY, diameter, color);
            }
            public void OverlayFillEllipse(int centerX, int centerY, int diameter, Color color)
            {
                SoupViewOverlayRenderer.OverlayFillEllipse(Graphics, centerX + OffsetX, centerY + OffsetY, diameter, color);
            }

            public void OverlayDrawLine(int startX, int startY, int endX, int endY, Color color)
            {
                SoupViewOverlayRenderer.OverlayDrawLine(Graphics, startX + OffsetX, startY + OffsetY, endX + OffsetX, endY + OffsetY, color);
            }

            public void OverlayDrawString(string fontName, int size, string text, int startX, int startY, Color color)
            {
                SoupViewOverlayRenderer.OverlayDrawString(Graphics, fontName, size, text, startX + OffsetX, startY + OffsetY, color);
            }
        }

        public static void OverlayDrawRectangle(in Graphics targetGraphics, int startX, int startY, int endX, int endY, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawRectangle(colorPen, startX, startY, endX - startX, endY - startY);
            colorPen.Dispose();
        }

        public static void OverlayFillRectangle(in Graphics targetGraphics, int startX, int startY, int endX, int endY, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillRectangle(colorBrush, startX, startY, endX - startX, endY - startY);
            colorBrush.Dispose();
        }

        public static void OverlayDrawEllipse(in Graphics targetGraphics, int centerX, int centerY, int diameter, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawEllipse(colorPen, centerX - diameter / 2, centerY - diameter / 2, diameter, diameter);
            colorPen.Dispose();
        }

        public static void OverlayFillEllipse(in Graphics targetGraphics, int centerX, int centerY, int diameter, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillEllipse(colorBrush, centerX - diameter / 2, centerY - diameter / 2, diameter, diameter);
            colorBrush.Dispose();
        }

        public static void OverlayDrawLine(in Graphics targetGraphics, int startX, int startY, int endX, int endY, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawLine(colorPen, startX, startY, endX, endY);
            colorPen.Dispose();
        }

        public static void OverlayDrawString(in Graphics targetGraphics, string fontName, int size, string text, int startX, int startY, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            Font fnt = new Font(fontName, size);
            targetGraphics.DrawString(text, fnt, colorBrush, startX, startY);
            fnt.Dispose();
            colorBrush.Dispose();
        }

        public enum SelectedObjectType
        {
            None,
            Tile,
            Plant,
            Animal
        }
    }

    public static class SoupViewDrawShape
    {

        public static void DrawEllipse(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d centerPosition, double radius, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawEllipse(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.X - radius),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.Y - radius),
                (float)(radius * 2d * cameraZoomFactor),
                (float)(radius * 2d * cameraZoomFactor)
            );
            colorPen.Dispose();
        }
        public static void FillEllipse(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d centerPosition, double radius, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillEllipse(
                colorBrush,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.X - radius),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.Y - radius),
                (float)(radius * 2d * cameraZoomFactor),
                (float)(radius * 2d * cameraZoomFactor)
            );
            colorBrush.Dispose();
        }

        public static void DrawRectangle(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawRectangle(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)((endPosition.X - startPosition.X) * cameraZoomFactor),
                (float)((endPosition.Y - startPosition.Y) * cameraZoomFactor)
            );
            colorPen.Dispose();
        }
        public static void FillRectangle(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillRectangle(
                colorBrush,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)((endPosition.X - startPosition.X) * cameraZoomFactor),
                (float)((endPosition.Y - startPosition.Y) * cameraZoomFactor)
            );
            colorBrush.Dispose();
        }

        public static void DrawLine(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawLine(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, endPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, endPosition.Y)
            );
            colorPen.Dispose();
        }

        public static double Lerp(double start, double end, double t)
        {
            return (1 - t) * start + t * end;
        }
        public static Color Lerp(Color start, Color end, double t)
        {
            return Color.FromArgb(
                int.Max(0, int.Min(255, (int)Lerp(start.A, end.A, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.R, end.R, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.G, end.G, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.B, end.B, t)))
            );
        }
    }

    public static class WorldPosViewPosConversion
    {
        public static double WorldPosToViewPosX(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, double worldPosX)
        {
            return (worldPosX - cameraPosition.X) * cameraZoomFactor + targetBitmap.Width / 2d;
        }
        public static double WorldPosToViewPosX(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, double worldPosX)
        {
            return (worldPosX - cameraPosition.X) * cameraZoomFactor + targetWidth / 2d;
        }
        public static double WorldPosToViewPosY(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, double worldPosY)
        {
            return (worldPosY - cameraPosition.Y) * cameraZoomFactor + targetBitmap.Height / 2d;
        }
        public static double WorldPosToViewPosY(int targetHeight, Double2d cameraPosition, double cameraZoomFactor, double worldPosY)
        {
            return (worldPosY - cameraPosition.Y) * cameraZoomFactor + targetHeight / 2d;
        }

        public static double ViewPosToWorldPosX(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, int viewPosX)
        {
            return (viewPosX - targetBitmap.Width / 2d) / cameraZoomFactor + cameraPosition.X;
        }
        public static double ViewPosToWorldPosX(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, int viewPosX)
        {
            return (viewPosX - targetWidth / 2d) / cameraZoomFactor + cameraPosition.X;
        }
        public static double ViewPosToWorldPosY(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, int viewPosY)
        {
            return (viewPosY - targetBitmap.Height / 2d) / cameraZoomFactor + cameraPosition.Y;
        }
        public static double ViewPosToWorldPosY(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, int viewPosY)
        {
            return (viewPosY - targetWidth / 2d) / cameraZoomFactor + cameraPosition.Y;
        }
    }
}
