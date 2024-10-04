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

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

            Bitmap backgroundCanvas = new Bitmap(soupSizeX, soupSizeY, PixelFormat.Format32bppRgb);

            BitmapData backgroundCanvasData = backgroundCanvas.LockBits(
                new Rectangle(0, 0, soupSizeX, soupSizeY),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb
            );

            byte[] backgroundCanvasByte = new byte[backgroundCanvas.Width * backgroundCanvas.Height * 4];

            for (int x = 0; x < soupSizeX; x++)
            {
                for (int y = 0; y < soupSizeY; y++)
                {
                    Color pixelColor = Color.FromArgb(0, 0, 32);

                    Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

                    if (targetTile.Type == TileType.Default)
                    {
                        if (targetTile.Element <= 0) pixelColor = Color.FromArgb(0, 0, 32);
                        else if (targetTile.Element <= g_Soup.Settings.TotalElementAmount / (soupSizeX * soupSizeY)) pixelColor = Lerp(Color.FromArgb(0, 0, 32), Color.FromArgb(0, 64, 0), targetTile.Element / (g_Soup.Settings.TotalElementAmount / (soupSizeX * soupSizeY)));
                        else if (targetTile.Element <= g_Soup.Settings.TotalElementAmount / (soupSizeX * soupSizeY) * 2d) pixelColor = Lerp(Color.FromArgb(0, 64, 0), Color.FromArgb(0, 128, 0), (targetTile.Element - (g_Soup.Settings.TotalElementAmount / (soupSizeX * soupSizeY))) / (g_Soup.Settings.TotalElementAmount / (soupSizeX * soupSizeY)));
                        else pixelColor = Color.FromArgb(0, 128, 0);

                        pixelColor = Color.FromArgb(
                            int.Max(0, int.Min(255, pixelColor.R + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneRed)) * (255 - pixelColor.R)))),
                            int.Max(0, int.Min(255, pixelColor.G + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneGreen)) * (255 - pixelColor.G)))),
                            int.Max(0, int.Min(255, pixelColor.B + (int)(double.Max(0d, double.Min(1d, targetTile.PheromoneBlue)) * (255 - pixelColor.B))))
                        );

                        if (targetTile.Element == double.PositiveInfinity || targetTile.Element == double.NegativeInfinity || targetTile.Element == double.NaN) pixelColor = Color.FromArgb(255, 0, 255);

                        if (cameraZoomLevel < 4)
                        {
                            if (targetTile.LocalPlantPopulation > 0) pixelColor = Color.FromArgb(0, 255, 0);
                            if (targetTile.LocalAnimalPopulation > 0) pixelColor = Color.FromArgb(255, 255, 255);
                        }
                    }
                    else if (g_Soup.Tiles[y * soupSizeX + x].Type == TileType.Wall)
                    {
                        pixelColor = Color.FromArgb(128, 128, 128);
                    }

                    backgroundCanvasByte[(y * soupSizeX + x) * 4] = (byte)int.Max(0, int.Min(255, pixelColor.B));
                    backgroundCanvasByte[(y * soupSizeX + x) * 4 + 1] = (byte)int.Max(0, int.Min(255, pixelColor.G));
                    backgroundCanvasByte[(y * soupSizeX + x) * 4 + 2] = (byte)int.Max(0, int.Min(255, pixelColor.R));
                    backgroundCanvasByte[(y * soupSizeX + x) * 4 + 3] = (byte)int.Max(0, int.Min(255, pixelColor.A));
                }
            }

            Marshal.Copy(backgroundCanvasByte, 0, backgroundCanvasData.Scan0, backgroundCanvasByte.Length);

            backgroundCanvas.UnlockBits(backgroundCanvasData);

            Bitmap backgroundCanvasDrawImageBuffer = new Bitmap(soupSizeX + 1, soupSizeY + 1);

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

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

            for (int x = int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

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

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

            for (int x = int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(soupSizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(soupSizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * soupSizeX + x];

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

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

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

                            if (targetAnimal.Exist)
                            {
                                FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius, Color.FromArgb(targetAnimal.ColorRed, targetAnimal.ColorGreen, targetAnimal.ColorBlue));
                                DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position, targetAnimal.Radius, Color.FromArgb(255, 255, 255));
                                //DrawLine(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d, targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.75d, Color.FromArgb(255, 255, 255));

                                if (cameraZoomLevel >= 5)
                                {
                                    Random tailAngleRandom = new Random((int)((targetAnimal.Id + targetAnimal.Age) % int.MaxValue));
                                    Random tailLengthRandom = new Random((int)(targetAnimal.Id % int.MaxValue));

                                    for (int j = 0; j < 3; j++)
                                    {
                                        DrawLine(
                                            targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor,
                                            targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d,
                                            targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d + Double2d.FromAngle(targetAnimal.Angle + 0.5d + (tailAngleRandom.NextDouble() * 2d - 1d) * 0.05d * targetAnimal.BrainOutput.Acceleration + double.Max(-1d, double.Min(1d, -targetAnimal.BrainOutput.Rotation)) * 0.15d) * (0.4d + tailLengthRandom.NextDouble() * 0.2d),
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
}
