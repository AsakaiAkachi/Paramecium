using Paramecium.Simulation;
using Paramecium.Libs;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Windows.Forms.Design.AxImporter;
using System;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.ComponentModel.Design;
using System.Drawing;
using System.Net.Cache;

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
        int tpsOrigin;
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

                        if (SelectedCellIndex != -1 && SelectedCellId != -1)
                        {
                            if (g_Soup.Particles[SelectedCellIndex] is null)
                            {
                                SelectedCellIndex = -1;
                                SelectedCellId = -1;
                            }
                            else if (g_Soup.Particles[SelectedCellIndex].Id != SelectedCellId || !g_Soup.Particles[SelectedCellIndex].IsAlive)
                            {
                                SelectedCellIndex = -1;
                                SelectedCellId = -1;
                            }
                        }

                        if (Tracking)
                        {
                            if (SelectedCellIndex != -1 && SelectedCellId != -1)
                            {
                                if (g_Soup.Particles[SelectedCellIndex] is not null && g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                                {
                                    cameraX += (g_Soup.Particles[SelectedCellIndex].Position.X - cameraX) * 0.1d;
                                    cameraY += (g_Soup.Particles[SelectedCellIndex].Position.Y - cameraY) * 0.1d;
                                }
                            }
                        }

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
                                    try
                                    {
                                        if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalParticleCount > 0)
                                        {
                                            List<int> LocalParticles = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalParticles);
                                            for (int i = 0; i < LocalParticles.Count; i++)
                                            {
                                                Particle Target = g_Soup.Particles[LocalParticles[i]];

                                                if (Target is not null)
                                                {
                                                    SolidBrush TargetColor = SolidBrushPlaceholder;
                                                    double TargetPosX = Target.Position.X;
                                                    double TargetPosY = Target.Position.Y;
                                                    double TargetAngle = Target.Angle;
                                                    double TargetRadius = Target.Radius;

                                                    switch (DisplayMode)
                                                    {
                                                        case 0:
                                                            if (Target.Type == ParticleType.Animal && Target.Age < 0)
                                                            {
                                                                TargetColor = new SolidBrush(Color.FromArgb(255, 127, 255));
                                                            }
                                                            else TargetColor = new SolidBrush(Target.Color);
                                                            break;
                                                        case 1:
                                                            if (Target.Type == ParticleType.Plant)
                                                            {
                                                                TargetColor = BrushCellColorGray;
                                                            }
                                                            else if (Target.Type == ParticleType.Animal)
                                                            {
                                                                TargetColor = new SolidBrush(Color.FromArgb((int)(((Target.Genes.GeneDiet + 1d) / 2d) * 255d), 255 - (int)(((Target.Genes.GeneDiet + 1d) / 2d) * 255d), 0));
                                                            }
                                                            break;
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

                                                    if (Target.Type == ParticleType.Animal && zoomFactor >= 6)
                                                    {
                                                        Random rnd3 = new Random(g_Soup.ElapsedTimeStep + x + y * g_Soup.SizeX + Target.Index);
                                                        if (Target.Age >= 0)
                                                        {
                                                            double TargetSpeed = Vector2D.Size(Target.Velocity) * 9d + 0.1d;
                                                            for (int j = 0; j < 3; j++)
                                                            {
                                                                DrawLine(
                                                                    canvas_g,
                                                                    Pens.White,
                                                                    TargetPosX - (Vector2D.FromAngle(TargetAngle) * TargetRadius).X,
                                                                    TargetPosY - (Vector2D.FromAngle(TargetAngle) * TargetRadius).Y,
                                                                    TargetPosX - (Vector2D.FromAngle(TargetAngle) * TargetRadius).X - (Vector2D.FromAngle(TargetAngle + (rnd3.NextDouble() * (60d * TargetSpeed) - (30d * TargetSpeed))) * TargetRadius * 1.5d).X,
                                                                    TargetPosY - (Vector2D.FromAngle(TargetAngle) * TargetRadius).Y - (Vector2D.FromAngle(TargetAngle + (rnd3.NextDouble() * (60d * TargetSpeed) - (30d * TargetSpeed))) * TargetRadius * 1.5d).Y
                                                                );
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
                                                    }

                                                    /**
                                                    if (Target.Type == ParticleType.Animal)
                                                    {
                                                        DrawLine(
                                                            canvas_g,
                                                            Pens.White,
                                                            Target.Position.X,
                                                            Target.Position.Y,
                                                            Target.Position.X + (Vector2D.FromAngle(Target.Angle) * 2d).X,
                                                            Target.Position.Y + (Vector2D.FromAngle(Target.Angle) * 2d).Y
                                                        );

                                                        for (int j = -6; j <= 6; j++)
                                                        {
                                                            DrawLine(
                                                                canvas_g,
                                                                Pens.Yellow,
                                                                Target.Position.X,
                                                                Target.Position.Y,
                                                                Target.Position.X + (Vector2D.FromAngle(Target.Angle + (10 * j)) * 8d).X,
                                                                Target.Position.Y + (Vector2D.FromAngle(Target.Angle + (10 * j)) * 8d).Y
                                                            );
                                                        }
                                                    }
                                                    **/

                                                    if (TargetColor != BrushCellColorGray) TargetColor.Dispose();

                                                    if (Clicked)
                                                    {
                                                        if (Vector2D.Size(Target.Position - new Vector2D(ClickedPosX, ClickedPosY)) < Target.Radius)
                                                        {
                                                            SelectedCellIndex = Target.Index;
                                                            SelectedCellId = Target.Id;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ConsoleLog(LogLevel.Warning, ex.ToString());
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

                            if (SelectedCellIndex != -1 && SelectedCellId != -1)
                            {
                                if (g_Soup.Particles[SelectedCellIndex] is not null)
                                {
                                    if (g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                                    {
                                        for (int x = x1; x < x2; x++)
                                        {
                                            for (int y = y1; y < y2; y++)
                                            {
                                                try
                                                {
                                                    if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalParticleCount > 0)
                                                    {
                                                        List<int> LocalParticles = new List<int>(g_Soup.GridMap[x + y * g_Soup.SizeX].LocalParticles);
                                                        for (int i = 0; i < LocalParticles.Count; i++)
                                                        {
                                                            Particle Target = g_Soup.Particles[LocalParticles[i]];

                                                            if (Target is not null)
                                                            {
                                                                if (Target.Index == SelectedCellIndex && Target.Id == SelectedCellId)
                                                                {
                                                                    DrawCircle(
                                                                        canvas_g,
                                                                        Pens.Yellow,
                                                                        Target.Position.X,
                                                                        Target.Position.Y,
                                                                        Target.Radius + 0.5d
                                                                    );
                                                                    if (Target.IsGrabbed)
                                                                    {
                                                                        FillCircle(
                                                                            canvas_g,
                                                                            Brushes.Yellow,
                                                                            Target.Position.X,
                                                                            Target.Position.Y,
                                                                            Target.Radius * 0.5d
                                                                        );
                                                                    }
                                                                    if (Target.TargetIndex != -1)
                                                                    {
                                                                        DrawLine(
                                                                            canvas_g,
                                                                            Pens.Yellow,
                                                                            Target.Position.X,
                                                                            Target.Position.Y,
                                                                            g_Soup.Particles[Target.TargetIndex].Position.X,
                                                                            g_Soup.Particles[Target.TargetIndex].Position.Y
                                                                        );
                                                                        FillCircle(
                                                                            canvas_g,
                                                                            Brushes.Yellow,
                                                                            g_Soup.Particles[Target.TargetIndex].Position.X,
                                                                            g_Soup.Particles[Target.TargetIndex].Position.Y,
                                                                            g_Soup.Particles[Target.TargetIndex].Radius * 0.5d
                                                                        );
                                                                    }
                                                                }
                                                                else if (SelectedCellIndex != -1 && SelectedCellId != -1)
                                                                {
                                                                    if (g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId && Target.Type == ParticleType.Animal)
                                                                    {
                                                                        if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                                                        {
                                                                            if (g_Soup.Particles[SelectedCellIndex].Genes.RaceId == Target.Genes.RaceId)
                                                                            {
                                                                                DrawCircle(
                                                                                    canvas_g,
                                                                                    Pens.LightYellow,
                                                                                    Target.Position.X,
                                                                                    Target.Position.Y,
                                                                                    Target.Radius + 0.5d
                                                                                );
                                                                            }
                                                                            else if (Math.Sqrt(Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorRed - Target.Genes.GeneColorRed, 2) + Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorGreen - Target.Genes.GeneColorGreen, 2) + Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorBlue - Target.Genes.GeneColorBlue, 2)) <= 16d)
                                                                            {
                                                                                DrawCircle(
                                                                                    canvas_g,
                                                                                    Pens.LightGreen,
                                                                                    Target.Position.X,
                                                                                    Target.Position.Y,
                                                                                    Target.Radius + 0.5d
                                                                                );
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch { }
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

                        Font fnt = new Font("MS UI Gothic", 12);

                        if (SelectedCellIndex != -1 && SelectedCellId != -1)
                        {
                            try
                            {
                                if (g_Soup.Particles[SelectedCellIndex] is not null)
                                {
                                    if (g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                                    {
                                        int OffsetY = 0;
                                        Particle Target = g_Soup.Particles[SelectedCellIndex];

                                        {
                                            string Text = $"Cell #{Base36[(int)(Target.Id / 131621703842267136 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 3656158440062976 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 101559956668416 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 2821109907456 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 78364164096 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 2176782336 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 60466176 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 1679616 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 46656 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 1296 % 36)]}" +
                                                $"{Base36[(int)(Target.Id / 36 % 36)]}" +
                                                $"{Base36[(int)(Target.Id % 36)]}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        {
                                            string Text = $"タイプ : ";
                                            if (Target.Type == ParticleType.Plant) Text += "植物";
                                            else if (Target.Type == ParticleType.Animal && Target.Age >= 0) Text += "動物";
                                            else if (Target.Type == ParticleType.Animal && Target.Age < 0) Text += "胞子";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"種族 : #{Base36[(int)(Target.Genes.RaceId / 60466176 % 36)]}" +
                                                $"{Base36[(int)(Target.Genes.RaceId / 1679616 % 36)]}" +
                                                $"{Base36[(int)(Target.Genes.RaceId / 46656 % 36)]}" +
                                                $"{Base36[(int)(Target.Genes.RaceId / 1296 % 36)]}" +
                                                $"{Base36[(int)(Target.Genes.RaceId / 36 % 36)]}" +
                                                $"{Base36[(int)(Target.Genes.RaceId % 36)]}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"世代 : {Target.Generation}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal && Target.Age >= 0)
                                        {
                                            string Text = $"年齢 : {Target.Age}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal && Target.Age < 0)
                                        {
                                            string Text = $"ふ化まであと : {Target.Age * -1}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (1d - Target.Age * -1 / (double)g_Soup.HatchingTime)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        /**
                                        if (Target.Type == ParticleType.Animal && Target.Age > g_Soup.AgingBeginsAge)
                                        {
                                            string Text = $"老化係数 : {Target.AgingFactor:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * ((Target.AgingFactor - 1d) / 4d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        **/
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"子孫数 : {Target.OffspringCount}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        {
                                            string Text = $"位置 : ({Target.Position.X:0.000}, {Target.Position.Y:0.000})";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        {
                                            string Text = $"速度 : ({Target.Velocity.X:0.000}, {Target.Velocity.Y:0.000}) / {Vector2D.Size(Target.Velocity):0.000}u/t";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Vector2D.Size(Target.Velocity) / 0.1d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            SolidBrush brushColor = new SolidBrush(Color.FromArgb(255, Target.Genes.GeneColorRed, Target.Genes.GeneColorGreen, Target.Genes.GeneColorBlue));
                                            string Text = $"色 : ({Target.Genes.GeneColorRed}, {Target.Genes.GeneColorGreen}, {Target.Genes.GeneColorBlue})";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(brushColor, 0, OffsetY, 256, TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                            brushColor.Dispose();
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"食性 : {Target.Genes.GeneDiet:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayDietCarnivorous, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayDietHerbivore, 0, OffsetY, (int)(256 * (1d - (Target.Genes.GeneDiet + 1d) / 2d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Plant)
                                        {
                                            string Text = $"バイオマス : {Target.Biomass:0.000} / {g_Soup.PlantForkBiomass}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Biomass / g_Soup.PlantForkBiomass)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"バイオマス : {Target.Biomass:0.000} / {Target.Genes.ForkCost:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Biomass / (Target.Genes.ForkCost))), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"バイオマス(備蓄済み) : {Target.BiomassStockpiled:0.000} / {Target.Genes.ForkCost:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.BiomassStockpiled / (Target.Genes.ForkCost))), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"体力 : {g_Soup.Particles[SelectedCellIndex].Health:0.000} / {Target.Genes.GeneHealth * 10d:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Health / (Target.Genes.GeneHealth * 10d))), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"攻撃力 : {Target.Genes.GeneStrength:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Genes.GeneStrength / 4d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (Target.Type == ParticleType.Animal)
                                        {
                                            string Text = $"防御力 : {Target.Genes.GeneHardness:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Genes.GeneHardness / 4d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                        if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                        {
                                            string Text = $"機敏性 : {Target.Genes.GeneAgility:0.000}";
                                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                            canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                            canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Target.Genes.GeneAgility / 4d)), TextSize.Height);
                                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                            OffsetY += TextSize.Height;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            int OffsetY = 0;

                            Int2D MouseGridPos = Vector2D.ToGridPosition(new Vector2D(MousePosX, MousePosY));

                            Grid Target = g_Soup.GridMap[MouseGridPos.X + MouseGridPos.Y * g_Soup.SizeX];

                            {
                                string Text = $"Grid #{MouseGridPos.X + MouseGridPos.Y * g_Soup.SizeX}";
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
                                string Text = $"位置 : ({MouseGridPos.X}, {MouseGridPos.Y})";
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

                        if (fullScreen)
                        {
                            int OffsetY = 0;

                            {
                                string Text = $"Population : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationTotal}";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                            {
                                string Text = $"Time Step : {g_Soup.ElapsedTimeStep} (T{g_Soup.ParallelismLimit})";
                                Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, 256, TextSize.Height);
                                TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                OffsetY += TextSize.Height;
                            }
                        }

                        fnt.Dispose();
                        brush.Dispose();


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

        private void TopMenu_File_Open_Click(object sender, EventArgs e)
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

                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        jsonString = sr.ReadToEnd();
                        g_Soup = JsonSerializer.Deserialize<Soup>(jsonString);
                        g_Soup.SoupLoaded();
                        g_Soup.SoupRun();
                    }

                    FilePath = ofd.FileName;

                    if (g_Soup is not null)
                    {
                        g_FormMain.zoomFactor = 0;
                        g_FormMain.zoomFactorActual = 1;
                        g_FormMain.cameraX = g_Soup.SizeX / 2d;
                        g_FormMain.cameraY = g_Soup.SizeY / 2d;
                    }
                }
                catch
                {
                    if (prevSoup is not null) g_Soup = prevSoup;
                }
            }
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            if (g_Soup is not null)
            {
                g_Soup.SetSoupState(SoupState.Pause);

                string jsonString = JsonSerializer.Serialize(g_Soup);

                using (StreamWriter sw = new StreamWriter(FilePath, false))
                {
                    sw.Write(jsonString);
                }
            }
        }

        private void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Soup.soup";
            sfd.InitialDirectory = Path.GetDirectoryName($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\");
            sfd.Filter = "Paramecium スープファイル(*.soup)|*.soup|すべてのファイル(*.*)|*.*";
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Pause);

                string jsonString = JsonSerializer.Serialize(g_Soup);

                using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                {
                    sw.Write(jsonString);
                }

                FilePath = sfd.FileName;
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

            if (g_Soup is not null)
            {
                if (SelectedCellIndex != -1)
                {
                    if (g_Soup.Particles[SelectedCellIndex] is not null && g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                    {
                        if (g_Soup.Particles[SelectedCellIndex].IsGrabbed) g_Soup.Particles[SelectedCellIndex].IsGrabbed = false;
                    }
                }
            }
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
                                        if (SelectedCellIndex != -1)
                                        {
                                            if (g_Soup.Particles[SelectedCellIndex] is not null && g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                                            {
                                                Particle Target = g_Soup.Particles[SelectedCellIndex];
                                                Target.IsGrabbed = true;
                                                Target.Position = new Vector2D(CanvasPosXToWorldPosX(e.X), CanvasPosYToWorldPosY(e.Y));
                                                if (g_Soup.SoupState == SoupState.Pause)
                                                {
                                                    g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalParticles.Remove(Target.Index);
                                                    g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalParticleCount--;
                                                    if (Target.Type == ParticleType.Plant) g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalPlantCount--;
                                                    else g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalAnimalCount--;
                                                    Target.GridPosition = Vector2D.ToGridPosition(Target.Position);
                                                    g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalParticles.Add(Target.Index);
                                                    g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalParticleCount++;
                                                    if (Target.Type == ParticleType.Plant) g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalPlantCount++;
                                                    else g_Soup.GridMap[Target.GridPosition.X + Target.GridPosition.Y * g_Soup.SizeX].LocalAnimalCount++;
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
                                        Int2D MouseGridPosition = Vector2D.ToGridPosition(new Vector2D(MousePosX, MousePosY));
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
                                        Int2D MouseGridPosition = Vector2D.ToGridPosition(new Vector2D(MousePosX, MousePosY));
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
                        if (g_Soup.SoupState == SoupState.Pause) g_Soup.SetSoupState(SoupState.Running);
                        else g_Soup.SetSoupState(SoupState.Pause);
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
            }
        }
    }
}
