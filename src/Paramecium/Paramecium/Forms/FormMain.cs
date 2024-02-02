using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.Json;

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
        private bool hideOverlay = false;
        private bool autoSelect = false;
        private DateTime lastAutoSelectTime = DateTime.Now;

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
                                        SelectedCellIndex = -1;
                                        SelectedCellId = -1;
                                    }
                                }
                                else
                                {
                                    SelectedCellType = SelectedCellType.None;
                                    SelectedCellIndex = -1;
                                    SelectedCellId = -1;
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
                                            if (autoSelect) lastAutoSelectTime = DateTime.Now;
                                        }
                                    }
                                    else
                                    {
                                        SelectedCellType = SelectedCellType.None;
                                        SelectedCellIndex = -1;
                                        SelectedCellId = -1;
                                    }
                                }
                                else
                                {
                                    SelectedCellType = SelectedCellType.None;
                                    SelectedCellIndex = -1;
                                    SelectedCellId = -1;
                                }
                            }
                        }
                        else if (autoSelect && (DateTime.Now - lastAutoSelectTime).TotalMilliseconds >= 3000)
                        {
                            if (g_Soup.PopulationAnimal > 0)
                            {
                                while (true)
                                {
                                    int selectedCellIndex = new Random().Next(0, g_Soup.Animals.Length);
                                    Animal target = g_Soup.Animals[selectedCellIndex];

                                    if (target is not null)
                                    {
                                        if (target.IsValid)
                                        {
                                            SelectedCellType = SelectedCellType.Animal;
                                            SelectedCellIndex = selectedCellIndex;
                                            SelectedCellId = target.Id;
                                            lastAutoSelectTime = DateTime.Now;

                                            break;
                                        }
                                    }

                                    if (g_Soup.PopulationAnimal == 0) break;
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
                                        try
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

                                                    if (DisplayMode == 1)
                                                    {
                                                        TargetColor.Dispose();
                                                        TargetColor = new SolidBrush(Color.FromArgb(127, 127, 127));
                                                    }
                                                    if (DisplayMode == 2)
                                                    {
                                                        TargetColor.Dispose();
                                                        TargetColor = new SolidBrush(Color.FromArgb(127, 127, 127));
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
                                        catch (Exception ex) { ConsoleLog(LogLevel.Warning, ex.ToString()); }
                                    }
                                    if (g_Soup.GridMap[x + y * g_Soup.SizeX].LocalAnimalCount > 0)
                                    {
                                        try
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

                                                    if (DisplayMode == 1)
                                                    {
                                                        TargetColor.Dispose();

                                                        if (SelectedCellType == SelectedCellType.Animal)
                                                        {
                                                            Animal SecondTarget = g_Soup.Animals[SelectedCellIndex];

                                                            if (SecondTarget is not null)
                                                            {
                                                                int DifferentGeneCount = 0;
                                                                for (int j = 0; j < Target.Brain.Length; j++)
                                                                {
                                                                    if (Target.Brain[j] != SecondTarget.Brain[j]) DifferentGeneCount++;
                                                                }

                                                                double GeneIdentity = (Target.Brain.Length - DifferentGeneCount) / (double)Target.Brain.Length;

                                                                if (GeneIdentity > 0.75)
                                                                {
                                                                    double color = (GeneIdentity - 0.75d) * 4d;
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, (int)((255 * color) + (192 * (1d - color)) / 2d), (int)((0 * color) + (64 * (1d - color)) / 2d)));
                                                                }
                                                                else if (GeneIdentity > 0.5)
                                                                {
                                                                    double color = (GeneIdentity - 0.5d) * 4d;
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, (int)((192 * color) + (0 * (1d - color)) / 2d), (int)((64 * color) + (255 * (1d - color)) / 2d)));
                                                                }
                                                                else if (GeneIdentity > 0.25)
                                                                {
                                                                    double color = (GeneIdentity - 0.25d) * 4d;
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, 0, (int)((255 * color) + (64 * (1d - color)) / 2d)));
                                                                }
                                                                else
                                                                {
                                                                    double color = GeneIdentity * 4d;
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, 0, (int)((64 * color) + (0 * (1d - color)) / 2d)));
                                                                }
                                                            }
                                                        }
                                                        else TargetColor = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                    }
                                                    if (DisplayMode == 2)
                                                    {
                                                        TargetColor.Dispose();
                                                        if (SelectedCellType == SelectedCellType.Animal)
                                                        {
                                                            Animal SecondTarget = g_Soup.Animals[SelectedCellIndex];

                                                            if (SecondTarget is not null)
                                                            {
                                                                string Text = String.Empty;

                                                                if (Target.Id == SecondTarget.Id)
                                                                {
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, 255, 0));
                                                                }
                                                                else if (SecondTarget.ParentGenealogicalTree.Contains(Target.Id))
                                                                {
                                                                    TargetColor = new SolidBrush(Color.FromArgb(255, 255, 0));
                                                                }
                                                                else if (Target.ParentGenealogicalTree.Contains(SecondTarget.Id))
                                                                {
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, 255, 255));
                                                                }
                                                                else if (SecondTarget.ParentGenealogicalTree.FindAll(Target.ParentGenealogicalTree.Contains).Count != 0)
                                                                {
                                                                    int BranchGeneration = SecondTarget.ParentGenealogicalTree.IndexOf(Target.ParentGenealogicalTree.FindAll(SecondTarget.ParentGenealogicalTree.Contains)[0]) + 1;

                                                                    if (BranchGeneration < 20)
                                                                    {
                                                                        TargetColor = new SolidBrush(Color.FromArgb(255, 127, 0));
                                                                    }
                                                                    else
                                                                    {
                                                                        TargetColor = new SolidBrush(Color.FromArgb(63, 31, 0));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    TargetColor = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                                }
                                                            }
                                                            else TargetColor = new SolidBrush(Color.FromArgb(0, 0, 0));
                                                        }
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
                                                            if (zoomFactor >= 6)
                                                            {
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
                                        catch (Exception ex) { ConsoleLog(LogLevel.Warning, ex.ToString()); }
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

                            if (SelectedCellType != SelectedCellType.None && hideOverlay == false)
                            {
                                Font fnt_CellOverlay = new Font("MS UI Gothic", 8);

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
                                                            else if (g_Soup.Animals[SelectedCellIndex] is not null)
                                                            {
                                                                if (Target.RaceId == g_Soup.Animals[SelectedCellIndex].RaceId)
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

                                                            if (zoomFactor >= 6)
                                                            {
                                                                if (DisplayMode == 1)
                                                                {
                                                                    Animal SecondTarget = g_Soup.Animals[SelectedCellIndex];

                                                                    if (SecondTarget is not null)
                                                                    {
                                                                        double GeneDifferences = 0;

                                                                        for (int j = 0; j < Target.Brain.Length; j++)
                                                                        {
                                                                            if (Target.Brain[j] != SecondTarget.Brain[j])
                                                                            {
                                                                                GeneDifferences += 1.0d / Target.Brain.Length;
                                                                            }
                                                                        }

                                                                        TextRenderer.DrawText(canvas_g, $"Gene Identity : {(1d - GeneDifferences) * 100d:0.000}%", fnt_CellOverlay, new Point((int)WorldPosXToCanvasPosX(Target.Position.X + Target.Radius) + 4, (int)WorldPosYToCanvasPosY(Target.Position.Y - Target.Radius)), Color.White);
                                                                    }
                                                                }
                                                                else if (DisplayMode == 2)
                                                                {
                                                                    Animal SecondTarget = g_Soup.Animals[SelectedCellIndex];

                                                                    if (SecondTarget is not null)
                                                                    {
                                                                        string Text = $"#{LongToBase36(Target.Id, 12)}\r\nRelationships : ";

                                                                        if (Target.Id == SecondTarget.Id)
                                                                        {
                                                                            Text += "Selected";
                                                                        }
                                                                        else if (SecondTarget.ParentGenealogicalTree.Contains(Target.Id))
                                                                        {
                                                                            Text += $"Ancestors {SecondTarget.ParentGenealogicalTree.IndexOf(Target.Id) + 1} Generations Ago";
                                                                        }
                                                                        else if (Target.ParentGenealogicalTree.Contains(SecondTarget.Id))
                                                                        {
                                                                            Text += $"Descendants After {Target.ParentGenealogicalTree.IndexOf(SecondTarget.Id) + 1} Generations";
                                                                        }

                                                                        else if (Target.ParentGenealogicalTree.FindAll(SecondTarget.ParentGenealogicalTree.Contains).Count != 0)
                                                                        {
                                                                            int BranchGeneration = SecondTarget.ParentGenealogicalTree.IndexOf(Target.ParentGenealogicalTree.FindAll(SecondTarget.ParentGenealogicalTree.Contains)[0]) + 1;

                                                                            Text += $"Branched Out {BranchGeneration} Generations Ago";
                                                                        }
                                                                        else
                                                                        {
                                                                            Text += "No Relationship";
                                                                        }

                                                                        /*
                                                                        Text += "\r\nAncestors :";
                                                                        for (int j = 0; j < Target.ParentGenealogicalTree.Count; j++)
                                                                        {
                                                                            Text += $" \r\n#{LongToBase36(Target.ParentGenealogicalTree[j], 12)}";
                                                                        }
                                                                        */

                                                                        TextRenderer.DrawText(canvas_g, Text, fnt_CellOverlay, new Point((int)WorldPosXToCanvasPosX(Target.Position.X + Target.Radius) + 4, (int)WorldPosYToCanvasPosY(Target.Position.Y - Target.Radius)), Color.White);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ConsoleLog(LogLevel.Warning, ex.ToString());
                                                            EventLog.PushEventLog($"Exception Thrown :\r\n{ex}");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                fnt_CellOverlay.Dispose();
                            }
                        }

                        SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));

                        FillRectangle(canvas_g, brush, -16, -16, 16, g_Soup.SizeY + 32);
                        FillRectangle(canvas_g, brush, -16, -16, g_Soup.SizeX + 32, 16);
                        FillRectangle(canvas_g, brush, -16, g_Soup.SizeY, g_Soup.SizeX + 32, 16);
                        FillRectangle(canvas_g, brush, g_Soup.SizeX, -16, 16, g_Soup.SizeY + 32);
                        DrawRectangle(canvas_g, Pens.Red, 0, 0, g_Soup.SizeX, g_Soup.SizeY);

                        brush.Dispose();

                        if (!hideOverlay)
                        {
                            SolidBrush b_OverlayBackground = new SolidBrush(Color.FromArgb(127, 127, 127, 127));
                            Font fnt_small = new Font("MS UI Gothic", 8);
                            Font fnt = new Font("MS UI Gothic", 12);

                            {
                                int OffsetY = 0;

                                if (fullScreen)
                                {
                                    {
                                        string Text = $"Total Born/Die : {g_Soup.TotalBornCount}/{g_Soup.TotalDieCount}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Latest Generation : {g_Soup.LatestGeneration}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Population (P/A/T) : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationTotal}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Time Step : {g_Soup.ElapsedTimeStep} (T{g_Soup.ParallelismLimit})";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Status : {g_Soup.SoupState}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                }
                                {
                                    string Text = $"Display Mode : ";
                                    switch (DisplayMode)
                                    {
                                        case 1:
                                            Text += "Gene Identity";
                                            break;
                                        case 2:
                                            Text += "Genealogy";
                                            break;
                                        default:
                                            Text += "Normal";
                                            break;
                                    }
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (Tracking)
                                {
                                    string Text = "[Auto Tracking Enabled]";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (autoSelect)
                                {
                                    string Text = "[Auto Selection Enabled]";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                if (g_Soup.AutoSave)
                                {
                                    string Text = "[Auto Save Enabled]";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, canvas.Height - TextSize.Height - OffsetY, TextSize.Width, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, canvas.Height - TextSize.Height - OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }

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
                                    string Text = $"Type : ";
                                    if (Target.Type == TileType.None) Text += "Normal";
                                    else if (Target.Type == TileType.Wall) Text += "Wall";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"Position : ({MouseIntegerizedPosition.X}, {MouseIntegerizedPosition.Y})";
                                    Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                    canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                    TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                    OffsetY += TextSize.Height;
                                }
                                {
                                    string Text = $"Element : {Target.Fertility:0.000}";
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
                                        string Text = $"Type : Plant";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Element : {target.Element:0.000} / {g_Soup.PlantForkBiomass:0.000}";
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
                                        string Text = $"Type : ";
                                        if (target.Age >= 0) Text += "Animal";
                                        else Text += "Spore";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Race : #{LongToBase36(target.RaceId, 6)}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Generation : {target.Generation}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    if (target.Age >= 0)
                                    {
                                        string Text = $"Age : {target.Age}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    if (target.Age < 0)
                                    {
                                        string Text = $"Age : {target.Age * -1} More Steps to Hatching";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (1d - target.Age * -1 / (double)g_Soup.HatchingTime)), TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Number of Offspring : {target.OffspringCount}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Position : ({target.Position.X:0.000}, {target.Position.Y:0.000})";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        string Text = $"Velocity : ({target.Velocity.X:0.000}, {target.Velocity.Y:0.000}) / {Vector2D.Size(target.Velocity):0.000}u/t";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (Vector2D.Size(target.Velocity) / 0.1d)), TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }
                                    {
                                        SolidBrush brushColor = new SolidBrush(Color.FromArgb(255, target.GeneColorRed, target.GeneColorGreen, target.GeneColorBlue));
                                        string Text = $"Color : ({target.GeneColorRed}, {target.GeneColorGreen}, {target.GeneColorBlue})";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(brushColor, 0, OffsetY, 256, TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                        brushColor.Dispose();
                                    }
                                    {
                                        string Text = $"Element : {target.Element:0.000} / {g_Soup.AnimalForkBiomass * 2d:0.000}";
                                        Size TextSize = TextRenderer.MeasureText(Text, fnt);
                                        canvas_g.FillRectangle(BrushOverlayBackground, 0, OffsetY, 256, TextSize.Height);
                                        canvas_g.FillRectangle(BrushOverlayGauge, 0, OffsetY, (int)(256 * (target.Element / (g_Soup.AnimalForkBiomass * 2d))), TextSize.Height);
                                        TextRenderer.DrawText(canvas_g, Text, fnt, new Point(0, OffsetY), Color.White);
                                        OffsetY += TextSize.Height;
                                    }

                                    canvas_g.FillRectangle(b_OverlayBackground, 0 + 256, 0, 245, 200);
                                    TextRenderer.DrawText(canvas_g, $"Brain Input/Output", fnt, new Point(0 + 256, 0), Color.White);

                                    SolidBrush b_BrainInputViewRed = new SolidBrush(Color.FromArgb(255, 0, 0));
                                    SolidBrush b_BrainInputViewGreen = new SolidBrush(Color.FromArgb(0, 255, 0));
                                    SolidBrush b_BrainInputViewBlue = new SolidBrush(Color.FromArgb(0, 0, 255));

                                    for (int i = -14; i <= 14; i++)
                                    {
                                        for (int j = 9; j >= 0; j--)
                                        {
                                            float PieX = j * 10 * -1 + 356;
                                            float PieY = j * 10 * -1 + 110;
                                            float PieSize = (j + 1) * 10 * 2;

                                            if (target.BrainInput[(i + 14) * 10 + j] == 0)
                                            {
                                                canvas_g.FillPie(Brushes.Black, PieX, PieY, PieSize, PieSize, i * 6.4285714286f - 3.2142857143f - 90f, 6.4285714286f);
                                            }
                                            if (target.BrainInput[(i + 14) * 10 + j] == 1)
                                            {
                                                canvas_g.FillPie(Brushes.Gray, PieX, PieY, PieSize, PieSize, i * 6.4285714286f - 3.2142857143f - 90f, 6.4285714286f);
                                            }
                                            if (target.BrainInput[(i + 14) * 10 + j] == 2)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewGreen, PieX, PieY, PieSize, PieSize, i * 6.4285714286f - 3.2142857143f - 90f, 6.4285714286f);
                                            }
                                            if (target.BrainInput[(i + 14) * 10 + j] == 3)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewBlue, PieX, PieY, PieSize, PieSize, i * 6.4285714286f - 3.2142857143f - 90f, 6.4285714286f);
                                            }
                                            if (target.BrainInput[(i + 14) * 10 + j] == 4)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewRed, PieX, PieY, PieSize, PieSize, i * 6.4285714286f - 3.2142857143f - 90f, 6.4285714286f);
                                            }
                                        }
                                    }
                                    for (int i = -4; i <= 4; i++)
                                    {
                                        for (int j = 3; j >= 0; j--)
                                        {
                                            float PieX = j * 10 * -1 + 356;
                                            float PieY = j * 10 * -1 + 110;
                                            float PieSize = (j + 1) * 10 * 2;

                                            if (target.BrainInput[290 + (i + 4) * 4 + j] == 0)
                                            {
                                                canvas_g.FillPie(Brushes.Black, PieX, PieY, PieSize, PieSize, i * 22.5f - 11.25f - 270f, 22.5f);
                                            }
                                            if (target.BrainInput[290 + (i + 4) * 4 + j] == 1)
                                            {
                                                canvas_g.FillPie(Brushes.Gray, PieX, PieY, PieSize, PieSize, i * 22.5f - 11.25f - 270f, 22.5f);
                                            }
                                            if (target.BrainInput[290 + (i + 4) * 4 + j] == 2)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewGreen, PieX, PieY, PieSize, PieSize, i * 22.5f - 11.25f - 270f, 22.5f);
                                            }
                                            if (target.BrainInput[290 + (i + 4) * 4 + j] == 3)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewBlue, PieX, PieY, PieSize, PieSize, i * 22.5f - 11.25f - 270f, 22.5f);
                                            }
                                            if (target.BrainInput[290 + (i + 4) * 4 + j] == 4)
                                            {
                                                canvas_g.FillPie(b_BrainInputViewRed, PieX, PieY, PieSize, PieSize, i * 22.5f - 11.25f - 270f, 22.5f);
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

                                    canvas_g.FillEllipse(BrainOutputAcceleration, 155 + 256, 135, 15, 15);
                                    canvas_g.DrawEllipse(Pens.White, 155 + 256, 135, 15, 15);
                                    TextRenderer.DrawText(canvas_g, $"Acceleration", fnt_small, new Point(175 + 256, 135), Color.White);
                                    TextRenderer.DrawText(canvas_g, $"{target.BrainOutputAcceleration:0.000}", fnt_small, new Point(175 + 256, 145), Color.White);

                                    canvas_g.FillEllipse(BrainOutputRotation, 155 + 256, 155, 15, 15);
                                    canvas_g.DrawEllipse(Pens.White, 155 + 256, 155, 15, 15);
                                    TextRenderer.DrawText(canvas_g, $"Rotation", fnt_small, new Point(175 + 256, 155), Color.White);
                                    TextRenderer.DrawText(canvas_g, $"{target.BrainOutputRotation:0.000}", fnt_small, new Point(175 + 256, 165), Color.White);

                                    canvas_g.FillEllipse(BrainOutputAttack, 155 + 256, 175, 15, 15);
                                    canvas_g.DrawEllipse(Pens.White, 155 + 256, 175, 15, 15);
                                    TextRenderer.DrawText(canvas_g, $"Attack", fnt_small, new Point(175 + 256, 175), Color.White);
                                    TextRenderer.DrawText(canvas_g, $"{target.BrainOutputAttack:0.000}", fnt_small, new Point(175 + 256, 185), Color.White);

                                    BrainOutputAcceleration.Dispose();
                                    BrainOutputRotation.Dispose();
                                    BrainOutputAttack.Dispose();
                                }
                            }

                            b_OverlayBackground.Dispose();
                            fnt_small.Dispose();
                            fnt.Dispose();
                        }

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
                        StatPopulation.Text = $"Population (P/A/T) : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationTotal}";
                        StatLatestGeneration.Text = $"Latest Generation : {g_Soup.LatestGeneration}";
                        StatTotalBornDie.Text = $"Total Born/Die : {g_Soup.TotalBornCount}/{g_Soup.TotalDieCount}";

                        ParameciumNotifyIcon.Text = $"{Path.GetFileName(FilePath)} - {g_Soup.ElapsedTimeStep} Step / Gen {g_Soup.LatestGeneration}";
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

                if (g_Soup is not null) this.Text = $"{Path.GetFileName(FilePath)} - Paramecium {version}";
                else this.Text = $"Paramecium {version}";

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
            ofd.Filter = "Paramecium Soup File(*.soup)|*.soup|All Files(*.*)|*.*";
            ofd.Title = "Open";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Soup? prevSoup = g_Soup;
                if (g_Soup is not null) g_Soup.SetSoupState(SoupState.Stop);

                try
                {
                    string jsonString;

                    EventLog.PushEventLog($"Loading soup...");
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

                    EventLog.PushEventLog($"Soup file \"{Path.GetFileName(FilePath)}\" has been loaded.");
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
                SoupState prevSoupState = g_Soup.SoupState;

                g_Soup.SetSoupState(SoupState.Pause);

                EventLog.PushEventLog($"Saving soup...");
                await Task.Delay(100);

                string jsonString = JsonSerializer.Serialize(g_Soup);

                StreamWriter sw = new StreamWriter(FilePath, false);

                sw.Write(jsonString);

                sw.Dispose();

                jsonString = String.Empty;

                GC.Collect();

                EventLog.PushEventLog($"Soup has been saved.");

                g_Soup.SetSoupState(prevSoupState);
            }
        }

        private async void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Untitled.soup";
            sfd.InitialDirectory = Path.GetDirectoryName($@"{Path.GetDirectoryName(Application.ExecutablePath)}\saves\");
            sfd.Filter = "Paramecium Soup File(*.soup)|*.soup|All Files(*.*)|*.*";
            sfd.Title = "Save As";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (g_Soup is not null)
                {
                    SoupState prevSoupState = g_Soup.SoupState;

                    g_Soup.SetSoupState(SoupState.Pause);

                    EventLog.PushEventLog($"Saving soup...");
                    await Task.Delay(100);

                    string jsonString = JsonSerializer.Serialize(g_Soup);

                    StreamWriter sw = new StreamWriter(sfd.FileName, false);

                    sw.Write(jsonString);

                    sw.Dispose();

                    jsonString = String.Empty;

                    FilePath = sfd.FileName;

                    GC.Collect();

                    EventLog.PushEventLog($"Soup has been saved as \"{Path.GetFileName(FilePath)}\"");

                    g_Soup.SetSoupState(prevSoupState);
                }
            }
        }

        private void TopMenu_File_Exit_Click(object sender, EventArgs e)
        {
            Close();
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

        private async void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (g_Soup is not null)
                    {
                        if (g_Soup.SoupState == SoupState.Pause)
                        {
                            g_Soup.SetSoupState(SoupState.Running);
                            EventLog.PushEventLog("Simulation has been resumed");
                        }
                        else
                        {
                            g_Soup.SetSoupState(SoupState.Pause);
                            EventLog.PushEventLog("Simulation has been paused");
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
                        if (!((Control.ModifierKeys) == Keys.Shift)) TopMost = true;
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
                case Keys.S:
                    if ((Control.ModifierKeys) == Keys.Control)
                    {
                        if (g_Soup is not null)
                        {
                            SoupState prevSoupState = g_Soup.SoupState;

                            g_Soup.SetSoupState(SoupState.Pause);

                            EventLog.PushEventLog($"Saving soup...");
                            await Task.Delay(100);

                            string jsonString = JsonSerializer.Serialize(g_Soup);

                            StreamWriter sw = new StreamWriter(FilePath, false);

                            sw.Write(jsonString);

                            sw.Dispose();

                            jsonString = String.Empty;

                            GC.Collect();

                            EventLog.PushEventLog($"Soup has been saved.");

                            g_Soup.SetSoupState(prevSoupState);
                        }
                    }
                    break;
                case Keys.A:
                    if ((Control.ModifierKeys) == Keys.Shift)
                    {
                        if (!g_Soup.AutoSave) g_Soup.AutoSave = true;
                        else g_Soup.AutoSave = false;
                    }
                    break;
                case Keys.F1:
                    if (!hideOverlay) hideOverlay = true;
                    else hideOverlay = false;
                    break;
                case Keys.F2:
                    if (!autoSelect) autoSelect = true;
                    else autoSelect = false;
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

        private void ParameciumNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Activate();
        }
    }

    public enum SelectedCellType
    {
        None,
        Plant,
        Animal
    }
}
