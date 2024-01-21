using Paramecium.Simulation;
using Paramecium.Libs;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Windows.Forms.Design.AxImporter;
using System;
using System.Security.Cryptography;

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
            cameraX = Global.SoupInstance.env_SizeX / 2d;
            cameraY = Global.SoupInstance.env_SizeY / 2d;

            canvas = new Bitmap(1, 1);

            BottomStatLastRefreshTime = DateTime.Now;
            tpsOrigin = 0;
            tpsOriginTime = DateTime.Now;
            frameCount = 0;
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            Global.SoupInstance.SoupRun();

            FilePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\New Soup.soup";

            Random rnd2 = new Random();
            while (true)
            {
                if (SimulationView.Width > 0 && SimulationView.Height > 0)
                {
                    Bitmap canvasOld = canvas;
                    canvas = new Bitmap(SimulationView.Width, SimulationView.Height);

                    Bitmap bgCanvas = new Bitmap(Global.SoupInstance.env_SizeX, Global.SoupInstance.env_SizeY, PixelFormat.Format8bppIndexed);
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
                        new Rectangle(0, 0, Global.SoupInstance.env_SizeX, Global.SoupInstance.env_SizeY),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed
                    );

                    if (zoomFactor >= 4)
                    {
                        Marshal.Copy(Global.SoupInstance.GridMapBg, 0, bmpdata.Scan0, Global.SoupInstance.GridMapBg.Length);
                    }
                    else
                    {
                        Marshal.Copy(Global.SoupInstance.GridMapBgParticle, 0, bmpdata.Scan0, Global.SoupInstance.GridMapBgParticle.Length);
                    }
                    bgCanvas.UnlockBits(bmpdata);

                    Bitmap bgCanvas2 = new Bitmap(Global.SoupInstance.env_SizeX + 1, Global.SoupInstance.env_SizeY + 1);

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

                    if (zoomFactor >= 4)
                    {
                        /**
                        float x1 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(0d));
                        float x2 = WorldPosXToCanvasPosX(CanvasPosXToWorldPosX(canvas.Width));
                        float y1 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(0d));
                        float y2 = WorldPosYToCanvasPosY(CanvasPosYToWorldPosY(canvas.Height));
                        canvas_g.DrawRectangle(Pens.Red, x1, y1, x2 - 1, y2 - 1);
                        **/

                        int x1 = (int)Math.Floor(CanvasPosXToWorldPosX(0d));
                        int y1 = (int)Math.Floor(CanvasPosYToWorldPosY(0d));
                        int x2 = (int)Math.Ceiling(CanvasPosXToWorldPosX(canvas.Width));
                        int y2 = (int)Math.Ceiling(CanvasPosYToWorldPosY(canvas.Height));

                        SolidBrush brush2 = new SolidBrush(Color.FromArgb(127, 127, 127));
                        SolidBrush brush3 = new SolidBrush(Color.FromArgb(127, 127, 127));
                        SolidBrush brush4 = new SolidBrush(Color.FromArgb(0, 255, 0));


                        for (int x = x1; x < x2; x++)
                        {
                            for (int y = y1; y < y2; y++)
                            {
                                if (x >= 0 && x < Global.SoupInstance.env_SizeX && y >= 0 && y < Global.SoupInstance.env_SizeY)
                                {
                                    try
                                    {
                                        if (Global.SoupInstance.GridMap[x + y * Global.SoupInstance.env_SizeX].LocalParticles.Count > 0)
                                        {
                                            for (int i = 0; i < Global.SoupInstance.GridMap[x + y * Global.SoupInstance.env_SizeX].LocalParticles.Count; i++)
                                            {
                                                Particle Target = Global.SoupInstance.Particles[Global.SoupInstance.GridMap[x + y * Global.SoupInstance.env_SizeX].LocalParticles[i]];

                                                if (Target is not null)
                                                {
                                                    SolidBrush b_plant = new SolidBrush(Target.Color);
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
                                                        Random rnd3 = new Random(Global.SoupInstance.timeSteps + x + y * Global.SoupInstance.env_SizeX + Target.Index);
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
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex) { }
                                }
                            }
                        }
                        for (int x = x1; x < x2; x++)
                        {
                            for (int y = y1; y < y2; y++)
                            {
                                if (x >= 0 && x < Global.SoupInstance.env_SizeX && y >= 0 && y < Global.SoupInstance.env_SizeY)
                                {
                                    if (Global.SoupInstance.GridMapByte[x + y * Global.SoupInstance.env_SizeX] == 0x01)
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

                        brush2.Dispose();
                        brush3.Dispose();
                        brush4.Dispose();
                    }

                    SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));

                    FillRectangle(canvas_g, brush, -16, -16, 16, Global.SoupInstance.env_SizeY + 32);
                    FillRectangle(canvas_g, brush, -16, -16, Global.SoupInstance.env_SizeX + 32, 16);
                    FillRectangle(canvas_g, brush, -16, Global.SoupInstance.env_SizeY, Global.SoupInstance.env_SizeX + 32, 16);
                    FillRectangle(canvas_g, brush, Global.SoupInstance.env_SizeX, -16, 16, Global.SoupInstance.env_SizeY + 32);
                    DrawRectangle(canvas_g, Pens.Red, 0, 0, Global.SoupInstance.env_SizeX, Global.SoupInstance.env_SizeY);
                    canvas_g.Dispose();

                    SimulationView.Image = canvas;
                    canvasOld.Dispose();

                    bgCanvas.Dispose();
                    bgCanvas2.Dispose();

                    frameCount++;
                }

                if ((DateTime.Now - BottomStatLastRefreshTime).TotalMilliseconds >= 50)
                {
                    switch (Global.SoupInstance.SoupState)
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

                    StatTimeStep.Text = $"Time Step : {Global.SoupInstance.timeSteps} (T{Global.SoupInstance.sim_ParallelLimit})";
                    StatPopulation.Text = $"Population : {Global.SoupInstance.PopulationPlant}/{Global.SoupInstance.PopulationAnimal}/{Global.SoupInstance.PopulationTotal}";
                    StatBiomassAmount.Text = $"BiomassAmount : {Global.SoupInstance.BiomassAmount}";
                    BottomStat.Refresh();
                }
                if ((DateTime.Now - tpsOriginTime).TotalMilliseconds >= 1000)
                {
                    StatTps.Text = $"TPS : {Global.SoupInstance.timeSteps - tpsOrigin}";
                    tpsOrigin = Global.SoupInstance.timeSteps;
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
                Global.SoupInstance.SetSoupState(SoupState.Stop);

                string jsonString;

                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    jsonString = sr.ReadToEnd();
                    Global.SoupInstance = JsonSerializer.Deserialize<Soup>(jsonString);
                    Global.SoupInstance.SoupRun();
                }

                FilePath = ofd.FileName;
            }
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            Global.SoupInstance.SetSoupState(SoupState.Pause);

            string jsonString = JsonSerializer.Serialize(Global.SoupInstance);

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
                Global.SoupInstance.SetSoupState(SoupState.Pause);

                string jsonString = JsonSerializer.Serialize(Global.SoupInstance);

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
            Global.SoupInstance.SetSoupState(SoupState.Pause);

            FormNewSimulation FormNewSimulation = new FormNewSimulation();
            FormNewSimulation.ShowDialog();
        }

        bool drag = false;
        int prevX, prevY;
        private void SimulationView_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                switch (e.Button)
                {
                    case (MouseButtons.Left):
                        if (zoomFactor < 8) zoomFactor++;
                        break;
                    case (MouseButtons.Right):
                        if (zoomFactor > 0) zoomFactor--;
                        break;
                }
                zoomFactorActual = Math.Pow(2, zoomFactor);
            }
            else if (e.Button == MouseButtons.Right)
            {
                drag = true;
                prevX = e.X;
                prevY = e.Y;
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
                    if (Global.SoupInstance.SoupState == SoupState.Pause) Global.SoupInstance.SetSoupState(SoupState.Running);
                    else Global.SoupInstance.SetSoupState(SoupState.Pause);
                    break;
                case Keys.OemPeriod:
                    Global.SoupInstance.sim_ParallelLimit++;
                    break;
                case Keys.Oemcomma:
                    if (Global.SoupInstance.sim_ParallelLimit > 1)
                    {
                        Global.SoupInstance.sim_ParallelLimit--;
                    }
                    break;
                case Keys.OemQuestion:
                    Global.SoupInstance.SetSoupState(SoupState.StepRun);
                    break;
            }
        }
    }
}
