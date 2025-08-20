using Paramecium.Engine;
using Paramecium.Variables;

namespace Paramecium.Rendering
{
    // ログのオーバーレイを描画するクラス (現在未使用)
    public static class LogOverlayRenderer
    {
        public static readonly Double4d LogBackgroundColor = new Double4d(0.5, 0.25, 0.25, 0.25);
        public static readonly Double4d LogTextColor = new Double4d(1, 1, 1, 1);

        public static readonly double LogEntryDisplayTime = 10;
        public static readonly double LogEntryAttenuationStartTime = 7.5;

        public static readonly int MaximumLogEntryCount = 15;

        public static List<LogEntry> LogEntries = new List<LogEntry>();
        public static object LogEntriesLockOjbect = new object();

        public static void DrawLogOverlay(Bitmap soupViewImage, OverlayToggles overlayToggles)
        {
            if ((overlayToggles & OverlayToggles.AllOverlays) != OverlayToggles.AllOverlays) return;

            if ((overlayToggles & OverlayToggles.LogOverlay) == OverlayToggles.LogOverlay)
            {
                Graphics graphics = Graphics.FromImage(soupViewImage);
                Soup? soup = Globals.Soup;

                if (soup is not null)
                {
                    lock (LogEntriesLockOjbect)
                    {
                        for (int i = LogEntries.Count - 1; i >= 0; i--)
                        {
                            if ((DateTime.Now - LogEntries[i].CreatedTime).TotalSeconds > LogEntryDisplayTime || i >= MaximumLogEntryCount) LogEntries.RemoveAt(i);
                        }

                        SoupViewOverlayRenderer.OverlayDrawInfo overlayDrawInfo = new SoupViewOverlayRenderer.OverlayDrawInfo(SoupViewOverlayRenderer.OverlayOrigin.LowerLeft, new Int2d(0, 16), new Int2d(soupViewImage.Width, soupViewImage.Height), new Int2d(800, 16));

                        for (int i = 0; i < LogEntries.Count; i++)
                        {
                            Double4d logBackgroundColor = Double4d.Lerp(LogBackgroundColor, Double4d.Zero, double.Max(0d, double.Min(1d, ((DateTime.Now - LogEntries[i].CreatedTime).TotalSeconds - LogEntryAttenuationStartTime) / (LogEntryDisplayTime - LogEntryAttenuationStartTime))));
                            Double4d logTextColor = Double4d.Lerp(LogTextColor, Double4d.Zero, double.Max(0d, double.Min(1d, ((DateTime.Now - LogEntries[i].CreatedTime).TotalSeconds - LogEntryAttenuationStartTime) / (LogEntryDisplayTime - LogEntryAttenuationStartTime))));

                            SoupViewOverlayRenderer.OverlayDrawInformation(overlayDrawInfo, graphics, (Color)logBackgroundColor, (Color)logTextColor, $"[{LogEntries[i].CreatedTime.ToString("hh:mm:ss.fff")}] {LogEntries[i].Text}");
                        }
                    }
                }

                graphics.Dispose();
            }
        }

        public static void CreateNewLogEntry(string logText)
        {
            lock (LogEntriesLockOjbect)
            {
                LogEntries.Insert(0, new LogEntry(logText));
            }
        }

        public class LogEntry
        {
            public string Text;
            public DateTime CreatedTime;

            public LogEntry(string text)
            {
                Text = text;
                CreatedTime = DateTime.Now;
            }
        }
    }
}
