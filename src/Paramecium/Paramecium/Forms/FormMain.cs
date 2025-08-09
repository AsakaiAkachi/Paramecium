using Paramecium.Engine;
using Paramecium.Rendering;
using Paramecium.Utils;
using Paramecium.Variables;
using System.Diagnostics;

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        // Soup Viewに表示する画像
        private Bitmap _soupViewImage = new Bitmap(1, 1);

        // カメラの位置とズーム
        private Double2d _cameraPosition = Double2d.Zero;
        private int _cameraZoomLevel = 0;
        private double _unitPerPixel = 1d;

        // カメラの追跡
        private CameraTrackingMode _trackingMode = CameraTrackingMode.Disabled;
        private int _randomTrackingTargetIndex = -1;
        private long _randomTrackingTargetId = -1;
        private bool _autoSelectTrackedCell = false;

        // マウス操作関係
        private bool _isDragging = false;
        private int _mousePosX = 0;
        private int _mousePosY = 0;

        // オブジェクトの選択
        private SoupObjectType _selectedCellType = SoupObjectType.None;
        private int _selectedCellIndex = -1;
        private long _selectedCellId = -1;

        // サブウィンドウ

        public FormMain()
        {
            InitializeComponent();

            Text = $"{Globals.AppName} {Globals.AppVersion}";

            LoadSoupDialog.InitialDirectory = Globals.SavesDirectoryPath;
            SaveSoupDialog.InitialDirectory = Globals.SavesDirectoryPath;

            //Globals.Soup = new Soup(new SoupSettings());
            //Globals.Soup.StartSoupThread();

            //_cameraPosition = new Double2d(Globals.Soup.Settings.SizeX / 2d, Globals.Soup.Settings.SizeY / 2d);
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            Stopwatch frameTime = Stopwatch.StartNew();

            while (true)
            {
                //frameTime.Restart();

                Soup? soup = Globals.Soup;

                if (soup is not null)
                {
                    // カメラの追跡
                    try
                    {
                        if (_trackingMode == CameraTrackingMode.Disabled)
                        {
                            _randomTrackingTargetIndex = -1;
                            _randomTrackingTargetId = -1;

                            TopMenu_View_TrackingSelectedCell.Checked = false;
                            TopMenu_View_TrackingRandomAnimal.Checked = false;
                        }
                        else if (_trackingMode == CameraTrackingMode.TrackingSelectedCell)  // 選択されたセルを追跡する
                        {
                            _randomTrackingTargetIndex = -1;
                            _randomTrackingTargetId = -1;

                            if (_selectedCellType == SoupObjectType.None) _trackingMode = CameraTrackingMode.Disabled;
                            else if (_selectedCellType == SoupObjectType.Plant)
                            {
                                Plant? targetPlant = soup.Plants[_selectedCellIndex];
                                if (targetPlant is not null)
                                {
                                    if (targetPlant.Id == _selectedCellId)
                                    {
                                        _cameraPosition = Double2d.Lerp(_cameraPosition, targetPlant.Position, 0.1d);
                                    }
                                    else _trackingMode = CameraTrackingMode.Disabled;
                                }
                                else _trackingMode = CameraTrackingMode.Disabled;
                            }
                            else if (_selectedCellType == SoupObjectType.Animal)
                            {
                                Animal? targetAnimal = soup.Animals[_selectedCellIndex];
                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.Id == _selectedCellId)
                                    {
                                        _cameraPosition = Double2d.Lerp(_cameraPosition, targetAnimal.Position, 0.1d);
                                    }
                                    else _trackingMode = CameraTrackingMode.Disabled;
                                }
                                else _trackingMode = CameraTrackingMode.Disabled;
                            }

                            TopMenu_View_TrackingSelectedCell.Checked = true;
                            TopMenu_View_TrackingRandomAnimal.Checked = false;
                        }
                        else if (_trackingMode == CameraTrackingMode.TrackingRandomAnimal)  // ランダムな動物を追跡する
                        {
                            if (soup.Animals.Count == 0 || soup.AnimalPopulation == 0) _trackingMode = CameraTrackingMode.Disabled;
                            else if (_randomTrackingTargetIndex == -1)
                            {
                                Random rand = new Random();
                                Animal? targetAnimal = soup.Animals[rand.Next(0, soup.Animals.Count)];
                                if (targetAnimal is not null)
                                {
                                    _randomTrackingTargetIndex = targetAnimal.Index;
                                    _randomTrackingTargetId = targetAnimal.Id;
                                }
                            }
                            else
                            {
                                Animal? targetAnimal = soup.Animals[_randomTrackingTargetIndex];
                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.Id == _randomTrackingTargetId)
                                    {
                                        _cameraPosition = Double2d.Lerp(_cameraPosition, targetAnimal.Position, 0.2d);

                                        if (_autoSelectTrackedCell)
                                        {
                                            SelectCell(SoupObjectType.Animal, _randomTrackingTargetIndex, _randomTrackingTargetId);
                                        }
                                    }
                                    else
                                    {
                                        _randomTrackingTargetIndex = -1;
                                        _randomTrackingTargetId = -1;
                                    }
                                }
                                else
                                {
                                    _randomTrackingTargetIndex = -1;
                                    _randomTrackingTargetId = -1;
                                }
                            }

                            TopMenu_View_TrackingSelectedCell.Checked = false;
                            TopMenu_View_TrackingRandomAnimal.Checked = true;
                        }

                        if (_selectedCellType == SoupObjectType.Plant)
                        {
                            Plant? targetPlant = soup.Plants[_selectedCellIndex];

                            if (targetPlant is not null)
                            {
                                if (targetPlant.Id != _selectedCellId)
                                {
                                    SelectCell(SoupObjectType.None, -1, -1);
                                }
                            }
                            else
                            {
                                SelectCell(SoupObjectType.None, -1, -1);
                            }
                        }
                        if (_selectedCellType == SoupObjectType.Animal)
                        {
                            Animal? targetAnimal = soup.Animals[_selectedCellIndex];

                            if (targetAnimal is not null)
                            {
                                if (targetAnimal.Id != _selectedCellId)
                                {
                                    SelectCell(SoupObjectType.None, -1, -1);
                                }
                            }
                            else
                            {
                                SelectCell(SoupObjectType.None, -1, -1);
                            }
                        }
                    }
                    catch { }

                    if (SoupView.Width > 0 && SoupView.Height > 0)
                    {
                        Bitmap prevFrameSoupViewImage = _soupViewImage;

                        try
                        {
                            Int2d SoupViewSize = new Int2d(SoupView.Width, SoupView.Height);
                            _soupViewImage = SoupViewRenderer.DrawSoupView(soup, soup.Settings, SoupViewSize, _cameraPosition, _cameraZoomLevel, _unitPerPixel, new Int2d(_mousePosX, _mousePosY), _selectedCellType, _selectedCellIndex, _selectedCellId);
                            SoupView.Image = _soupViewImage;
                        }
                        catch { }

                        prevFrameSoupViewImage.Dispose();
                    }

                    if (soup.Modified) Text = $"*{Globals.SoupFileName} - {Globals.AppName} {Globals.AppVersion}";
                    else Text = $"{Globals.SoupFileName} - {Globals.AppName} {Globals.AppVersion}";

                    BottomStat_SoupState.Text = $"Status : {soup.SoupState}";
                    BottomStat_TimeSteps.Text = $"Time Step : {soup.ElapsedTimeSteps} (T{soup.ThreadCount})";
                    BottomStat_Population.Text = $"Population (P/A/T) : {soup.PlantPopulation}/{soup.AnimalPopulation}/{soup.TotalPopulation}";
                    BottomStat_Generation.Text = $"Generation : {soup.LatestGeneration}";
                    BottomStat_TotalBornDie.Text = $"Total Born/Die : {soup.TotalBornCount}/{soup.TotalDieCount}";
                    BottomStat_Tps.Text = $"TPS : {(1d / soup.StepTime).ToString("0.0")} ({(soup.StepTime * 1000d).ToString("0.0000")}ms)";
                    BottomStat_Fps.Text = $"FPS : {(1d / frameTime.Elapsed.TotalSeconds).ToString("0.0")} ({(frameTime.Elapsed.TotalMilliseconds).ToString("0.0000")}ms)";

                    TopMenu_File_Save.Enabled = true;
                    TopMenu_File_SaveAs.Enabled = true;
                    TopMenu_Soup.Enabled = true;
                    TopMenu_View.Enabled = true;

                    if (soup.ThreadCount > 1) TopMenu_Soup_ThreadCountDec.Enabled = true;
                    else TopMenu_Soup_ThreadCountDec.Enabled = false;

                    if (_cameraZoomLevel > 0) TopMenu_View_ZoomOut.Enabled = true;
                    else TopMenu_View_ZoomOut.Enabled = false;
                    if (_cameraZoomLevel < 10) TopMenu_View_ZoomIn.Enabled = true;
                    else TopMenu_View_ZoomIn.Enabled = false;

                    if (_selectedCellType != SoupObjectType.None) TopMenu_View_TrackingSelectedCell.Enabled = true;
                    else TopMenu_View_TrackingSelectedCell.Enabled = false;
                    if (soup.Animals.Count > 0 && soup.AnimalPopulation > 0) TopMenu_View_TrackingRandomAnimal.Enabled = true;
                    else TopMenu_View_TrackingRandomAnimal.Enabled = false;
                }

                frameTime.Restart();

                await Task.Delay(1);
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.N:
                    if (ModifierKeys == Keys.Control) NewSoup();
                    break;
                case Keys.O:
                    if (ModifierKeys == Keys.Control) OpenSoup();
                    break;
            }

            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        if (ModifierKeys == (Keys.Control & Keys.Shift)) SaveAsSoup();
                        else if (ModifierKeys == Keys.Control) SaveSoup();
                        break;
                    case Keys.Space:        // スープの一時停止/再開
                        SoupRunPause();
                        break;
                    case Keys.OemQuestion:  // スープを1ステップだけ実行
                        SoupStepRun();
                        break;
                    case Keys.Oemcomma:     // スープのスレッド数を1減らす
                        SoupThreadCountDec();
                        break;
                    case Keys.OemPeriod:    // スープのスレッド数を1増やす
                        SoupThreadCountInc();
                        break;
                    case Keys.C:            // カメラの位置と倍率をリセットする
                        ResetCameraPosition();
                        break;
                    case Keys.T:            // カメラの追跡モードを「選択中のセルの追跡」に変更、または追跡を無効化
                        if (_trackingMode == CameraTrackingMode.TrackingSelectedCell) _trackingMode = CameraTrackingMode.Disabled;
                        else _trackingMode = CameraTrackingMode.TrackingSelectedCell;
                        break;
                    case Keys.R:            // カメラの追跡モードを「ランダムな動物の追跡」に変更、または追跡を無効化
                        if (_trackingMode == CameraTrackingMode.TrackingRandomAnimal) _trackingMode = CameraTrackingMode.Disabled;
                        else _trackingMode = CameraTrackingMode.TrackingRandomAnimal;
                        break;
                }
            }
        }

        private void SoupView_MouseDown(object sender, MouseEventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                Double2d mousePositionInSoup = new Double2d((e.X - SoupView.Width / 2d) * _unitPerPixel + _cameraPosition.X, (e.Y - SoupView.Height / 2d) * _unitPerPixel + _cameraPosition.Y);
                Int2d mouseTilePosition = soup.GetTilePositionFromPosition(mousePositionInSoup);
                int mouseTileIndex = soup.GetTileIndexFromTilePosition(mouseTilePosition);

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        switch (ModifierKeys)
                        {
                            case Keys.Shift:    // セルを移動する
                                if (soup.SoupState == SoupState.Pause)
                                {
                                    if (_selectedCellType == SoupObjectType.Plant)
                                    {
                                        Plant? targetPlant = GetSelectedPlant();

                                        if (targetPlant is not null)
                                        {
                                            targetPlant.Position = mousePositionInSoup;

                                            soup.Tiles[targetPlant.TileIndex].PlantIndexes.Remove(targetPlant.Index);
                                            targetPlant.TileIndex = mouseTileIndex;
                                            soup.Tiles[targetPlant.TileIndex].PlantIndexes.Add(targetPlant.Index);
                                        }
                                    }
                                    if (_selectedCellType == SoupObjectType.Animal)
                                    {
                                        Animal? targetAnimal = GetSelectedAnimal();

                                        if (targetAnimal is not null)
                                        {
                                            targetAnimal.Position = mousePositionInSoup;

                                            soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Remove(targetAnimal.Index);
                                            targetAnimal.TileIndex = mouseTileIndex;
                                            soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Add(targetAnimal.Index);
                                        }
                                    }
                                }
                                break;
                            case Keys.Control:  // ズームイン
                                if (_cameraZoomLevel < 10)
                                {
                                    _cameraZoomLevel++;
                                    UpdateUnitPerPixel();

                                    _cameraPosition.X += (e.X - SoupView.Width / 2d) * _unitPerPixel;
                                    _cameraPosition.Y += (e.Y - SoupView.Height / 2d) * _unitPerPixel;
                                }
                                break;
                            case Keys.Alt:      // 壁を作成する
                                if (soup.SoupState == SoupState.Pause)
                                {
                                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                    {
                                        soup.Tiles[mouseTileIndex].Type = TileType.Wall;
                                    }
                                }
                                break;
                            default:            // セルを選択する
                                SelectCell(SoupObjectType.None, -1, -1);

                                try
                                {
                                    if (_cameraZoomLevel >= 5)
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                        {
                                            for (int x = -1; x <= 1; x++)
                                            {
                                                for (int y = -1; y <= 1; y++)
                                                {
                                                    Int2d targetTilePosition = mouseTilePosition + new Int2d(x, y);

                                                    if (targetTilePosition.X >= 0 && targetTilePosition.X < soup.Settings.SizeX && targetTilePosition.Y >= 0 && targetTilePosition.Y < soup.Settings.SizeY)
                                                    {
                                                        int targetTileIndex = soup.GetTileIndexFromTilePosition(targetTilePosition);
                                                        Tile targetTile = soup.Tiles[targetTileIndex];

                                                        lock (targetTile.LockObject)
                                                        {
                                                            for (int i = 0; i < targetTile.PlantPopulation; i++)
                                                            {
                                                                Plant? targetPlant = soup.Plants[targetTile.PlantIndexes[i]];

                                                                if (targetPlant is not null)
                                                                {
                                                                    if (Double2d.DistanceSquared(mousePositionInSoup, targetPlant.Position) < targetPlant.Radius * targetPlant.Radius)
                                                                    {
                                                                        SelectCell(SoupObjectType.Plant, targetPlant.Index, targetPlant.Id);
                                                                    }
                                                                }
                                                            }
                                                            for (int i = 0; i < targetTile.AnimalPopulation; i++)
                                                            {
                                                                Animal? targetAnimal = soup.Animals[targetTile.AnimalIndexes[i]];

                                                                if (targetAnimal is not null)
                                                                {
                                                                    if (Double2d.DistanceSquared(mousePositionInSoup, targetAnimal.Position) < targetAnimal.Radius * targetAnimal.Radius)
                                                                    {
                                                                        SelectCell(SoupObjectType.Animal, targetAnimal.Index, targetAnimal.Id);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                        {
                                            Tile targetTile = soup.Tiles[mouseTileIndex];

                                            if (targetTile.AnimalPopulation > 0)
                                            {
                                                Animal? targetAnimal = soup.Animals[targetTile.AnimalIndexes[targetTile.AnimalIndexes.Count - 1]];

                                                if (targetAnimal is not null)
                                                {
                                                    SelectCell(SoupObjectType.Animal, targetAnimal.Index, targetAnimal.Id);
                                                }
                                            }
                                            else if (targetTile.PlantPopulation > 0)
                                            {
                                                Plant? targetPlant = soup.Plants[targetTile.PlantIndexes[targetTile.PlantIndexes.Count - 1]];

                                                if (targetPlant is not null)
                                                {
                                                    SelectCell(SoupObjectType.Plant, targetPlant.Index, targetPlant.Id);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                                break;
                        }
                        break;
                    case MouseButtons.Right:
                        switch (ModifierKeys)
                        {
                            case Keys.Shift:
                                break;
                            case Keys.Control:  // ズームアウト
                                if (_cameraZoomLevel > 0)
                                {
                                    _cameraPosition.X -= (e.X - SoupView.Width / 2d) * _unitPerPixel;
                                    _cameraPosition.Y -= (e.Y - SoupView.Height / 2d) * _unitPerPixel;

                                    _cameraZoomLevel--;
                                    UpdateUnitPerPixel();
                                }
                                break;
                            case Keys.Alt:      // 壁を削除する
                                if (soup.SoupState == SoupState.Pause)
                                {
                                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                    {
                                        soup.Tiles[mouseTileIndex].Type = TileType.Default;
                                    }
                                }
                                break;
                            default:            // カメラの移動を開始する
                                _isDragging = true;
                                _mousePosX = e.X;
                                _mousePosY = e.Y;
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
                }
            }
        }

        private void SoupView_MouseMove(object sender, MouseEventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_isDragging)    // カメラを移動させる
                {
                    _cameraPosition.X += (_mousePosX - e.X) * _unitPerPixel;
                    _cameraPosition.Y += (_mousePosY - e.Y) * _unitPerPixel;

                    _mousePosX = e.X;
                    _mousePosY = e.Y;

                    _trackingMode = CameraTrackingMode.Disabled;
                }
                else
                {
                    _mousePosX = e.X;
                    _mousePosY = e.Y;

                    Double2d mousePositionInSoup = new Double2d((e.X - SoupView.Width / 2d) * _unitPerPixel + _cameraPosition.X, (e.Y - SoupView.Height / 2d) * _unitPerPixel + _cameraPosition.Y);
                    Int2d mouseTilePosition = soup.GetTilePositionFromPosition(mousePositionInSoup);
                    int mouseTileIndex = soup.GetTileIndexFromTilePosition(mouseTilePosition);

                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            switch (ModifierKeys)
                            {
                                case Keys.Shift:    // セルを移動する
                                    if (soup.SoupState == SoupState.Pause)
                                    {
                                        if (_selectedCellType == SoupObjectType.Plant)
                                        {
                                            Plant? targetPlant = GetSelectedPlant();

                                            if (targetPlant is not null)
                                            {
                                                targetPlant.Position = mousePositionInSoup;

                                                soup.Tiles[targetPlant.TileIndex].PlantIndexes.Remove(targetPlant.Index);
                                                targetPlant.TileIndex = mouseTileIndex;
                                                soup.Tiles[targetPlant.TileIndex].PlantIndexes.Add(targetPlant.Index);
                                            }
                                        }
                                        if (_selectedCellType == SoupObjectType.Animal)
                                        {
                                            Animal? targetAnimal = GetSelectedAnimal();

                                            if (targetAnimal is not null)
                                            {
                                                targetAnimal.Position = mousePositionInSoup;

                                                soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Remove(targetAnimal.Index);
                                                targetAnimal.TileIndex = mouseTileIndex;
                                                soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Add(targetAnimal.Index);
                                            }
                                        }
                                    }
                                    break;
                                case Keys.Control:
                                    break;
                                case Keys.Alt:      // 壁を作成する
                                    if (soup.SoupState == SoupState.Pause)
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                        {
                                            soup.Tiles[mouseTileIndex].Type = TileType.Wall;
                                        }
                                    }
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
                                    break;
                                case Keys.Alt:      // 壁を削除する
                                    if (soup.SoupState == SoupState.Pause)
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SizeY)
                                        {
                                            soup.Tiles[mouseTileIndex].Type = TileType.Default;
                                        }
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
                    }
                }
            }
        }

        private void SoupView_MouseUp(object sender, MouseEventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_isDragging)    // カメラの移動を終了する
                {
                    _isDragging = false;
                }
            }
        }

        private void TopMenu_File_New_Click(object sender, EventArgs e)
        {
            NewSoup();
        }

        private void TopMenu_File_Open_Click(object sender, EventArgs e)
        {
            OpenSoup();
        }

        private void TopMenu_File_Save_Click(object sender, EventArgs e)
        {
            SaveSoup();
        }

        private void TopMenu_File_SaveAs_Click(object sender, EventArgs e)
        {
            SaveAsSoup();
        }

        private void TopMenu_File_Exit_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;
            SoupState soupState = SoupState.Pause;

            if (soup is not null)
            {
                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (soup.Modified)
                {
                    bool soupSavedOrDiscard = ShowSaveSoupChangesDialog();
                    if (!soupSavedOrDiscard)
                    {
                        soup.SetSoupState(soupState);
                        return;
                    }
                }
            }

            Application.Exit();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Soup? soup = Globals.Soup;
            SoupState soupState = SoupState.Pause;

            if (soup is not null)
            {
                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (soup.Modified)
                {
                    bool soupSavedOrDiscard = ShowSaveSoupChangesDialog();
                    if (!soupSavedOrDiscard)
                    {
                        soup.SetSoupState(soupState);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void TopMenu_Soup_RunPause_Click(object sender, EventArgs e)
        {
            SoupRunPause();
        }

        private void TopMenu_Soup_StepRun_Click(object sender, EventArgs e)
        {
            SoupStepRun();
        }

        private void TopMenu_Soup_ThreadCountInc_Click(object sender, EventArgs e)
        {
            SoupThreadCountInc();
        }

        private void TopMenu_Soup_ThreadCountDec_Click(object sender, EventArgs e)
        {
            SoupThreadCountDec();
        }

        private void TopMenu_Soup_EditSoupSettings_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                SoupSettingsSetter soupSettingsSetter = new SoupSettingsSetter(soup.Settings);

                SoupState soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                FormSoupSettings formSoupSettings = new FormSoupSettings(soupSettingsSetter, true);
                formSoupSettings.ShowDialog();

                if (soupSettingsSetter.SoupSettings is not null)
                {
                    soup.Settings = soupSettingsSetter.SoupSettings;
                    soup.Modified = true;
                }

                soup.SetSoupState(soupState);
            }
        }

        private void TopMenu_View_ZoomIn_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_cameraZoomLevel < 10)
                {
                    _cameraZoomLevel++;
                    UpdateUnitPerPixel();
                }
            }
        }

        private void TopMenu_View_ZoomOut_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_cameraZoomLevel > 0)
                {
                    _cameraZoomLevel--;
                    UpdateUnitPerPixel();
                }
            }
        }

        private void TopMenu_View_Reset_Click(object sender, EventArgs e)
        {
            ResetCameraPosition();
        }

        private void TopMenu_View_TrackingSelectedCell_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_trackingMode != CameraTrackingMode.TrackingSelectedCell) _trackingMode = CameraTrackingMode.TrackingSelectedCell;
                else _trackingMode = CameraTrackingMode.Disabled;
            }
        }

        private void TopMenu_View_TrackingRandomAnimal_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_trackingMode != CameraTrackingMode.TrackingRandomAnimal) _trackingMode = CameraTrackingMode.TrackingRandomAnimal;
                else _trackingMode = CameraTrackingMode.Disabled;
            }
        }

        private void TopMenu_View_AutoSelectTrackedCells_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (!_autoSelectTrackedCell)
                {
                    _autoSelectTrackedCell = true;
                    TopMenu_View_AutoSelectTrackedCells.Checked = true;
                }
                else
                {
                    _autoSelectTrackedCell = false;
                    TopMenu_View_AutoSelectTrackedCells.Checked = false;
                }
            }
        }

        private void NewSoup()
        {
            Soup? soup = Globals.Soup;
            SoupState soupState = SoupState.Pause;

            if (soup is not null)
            {
                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (soup.Modified)
                {
                    bool soupSavedOrDiscard = ShowSaveSoupChangesDialog();
                    if (!soupSavedOrDiscard)
                    {
                        soup.SetSoupState(soupState);
                        return;
                    }
                }
            }

            SoupSettingsSetter soupSettingsSetter = new SoupSettingsSetter(null);

            FormSoupSettings formSoupSettings = new FormSoupSettings(soupSettingsSetter, false);
            formSoupSettings.ShowDialog();

            if (soupSettingsSetter.SoupSettings is not null)
            {
                if (soup is not null)
                {
                    SelectCell(SoupObjectType.None, -1, -1);
                    soup.SetSoupState(SoupState.Stop);
                }

                Globals.Soup = new Soup(soupSettingsSetter.SoupSettings);
                Globals.Soup.StartSoupThread();

                Globals.SoupFilePath = string.Empty;
                Globals.SoupFileName = "untitled.soup";

                _cameraPosition = new Double2d(Globals.Soup.Settings.SizeX / 2d, Globals.Soup.Settings.SizeY / 2d);
            }
            else if (soup is not null)
            {
                soup.SetSoupState(soupState);
            }
        }
        private void OpenSoup()
        {
            Soup? soup = Globals.Soup;
            SoupState soupState = SoupState.Pause;

            if (soup is not null)
            {
                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (soup.Modified)
                {
                    bool soupSavedOrDiscard = ShowSaveSoupChangesDialog();
                    if (!soupSavedOrDiscard)
                    {
                        SelectCell(SoupObjectType.None, -1, -1);
                        soup.SetSoupState(soupState);
                        return;
                    }
                }
            }

            if (LoadSoupDialog.ShowDialog() == DialogResult.OK)
            {
                Soup? loadedSoup = JsonFileImportAndExport.Import<Soup>(LoadSoupDialog.FileName);

                if (loadedSoup is not null)
                {
                    if (soup is not null)
                    {
                        soup.SetSoupState(SoupState.Stop);
                    }

                    Globals.Soup = loadedSoup;
                    loadedSoup.StartSoupThread();

                    Globals.SoupFilePath = LoadSoupDialog.FileName;
                    Globals.SoupFileName = Path.GetFileName(LoadSoupDialog.FileName);

                    _cameraPosition = new Double2d(Globals.Soup.Settings.SizeX / 2d, Globals.Soup.Settings.SizeY / 2d);
                }
            }
            else if (soup is not null)
            {
                soup.SetSoupState(soupState);
            }
        }
        private bool SaveSoup()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                SoupState soupState = SoupState.Pause;

                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (File.Exists(Globals.SoupFilePath))
                {
                    soup.Modified = false;
                    JsonFileImportAndExport.Export(Globals.SoupFilePath, soup);

                    soup.SetSoupState(soupState);
                    return true;
                }
                else if (SaveSoupDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.SoupFilePath = SaveSoupDialog.FileName;
                    Globals.SoupFileName = Path.GetFileName(SaveSoupDialog.FileName);
                    soup.Modified = false;

                    JsonFileImportAndExport.Export(SaveSoupDialog.FileName, soup);

                    soup.SetSoupState(soupState);
                    return true;
                }
                else
                {
                    soup.SetSoupState(soupState);
                    return false;
                }
            }

            return true;
        }
        private bool SaveAsSoup()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                SoupState soupState = SoupState.Pause;

                soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                if (SaveSoupDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.SoupFilePath = SaveSoupDialog.FileName;
                    Globals.SoupFileName = Path.GetFileName(SaveSoupDialog.FileName);
                    soup.Modified = false;

                    JsonFileImportAndExport.Export(SaveSoupDialog.FileName, soup);

                    soup.SetSoupState(soupState);
                    return true;
                }
                else
                {
                    soup.SetSoupState(soupState);
                    return false;
                }
            }

            return true;
        }
        private bool ShowSaveSoupChangesDialog()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                DialogResult result = MessageBox.Show(
                    $"Save changes to {Path.GetFileName(Globals.SoupFileName)}?",
                    $"{Globals.AppName}",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );

                if (result == DialogResult.Yes)
                {
                    return SaveSoup();
                }
                else if (result == DialogResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void SoupRunPause()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause) soup.SetSoupState(SoupState.Running);
                else soup.SetSoupState(SoupState.Pause);
            }
        }
        private void SoupStepRun()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                soup.SetSoupState(SoupState.StepRun);
            }
        }
        private void SoupThreadCountInc()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                soup.SetThreadCount(soup.ThreadCount + 1);
            }
        }
        private void SoupThreadCountDec()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.ThreadCount > 1) soup.SetThreadCount(soup.ThreadCount - 1);
            }
        }

        private void ResetCameraPosition()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                _cameraPosition = new Double2d(soup.Settings.SizeX / 2d, soup.Settings.SizeY / 2d);
                _cameraZoomLevel = 0;
                UpdateUnitPerPixel();
            }
        }

        private void UpdateUnitPerPixel()
        {
            _unitPerPixel = 1d / Math.Pow(2, _cameraZoomLevel);
        }

        private void SelectCell(SoupObjectType type, int index, long id)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (type != SoupObjectType.None)
                {
                    if (type == SoupObjectType.Plant)
                    {
                        Plant? targetPlant = soup.Plants[index];
                        if (targetPlant is not null)
                        {
                            if (targetPlant.Id == id)
                            {
                                _selectedCellType = type;
                                _selectedCellIndex = index;
                                _selectedCellId = id;
                            }
                        }
                    }
                    if (type == SoupObjectType.Animal)
                    {
                        Animal? targetAnimal = soup.Animals[index];
                        if (targetAnimal is not null)
                        {
                            if (targetAnimal.Id == id)
                            {
                                _selectedCellType = type;
                                _selectedCellIndex = index;
                                _selectedCellId = id;
                            }
                        }
                    }
                }
                else
                {
                    _selectedCellType = type;
                    _selectedCellIndex = index;
                    _selectedCellId = id;
                }
            }
        }

        private Plant? GetSelectedPlant()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_selectedCellType == SoupObjectType.Plant)
                {
                    Plant? targetPlant = soup.Plants[_selectedCellIndex];

                    if (targetPlant is not null)
                    {
                        if (targetPlant.Id == _selectedCellId) return targetPlant;
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }

            return null;
        }
        private Animal? GetSelectedAnimal()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_selectedCellType == SoupObjectType.Animal)
                {
                    Animal? targetAnimal = soup.Animals[_selectedCellIndex];

                    if (targetAnimal is not null)
                    {
                        if (targetAnimal.Id == _selectedCellId) return targetAnimal;
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }

            return null;
        }
    }
}
