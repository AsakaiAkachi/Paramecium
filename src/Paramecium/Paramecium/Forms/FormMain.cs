﻿using Paramecium.Forms.Renderer;
using Paramecium.Engine;
using System.Diagnostics;
using System.Text.Json;
using System.Text;

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        Bitmap SoupViewCanvas;

        Double2d CameraPosition;
        int ZoomLevel;

        double FramesPerSecond;

        SoupViewOverlayRenderer.SelectedObjectType SelectedObjectType;
        int SelectedObjectIndex;
        long SelectedObjectId;
        bool CameraTracking;
        bool CameraAutoTracking;

        public FormMain()
        {
            InitializeComponent();
            SoupView.MouseWheel += SoupView_MouseWheel;

            Text = $"{Path.GetFileName(g_FilePath)} - {g_AppName} {g_AppVersion}";
            OpenFileDialog_LoadSoup.InitialDirectory = $@"{Path.GetDirectoryName(g_FilePath)}";
            SaveFileDialog_SaveSoup.InitialDirectory = $@"{Path.GetDirectoryName(g_FilePath)}";
            SaveFileDialog_SaveSoup.FileName = "New Soup.soup";

            SoupViewCanvas = new Bitmap(1, 1);

            CameraPosition = new Double2d(g_Soup.SizeX / 2d, g_Soup.SizeY / 2d);
            ZoomLevel = 0;
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                if (SoupView.Width > 0 && SoupView.Height > 0)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    SoupViewCanvas.Dispose();

                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None)
                    {
                        if (!CameraAutoTracking) CameraTracking = false;
                    }
                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Plant)
                    {
                        if (!g_Soup.Plants[SelectedObjectIndex].Exist || SelectedObjectId != g_Soup.Plants[SelectedObjectIndex].Id)
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            if (!CameraAutoTracking) CameraTracking = false;
                        }
                        else if (CameraTracking)
                        {
                            CameraPosition = g_Soup.Plants[SelectedObjectIndex].Position;
                        }
                    }
                    if (SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.Animal)
                    {
                        if (!g_Soup.Animals[SelectedObjectIndex].Exist || SelectedObjectId != g_Soup.Animals[SelectedObjectIndex].Id)
                        {
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            if (!CameraAutoTracking) CameraTracking = false;
                        }
                        else if (CameraTracking)
                        {
                            CameraPosition = g_Soup.Animals[SelectedObjectIndex].Position;
                        }
                    }

                    if (CameraTracking && CameraAutoTracking && SelectedObjectType == SoupViewOverlayRenderer.SelectedObjectType.None)
                    {
                        SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.Animal;
                        SelectedObjectIndex = new Random().Next(0, g_Soup.Animals.Count);
                        while (!g_Soup.Animals[SelectedObjectIndex].Exist) SelectedObjectIndex = new Random().Next(0, g_Soup.Animals.Count);
                        SelectedObjectId = g_Soup.Animals[SelectedObjectIndex].Id;
                    }

                    SoupViewCanvas = new Bitmap(SoupView.Width, SoupView.Height);
                    SoupViewRenderer.DrawSoupView(ref SoupViewCanvas, CameraPosition, ZoomLevel);
                    SoupViewOverlayRenderer.DrawSoupOverlayView(ref SoupViewCanvas, CameraPosition, ZoomLevel, SelectedObjectType, SelectedObjectIndex);

                    SoupView.Image = SoupViewCanvas;

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

                await Task.Delay(1);
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (g_Soup.SoupState == SoupState.Pause) g_Soup.SetSoupState(SoupState.Running);
                    else if (g_Soup.SoupState == SoupState.Running) g_Soup.SetSoupState(SoupState.Pause);
                    break;
                case Keys.OemQuestion:
                    g_Soup.SetSoupState(SoupState.StepRun);
                    break;
                case Keys.OemPeriod:
                    g_Soup.SetThreadCount(g_Soup.ThreadCount + 1);
                    break;
                case Keys.Oemcomma:
                    g_Soup.SetThreadCount(g_Soup.ThreadCount - 1);
                    break;
                case Keys.T:
                    if (!CameraTracking)
                    {
                        if (ModifierKeys == Keys.Control)
                        {
                            CameraTracking = true;
                            CameraAutoTracking = true;
                        }
                        else
                        {
                            if (SelectedObjectType != SoupViewOverlayRenderer.SelectedObjectType.None)
                            {
                                CameraTracking = true;
                            }
                        }
                    }
                    else
                    {
                        CameraTracking = false;
                        CameraAutoTracking = false;
                    }
                    break;
            }
        }

        bool Dragging;
        int MousePrevX, MousePrevY;

        private void SoupView_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (ModifierKeys)
                    {
                        case Keys.Shift:
                            break;
                        case Keys.Control:
                            if (ZoomLevel < 8) ZoomLevel++;
                            break;
                        case Keys.Alt:
                            break;
                        default:
                            SelectedObjectType = SoupViewOverlayRenderer.SelectedObjectType.None;
                            SelectedObjectIndex = -1;
                            CameraTracking = false;
                            CameraAutoTracking = false;

                            if (ZoomLevel >= 4)
                            {
                                try
                                {
                                    Double2d clickedWorldPos = new Double2d(
                                        WorldPosViewPosConversion.ViewPosToWorldPosX(SoupView.Width, CameraPosition, double.Pow(2, ZoomLevel), e.X),
                                        WorldPosViewPosConversion.ViewPosToWorldPosY(SoupView.Height, CameraPosition, double.Pow(2, ZoomLevel), e.Y)
                                    );

                                    if (clickedWorldPos.X >= 0 && clickedWorldPos.X <= g_Soup.SizeX && clickedWorldPos.Y >= 0 && clickedWorldPos.Y <= g_Soup.SizeY)
                                    {
                                        int clickedWorldPosIntegerizedX = int.Max(0, int.Min(g_Soup.SizeX - 1, (int)double.Floor(clickedWorldPos.X)));
                                        int clickedWorldPosIntegerizedY = int.Max(0, int.Min(g_Soup.SizeY - 1, (int)double.Floor(clickedWorldPos.Y)));

                                        for (int x = int.Max(0, clickedWorldPosIntegerizedX - 1); x <= int.Min(g_Soup.SizeX - 1, clickedWorldPosIntegerizedX + 1); x++)
                                        {
                                            for (int y = int.Max(0, clickedWorldPosIntegerizedY - 1); y <= int.Min(g_Soup.SizeY - 1, clickedWorldPosIntegerizedY + 1); y++)
                                            {
                                                Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

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
                                                                SelectedObjectIndex = targetTile.LocalPlantIndexes[i];
                                                                SelectedObjectId = targetPlant.Id;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        for (int x = int.Max(0, clickedWorldPosIntegerizedX - 1); x <= int.Min(g_Soup.SizeX - 1, clickedWorldPosIntegerizedX + 1); x++)
                                        {
                                            for (int y = int.Max(0, clickedWorldPosIntegerizedY - 1); y <= int.Min(g_Soup.SizeY - 1, clickedWorldPosIntegerizedY + 1); y++)
                                            {
                                                Tile targetTile = g_Soup.Tiles[y * g_Soup.SizeX + x];

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
                                                                SelectedObjectIndex = targetTile.LocalAnimalIndexes[i];
                                                                SelectedObjectId = targetAnimal.Id;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
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
                            break;
                        default:
                            Dragging = true;
                            MousePrevX = e.X;
                            MousePrevY = e.Y;
                            break;
                    }
                    break;
            }
        }

        private void SoupView_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                CameraPosition += new Double2d(
                    (MousePrevX - e.X) / double.Pow(2, ZoomLevel),
                    (MousePrevY - e.Y) / double.Pow(2, ZoomLevel)
                );

                //if (CameraPosition.X < 0 || CameraPosition.X > g_Soup.SizeX || CameraPosition.Y < 0 || CameraPosition.Y > g_Soup.SizeY) CameraPosition = new Double2d(double.Max(0, double.Min(g_Soup.SizeX, CameraPosition.X)), double.Max(0, double.Min(g_Soup.SizeY, CameraPosition.Y)));

                MousePrevX = e.X;
                MousePrevY = e.Y;
            }
        }

        private void SoupView_MouseUp(object sender, MouseEventArgs e)
        {
            if (Dragging) Dragging = false;
        }

        private void SoupView_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (e.Delta > 0) if (ZoomLevel < 8) ZoomLevel++;
            if (e.Delta < 0) if (ZoomLevel > 0) ZoomLevel--;
        }

        private void TopMenu_File_Open_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog_LoadSoup.ShowDialog() == DialogResult.OK)
            {
                g_Soup.SetSoupState(SoupState.Stop);
                StreamReader streamWriter = new StreamReader(OpenFileDialog_LoadSoup.FileName, Encoding.UTF8);
                Soup? loadedSoup = JsonSerializer.Deserialize<Soup>(streamWriter.ReadToEnd());
                if (loadedSoup is not null) g_Soup = loadedSoup;
                g_Soup.StartSoupThread();
                streamWriter.Close();

                g_FilePath = OpenFileDialog_LoadSoup.FileName;
                Text = $"{Path.GetFileName(g_FilePath)} - {g_AppName} {g_AppVersion}";
            }
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            StreamWriter streamWriter = new StreamWriter(g_FilePath, false, Encoding.UTF8);
            streamWriter.Write(JsonSerializer.Serialize(g_Soup));
            streamWriter.Close();
        }

        private void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            if (SaveFileDialog_SaveSoup.ShowDialog() == DialogResult.OK)
            {
                g_Soup.SetSoupState(SoupState.Pause);
                StreamWriter streamWriter = new StreamWriter(SaveFileDialog_SaveSoup.FileName, false, Encoding.UTF8);
                streamWriter.Write(JsonSerializer.Serialize(g_Soup));
                streamWriter.Close();

                g_FilePath = SaveFileDialog_SaveSoup.FileName;
                Text = $"{Path.GetFileName(g_FilePath)} - {g_AppName} {g_AppVersion}";
            }
        }

        private void TopMenu_File_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
