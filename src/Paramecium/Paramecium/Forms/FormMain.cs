using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public string FilePath;

        public int zoomFactor;
        public double zoomFactorActual;
        public double cameraX;
        public double cameraY;

        private SelectedCellType SelectedCellType = SelectedCellType.None;
        private int SelectedCellIndex = -1;
        private long SelectedCellId = -1;
        private bool Clicked = false;
        private double ClickedPosX = 0d;
        private double ClickedPosY = 0d;
        private double MousePosX = 0d;
        private double MousePosY = 0d;
        private bool Tracking = false;
        private int DisplayMode = 0;

        private bool fullScreen = false;

        private Bitmap canvas;

        DateTime BottomStatLastRefreshTime;
        long tpsOrigin;
        DateTime tpsOriginTime;
        int frameCount;

        public FormMain()
        {
            InitializeComponent();

            MouseWheel += SimulationView_MouseWheel;

            zoomFactor = 0;
            zoomFactorActual = 1d;
            cameraX = 0;
            cameraY = 0;

            canvas = new Bitmap(1, 1);

            BottomStatLastRefreshTime = DateTime.Now;
            tpsOrigin = 0;
            tpsOriginTime = DateTime.Now;
            frameCount = 0;
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            //g_Soup.SoupRun();

            //FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\New Soup.soup";

            Random rnd2 = new Random();

            string Base36 = "0123456789abcdefghijklmnopqrstuvwxyz";

            SolidBrush BrushBackgroundWall = new SolidBrush(Color.FromArgb(127, 127, 127));
            SolidBrush BrushOverlayBackground = new SolidBrush(Color.FromArgb(127, 127, 127, 127));
            SolidBrush BrushOverlayGauge = new SolidBrush(Color.FromArgb(255, 191, 191, 191));
            SolidBrush BrushOverlayDietHerbivore = new SolidBrush(Color.FromArgb(255, 0, 223, 0));
            SolidBrush BrushOverlayDietCarnivorous = new SolidBrush(Color.FromArgb(255, 255, 0, 0));

            SolidBrush BrushCellColorGray = new SolidBrush(Color.FromArgb(127, 127, 127));

            while (true)
            {
                if (g_Soup is not null)
                {
                    if (SimulationView.Width > 0 && SimulationView.Height > 0)
                    {
                        Bitmap canvasOld = canvas;
                        canvas = new Bitmap(SimulationView.Width, SimulationView.Height);

                        Bitmap bgCanvas = new Bitmap(g_Soup.SizeX, g_Soup.SizeY, PixelFormat.Format8bppIndexed);
                        SolidBrush brush1 = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
                        brush1.Dispose();

                        ColorPalette pal = bgCanvas.Palette;
                        pal.Entries[0] = Color.FromArgb(0, 0, 32);
                        pal.Entries[1] = Color.FromArgb(127, 127, 127);
                        pal.Entries[2] = Color.FromArgb(0, 255, 0);
                        pal.Entries[3] = Color.FromArgb(255, 255, 255);
                        for (int i = 0; i <= 32; i++)
                        {
                            pal.Entries[16 + i] = Color.FromArgb(0, 4 * i, 32 - i);
                        }
                        bgCanvas.Palette = pal;

                        BitmapData bmpdata = bgCanvas.LockBits(
                            new Rectangle(0, 0, g_Soup.SizeX, g_Soup.SizeY),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format8bppIndexed
                        );

                        if (zoomFactor >= 4)
                        {
                            Marshal.Copy(g_Soup.GridMapBg, 0, bmpdata.Scan0, g_Soup.GridMapBg.Length);
                        }
                        else
                        {
                            Marshal.Copy(g_Soup.GridMapBgParticle, 0, bmpdata.Scan0, g_Soup.GridMapBgParticle.Length);
                        }
                        bgCanvas.UnlockBits(bmpdata);

                        Bitmap bgCanvas2 = new Bitmap(g_Soup.SizeX + 1, g_Soup.SizeY + 1);

                        Graphics bgCanvas2_g = Graphics.FromImage(bgCanvas2);
                        bgCanvas2_g.DrawImage(bgCanvas, 1, 1, bgCanvas.Width, bgCanvas.Height);
                        bgCanvas2_g.Dispose();

                        if (SelectedCellType != SelectedCellType.None)
                        {
                            if (SelectedCellType == SelectedCellType.Plant)
                            {
                                if (g_Soup.Plants[SelectedCellIndex] is not null)
                                {
                                    if (g_Soup.Plants[SelectedCellIndex].Id == SelectedCellId)
                                    {
                                        if (Tracking)
                                        {
                                            cameraX -= (cameraX - g_Soup.Plants[SelectedCellIndex].Position.X) * 0.1d;
                                            cameraY -= (cameraY - g_Soup.Plants[SelectedCellIndex].Position.Y) * 0.1d;
                                        }
                                    }
                                    else
                                    {
                                        SelectedCellType = SelectedCellType.None;
                                    }
                                }
                                else
                                {
                                    SelectedCellType = SelectedCellType.None;
                                }
                            }
                            if (SelectedCellType == SelectedCellType.Animal)
                            {
                                if (g_Soup.Animals[SelectedCellIndex] is not null)
                                {
                                    if (g_Soup.Animals[SelectedCellIndex].Id == SelectedCellId)
                                    {
                                        if (Tracking)
                                        {
                                            cameraX -= (cameraX - g_Soup.Animals[SelectedCellIndex].Position.X) * 0.1d;
                                            cameraY -= (cameraY - g_Soup.Animals[SelectedCellIndex].Position.Y) * 0.1d;
                                        }
                                    }
                                    else
                                    {
                                        SelectedCellType = SelectedCellType.None;
                                    }
                                }
                                else
                                {
                                    SelectedCellType = SelectedCellType.None;
                                }
                            }
                        }

                        Graphics canvas_g = Graphics.FromImage(canvas);
                        canvas_g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        canvas_g.DrawImage(
                            bgCanvas2,
                            (float)(canvas.Width / 2d - cameraX * zoomFactorActual - Math.Floor(0.5d * zoomFactorActual)),
                            (float)(canvas.Height / 2d - cameraY * zoomFactorActual - Math.Floor(0.5d * zoomFactorActual)),
                            (float)(bgCanvas2.Width * zoomFactorActual),
                            (float)(bgCanvas2.Height * zoomFactorActual)
                        );

                        int x1 = (int)Math.Max(Math.Floor(CanvasPosXToWorldPosX(0d) - 1), 0);
                        int y1 = (int)Math.Max(Math.Floor(CanvasPosYToWorldPosY(0d) - 1), 0);
                        int x2 = (int)Math.Min(Math.Ceiling(CanvasPosXToWorldPosX(canvas.Width) + 1), g_Soup.SizeX - 1);
                        int y2 = (int)Math.Min(Math.Ceiling(CanvasPosYToWorldPosY(canvas.Height) + 1), g_Soup.SizeY - 1);

                        if (zoomFactor >= 4)
                        {
                            /**
                            float x1 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(0d));
                            float x2 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(canvas.Width));
                            float y1 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(0d));
                            float y2 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(canvas.Height));
                            canvas_g.DrawRectangle(Pens.Red, x1, y1, x2 - 1, y2 - 1);
                            **/


                            for (int x = x1; x <= x2; x++)
                            {
                                for (int y = y1; y <= y2; y++)
                                {
                                    if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalPlantCount > 0)
                                    {
                                        List<int> LocalPlants = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalPlants);
                                        for (int i = 0; i < LocalPlants.Count; i++)
                                        {
                                            Plant Target = g_Soup.Plants[LocalPlants[i]];

                                            if (Target is not null)
                                            {
                                                SolidBrush TargetColor = new SolidBrush(Color.FromArgb(0, Math.Min(63 + (int)(192 / g_Soup.PlantForkBiomass * Target.Element), 255), 0));
                                                double TargetPosX = Target.Position.X;
                                                double TargetPosY = Target.Position.Y;
                                                double TargetRadius = Target.Radius;

                                                if (DisplayMode == 5)
                                                {
                                                    TargetColor.Dispose();
                                                    if (Target.CollisionIsDisabled == false) TargetColor = new SolidBrush(Color.FromArgb(255, 255, 0));
                                                    else TargetColor = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                }
                                                if (DisplayMode == 6)
                                                {
                                                    TargetColor.Dispose();
                                                    if (Target.ElementCollectionIsDisabled < 0) TargetColor = new SolidBrush(Color.FromArgb(255, 255, 0));
                                                    else if (Target.ElementCollectionIsDisabled == 0) TargetColor = new SolidBrush(Color.FromArgb(255, 255, 255));
                                                    else TargetColor = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                }

                                                FillCircle(
                                                    canvas_g,
                                                    TargetColor,
                                                    TargetPosX,
                                                    TargetPosY,
                                                    Target.Radius
                                                );
                                                DrawCircle(
                                                    canvas_g,
                                                    Pens.White,
                                                    TargetPosX,
                                                    TargetPosY,
                                                    Target.Radius
                                                );

                                                if (TargetColor != BrushCellColorGray) TargetColor.Dispose();

                                                if (Clicked)
                                                {
                                                    if (Vector2D.Size(Target.Position - new Vector2D(ClickedPosX, ClickedPosY)) < Target.Radius)
                                                    {
                                                        SelectedCellType = SelectedCellType.Plant;
                                                        SelectedCellIndex = Target.Index;
                                                        SelectedCellId = Target.Id;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalAnimalCount > 0)
                                    {
                                        List<int> LocalAnimals = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalAnimals);
                                        for (int i = 0; i < LocalAnimals.Count; i++)
                                        {
                                            Animal Target = g_Soup.Animals[LocalAnimals[i]];

                                            if (Target is not null)
                                            {
                                                SolidBrush TargetColor = new SolidBrush(Color.FromArgb(Target.GeneColorRed, Target.GeneColorGreen, Target.GeneColorBlue));
                                                double TargetPosX = Target.Position.X;
                                                double TargetPosY = Target.Position.Y;
                                                double TargetRadius = Target.Radius;
                                                double TargetAngle = Target.Angle;

                                                if (Target.Age < 0)
                                                {
                                                    TargetColor.Dispose();
                                                    TargetColor = new SolidBrush(Color.FromArgb(255, 127, 255));
                                                }

                                                /*
                                                if (DisplayMode == 1)
                                                {
                                                    TargetColor.Dispose();
                                                    TargetColor = new SolidBrush(Color.FromArgb((int)Math.Min(255d * Target.CumulativeMutationRate / (g_Soup.MutationRate * 10d), 255d), 0, 0));
                                                }
                                                */

                                                FillCircle(
                                                    canvas_g,
                                                    TargetColor,
                                                    TargetPosX,
                                                    TargetPosY,
                                                    Target.Radius
                                                );
                                                DrawCircle(
                                                    canvas_g,
                                                    Pens.White,
                                                    TargetPosX,
                                                    TargetPosY,
                                                    Target.Radius
                                                );

                                                if (zoomFactor >= 5)
                                                {
                                                    Random rnd3 = new Random((int)(Target.Id % 1073741823) + (int)(g_Soup.ElapsedTimeStep % 1073741823));
                                                    if (Target.Age >= 0)
                                                    {
                                                        double TargetAcc = Math.Min(Math.Max(Target.BrainOutputAcceleration, -10d), 10d) * 0.1d;
                                                        double TargetRot = Math.Min(Math.Max(Target.BrainOutputRotation, -10), 10) * -1.5d;
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            DrawLine(
                                                                canvas_g,
                                                                Pens.White,
                                                                TargetPosX - (Vector2D.FromAngle(TargetAngle) * TargetRadius).X,
                                                                TargetPosY - (Vector2D.FromAngle(TargetAngle) * TargetRadius).Y,
                                                                TargetPosX - (Vector2D.FromAngle(TargetAngle) * TargetRadius).X - (Vector2D.FromAngle(TargetAngle + (rnd3.NextDouble() * (60d * TargetAcc) - (30d * TargetAcc)) + TargetRot) * TargetRadius * 1.5d).X,
                                                                TargetPosY - (Vector2D.FromAngle(TargetAngle) * TargetRadius).Y - (Vector2D.FromAngle(TargetAngle + (rnd3.NextDouble() * (60d * TargetAcc) - (30d * TargetAcc)) + TargetRot) * TargetRadius * 1.5d).Y
                                                            );
                                                        }
                                                        SolidBrush bBlack = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                        FillCircle(
                                                            canvas_g,
                                                            bBlack,
                                                            TargetPosX + (Vector2D.FromAngle(TargetAngle - 30) * 0.7d * TargetRadius).X,
                                                            TargetPosY + (Vector2D.FromAngle(TargetAngle - 30) * 0.7d * TargetRadius).Y,
                                                            0.1d * Target.Radius
                                                        );
                                                        FillCircle(
                                                            canvas_g,
                                                            bBlack,
                                                            TargetPosX + (Vector2D.FromAngle(TargetAngle + 30) * 0.7d * TargetRadius).X,
                                                            TargetPosY + (Vector2D.FromAngle(TargetAngle + 30) * 0.7d * TargetRadius).Y,
                                                            0.1d * Target.Radius
                                                        );
                                                        bBlack.Dispose();
                                                    }
                                                }

                                                if (TargetColor != BrushCellColorGray) TargetColor.Dispose();

                                                if (Clicked)
                                                {
                                                    if (Vector2D.Size(Target.Position - new Vector2D(ClickedPosX, ClickedPosY)) < Target.Radius)
                                                    {
                                                        SelectedCellType = SelectedCellType.Animal;
                                                        SelectedCellIndex = Target.Index;
                                                        SelectedCellId = Target.Id;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            for (int x = x1; x <= x2; x++)
                            {
                                for (int y = y1; y <= y2; y++)
                                {
                                    if (g_Soup.GridMapByte[x + y * g_Soup.SizeX] == 0x01)
                                    {
                                        double WallPosX = x + 0.5d;
                                        double WallPosY = y + 0.5d;
                                        FillCircle(canvas_g, BrushBackgroundWall, WallPosX - 0.25, WallPosY - 0.25, 0.353553);
                                        FillCircle(canvas_g, BrushBackgroundWall, WallPosX + 0.25, WallPosY - 0.25, 0.353553);
                                        FillCircle(canvas_g, BrushBackgroundWall, WallPosX - 0.25, WallPosY + 0.25, 0.353553);
                                        FillCircle(canvas_g, BrushBackgroundWall, WallPosX + 0.25, WallPosY + 0.25, 0.353553);
                                    }
                                }
                            }

                            if (SelectedCellType != SelectedCellType.None)
                            {
                                for (int x = x1; x <= x2; x++)
                                {
                                    for (int y = y1; y <= y2; y++)
                                    {
                                        if (SelectedCellType == SelectedCellType.Plant)
                                        {
                                            if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalPlantCount > 0)
                                            {
                                                List<int> LocalPlants = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalPlants);
                                                for (int i = 0; i < LocalPlants.Count; i++)
                                                {
                                                    Plant Target = g_Soup.Plants[LocalPlants[i]];

                                                    if (Target is not null)
                                                    {
                                                        double TargetPosX = Target.Position.X;
                                                        double TargetPosY = Target.Position.Y;
                                                        double TargetRadius = Target.Radius;

                                                        if (Target.Index == SelectedCellIndex && Target.Id == SelectedCellId)
                                                        {
                                                            DrawCircle(
                                                                canvas_g,
                                                                Pens.Yellow,
                                                                TargetPosX,
                                                                TargetPosY,
                                                                Target.Radius + 0.5d
                                                            );
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (SelectedCellType == SelectedCellType.Animal)
                                        {
                                            if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalAnimalCount > 0)
                                            {
                                                List<int> LocalAnimals = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalAnimals);
                                                for (int i = 0; i < LocalAnimals.Count; i++)
                                                {
                                                    Animal Target = g_Soup.Animals[LocalAnimals[i]];

                                                    if (Target is not null)
                                                    {
                                                        try
                                                        {
                                                            double TargetPosX = Target.Position.X;
                                                            double TargetPosY = Target.Position.Y;
                                                            double TargetRadius = Target.Radius;

                                                            if (Target.Index == SelectedCellIndex && Target.Id == SelectedCellId)
                                                            {
                                                                DrawCircle(
                                                                    canvas_g,
                                                                    Pens.Yellow,
                                                                    TargetPosX,
                                                                    TargetPosY,
                                                                    Target.Radius + 0.5d
                                                                );
                                                            }
                                                            else if (Target.RaceId == g_Soup.Animals[SelectedCellIndex].RaceId)
                                                            {
                                                                DrawCircle(
                                                                    canvas_g,
                                                                    Pens.LightYellow,
                                                                    TargetPosX,
                                                                    TargetPosY,
                                                                    Target.Radius + 0.5d
                                                                );
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ConsoleLog(LogLevel.Warning, ex.ToString());
                                                            EventLog.PushEventLog($"例外がスローされました :\r\n{ex}");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));

                        FillRectangle(canvas_g, brush, -16, -16, 16, g_Soup.SizeY + 32);
                        FillRectangle(canvas_g, brush, -16, -16, g_Soup.SizeX + 32, 16);
                        FillRectangle(canvas_g, brush, -16, g_Soup.SizeY, g_Soup.SizeX + 32, 16);
                        FillRectangle(canvas_g, brush, g_Soup.SizeX, -16, 16, g_Soup.SizeY + 32);
                        DrawRectangle(canvas_g, Pens.Red, 0, 0, g_Soup.SizeX, g_Soup.SizeY);

                        brush.Dispose();

                        SolidBrush b_OverlayBackground = new SolidBrush(Color.FromArgb(127, 127, 127, 127));
                        Font fnt_small = new Font("MS UI Gothic", 8);
                        Font fnt = new Font("MS UI Gothic", 12);

                        {
                            int OffsetY = 0;

                            OffsetY += 16;

                            List<EventLog> LoadedEventLog = EventLog.LoadEventLog();
                            for (int i = LoadedEventLog.Count - 1; i >= 0; i--)
                            {
                                string Text = $"{LoadedEventLog[i].EventLogText}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }

                            /*
                            {
                                string Text = $"BiomassAmountMultiplier : {g_Soup.BiomassAmountMultiplier:0.000}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"CurrentBiomassAmount : {g_Soup.CurrentBiomassAmount:0.000}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"Plants.Length : {g_Soup.Plants.Length}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"PlantUnassignedIndexes.Count : {g_Soup.PlantUnassignedIndexes.Count}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"Animals.Length : {g_Soup.Animals.Length}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"AnimalUnassignedIndexes.Count : {g_Soup.AnimalUnassignedIndexes.Count}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            */
                        }

                        if (SelectedCellType == SelectedCellType.None)
                        {
                            Int2D MouseIntegerizedPosition = Vector2D.ToIntegerizedPosition(new Vector2D(MousePosX, MousePosY));

                            int OffsetY = 0;

                            Grid Target = g_Soup.GridMap[MouseIntegerizedPosition.X + MouseIntegerizedPosition.Y * g_Soup.SizeX];

                            {
                                string Text = $"Grid #{MouseIntegerizedPosition.X + MouseIntegerizedPosition.Y * g_Soup.SizeX}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"タイプ : ";
                                if (Target.Type == TileType.None) Text += "通常";
                                else if (Target.Type == TileType.Wall) Text += "壁";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"位置 : ({MouseIntegerizedPosition.X}, {MouseIntegerizedPosition.Y})";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"バイオマス : {Target.Fertility:0.000}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Fertility / (g_Soup.TotalBiomassAmount / (g_Soup.SizeX * g_Soup.SizeY)))), TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                        }
                        else if (SelectedCellType == SelectedCellType.Plant)
                        {
                            Plant target = g_Soup.Plants[SelectedCellIndex];

                            int OffsetY = 0;

                            if (target is not null)
                            {
                                {
                                    string Text = $"Cell #{LongToBase36(target.Id, 12)}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"タイプ : 植物";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"エレメント : {target.Element:0.000} / {g_Soup.PlantForkBiomass:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (target.Element / g_Soup.PlantForkBiomass)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                            }
                        }
                        else if (SelectedCellType == SelectedCellType.Animal)
                        {
                            Animal target = g_Soup.Animals[SelectedCellIndex];

                            int OffsetY = 0;

                            if (target is not null)
                            {
                                {
                                    string Text = $"Cell #{LongToBase36(target.Id, 12)}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"タイプ : ";
                                    if (target.Age >= 0) Text += "動物";
                                    else Text += "胞子";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"種族 : #{LongToBase36(target.RaceId, 6)}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"世代 : {target.Generation}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                /*
                                {
                                    string Text = $"蓄積した変異 : {target.CumulativeMutationRate * 100d:0.000}%";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (target.CumulativeMutationRate / (g_Soup.MutationRate * 10d))), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                */
                                if (target.Age >= 0)
                                {
                                    string Text = $"年齢 : {target.Age}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (target.Age < 0)
                                {
                                    string Text = $"ふ化まであと : {target.Age * -1}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (1d - target.Age * -1 / (double)g_Soup.HatchingTime)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"子孫数 : {target.OffspringCount}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"位置 : ({target.Position.X:0.000}, {target.Position.Y:0.000})";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"速度 : ({target.Velocity.X:0.000}, {target.Velocity.Y:0.000}) / {Vector2D.Size(target.Velocity):0.000}u/t";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Vector2D.Size(target.Velocity) / 0.1d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    SolidBrush brushColor = new SolidBrush(Color.FromArgb(255, target.GeneColorRed, target.GeneColorGreen, target.GeneColorBlue));
                                    string Text = $"色 : ({target.GeneColorRed}, {target.GeneColorGreen}, {target.GeneColorBlue})";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushColor, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                    brushColor.Dispose();
                                }
                                {
                                    string Text = $"エレメント : {target.Element:0.000} / {g_Soup.AnimalForkBiomass:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (target.Element / g_Soup.AnimalForkBiomass)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"繁殖進捗 : {target.OffspringProgress:0.000} / {g_Soup.AnimalForkBiomass:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (target.OffspringProgress / g_Soup.AnimalForkBiomass)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }

                                canvas_g.FillRectangle(b_OverlayBackground, 0 + 256, 0, 320, 250);
                                TextRenderer.DrawText(canvas_g, $"Brain Input/Output", fnt, new Point(0 + 256, 0), Color.White);

                                SolidBrush b_BrainInputViewRed = new SolidBrush(Color.FromArgb(255, 0, 0));
                                SolidBrush b_BrainInputViewGreen = new SolidBrush(Color.FromArgb(0, 255, 0));
                                SolidBrush b_BrainInputViewBlue = new SolidBrush(Color.FromArgb(0, 0, 255));

                                for (int i = -16; i <= 16; i++)
                                {
                                    for (int j = 14; j >= 0; j--)
                                    {
                                        float PieX = 150 - j * 10 + 256;
                                        float PieY = 150 - j * 10;
                                        float PieSize = (j + 1) * 10 * 2;

                                        if (target.BrainInput[(i + 16) * 15 + j] == 0)
                                        {
                                            canvas_g.FillPie(Brushes.Black, PieX, PieY, PieSize, PieSize, i * 5.625f - 2.8125f - 90f, 5.625f);
                                        }
                                        if (target.BrainInput[(i + 16) * 15 + j] == 1)
                                        {
                                            canvas_g.FillPie(Brushes.Gray, PieX, PieY, PieSize, PieSize, i * 5.625f - 2.8125f - 90f, 5.625f);
                                        }
                                        if (target.BrainInput[(i + 16) * 15 + j] == 2)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewGreen, PieX, PieY, PieSize, PieSize, i * 5.625f - 2.8125f - 90f, 5.625f);
                                        }
                                        if (target.BrainInput[(i + 16) * 15 + j] == 3)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewBlue, PieX, PieY, PieSize, PieSize, i * 5.625f - 2.8125f - 90f, 5.625f);
                                        }
                                        if (target.BrainInput[(i + 16) * 15 + j] == 4)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewRed, PieX, PieY, PieSize, PieSize, i * 5.625f - 2.8125f - 90f, 5.625f);
                                        }
                                    }
                                }
                                for (int i = -4; i <= 4; i++)
                                {
                                    for (int j = 4; j >= 0; j--)
                                    {
                                        float PieX = 150 - j * 10 + 256;
                                        float PieY = 150 - j * 10;
                                        float PieSize = (j + 1) * 10 * 2;

                                        if (target.BrainInput[495 + (i + 4) * 5 + j] == 0)
                                        {
                                            canvas_g.FillPie(Brushes.Black, PieX, PieY, PieSize, PieSize, i * 19.375f - 9.6875f - 270f, 19.375f);
                                        }
                                        if (target.BrainInput[495 + (i + 4) * 5 + j] == 1)
                                        {
                                            canvas_g.FillPie(Brushes.Gray, PieX, PieY, PieSize, PieSize, i * 19.375f - 9.6875f - 270f, 19.375f);
                                        }
                                        if (target.BrainInput[495 + (i + 4) * 5 + j] == 2)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewGreen, PieX, PieY, PieSize, PieSize, i * 19.375f - 9.6875f - 270f, 19.375f);
                                        }
                                        if (target.BrainInput[495 + (i + 4) * 5 + j] == 3)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewBlue, PieX, PieY, PieSize, PieSize, i * 19.375f - 9.6875f - 270f, 19.375f);
                                        }
                                        if (target.BrainInput[495 + (i + 4) * 5 + j] == 4)
                                        {
                                            canvas_g.FillPie(b_BrainInputViewRed, PieX, PieY, PieSize, PieSize, i * 19.375f - 9.6875f - 270f, 19.375f);
                                        }
                                    }
                                }

                                b_BrainInputViewRed.Dispose();
                                b_BrainInputViewGreen.Dispose();
                                b_BrainInputViewBlue.Dispose();

                                SolidBrush BrainOutputAcceleration = new SolidBrush(Color.FromArgb(
                                    (int)Math.Min(Math.Max((target.BrainOutputAcceleration) * -25.5d, 0), 255),
                                    (int)Math.Min(Math.Max((target.BrainOutputAcceleration) * 25.5d, 0), 255),
                                    0
                                ));
                                SolidBrush BrainOutputRotation = new SolidBrush(Color.FromArgb(
                                    (int)Math.Min(Math.Max((target.BrainOutputRotation) * -25.5d, 0), 255),
                                    (int)Math.Min(Math.Max((target.BrainOutputRotation) * 25.5d, 0), 255),
                                    0
                                ));
                                SolidBrush BrainOutputAttack = new SolidBrush(Color.FromArgb(
                                    (int)Math.Min(Math.Max((target.BrainOutputAttack) * -25.5d, 0), 255),
                                    (int)Math.Min(Math.Max((target.BrainOutputAttack) * 25.5d, 0), 255),
                                    0
                                ));

                                canvas_g.FillEllipse(BrainOutputAcceleration, 225 + 256, 180, 15, 15);
                                canvas_g.DrawEllipse(Pens.White, 225 + 256, 180, 15, 15);
                                TextRenderer.DrawText(canvas_g, $"Acceleration", fnt_small, new Point(245 + 256, 180), Color.White);
                                TextRenderer.DrawText(canvas_g, $"{target.BrainOutputAcceleration:0.000}", fnt_small, new Point(245 + 256, 190), Color.White);

                                canvas_g.FillEllipse(BrainOutputRotation, 225 + 256, 200, 15, 15);
                                canvas_g.DrawEllipse(Pens.White, 225 + 256, 200, 15, 15);
                                TextRenderer.DrawText(canvas_g, $"Rotation", fnt_small, new Point(245 + 256, 200), Color.White);
                                TextRenderer.DrawText(canvas_g, $"{target.BrainOutputRotation:0.000}", fnt_small, new Point(245 + 256, 210), Color.White);

                                canvas_g.FillEllipse(BrainOutputAttack, 225 + 256, 220, 15, 15);
                                canvas_g.DrawEllipse(Pens.White, 225 + 256, 220, 15, 15);
                                TextRenderer.DrawText(canvas_g, $"Attack", fnt_small, new Point(245 + 256, 220), Color.White);
                                TextRenderer.DrawText(canvas_g, $"{target.BrainOutputAttack:0.000}", fnt_small, new Point(245 + 256, 230), Color.White);

                                BrainOutputAcceleration.Dispose();
                                BrainOutputRotation.Dispose();
                                BrainOutputAttack.Dispose();
                            }
                        }

                        b_OverlayBackground.Dispose();
                        fnt_small.Dispose();
                        fnt.Dispose();

                        canvas_g.Dispose();

                        SimulationView.Image = canvas;
                        canvasOld.Dispose();

                        bgCanvas.Dispose();
                        bgCanvas2.Dispose();

                        Clicked = false;

                        frameCount++;
                    }
                }

                if ((DateTime.Now - BottomStatLastRefreshTime).TotalMilliseconds >= 50)
                {
                    if (g_Soup is not null)
                    {
                        switch (g_Soup.SoupState)
                        {
                            case SoupState.Stop:
                                StatSoupStatus.Text = "Status : Stop";
                                break;
                            case SoupState.Pause:
                                StatSoupStatus.Text = "Status : Pause";
                                break;
                            case SoupState.ResourceLock:
                                StatSoupStatus.Text = "Status : Running";
                                break;
                            case SoupState.Running:
                                StatSoupStatus.Text = "Status : Running";
                                break;
                            case SoupState.StepRun:
                                StatSoupStatus.Text = "Status : Step Run";
                                break;
                        }
                        StatTimeStep.Text = $"Time Step : {g_Soup.ElapsedTimeStep} (T{g_Soup.ParallelismLimit})";
                        StatPopulation.Text = $"Population : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationTotal}";
                        StatLatestGeneration.Text = $"Latest Generation : {g_Soup.LatestGeneration}";
                        StatTotalBornDie.Text = $"Total Born/Die : {g_Soup.TotalBornCount}/{g_Soup.TotalDieCount}";
                    }
                    else
                    {
                        StatSoupStatus.Text = "Status : Stop";
                    }

                    //StatBiomassAmount.Text = $"BiomassAmount : {g_Soup.BiomassAmount:0.000}";
                    BottomStat.Refresh();
                }
                if ((DateTime.Now - tpsOriginTime).TotalMilliseconds >= 1000)
                {
                    if (g_Soup is not null)
                    {
                        StatTps.Text = $"TPS : {g_Soup.ElapsedTimeStep - tpsOrigin}";
                        tpsOrigin = g_Soup.ElapsedTimeStep;
                        tpsOriginTime = DateTime.Now;
                    }
                    StatFps.Text = $"FPS : {frameCount}";
                    frameCount = 0;
                    StatMemory.Text = $"Memory : {Environment.WorkingSet / 1048576d:F1} Mbytes";
                    BottomStat.Refresh();
                }

                this.Text = $"{Path.GetFileName(FilePath)} - Paramecium";

                await Task.Delay(1);
            }
        }

        private float WorldPosXToCanvasPosX(double worldPosX)
        {
            double result = canvas.Width / 2d - (cameraX - worldPosX) * zoomFactorActual;

            return (float)result;
        }
        private double CanvasPosXToWorldPosX(double canvasPosX)
        {
            double result = (canvasPosX - canvas.Width / 2d) / zoomFactorActual + cameraX;

            return result;
        }

        private float WorldPosYToCanvasPosY(double worldPosY)
        {
            double result = canvas.Height / 2d - (cameraY - worldPosY) * zoomFactorActual;

            return (float)result;
        }
        private double CanvasPosYToWorldPosY(double canvasPosY)
        {
            double result = (canvasPosY - canvas.Height / 2d) / zoomFactorActual + cameraY;

            return result;
        }

        private void DrawCircle(in Graphics graphics, in Pen pen, double x, double y, double radius)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float Radius = (float)(radius * zoomFactorActual);

            graphics.DrawEllipse(pen, X - Radius, Y - Radius, Radius * 2f, Radius * 2f);
        }
        private void FillCircle(in Graphics graphics, in SolidBrush brush, double x, double y, double radius)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float Radius = (float)(radius * zoomFactorActual);

            graphics.FillEllipse(brush, X - Radius, Y - Radius, Radius * 2f, Radius * 2f);
        }
        private void FillCircle(in Graphics graphics, in Brush brush, double x, double y, double radius)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float Radius = (float)(radius * zoomFactorActual);

            graphics.FillEllipse(brush, X - Radius, Y - Radius, Radius * 2f, Radius * 2f);
        }
        private void DrawRectangle(in Graphics graphics, in Pen pen, double x, double y, double sizeX, double sizeY)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float SizeX = (float)(sizeX * zoomFactorActual);
            float SizeY = (float)(sizeY * zoomFactorActual);

            graphics.DrawRectangle(pen, X, Y, SizeX, SizeY);
        }
        private void FillRectangle(in Graphics graphics, in SolidBrush brush, double x, double y, double sizeX, double sizeY)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float SizeX = (float)(sizeX * zoomFactorActual);
            float SizeY = (float)(sizeY * zoomFactorActual);

            graphics.FillRectangle(brush, X, Y, SizeX, SizeY);
        }
        private void FillRectangle(in Graphics graphics, in Brush brush, double x, double y, double sizeX, double sizeY)
        {
            float X = WorldPosXToCanvasPosX(x);
            float Y = WorldPosYToCanvasPosY(y);
            float SizeX = (float)(sizeX * zoomFactorActual);
            float SizeY = (float)(sizeY * zoomFactorActual);

            graphics.FillRectangle(brush, X, Y, SizeX, SizeY);
        }
        private void DrawLine(in Graphics graphics, in Pen pen, double startX, double startY, double endX, double endY)
        {
            float StartX = WorldPosXToCanvasPosX(startX);
            float StartY = WorldPosYToCanvasPosY(startY);
            float EndX = WorldPosXToCanvasPosX(endX);
            float EndY = WorldPosYToCanvasPosY(endY);

            graphics.DrawLine(pen, StartX, StartY, EndX, EndY);
        }

        private async void TopMenu_File_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.FileName = "";
            ofd.InitialDirectory = Path.GetDirectoryName($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\");
            ofd.Filter = "Paramecium スープファイル(*.soup)|*.soup|すべてのファイル(*.*)|*.*";
            ofd.Title = "開く";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Soup? prevSoup = g_Soup;
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Stop);

                try
                {
                    string jsonString;

                    EventLog.PushEventLog($"スープを読み込んでいます...");
                    await Task.Delay(100);

                    StreamReader sr = new StreamReader(ofd.FileName);

                    jsonString = sr.ReadToEnd();
                    g_Soup = JsonSerializer.Deserialize<Soup>(jsonString);
                    g_Soup.SoupLoaded();
                    g_Soup.SoupRun();

                    jsonString = String.Empty;

                    sr.Dispose();

                    FilePath = ofd.FileName;

                    if (g_Soup is not null)
                    {
                        g_FormMain.zoomFactor = 0;
                        g_FormMain.zoomFactorActual = 1;
                        g_FormMain.cameraX = g_Soup.SizeX / 2d;
                        g_FormMain.cameraY = g_Soup.SizeY / 2d;
                    }

                    GC.Collect();

                    EventLog.PushEventLog($"スープファイル「{Path.GetFileName(FilePath)}」を読み込みました");
                }
                catch
                {
                    if (prevSoup is not null) g_Soup = prevSoup;
                }
            }
        }

        private async void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            if (g_Soup is not null)
            {
                g_Soup.SetSoupState(SoupState.Pause);

                EventLog.PushEventLog($"スープを保存しています...");
                await Task.Delay(100);

                string jsonString = JsonSerializer.Serialize(g_Soup);

                StreamWriter sw = new StreamWriter(FilePath, false);

                sw.Write(jsonString);

                sw.Dispose();

                jsonString = String.Empty;

                GC.Collect();

                EventLog.PushEventLog($"現在のスープを「{Path.GetFileName(FilePath)}」に上書き保存しました");
            }
        }

        private async void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Untitled.soup";
            sfd.InitialDirectory = Path.GetDirectoryName($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\");
            sfd.Filter = "Paramecium スープファイル(*.soup)|*.soup|すべてのファイル(*.*)|*.*";
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (g_Soup is not null)
                {
                    g_Soup.SetSoupState(SoupState.Pause);

                    EventLog.PushEventLog($"スープを保存しています...");
                    await Task.Delay(100);

                    string jsonString = JsonSerializer.Serialize(g_Soup);

                    StreamWriter sw = new StreamWriter(sfd.FileName, false);

                    sw.Write(jsonString);

                    sw.Dispose();

                    jsonString = String.Empty;

                    FilePath = sfd.FileName;

                    GC.Collect();

                    EventLog.PushEventLog($"現在のスープが「{Path.GetFileName(FilePath)}」として保存されました");
                }
            }
        }

        private void TopMenu_File_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        bool EventViewer = false;
        FormEventViewer? FormEventViewer;
        private void TopMenu_Window_EventViewer_Click(object sender, EventArgs e)
        {
            if (!EventViewer)
            {
                FormEventViewer = new FormEventViewer(this);
                FormEventViewer.Show();
                EventViewer = true;
                TopMenu_Window_EventViewer.Checked = true;
            }
            else
            {
                if (FormEventViewer is not null) FormEventViewer.Close();
                EventViewer = false;
                TopMenu_Window_EventViewer.Checked = false;
            }
        }
        public void FormEventViewer_Closed()
        {
            EventViewer = false;
            TopMenu_Window_EventViewer.Checked = false;
        }

        bool DebugConsole = false;
        private void TopMenu_Window_DebugConsole_Click(object sender, EventArgs e)
        {
            if (!DebugConsole)
            {
                AllocConsole();
                DebugConsole = true;
                TopMenu_Window_DebugConsole.Checked = true;
            }
            else
            {
                FreeConsole();
                DebugConsole = false;
                TopMenu_Window_DebugConsole.Checked = false;
            }
        }

        private void TopMenu_Simulation_NewSimulation_Click(object sender, EventArgs e)
        {
            //if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Pause);

            g_FormNewSimulation.ShowDialog();
        }

        bool drag = false;
        int prevX, prevY;
        private void SimulationView_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (Control.ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            //if (zoomFactor < 8) zoomFactor++;
                            //zoomFactorActual = Math.Pow(2, zoomFactor);
                            break;
                        case Keys.Alt:
                            break;
                        default:
                            Clicked = true;
                            SelectedCellType = SelectedCellType.None;
                            SelectedCellIndex = -1;
                            SelectedCellId = -1;
                            ClickedPosX = CanvasPosXToWorldPosX(e.X);
                            ClickedPosY = CanvasPosYToWorldPosY(e.Y);
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (Control.ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            //if (zoomFactor > 0) zoomFactor--;
                            //zoomFactorActual = Math.Pow(2, zoomFactor);
                            break;
                        case Keys.Alt:
                            break;
                        default:
                            drag = true;
                            prevX = e.X;
                            prevY = e.Y;
                            Tracking = false;
                            break;
                    }
                    break;
            }
        }

        private void SimulationView_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void SimulationView_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                cameraX += (prevX - e.X) / zoomFactorActual;
                cameraY += (prevY - e.Y) / zoomFactorActual;
                prevX = e.X;
                prevY = e.Y;
            }
            else
            {
                MousePosX = CanvasPosXToWorldPosX(e.X);
                MousePosY = CanvasPosYToWorldPosY(e.Y);

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        switch (Control.ModifierKeys)
                        {
                            case Keys.Shift:
                                if (g_Soup is not null)
                                {
                                    if (g_Soup.SoupState == SoupState.Pause)
                                    {
                                        if (SelectedCellType != SelectedCellType.None)
                                        {
                                            if (SelectedCellType == SelectedCellType.Plant)
                                            {
                                                if (g_Soup.SoupState == SoupState.Pause)
                                                {
                                                    Plant Target = g_Soup.Plants[SelectedCellIndex];

                                                    if (Target.IsValid)
                                                    {
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalPlants.Remove(Target.Index);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalPlantCount--;
                                                        Target.Position = new Vector2D(CanvasPosXToWorldPosX(e.X), CanvasPosYToWorldPosY(e.Y));
                                                        Target.IntegerizedPosition = Vector2D.ToIntegerizedPosition(Target.Position);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalPlants.Add(Target.Index);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalPlantCount++;
                                                    }
                                                }
                                            }
                                            if (SelectedCellType == SelectedCellType.Animal)
                                            {
                                                if (g_Soup.SoupState == SoupState.Pause)
                                                {
                                                    Animal Target = g_Soup.Animals[SelectedCellIndex];

                                                    if (Target.IsValid)
                                                    {
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalAnimals.Remove(Target.Index);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalAnimalCount--;
                                                        Target.Position = new Vector2D(CanvasPosXToWorldPosX(e.X), CanvasPosYToWorldPosY(e.Y));
                                                        Target.IntegerizedPosition = Vector2D.ToIntegerizedPosition(Target.Position);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalAnimals.Add(Target.Index);
                                                        g_Soup.GridMap[Target.IntegerizedPosition.X + Target.IntegerizedPosition.Y * g_Soup.SizeX].LocalAnimalCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case Keys.Control:
                                if (g_Soup is not null)
                                {
                                    if (g_Soup.SoupState == SoupState.Pause)
                                    {
                                        Int2D MouseGridPosition = Vector2D.ToIntegerizedPosition(new Vector2D(MousePosX, MousePosY));
                                        if (MouseGridPosition.X >= 0 && MouseGridPosition.X < g_Soup.SizeX && MouseGridPosition.Y >= 0 && MouseGridPosition.Y < g_Soup.SizeY)
                                        {
                                            g_Soup.GridMap[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX].Type = TileType.Wall;
                                            g_Soup.GridMapBg[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x01;
                                            g_Soup.GridMapBgParticle[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x01;
                                            g_Soup.GridMapByte[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x01;
                                        }
                                    }
                                }
                                break;
                            case Keys.Alt:
                                break;
                            default:
                                break;
                        }
                        break;
                    case MouseButtons.Right:
                        switch (Control.ModifierKeys)
                        {
                            case Keys.Shift:
                                break;
                            case Keys.Control:
                                if (g_Soup is not null)
                                {
                                    if (g_Soup.SoupState == SoupState.Pause)
                                    {
                                        Int2D MouseGridPosition = Vector2D.ToIntegerizedPosition(new Vector2D(MousePosX, MousePosY));
                                        if (MouseGridPosition.X >= 0 && MouseGridPosition.X < g_Soup.SizeX && MouseGridPosition.Y >= 0 && MouseGridPosition.Y < g_Soup.SizeY)
                                        {
                                            g_Soup.GridMap[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX].Type = TileType.None;
                                            g_Soup.GridMapBg[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x00;
                                            g_Soup.GridMapBgParticle[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x00;
                                            g_Soup.GridMapByte[MouseGridPosition.X + MouseGridPosition.Y * g_Soup.SizeX] = 0x00;
                                        }
                                    }
                                }
                                break;
                            case Keys.Alt:
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }
        private void SimulationView_MouseWheel(object? sender, MouseEventArgs e)
        {
            zoomFactor = Math.Min(Math.Max(zoomFactor + e.Delta / 100, 0), 8);
            zoomFactorActual = Math.Pow(2, zoomFactor);
        }

        FormWindowState premWindowState;

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (g_Soup is not null)
                    {
                        if (g_Soup.SoupState == SoupState.Pause)
                        {
                            g_Soup.SetSoupState(SoupState.Running);
                            EventLog.PushEventLog("シミュレーションを再開しました");
                        }
                        else
                        {
                            g_Soup.SetSoupState(SoupState.Pause);
                            EventLog.PushEventLog("シミュレーションを一時停止しました");
                        }
                    }
                    break;
                case Keys.OemPeriod:
                    if (g_Soup is not null) g_Soup.ParallelismLimit++;
                    break;
                case Keys.Oemcomma:
                    if (g_Soup is not null)
                    {
                        if (g_Soup.ParallelismLimit > 1)
                        {
                            g_Soup.ParallelismLimit--;
                        }
                    }
                    break;
                case Keys.OemQuestion:
                    if (g_Soup is not null) g_Soup.SetSoupState(SoupState.StepRun);
                    break;
                case Keys.R:
                    if (!Tracking) Tracking = true;
                    else Tracking = false;
                    break;
                case Keys.C:
                    if (g_Soup is not null)
                    {
                        g_FormMain.zoomFactor = 0;
                        g_FormMain.zoomFactorActual = 1;
                        g_FormMain.cameraX = g_Soup.SizeX / 2d;
                        g_FormMain.cameraY = g_Soup.SizeY / 2d;
                        Tracking = false;
                    }
                    break;
                case Keys.F:
                    if (!fullScreen)
                    {
                        fullScreen = true;
                        premWindowState = WindowState;
                        WindowState = FormWindowState.Normal;
                        FormBorderStyle = FormBorderStyle.None;
                        WindowState = FormWindowState.Maximized;
                        TopMost = true;
                        TopMenu.Visible = false;
                        BottomStat.Visible = false;
                        SimulationView.Dock = DockStyle.Fill;
                    }
                    else
                    {
                        fullScreen = false;
                        TopMost = false;
                        WindowState = FormWindowState.Normal;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        if (WindowState != premWindowState) WindowState = premWindowState;
                        TopMenu.Visible = true;
                        BottomStat.Visible = true;
                        SimulationView.Dock = DockStyle.None;
                        SimulationView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                        SimulationView.Location = new Point(0, 24);
                        SimulationView.Size = new Size(ClientSize.Width, ClientSize.Height - 46);
                    }
                    break;
                case Keys.D1:
                    DisplayMode = 0;
                    break;
                case Keys.D2:
                    DisplayMode = 1;
                    break;
                case Keys.D3:
                    DisplayMode = 2;
                    break;
                case Keys.D4:
                    DisplayMode = 3;
                    break;
                case Keys.D5:
                    DisplayMode = 4;
                    break;
                case Keys.D6:
                    DisplayMode = 5;
                    break;
                case Keys.D7:
                    DisplayMode = 6;
                    break;
                case Keys.D8:
                    DisplayMode = 7;
                    break;
                case Keys.D9:
                    DisplayMode = 8;
                    break;
                case Keys.D0:
                    DisplayMode = 9;
                    break;
            }
        }
    }

    public enum SelectedCellType
    {
        None,
        Plant,
        Animal
    }
}
