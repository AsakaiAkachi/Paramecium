using Paramecium.Forms.Renderer;
using Paramecium.Engine;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.IO;

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        FormInspector FormInspector = new FormInspector();

        Bitmap SoupViewCanvas;

        CameraState CameraState;
        public Double2d CameraPosition;
        public int ZoomLevel;
        long AutoTrackingTime;

        Point mousePointClient;

        double FramesPerSecond;

        bool ShowOverray = true;
        SoupViewOverlayRenderer.SelectedObjectType SelectedObjectType;
        int SelectedObjectIndex = -1;
        long SelectedObjectId = -1;

        bool IsFullScreen = false;
        FormWindowState PrevWindowState;

        public FormMain()
        {
            InitializeComponent();
            SoupView.MouseWheel += SoupView_MouseWheel;

            Text = $"{g_AppName} {g_AppVersion}";
            OpenFileDialog_LoadSoup.InitialDirectory = $"{g_SoupDefaultFilePath}";
            SaveFileDialog_SaveSoup.InitialDirectory = $"{g_SoupDefaultFilePath}";
            SaveFileDialog_SaveSoup.FileName = $"{g_SoupDefaultFileName}";

            SoupViewCanvas = new Bitmap(1, 1);

            CameraPosition = new Double2d(0, 0);
            ZoomLevel = 0;
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            long SoupElapsedTimeSteps = 0;

            while (true)
            {
                if (g_Soup is not null && g_Soup.Initialized)
                {
                    TopMenu_File_Save.Enabled = true;
                    TopMenu_File_SaveAs.Enabled = true;

                    TopMenu_Soup.Enabled = true;

                    if (!g_Soup.Modified) Text = $"{Path.GetFileName(g_Soup.FilePath)} - {g_AppName} {g_AppVersion}";
                    else Text = $"*{Path.GetFileName(g_Soup.FilePath)} - {g_AppName} {g_AppVersion}";
                }
                else
                {
                    TopMenu_File_Save.Enabled = false;
                    TopMenu_File_SaveAs.Enabled = false;

                    TopMenu_Soup.Enabled = false;

                    Text = $"{g_AppName} {g_AppVersion}";
                }

                if (g_Soup is not null && g_Soup.Initialized)
                {
                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None || SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                    {
                        if (CameraState == CameraState.Tracking) CameraState = CameraState.Default;
                    }
                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant)
                    {
                        if (!g_Soup.Plants[SelectedObjectIndex].Exist || SelectedObjectId != g_Soup.Plants[SelectedObjectIndex].Id)
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            if (CameraState == CameraState.Tracking) CameraState = CameraState.Default;
                        }
                        else if (CameraState == CameraState.Tracking || CameraState == CameraState.AutoTracking)
                        {
                            CameraPosition = g_Soup.Plants[SelectedObjectIndex].Position;
                        }
                    }
                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                    {
                        if (!g_Soup.Animals[SelectedObjectIndex].Exist || SelectedObjectId != g_Soup.Animals[SelectedObjectIndex].Id)
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            if (CameraState == CameraState.Tracking) CameraState = CameraState.Default;
                        }
                        else if (CameraState == CameraState.Tracking || CameraState == CameraState.AutoTracking)
                        {
                            CameraPosition = g_Soup.Animals[SelectedObjectIndex].Position;
                        }
                    }

                    try
                    {
                        Point mousePoint = Cursor.Position;
                        mousePointClient = SoupView.PointToClient(mousePoint);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None || SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                    {
                        Double2d mouseWorldPos = new Double2d(
                            WorldPosViewPosConversion.ViewPosToWorldPosX(SoupView.Width, CameraPosition, double.Pow(2, ZoomLevel), mousePointClient.X),
                            WorldPosViewPosConversion.ViewPosToWorldPosY(SoupView.Height, CameraPosition, double.Pow(2, ZoomLevel), mousePointClient.Y)
                        );

                        if (mouseWorldPos.X >= 0 && mouseWorldPos.X <= g_Soup.Settings.SizeX && mouseWorldPos.Y >= 0 && mouseWorldPos.Y <= g_Soup.Settings.SizeY)
                        {
                            int mouseWorldPosIntegerizedX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(mouseWorldPos.X)));
                            int mouseWorldPosIntegerizedY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(mouseWorldPos.Y)));

                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Tile;
                            SelectedObjectIndex = mouseWorldPosIntegerizedY * g_Soup.Settings.SizeX + mouseWorldPosIntegerizedX;
                            SelectedObjectId = -1;
                        }
                        else
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            SelectedObjectIndex = -1;
                            SelectedObjectId = -1;
                        }
                    }

                    if (CameraState == CameraState.Default)
                    {
                        AutoTrackingTime = 0;
                    }
                    else if (CameraState == CameraState.AutoTracking)
                    {
                        if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None || SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile || AutoTrackingTime >= 5000)
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Animal;
                            SelectedObjectIndex = new Random().Next(0, g_Soup.Animals.Count);
                            while (!g_Soup.Animals[SelectedObjectIndex].Exist) SelectedObjectIndex = new Random().Next(0, g_Soup.Animals.Count);
                            SelectedObjectId = g_Soup.Animals[SelectedObjectIndex].Id;

                            AutoTrackingTime = 0;
                        }
                        else AutoTrackingTime += g_Soup.ElapsedTimeSteps - SoupElapsedTimeSteps;
                    }

                    SoupElapsedTimeSteps = g_Soup.ElapsedTimeSteps;

                    if (SoupView.Width > 0 && SoupView.Height > 0)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        SoupViewCanvas.Dispose();

                        try
                        {
                            SoupViewCanvas = new Bitmap(SoupView.Width, SoupView.Height);
                            SoupViewRenderer.DrawSoupView(ref SoupViewCanvas, CameraPosition, ZoomLevel);
                            if (ShowOverray)
                            {
                                SoupViewOverlayRenderer.DrawSoupViewOverlay(ref SoupViewCanvas, CameraPosition, ZoomLevel, mousePointClient, SelectedObjectType, SelectedObjectIndex);
                                if (IsFullScreen) SoupViewFullScreenOverlayRenderer.DrawSoupViewFullScreenOverlay(ref SoupViewCanvas, FramesPerSecond);
                            }
                            SoupView.Image = SoupViewCanvas;
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }

                        sw.Stop();
                        FramesPerSecond *= 0.96d;
                        FramesPerSecond += 1000000d / sw.Elapsed.TotalMicroseconds * 0.04d;
                    }

                    BottomStat_SoupState.Text = $"Status : {g_Soup.SoupState}";
                    BottomStat_ElapsedTimeSteps.Text = $"Time Step : {g_Soup.ElapsedTimeSteps} (T{g_Soup.ThreadCount})";
                    BottomStat_Population.Text = $"Population (P/A/T) : {g_Soup.PopulationPlant}/{g_Soup.PopulationAnimal}/{g_Soup.PopulationPlant + g_Soup.PopulationAnimal}";
                    BottomStat_LatestGeneration.Text = $"Generation : {g_Soup.LatestGeneration}";
                    BottomStat_TotalBornDieCount.Text = $"Total Born/Die : {g_Soup.TotalBornCount}/{g_Soup.TotalDieCount}";
                    BottomStat_Tps.Text = $"TPS : {g_Soup.TimeStepsPerSecond.ToString("0.0")}";
                    BottomStat_Fps.Text = $"FPS : {FramesPerSecond.ToString("0.0")}";
                }

                TopMenu.Refresh();
                BottomStat.Refresh();

                await Task.Delay(1);
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (ModifierKeys == Keys.None)
                    {
                        if (g_Soup.SoupState == SoupState.Pause) g_Soup.SetSoupState(SoupState.Running);
                        else if (g_Soup.SoupState == SoupState.Running) g_Soup.SetSoupState(SoupState.Pause);
                    }
                    break;
                case Keys.OemQuestion:
                    if (ModifierKeys == Keys.None)
                    {
                        g_Soup.SetSoupState(SoupState.StepRun);
                    }
                    break;
                case Keys.OemPeriod:
                    if (ModifierKeys == Keys.None)
                    {
                        g_Soup.SetThreadCount(g_Soup.ThreadCount + 1);
                    }
                    break;
                case Keys.Oemcomma:
                    if (ModifierKeys == Keys.None)
                    {
                        g_Soup.SetThreadCount(g_Soup.ThreadCount - 1);
                    }
                    break;
                case Keys.T:
                    if (CameraState == CameraState.Default)
                    {
                        if (ModifierKeys == Keys.Control) CameraState = CameraState.AutoTracking;
                        else
                        {
                            if (SelectedObjectType != SoupViewOverlayRenderer.SelectedObjectType.None) CameraState = CameraState.Tracking;
                            else CameraState = CameraState.Default;
                        }
                    }
                    else CameraState = CameraState.Default;
                    break;
                case Keys.C:
                    if (ModifierKeys == Keys.None)
                    {
                        CameraPosition = new Double2d(g_Soup.Settings.SizeX / 2d, g_Soup.Settings.SizeY / 2d);
                        ZoomLevel = 0;
                    }
                    break;
                case Keys.W:
                    if (ModifierKeys == Keys.None)
                    {
                        CameraPosition = new Double2d((int)CameraPosition.X + 0.5d, (int)CameraPosition.Y + 0.5d - 1d);
                    }
                    break;
                case Keys.S:
                    if (ModifierKeys == Keys.None)
                    {
                        CameraPosition = new Double2d((int)CameraPosition.X + 0.5d, (int)CameraPosition.Y + 0.5d + 1d);
                    }
                    break;
                case Keys.A:
                    if (ModifierKeys == Keys.None)
                    {
                        CameraPosition = new Double2d((int)CameraPosition.X + 0.5d - 1d, (int)CameraPosition.Y + 0.5d);
                    }
                    break;
                case Keys.D:
                    if (ModifierKeys == Keys.Control)
                    {
                        if (FormInspector.IsDisposed)
                        {
                            FormInspector = new FormInspector();
                        }
                        FormInspector.Show();
                        FormInspector.Activate();

                        if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                        {
                            FormInspector.Inspect(0, SelectedObjectIndex);
                        }
                        else if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant)
                        {
                            FormInspector.Inspect(1, SelectedObjectIndex);
                        }
                        else if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                        {
                            FormInspector.Inspect(2, SelectedObjectIndex);
                        }
                    }
                    else if (ModifierKeys == Keys.None)
                    {
                        CameraPosition = new Double2d((int)CameraPosition.X + 0.5d + 1d, (int)CameraPosition.Y + 0.5d);
                    }
                    break;
                case Keys.Z:
                    if (ModifierKeys == Keys.None)
                    {
                        if (ZoomLevel < 8) ZoomLevel++;
                    }
                    break;
                case Keys.X:
                    if (ModifierKeys == Keys.None)
                    {
                        if (ZoomLevel > 0) ZoomLevel--;
                    }
                    break;
                case Keys.F1:
                    if (ModifierKeys == Keys.None)
                    {
                        if (!ShowOverray) ShowOverray = true;
                        else ShowOverray = false;
                    }
                    break;
                case Keys.F:
                    if (ModifierKeys == Keys.Control)
                    {
                        if (!IsFullScreen)
                        {
                            IsFullScreen = true;

                            TopMenu.Hide();
                            BottomStat.Hide();

                            SoupView.Location = new Point(0, 0);
                            SoupView.Size = ClientSize;

                            FormBorderStyle = FormBorderStyle.None;

                            PrevWindowState = WindowState;
                            WindowState = FormWindowState.Normal;
                            WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            IsFullScreen = false;

                            TopMenu.Show();
                            BottomStat.Show();

                            SoupView.Location = new Point(0, 24);
                            SoupView.Size = new Size(ClientSize.Width, ClientSize.Height - 46);

                            FormBorderStyle = FormBorderStyle.Sizable;

                            WindowState = FormWindowState.Normal;
                            WindowState = PrevWindowState;
                        }
                    }
                    break;
            }
        }

        bool Dragging;
        int MousePrevX, MousePrevY;

        bool Grabbing;

        private void SoupView_MouseDown(object sender, MouseEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (ModifierKeys)
                    {
                        case Keys.Shift:
                            if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant || SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                            {
                                Grabbing = true;

                                Double2d clickedWorldPos = new Double2d(
                                    WorldPosViewPosConversion.ViewPosToWorldPosX(SoupView.Width, CameraPosition, double.Pow(2, ZoomLevel), e.X),
                                    WorldPosViewPosConversion.ViewPosToWorldPosY(SoupView.Height, CameraPosition, double.Pow(2, ZoomLevel), e.Y)
                                );
                                int clickedWorldPosIntegerizedX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(clickedWorldPos.X)));
                                int clickedWorldPosIntegerizedY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(clickedWorldPos.Y)));

                                if (clickedWorldPos.X >= 0 && clickedWorldPos.X <= g_Soup.Settings.SizeX && clickedWorldPos.Y >= 0 && clickedWorldPos.Y <= g_Soup.Settings.SizeY)
                                {
                                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant)
                                    {
                                        g_Soup.SetSoupState(SoupState.Pause);

                                        Plant target = g_Soup.Plants[SelectedObjectIndex];

                                        target.Position = clickedWorldPos;

                                        g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalPlantIndexes.Remove(target.Index);
                                        target.IntegerizedPositionX = clickedWorldPosIntegerizedX;
                                        target.IntegerizedPositionY = clickedWorldPosIntegerizedY;
                                        g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalPlantIndexes.Add(target.Index);

                                        g_Soup.Modified = true;
                                    }
                                    else if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                                    {
                                        g_Soup.SetSoupState(SoupState.Pause);

                                        Animal target = g_Soup.Animals[SelectedObjectIndex];

                                        target.Position = clickedWorldPos;

                                        g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalAnimalIndexes.Remove(target.Index);
                                        target.IntegerizedPositionX = clickedWorldPosIntegerizedX;
                                        target.IntegerizedPositionY = clickedWorldPosIntegerizedY;
                                        g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalAnimalIndexes.Add(target.Index);

                                        g_Soup.Modified = true;
                                    }
                                }
                            }
                            break;
                        case Keys.Control:
                            if (ZoomLevel < 8) ZoomLevel++;
                            break;
                        case Keys.Alt:
                            if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                            {
                                g_Soup.SetSoupState(SoupState.Pause);
                                g_Soup.Tiles[SelectedObjectIndex].Type = TileType.Wall;
                            }
                            break;
                        default:
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            SelectedObjectIndex = -1;
                            SelectedObjectId = -1;
                            if (CameraState == CameraState.AutoTracking) CameraState = CameraState.Tracking;

                            try
                            {
                                Double2d clickedWorldPos = new Double2d(
                                    WorldPosViewPosConversion.ViewPosToWorldPosX(SoupView.Width, CameraPosition, double.Pow(2, ZoomLevel), e.X),
                                    WorldPosViewPosConversion.ViewPosToWorldPosY(SoupView.Height, CameraPosition, double.Pow(2, ZoomLevel), e.Y)
                                );

                                if (clickedWorldPos.X >= 0 && clickedWorldPos.X <= g_Soup.Settings.SizeX && clickedWorldPos.Y >= 0 && clickedWorldPos.Y <= g_Soup.Settings.SizeY)
                                {
                                    int clickedWorldPosIntegerizedX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(clickedWorldPos.X)));
                                    int clickedWorldPosIntegerizedY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(clickedWorldPos.Y)));

                                    if (ZoomLevel >= 4)
                                    {
                                        for (int x = int.Max(0, clickedWorldPosIntegerizedX - 1); x <= int.Min(g_Soup.Settings.SizeX - 1, clickedWorldPosIntegerizedX + 1); x++)
                                        {
                                            for (int y = int.Max(0, clickedWorldPosIntegerizedY - 1); y <= int.Min(g_Soup.Settings.SizeY - 1, clickedWorldPosIntegerizedY + 1); y++)
                                            {
                                                Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                                                if (targetTile.Type == TileType.Default)
                                                {
                                                    if (targetTile.LocalPlantPopulation > 0)
                                                    {
                                                        for (int i = targetTile.LocalPlantPopulation - 1; i >= 0; i--)
                                                        {
                                                            Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[i]];

                                                            if (Double2d.Distance(clickedWorldPos, targetPlant.Position) < targetPlant.Radius)
                                                            {
                                                                SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Plant;
                                                                SelectedObjectIndex = targetPlant.Index;
                                                                SelectedObjectId = targetPlant.Id;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        for (int x = int.Max(0, clickedWorldPosIntegerizedX - 1); x <= int.Min(g_Soup.Settings.SizeX - 1, clickedWorldPosIntegerizedX + 1); x++)
                                        {
                                            for (int y = int.Max(0, clickedWorldPosIntegerizedY - 1); y <= int.Min(g_Soup.Settings.SizeY - 1, clickedWorldPosIntegerizedY + 1); y++)
                                            {
                                                Tile targetTile = g_Soup.Tiles[y * g_Soup.Settings.SizeX + x];

                                                if (targetTile.Type == TileType.Default)
                                                {
                                                    if (targetTile.LocalAnimalPopulation > 0)
                                                    {
                                                        for (int i = targetTile.LocalAnimalPopulation - 1; i >= 0; i--)
                                                        {
                                                            Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[i]];

                                                            if (Double2d.Distance(clickedWorldPos, targetAnimal.Position) < targetAnimal.Radius)
                                                            {
                                                                SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Animal;
                                                                SelectedObjectIndex = targetAnimal.Index;
                                                                SelectedObjectId = targetAnimal.Id;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Tile targetTile = g_Soup.Tiles[clickedWorldPosIntegerizedY * g_Soup.Settings.SizeX + clickedWorldPosIntegerizedX];

                                        if (targetTile.Type == TileType.Default)
                                        {
                                            if (targetTile.LocalAnimalPopulation > 0)
                                            {
                                                Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[targetTile.LocalAnimalPopulation - 1]];

                                                SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Animal;
                                                SelectedObjectIndex = targetAnimal.Index;
                                                SelectedObjectId = targetAnimal.Id;
                                            }
                                            else if (targetTile.LocalPlantPopulation > 0)
                                            {
                                                Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[targetTile.LocalPlantPopulation - 1]];

                                                SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Plant;
                                                SelectedObjectIndex = targetPlant.Index;
                                                SelectedObjectId = targetPlant.Id;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }

                            if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None) CameraState = CameraState.Default;
                            break;
                    }
                    break;
                case MouseButtons.Middle:
                    switch (ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            break;
                        case Keys.Alt:
                            break;
                        default:
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            if (ZoomLevel > 0) ZoomLevel--;
                            break;
                        case Keys.Alt:
                            if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                            {
                                g_Soup.SetSoupState(SoupState.Pause);
                                g_Soup.Tiles[SelectedObjectIndex].Type = TileType.Default;
                            }
                            break;
                        default:
                            Dragging = true;
                            MousePrevX = e.X;
                            MousePrevY = e.Y;
                            CameraState = CameraState.Default;
                            break;
                    }
                    break;
            }
        }

        private void SoupView_MouseMove(object sender, MouseEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (Dragging)
            {
                CameraPosition += new Double2d(
                    (MousePrevX - e.X) / double.Pow(2, ZoomLevel),
                    (MousePrevY - e.Y) / double.Pow(2, ZoomLevel)
                );

                //if (CameraPosition.X < 0 || CameraPosition.X > g_Soup.Settings.SizeX || CameraPosition.Y < 0 || CameraPosition.Y > g_Soup.Settings.SizeY) CameraPosition = new Double2d(double.Max(0, double.Min(g_Soup.Settings.SizeX, CameraPosition.X)), double.Max(0, double.Min(g_Soup.Settings.SizeY, CameraPosition.Y)));

                MousePrevX = e.X;
                MousePrevY = e.Y;
            }
            else
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        switch (ModifierKeys)
                        {
                            case Keys.Shift:
                                if ((SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant || SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal) && Grabbing)
                                {
                                    Double2d clickedWorldPos = new Double2d(
                                        WorldPosViewPosConversion.ViewPosToWorldPosX(SoupView.Width, CameraPosition, double.Pow(2, ZoomLevel), e.X),
                                        WorldPosViewPosConversion.ViewPosToWorldPosY(SoupView.Height, CameraPosition, double.Pow(2, ZoomLevel), e.Y)
                                    );
                                    int clickedWorldPosIntegerizedX = int.Max(0, int.Min(g_Soup.Settings.SizeX - 1, (int)double.Floor(clickedWorldPos.X)));
                                    int clickedWorldPosIntegerizedY = int.Max(0, int.Min(g_Soup.Settings.SizeY - 1, (int)double.Floor(clickedWorldPos.Y)));

                                    if (clickedWorldPos.X >= 0 && clickedWorldPos.X <= g_Soup.Settings.SizeX && clickedWorldPos.Y >= 0 && clickedWorldPos.Y <= g_Soup.Settings.SizeY)
                                    {
                                        if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant)
                                        {
                                            g_Soup.SetSoupState(SoupState.Pause);

                                            Plant target = g_Soup.Plants[SelectedObjectIndex];

                                            target.Position = clickedWorldPos;

                                            g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalPlantIndexes.Remove(target.Index);
                                            target.IntegerizedPositionX = clickedWorldPosIntegerizedX;
                                            target.IntegerizedPositionY = clickedWorldPosIntegerizedY;
                                            g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalPlantIndexes.Add(target.Index);

                                            g_Soup.Modified = true;
                                        }
                                        else if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                                        {
                                            g_Soup.SetSoupState(SoupState.Pause);

                                            Animal target = g_Soup.Animals[SelectedObjectIndex];

                                            target.Position = clickedWorldPos;

                                            g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalAnimalIndexes.Remove(target.Index);
                                            target.IntegerizedPositionX = clickedWorldPosIntegerizedX;
                                            target.IntegerizedPositionY = clickedWorldPosIntegerizedY;
                                            g_Soup.Tiles[target.IntegerizedPositionY * g_Soup.Settings.SizeX + target.IntegerizedPositionX].LocalAnimalIndexes.Add(target.Index);

                                            g_Soup.Modified = true;
                                        }
                                    }
                                }
                                break;
                            case Keys.Control:
                                break;
                            case Keys.Alt:
                                if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                                {
                                    g_Soup.SetSoupState(SoupState.Pause);
                                    g_Soup.Tiles[SelectedObjectIndex].Type = TileType.Wall;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case MouseButtons.Middle:
                        switch (ModifierKeys)
                        {
                            case Keys.Shift:
                                break;
                            case Keys.Control:
                                break;
                            case Keys.Alt:
                                break;
                            default:
                                break;
                        }
                        break;
                    case MouseButtons.Right:
                        switch (ModifierKeys)
                        {
                            case Keys.Shift:
                                break;
                            case Keys.Control:
                            case Keys.Alt:
                                if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Tile)
                                {
                                    g_Soup.SetSoupState(SoupState.Pause);
                                    g_Soup.Tiles[SelectedObjectIndex].Type = TileType.Default;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        private void SoupView_MouseUp(object sender, MouseEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (Dragging) Dragging = false;
            if (Grabbing) Grabbing = false;
        }

        private void SoupView_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (ModifierKeys == Keys.None)
            {
                if (e.Delta > 0) if (ZoomLevel < 8) ZoomLevel++;
                if (e.Delta < 0) if (ZoomLevel > 0) ZoomLevel--;
            }
        }

        private void TopMenu_File_New_Click(object sender, EventArgs e)
        {
            if (g_Soup is not null && g_Soup.Initialized)
            {
                if (g_Soup.Modified)
                {
                    DialogResult result = MessageBox.Show(
                        $"Save changes to {Path.GetFileName(g_Soup.FilePath)}?",
                        $"{g_AppName}",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1
                    );

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(g_Soup.FilePath))
                        {
                            g_Soup.SaveSoup();
                        }
                        else
                        {
                            if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
                            {
                                g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                            }
                            else return;
                        }
                    }
                    else if (result == DialogResult.Cancel) return;
                }
            }

            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
            SelectedObjectId = -1;
            SelectedObjectIndex = -1;

            new FormCreateNewSoup().ShowDialog(this);
            SaveFileDialog_SaveSoup.InitialDirectory = $"{g_SoupDefaultFilePath}";
            SaveFileDialog_SaveSoup.FileName = $"{g_SoupDefaultFileName}";
        }

        private void TopMenu_File_Open_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog_LoadSoup.ShowDialog() == DialogResult.OK)
            {
                if (g_Soup is not null && g_Soup.Initialized)
                {
                    if (g_Soup.Modified)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Save changes to {Path.GetFileName(g_Soup.FilePath)}?",
                            $"{g_AppName}",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1
                        );

                        if (result == DialogResult.Yes)
                        {
                            if (File.Exists(g_Soup.FilePath))
                            {
                                g_Soup.SaveSoup();
                            }
                            else
                            {
                                if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
                                {
                                    g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                                }
                                else return;
                            }
                        }
                        else if (result == DialogResult.Cancel) return;
                    }
                    g_Soup.SetSoupState(SoupState.Stop);
                }

                StreamReader streamReader = new StreamReader(OpenFileDialog_LoadSoup.FileName, Encoding.UTF8);
                try
                {
                    Soup? loadedSoup = JsonSerializer.Deserialize<Soup>(streamReader.ReadToEnd(), new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals });
                    if (loadedSoup is not null)
                    {
                        SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                        SelectedObjectId = -1;
                        SelectedObjectIndex = -1;

                        g_Soup = loadedSoup;
                        if (!g_Soup.Initialized) g_Soup.InitializeSoup();
                        g_Soup.FilePath = OpenFileDialog_LoadSoup.FileName;
                        g_Soup.StartSoupThread();

                        SaveFileDialog_SaveSoup.FileName = $"{Path.GetFileName(g_Soup.FilePath)}";

                        CameraPosition = new Double2d(g_Soup.Settings.SizeX / 2d, g_Soup.Settings.SizeY / 2d);
                        ZoomLevel = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    MessageBox.Show(
                        $"Could not load soup.\r\nThe soup file may be malformed or corrupted.",
                        $"{g_AppName}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1
                    );
                }
                finally
                {
                    streamReader.Close();
                }
            }
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (File.Exists(g_Soup.FilePath))
            {
                g_Soup.SaveSoup();
            }
            else
            {
                if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
                {
                    g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                }
                else return;
            }
        }

        private void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
            {
                g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                SaveFileDialog_SaveSoup.FileName = $"{Path.GetFileName(g_Soup.FilePath)}";
            }
        }

        private void TopMenu_File_Exit_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (g_Soup.Modified)
            {
                DialogResult result = MessageBox.Show(
                    $"Save changes to {Path.GetFileName(g_Soup.FilePath)}?",
                    $"{g_AppName}",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );

                if (result == DialogResult.Yes)
                {
                    if (File.Exists(g_Soup.FilePath))
                    {
                        g_Soup.SaveSoup();
                    }
                    else
                    {
                        if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
                        {
                            g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                        }
                        else return;
                    }
                }
                else if (result == DialogResult.Cancel) return;
            }

            Application.Exit();
        }

        private void TopMenu_Soup_EditSoupSettings_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            g_Soup.SetSoupState(SoupState.Pause);
            new FormSoupSettings(g_Soup.Settings, false).ShowDialog(this);
        }

        private void TopMenuBar_Soup_EditAutosaveSettings_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            g_Soup.SetSoupState(SoupState.Pause);
            new FormAutosaveSettings().ShowDialog();
        }

        private void TopMenu_Help_AboutParamecium_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            if (g_Soup.Modified)
            {
                DialogResult result = MessageBox.Show(
                    $"Save changes to {Path.GetFileName(g_Soup.FilePath)}?",
                    $"{g_AppName}",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );

                if (result == DialogResult.Yes)
                {
                    if (File.Exists(g_Soup.FilePath))
                    {
                        g_Soup.SaveSoup();
                    }
                    else
                    {
                        if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
                        {
                            g_Soup.SaveSoup(SaveFileDialog_SaveSoup.FileName, true);
                        }
                        else e.Cancel = true;
                    }
                }
                else if (result == DialogResult.Cancel) e.Cancel = true;
            }
        }
    }

    public enum CameraState
    {
        Default,
        Tracking,
        AutoTracking
    }
}
