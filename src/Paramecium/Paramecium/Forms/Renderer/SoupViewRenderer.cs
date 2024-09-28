using Paramecium.Engine;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Paramecium.Forms.Renderer.WorldPosViewPosConversion;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewRenderer
    {
        public static void DrawSoupView(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel)
        {
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
            //FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(0d, 0d), new Double2d(g_Soup.SizeX, g_Soup.SizeY), Color.FromArgb(0, 0, 32));

            Bitmap backgroundCanvas = new Bitmap(g_Soup.SizeX, g_Soup.SizeY, PixelFormat.Format32bppRgb);

            BitmapData backgroundCanvasData = backgroundCanvas.LockBits(
                new Rectangle(0, 0, g_Soup.SizeX, g_Soup.SizeY),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb
            );

            byte[] backgroundCanvasByte = new byte[backgroundCanvas.Width * backgroundCanvas.Height * 4];

            for (int x = 0; x < g_Soup.SizeX; x++)
            {
                for (int y = 0; y < g_Soup.SizeY; y++)
                {
                    Color pixelColor = Color.FromArgb(0, 0, 0);

                    Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

                    if (targetTile.Type == TileType.Default)
                    {
                        if (targetTile.Element <= 0) pixelColor = Color.FromArgb(0, 0, 32);
                        else if (targetTile.Element <= g_Soup.TotalElementAmount / (g_Soup.SizeX * g_Soup.SizeY)) pixelColor = Lerp(Color.FromArgb(0, 0, 32), Color.FromArgb(0, 64, 0), targetTile.Element / (g_Soup.TotalElementAmount / (g_Soup.SizeX * g_Soup.SizeY)));
                        else if (targetTile.Element <= g_Soup.TotalElementAmount / (g_Soup.SizeX * g_Soup.SizeY) * 2d) pixelColor = Lerp(Color.FromArgb(0, 64, 0), Color.FromArgb(0, 128, 0), (targetTile.Element - (g_Soup.TotalElementAmount / (g_Soup.SizeX * g_Soup.SizeY))) / (g_Soup.TotalElementAmount / (g_Soup.SizeX * g_Soup.SizeY)));
                        else pixelColor = Color.FromArgb(0, 128, 0);

                        if (cameraZoomLevel < 4)
                        {
                            if (targetTile.LocalPlantPopulation > 0) pixelColor = Color.FromArgb(0, 255, 0);
                            if (targetTile.LocalAnimalPopulation > 0) pixelColor = Color.FromArgb(255, 255, 255);
                        }
                    }
                    else if (g_Soup.Tiles[y * g_Soup.SizeX + x].Type == TileType.Wall)
                    {
                        pixelColor = Color.FromArgb(128, 128, 128);
                    }

                    backgroundCanvasByte[(y * g_Soup.SizeX + x) * 4] = (byte)int.Max(0, int.Min(255, pixelColor.B));
                    backgroundCanvasByte[(y * g_Soup.SizeX + x) * 4 + 1] = (byte)int.Max(0, int.Min(255, pixelColor.G));
                    backgroundCanvasByte[(y * g_Soup.SizeX + x) * 4 + 2] = (byte)int.Max(0, int.Min(255, pixelColor.R));
                    backgroundCanvasByte[(y * g_Soup.SizeX + x) * 4 + 3] = (byte)int.Max(0, int.Min(255, pixelColor.A));
                }
            }

            Marshal.Copy(backgroundCanvasByte, 0, backgroundCanvasData.Scan0, backgroundCanvasByte.Length);

            backgroundCanvas.UnlockBits(backgroundCanvasData);

            Bitmap backgroundCanvasDrawImageBuffer = new Bitmap(g_Soup.SizeX + 1, g_Soup.SizeY + 1);

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
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, -1d), new Double2d(g_Soup.SizeX + 1, 0), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, g_Soup.SizeY), new Double2d(g_Soup.SizeX + 1, g_Soup.SizeY + 1), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(-1d, -1d), new Double2d(0, g_Soup.SizeY + 1), Color.FromArgb(0, 0, 0));
            FillRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(g_Soup.SizeX, -1d), new Double2d(g_Soup.SizeX + 1, g_Soup.SizeY + 1), Color.FromArgb(0, 0, 0));

            DrawRectangle(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, new Double2d(0d, 0d), new Double2d(g_Soup.SizeX, g_Soup.SizeY), Color.FromArgb(255, 0, 0));
        }

        public static void DrawSoupWall(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor)
        {
            for (int x = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

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
            for (int x = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

                    if (targetTile.LocalPlantPopulation > 0)
                    {
                        for (int i = 0; i < targetTile.LocalPlantPopulation; i++)
                        {
                            try
                            {
                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];

                                if (targetPlant.Exist)
                                {
                                    FillEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetPlant.Position, targetPlant.Radius, Lerp(Color.FromArgb(0, 64, 0), Color.FromArgb(0, 255, 0), targetPlant.Element / g_Soup.PlantForkCost));
                                    DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, targetPlant.Position, targetPlant.Radius, Color.FromArgb(255, 255, 255));
                                }
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                        }
                    }
                }
            }
        }

        public static void DrawSoupAnimal(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor)
        {
            for (int x = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); x <= int.Max(0, int.Min(g_Soup.SizeX - 1, (int)ViewPosToWorldPosX(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Width) + 1)); x++)
            {
                for (int y = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, 0) - 1)); y <= int.Max(0, int.Min(g_Soup.SizeY - 1, (int)ViewPosToWorldPosY(targetBitmap, cameraPosition, cameraZoomFactor, targetBitmap.Height) + 1)); y++)
                {
                    Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

                    if (targetTile.LocalAnimalPopulation > 0)
                    {
                        for (int i = 0; i < targetTile.LocalAnimalPopulation; i++)
                        {
                            try
                            {
                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];

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
                                                targetAnimal.Position + Double2d.FromAngle(targetAnimal.Angle + 0.5d) * 0.5d + Double2d.FromAngle(targetAnimal.Angle + 0.5d + (tailAngleRandom.NextDouble() * 2d - 1d) * 0.05d * targetAnimal.NeuralNetOutputs[8] + double.Max(-1d, double.Min(1d, targetAnimal.NeuralNetOutputs[9])) * 0.15d) * 0.5d,
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
                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                        }
                    }
                }
            }
        }

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

        private static double Lerp(double start, double end, double t)
        {
            return (1 - t) * start + t * end;
        }
        private static Color Lerp(Color start, Color end, double t)
        {
            return Color.FromArgb(
                int.Max(0, int.Min(255, (int)Lerp(start.A, end.A, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.R, end.R, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.G, end.G, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.B, end.B, t)))
            );
        }
    }

    public static class SoupViewOverlayRenderer
    {
        public static void DrawSoupOverlayView(ref Bitmap targetBitmap, Double2d cameraPosition, int cameraZoomLevel, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            Graphics targetGraphics = Graphics.FromImage(targetBitmap);
            double cameraZoomFactor = double.Pow(2, cameraZoomLevel);

            DrawSelectedObjectFrame(targetBitmap, targetGraphics, cameraPosition, cameraZoomLevel, cameraZoomFactor, selectedObjectType, selectedObjectIndex);

            targetGraphics.Dispose();
        }

        public static void DrawSelectedObjectFrame(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, int cameraZoomLevel, double cameraZoomFactor, SelectedObjectType selectedObjectType, int selectedObjectIndex)
        {
            if (cameraZoomLevel >= 4)
            {
                if (selectedObjectType == SelectedObjectType.Plant)
                {
                    DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, g_Soup.Plants[selectedObjectIndex].Position, g_Soup.Plants[selectedObjectIndex].Radius + 0.5d, Color.FromArgb(255, 255, 0));
                }
                if (selectedObjectType == SelectedObjectType.Animal)
                {
                    DrawEllipse(targetBitmap, targetGraphics, cameraPosition, cameraZoomFactor, g_Soup.Animals[selectedObjectIndex].Position, g_Soup.Animals[selectedObjectIndex].Radius + 0.5d, Color.FromArgb(255, 255, 0));
                }
            }
        }

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

        public enum SelectedObjectType
        {
            None,
            Plant,
            Animal
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
