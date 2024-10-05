using Paramecium.Engine;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewFullScreenOverlayRenderer
    {
        public static void DrawSoupViewFullScreenOverlay(ref Bitmap targetBitmap, double fps)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            Graphics targetGraphics = Graphics.FromImage(targetBitmap);

            DrawFullScreenInformation(targetBitmap, targetGraphics, fps);

            targetGraphics.Dispose();
        }

        public static void DrawFullScreenInformation(in Bitmap targetBitmap, in Graphics targetGraphics, double fps)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new InvalidOperationException("The soup has not been created or initialized.");

            OverlayInformationRenderer overlayInformationRenderer = new OverlayInformationRenderer(targetGraphics);

            overlayInformationRenderer.OffsetY = targetBitmap.Height - 16;

            SizeF testSize;
            string text;

            text = $"Status : {g_Soup.SoupState}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"Time Step : {g_Soup.ElapsedTimeSteps} (T{g_Soup.ThreadCount})";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"Population (P/A/T) : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationPlant + g_Soup.PopulationAnimal}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"Generation : {g_Soup.LatestGeneration}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"Total Born/Die : {g_Soup.TotalBornCount}/{g_Soup.TotalDieCount}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"TPS : {g_Soup.TimeStepsPerSecond.ToString("0.0")}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            text = $"FPS : {fps.ToString("0.0")}";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;

            overlayInformationRenderer.OffsetX = 0;
            overlayInformationRenderer.OffsetY -= 16;

            if (!g_Soup.Modified) text = $"File Name : {Path.GetFileName(g_Soup.FilePath)}";
            else text = $"File Name : {Path.GetFileName(g_Soup.FilePath)} (Unsaved)";
            testSize = overlayInformationRenderer.OverlayMeasureString("MS UI Gothic", 12, text);
            overlayInformationRenderer.OverlayFillRectangle(0, 0, (int)testSize.Width + 20, 16, Color.FromArgb(128, 64, 64, 64));
            overlayInformationRenderer.OverlayDrawString("MS UI Gothic", 12, text, 0, 0, Color.FromArgb(255, 255, 255));
            overlayInformationRenderer.OffsetX += (int)testSize.Width + 20;
        }
    }
}
