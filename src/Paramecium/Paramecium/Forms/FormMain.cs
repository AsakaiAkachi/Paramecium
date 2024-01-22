﻿using Paramecium.Simulation;
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

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private string FilePath;

        private int zoomFactor;
        private double zoomFactorActual;
        private double cameraX;
        private double cameraY;

        private int SelectedCellIndex = -1;
        private int SelectedCellId = -1;
        private bool Clicked = false;
        private double ClickedPosX = 0d;
        private double ClickedPosY = 0d;
        private double MousePosX = 0d;
        private double MousePosY = 0d;
        private bool Tracking = false;
        private int DisplayMode = 0;

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
            cameraX = Global.g_Soup.SizeX / 2d;
            cameraY = Global.g_Soup.SizeY / 2d;

            canvas = new Bitmap(1, 1);

            BottomStatLastRefreshTime = DateTime.Now;
            tpsOrigin = 0;
            tpsOriginTime = DateTime.Now;
            frameCount = 0;
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            Global.g_Soup.SoupRun();

            FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\New Soup.soup";

            Random rnd2 = new Random();

            string RaceIdChars = "0123456789abcdefghijklmnopqrstuvwxyz";

            while (true)
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

                    Bitmap bgCanvas = new Bitmap(Global.g_Soup.SizeX, Global.g_Soup.SizeY, PixelFormat.Format8bppIndexed);
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
                        new Rectangle(0, 0, Global.g_Soup.SizeX, Global.g_Soup.SizeY),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed
                    );

                    if (zoomFactor >= 4)
                    {
                        Marshal.Copy(Global.g_Soup.GridMapBg, 0, bmpdata.Scan0, Global.g_Soup.GridMapBg.Length);
                    }
                    else
                    {
                        Marshal.Copy(Global.g_Soup.GridMapBgParticle, 0, bmpdata.Scan0, Global.g_Soup.GridMapBgParticle.Length);
                    }
                    bgCanvas.UnlockBits(bmpdata);

                    Bitmap bgCanvas2 = new Bitmap(Global.g_Soup.SizeX + 1, Global.g_Soup.SizeY + 1);

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

                    int x1 = (int)Math.Floor(CanvasPosXToWorldPosX(0d));
                    int y1 = (int)Math.Floor(CanvasPosYToWorldPosY(0d));
                    int x2 = (int)Math.Ceiling(CanvasPosXToWorldPosX(canvas.Width));
                    int y2 = (int)Math.Ceiling(CanvasPosYToWorldPosY(canvas.Height));

                    if (zoomFactor >= 4)
                    {
                        /**
                        float x1 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(0d));
                        float x2 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(canvas.Width));
                        float y1 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(0d));
                        float y2 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(canvas.Height));
                        canvas_g.DrawRectangle(Pens.Red, x1, y1, x2 - 1, y2 - 1);
                        **/

                        SolidBrush brush2 = new SolidBrush(Color.FromArgb(127, 127, 127));
                        SolidBrush brush3 = new SolidBrush(Color.FromArgb(127, 127, 127));
                        SolidBrush brush4 = new SolidBrush(Color.FromArgb(0, 255, 0));


                        for (int x = x1; x < x2; x++)
                        {
                            for (int y = y1; y < y2; y++)
                            {
                                if (x >= 0 && x < Global.g_Soup.SizeX && y >= 0 && y < Global.g_Soup.SizeY)
                                {
                                    try
                                    {
                                        if (Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].LocalParticles.Count > 0)
                                        {
                                            for (int i = 0; i < Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].LocalParticles.Count; i++)
                                            {
                                                Particle Target = Global.g_Soup.Particles[Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].LocalParticles[i]];

                                                if (Target is not null)
                                                {
                                                    SolidBrush b_plant = new SolidBrush(Target.Color);

                                                    switch(DisplayMode)
                                                    {
                                                        case 0:
                                                            break;
                                                        case 1:
                                                            b_plant.Dispose();
                                                            if (Target.Type == ParticleType.Plant)
                                                            {
                                                                b_plant = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                            }
                                                            else if (Target.Type == ParticleType.Animal)
                                                            {
                                                                b_plant = new SolidBrush(Color.FromArgb((int)(((Target.Genes.GeneDiet + 1d) / 2d) * 255d), 255 - (int)(((Target.Genes.GeneDiet + 1d) / 2d) * 255d), 0));
                                                            }
                                                            break;
                                                    }


                                                    FillCircle(
                                                        canvas_g,
                                                        b_plant,
                                                        Target.Position.X,
                                                        Target.Position.Y,
                                                        Target.Radius
                                                    );
                                                    DrawCircle(
                                                        canvas_g,
                                                        Pens.White,
                                                        Target.Position.X,
                                                        Target.Position.Y,
                                                        Target.Radius
                                                    );

                                                    if (Target.Type == ParticleType.Animal)
                                                    {
                                                        Random rnd3 = new Random(Global.g_Soup.ElapsedTimeStep + x + y * Global.g_Soup.SizeX + Target.Index);
                                                        if (Target.Age >= 0)
                                                        {
                                                            double TargetSpeed = Vector2D.Size(Target.Velocity) * 9d + 0.1d;
                                                            for (int j = 0; j < 3; j++)
                                                            {
                                                                DrawLine(
                                                                    canvas_g,
                                                                    Pens.White,
                                                                    Target.Position.X - (Vector2D.FromAngle(Target.Angle) * Target.Radius).X,
                                                                    Target.Position.Y - (Vector2D.FromAngle(Target.Angle) * Target.Radius).Y,
                                                                    Target.Position.X - (Vector2D.FromAngle(Target.Angle) * Target.Radius).X - (Vector2D.FromAngle(Target.Angle + (rnd3.NextDouble() * (60d * TargetSpeed) - (30d * TargetSpeed))) * Target.Radius * 1.5d).X,
                                                                    Target.Position.Y - (Vector2D.FromAngle(Target.Angle) * Target.Radius).Y - (Vector2D.FromAngle(Target.Angle + (rnd3.NextDouble() * (60d * TargetSpeed) - (30d * TargetSpeed))) * Target.Radius * 1.5d).Y
                                                                );
                                                                SolidBrush bBlack = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                                FillCircle(
                                                                    canvas_g,
                                                                    bBlack,
                                                                    Target.Position.X + (Vector2D.FromAngle(Target.Angle - 30) * 0.7d * Target.Radius).X,
                                                                    Target.Position.Y + (Vector2D.FromAngle(Target.Angle - 30) * 0.7d * Target.Radius).Y,
                                                                    0.1d * Target.Radius
                                                                );
                                                                FillCircle(
                                                                    canvas_g,
                                                                    bBlack,
                                                                    Target.Position.X + (Vector2D.FromAngle(Target.Angle + 30) * 0.7d * Target.Radius).X,
                                                                    Target.Position.Y + (Vector2D.FromAngle(Target.Angle + 30) * 0.7d * Target.Radius).Y,
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

                                                    b_plant.Dispose();

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
                                    catch { }
                                }
                            }
                        }

                        for (int x = x1; x < x2; x++)
                        {
                            for (int y = y1; y < y2; y++)
                            {
                                if (x >= 0 && x < Global.g_Soup.SizeX && y >= 0 && y < Global.g_Soup.SizeY)
                                {
                                    if (Global.g_Soup.GridMapByte[x + y * Global.g_Soup.SizeX] == 0x01)
                                    {
                                        double WallPosX = x + 0.5d;
                                        double WallPosY = y + 0.5d;
                                        FillCircle(canvas_g, brush2, WallPosX - 0.25, WallPosY - 0.25, 0.353553);
                                        FillCircle(canvas_g, brush2, WallPosX + 0.25, WallPosY - 0.25, 0.353553);
                                        FillCircle(canvas_g, brush2, WallPosX - 0.25, WallPosY + 0.25, 0.353553);
                                        FillCircle(canvas_g, brush2, WallPosX + 0.25, WallPosY + 0.25, 0.353553);
                                    }
                                }
                            }
                        }

                        if (SelectedCellIndex != -1 && SelectedCellId != -1)
                        {
                            if (g_Soup.Particles[SelectedCellIndex] is not null && g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                            {
                                for (int x = x1; x < x2; x++)
                                {
                                    for (int y = y1; y < y2; y++)
                                    {
                                        if (x >= 0 && x < Global.g_Soup.SizeX && y >= 0 && y < Global.g_Soup.SizeY)
                                        {
                                            try
                                            {
                                                for (int i = 0; i < Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].LocalParticles.Count; i++)
                                                {
                                                    Particle Target = Global.g_Soup.Particles[Global.g_Soup.GridMap[x + y * Global.g_Soup.SizeX].LocalParticles[i]];

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
                                                                DrawCircle(
                                                                    canvas_g,
                                                                    Pens.Yellow,
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
                                                                    if (Math.Sqrt(Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorRed - Target.Genes.GeneColorRed, 2) + Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorGreen - Target.Genes.GeneColorGreen, 2) + Math.Pow(g_Soup.Particles[SelectedCellIndex].Genes.GeneColorBlue - Target.Genes.GeneColorBlue, 2)) <= 16d)
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
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }

                        brush2.Dispose();
                        brush3.Dispose();
                        brush4.Dispose();
                    }

                    SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));

                    FillRectangle(canvas_g, brush, -16, -16, 16, Global.g_Soup.SizeY + 32);
                    FillRectangle(canvas_g, brush, -16, -16, Global.g_Soup.SizeX + 32, 16);
                    FillRectangle(canvas_g, brush, -16, Global.g_Soup.SizeY, Global.g_Soup.SizeX + 32, 16);
                    FillRectangle(canvas_g, brush, Global.g_Soup.SizeX, -16, 16, Global.g_Soup.SizeY + 32);
                    DrawRectangle(canvas_g, Pens.Red, 0, 0, Global.g_Soup.SizeX, Global.g_Soup.SizeY);

                    Font fnt = new Font("MS UI Gothic", 12);
                    SolidBrush brushBack = new SolidBrush(Color.FromArgb(127, 127, 127, 127));
                    SolidBrush brushBar = new SolidBrush(Color.FromArgb(255, 191, 191, 191));
                    SolidBrush brushRed = new SolidBrush(Color.FromArgb(223, 0, 0));
                    SolidBrush brushGreen = new SolidBrush(Color.FromArgb(0, 223, 0));

                    if (SelectedCellIndex != -1 && SelectedCellId != -1)
                    {
                        try
                        {
                            if (g_Soup.Particles[SelectedCellIndex] is not null && g_Soup.Particles[SelectedCellIndex].Id == SelectedCellId)
                            {
                                int OffsetY = 0;

                                {
                                    string Text = $"Cell #{g_Soup.Particles[SelectedCellIndex].Id}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"種族 : {RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId / 60466176) % 36]}{RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId / 1679616) % 36]}{RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId / 46656) % 36]}{RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId / 1296) % 36]}{RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId / 36) % 36]}{RaceIdChars[(int)(g_Soup.Particles[SelectedCellIndex].Genes.RaceId % 36)]}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"世代 : {g_Soup.Particles[SelectedCellIndex].Generation}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"年齢 : {g_Soup.Particles[SelectedCellIndex].Age}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"位置 : ({g_Soup.Particles[SelectedCellIndex].Position.X:0.000}, {g_Soup.Particles[SelectedCellIndex].Position.Y:0.000})";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"速度 : ({g_Soup.Particles[SelectedCellIndex].Velocity.X:0.000}, {g_Soup.Particles[SelectedCellIndex].Velocity.Y:0.000}) / {Vector2D.Size(g_Soup.Particles[SelectedCellIndex].Velocity):0.000}u/t";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (Vector2D.Size(g_Soup.Particles[SelectedCellIndex].Velocity) / 0.1d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    SolidBrush brushColor = new SolidBrush(Color.FromArgb(255, g_Soup.Particles[SelectedCellIndex].Genes.GeneColorRed, g_Soup.Particles[SelectedCellIndex].Genes.GeneColorGreen, g_Soup.Particles[SelectedCellIndex].Genes.GeneColorBlue));
                                    string Text = $"色 : ({g_Soup.Particles[SelectedCellIndex].Genes.GeneColorRed}, {g_Soup.Particles[SelectedCellIndex].Genes.GeneColorGreen}, {g_Soup.Particles[SelectedCellIndex].Genes.GeneColorBlue})";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushColor, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"食性 : {g_Soup.Particles[SelectedCellIndex].Genes.GeneDiet:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushRed, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushGreen, 0, OffsetY, (int)(256 * (1d - (g_Soup.Particles[SelectedCellIndex].Genes.GeneDiet + 1d) / 2d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Plant)
                                {
                                    string Text = $"バイオマス : {g_Soup.Particles[SelectedCellIndex].Satiety:0.000} / {g_Soup.PlantForkBiomass}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Satiety / g_Soup.PlantForkBiomass)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"バイオマス : {g_Soup.Particles[SelectedCellIndex].Satiety:0.000} / {g_Soup.Particles[SelectedCellIndex].Genes.ForkCost * 2:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Satiety / (g_Soup.Particles[SelectedCellIndex].Genes.ForkCost * 2d))), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"体力 : {g_Soup.Particles[SelectedCellIndex].Health:0.000} / {g_Soup.Particles[SelectedCellIndex].Genes.GeneHealth * 10d:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Health / (g_Soup.Particles[SelectedCellIndex].Genes.GeneHealth * 10d))), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"攻撃力 : {g_Soup.Particles[SelectedCellIndex].Genes.GeneStrength:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Genes.GeneStrength / 4d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"防御力 : {g_Soup.Particles[SelectedCellIndex].Genes.GeneHardness:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Genes.GeneHardness / 4d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.Particles[SelectedCellIndex].Type == ParticleType.Animal)
                                {
                                    string Text = $"機敏性 : {g_Soup.Particles[SelectedCellIndex].Genes.GeneAgility:0.000}";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                                    canvas_g.FillRectangle(brushBar, 0, OffsetY, (int)(256 * (g_Soup.Particles[SelectedCellIndex].Genes.GeneAgility / 4d)), TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        int OffsetY = 0;

                        Int2D MouseGridPos = Vector2D.ToGridPosition(new Vector2D(MousePosX, MousePosY));

                        {
                            string Text = $"Grid #{MouseGridPos.X + MouseGridPos.Y * g_Soup.SizeX}";
                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                            canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                            OffsetY += TextSize.Height;
                        }
                        {
                            string Text = $"位置 : ({MouseGridPos.X}, {MouseGridPos.Y})";
                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                            canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                            OffsetY += TextSize.Height;
                        }
                        {
                            string Text = $"バイオマス : {g_Soup.GridMap[MouseGridPos.X + MouseGridPos.Y * g_Soup.SizeX].Fertility:0.000}";
                            Size TextSize = TextRenderer.MeasureText(Text, fnt);
                            canvas_g.FillRectangle(brushBack, 0, OffsetY, 256, TextSize.Height);
                            TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                            OffsetY += TextSize.Height;
                        }
                    }

                    fnt.Dispose();
                    brush.Dispose();
                    brushBack.Dispose();
                    brushBar.Dispose();
                    brushRed.Dispose();
                    brushGreen.Dispose();


                    canvas_g.Dispose();

                    SimulationView.Image = canvas;
                    canvasOld.Dispose();

                    bgCanvas.Dispose();
                    bgCanvas2.Dispose();

                    Clicked = false;

                    frameCount++;
                }

                if ((DateTime.Now - BottomStatLastRefreshTime).TotalMilliseconds >= 50)
                {
                    switch (Global.g_Soup.SoupState)
                    {
                        case SoupState.Stop:
                            StatSoupStatus.Text = "Status : Stop";
                            break;
                        case SoupState.Pause:
                            StatSoupStatus.Text = "Status : Pause";
                            break;
                        case SoupState.Running:
                            StatSoupStatus.Text = "Status : Running";
                            break;
                        case SoupState.StepRun:
                            StatSoupStatus.Text = "Status : Step Run";
                            break;
                    }

                    StatTimeStep.Text = $"Time Step : {Global.g_Soup.ElapsedTimeStep} (T{Global.g_Soup.sim_ParallelLimit})";
                    StatPopulation.Text = $"Population : {Global.g_Soup.PopulationPlant}/{Global.g_Soup.PopulationAnimal}/{Global.g_Soup.PopulationTotal}";
                    //StatBiomassAmount.Text = $"BiomassAmount : {Global.g_Soup.BiomassAmount:0.000}";
                    BottomStat.Refresh();
                }
                if ((DateTime.Now - tpsOriginTime).TotalMilliseconds >= 1000)
                {
                    StatTps.Text = $"TPS : {Global.g_Soup.ElapsedTimeStep - tpsOrigin}";
                    tpsOrigin = Global.g_Soup.ElapsedTimeStep;
                    tpsOriginTime = DateTime.Now;
                    StatFps.Text = $"FPS : {frameCount}";
                    frameCount = 0;
                    StatMemory.Text = $"Memory : {System.Environment.WorkingSet / 1048576d:F1} Mbytes";
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
            ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            ofd.Filter = "Paramecium スープファイル(*.soup)|*.soup|すべてのファイル(*.*)|*.*";
            ofd.Title = "開く";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Global.g_Soup.SetSoupState(SoupState.Stop);

                string jsonString;

                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    jsonString = sr.ReadToEnd();
                    Global.g_Soup = JsonSerializer.Deserialize<Soup>(jsonString);
                    Global.g_Soup.SoupRun();
                }

                FilePath = ofd.FileName;
            }
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            Global.g_Soup.SetSoupState(SoupState.Pause);

            string jsonString = JsonSerializer.Serialize(Global.g_Soup);

            using (StreamWriter sw = new StreamWriter(FilePath, false))
            {
                sw.Write(jsonString);
            }
        }

        private void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Soup.soup";
            sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            sfd.Filter = "Paramecium スープファイル(*.soup)|*.soup|すべてのファイル(*.*)|*.*";
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Global.g_Soup.SetSoupState(SoupState.Pause);

                string jsonString = JsonSerializer.Serialize(Global.g_Soup);

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
                FormEventViewer.Close();
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
            Global.g_Soup.SetSoupState(SoupState.Pause);

            FormNewSimulation FormNewSimulation = new FormNewSimulation();
            FormNewSimulation.ShowDialog();
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
                            if (zoomFactor < 8) zoomFactor++;
                            zoomFactorActual = Math.Pow(2, zoomFactor);
                            break;
                        case Keys.Alt:
                            break;
                        default:
                            Clicked = true;
                            SelectedCellIndex = -1;
                            SelectedCellId = -1;
                            ClickedPosX = CanvasPosXToWorldPosX(e.X);
                            ClickedPosY = CanvasPosYToWorldPosY(e.Y);
                            Console.WriteLine($"{ClickedPosX}, {ClickedPosY}");
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (Control.ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            if (zoomFactor > 0) zoomFactor--;
                            zoomFactorActual = Math.Pow(2, zoomFactor);
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
            MousePosX = CanvasPosXToWorldPosX(e.X);
            MousePosY = CanvasPosYToWorldPosY(e.Y);
        }
        private void SimulationView_MouseWheel(object? sender, MouseEventArgs e)
        {
            zoomFactor = Math.Min(Math.Max(zoomFactor + e.Delta / 100, 0), 8);
            zoomFactorActual = Math.Pow(2, zoomFactor);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (Global.g_Soup.SoupState == SoupState.Pause) Global.g_Soup.SetSoupState(SoupState.Running);
                    else Global.g_Soup.SetSoupState(SoupState.Pause);
                    break;
                case Keys.OemPeriod:
                    Global.g_Soup.sim_ParallelLimit++;
                    break;
                case Keys.Oemcomma:
                    if (Global.g_Soup.sim_ParallelLimit > 1)
                    {
                        Global.g_Soup.sim_ParallelLimit--;
                    }
                    break;
                case Keys.OemQuestion:
                    Global.g_Soup.SetSoupState(SoupState.StepRun);
                    break;
                case Keys.R:
                    if (!Tracking) Tracking = true;
                    else Tracking = false;
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