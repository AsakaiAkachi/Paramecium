using Paramecium.Engine;
using Paramecium.Variables;

namespace Paramecium.Rendering
{
    public static class FullScreenOverlayRenderer
    {
        public static readonly SolidBrush OverlayBackgroundBrush = new SolidBrush(Color.FromArgb(127, 63, 63, 63));
        public static readonly SolidBrush OverlayTextBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

        public static void DrawFullScreenOverlay(Bitmap soupViewImage, double frameTime, OverlayToggles overlayToggles)
        {
            if ((overlayToggles & OverlayToggles.AllOverlays) != OverlayToggles.AllOverlays) return;

            if ((overlayToggles & OverlayToggles.FullScreenOverlay)  == OverlayToggles.FullScreenOverlay)
            {
                Graphics graphics = Graphics.FromImage(soupViewImage);
                Soup? soup = Globals.Soup;

                string statusString = string.Empty;

                if (soup is not null)
                {
                    statusString = $"Status : {soup.SoupState}   " +
                        $"Time Step : {soup.ElapsedTimeSteps} (T{soup.ThreadCount})   " +
                        $"Population (P/A/T) : {soup.PlantPopulation}/{soup.AnimalPopulation}/{soup.TotalPopulation}   " +
                        $"Generation : {soup.LatestGeneration}   " +
                        $"Total Born/Die : {soup.TotalBornCount}/{soup.TotalDieCount}   " +
                        $"TPS : {(1d / soup.StepTime).ToString("0.0")} ({(soup.StepTime * 1000d).ToString("0.0000")}ms)   " +
                        $"FPS : {(1000d / frameTime).ToString("0.0")} ({(frameTime).ToString("0.0000")}ms)   "
                    ;

                    if (soup.Modified) statusString = $"[*{Globals.SoupFileName}]   " + statusString;
                    else statusString = $"[{Globals.SoupFileName}]   " + statusString;
                }

                SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInformation = new SoupViewOverlayRenderer.OverlayDrawInfo(SoupViewOverlayRenderer.OverlayOrigin.LowerLeft, Int2d.Zero, new Int2d(soupViewImage.Width, soupViewImage.Height), new Int2d(soupViewImage.Width, 16));
                SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInformation, graphics, statusString);

                graphics.Dispose();
            }
        }
    }
}
