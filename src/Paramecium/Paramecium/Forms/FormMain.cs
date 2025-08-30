using Paramecium.Engine;
using Paramecium.Rendering;
using Paramecium.Utils;
using Paramecium.Variables;
using System.Diagnostics;

namespace Paramecium.Forms
{
    // メインのForm
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
        private SoupObjectPointer _selectedSoupObject = new SoupObjectPointer();

        // オーバーレイ
        private OverlayToggles _overlayToggles = (OverlayToggles.AllOverlays | OverlayToggles.SelectedObject | OverlayToggles.AnimalBrainDiagram | OverlayToggles.AnimalBrainInputOutput | OverlayToggles.FullScreenOverlay);

        // フルスクリーン
        private bool _isFullScreen = false;
        private FormWindowState _prevWindowState;
        private Size _prevClientSize;

        // サブウィンドウ
        private FormStatistics _formStatistics = new FormStatistics();
        private FormAutosaveSettings _formAutosaveSettings = new FormAutosaveSettings();
        private FormObjectEditor _formObjectEditor = new FormObjectEditor();
        private FormAboutParamecium _formAboutParamecium = new FormAboutParamecium();

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

                            if (_selectedSoupObject.ObjectType == SoupObjectType.Tile || _selectedSoupObject.ObjectType == SoupObjectType.None) _trackingMode = CameraTrackingMode.Disabled;
                            else if (_selectedSoupObject.ObjectType == SoupObjectType.Plant)
                            {
                                Plant? targetPlant = soup.Plants[_selectedSoupObject.ObjectIndex];
                                if (targetPlant is not null)
                                {
                                    if (targetPlant.Id == _selectedSoupObject.ObjectId)
                                    {
                                        _cameraPosition = Double2d.Lerp(_cameraPosition, targetPlant.Position, 0.1d);
                                    }
                                    else _trackingMode = CameraTrackingMode.Disabled;
                                }
                                else _trackingMode = CameraTrackingMode.Disabled;
                            }
                            else if (_selectedSoupObject.ObjectType == SoupObjectType.Animal)
                            {
                                Animal? targetAnimal = soup.Animals[_selectedSoupObject.ObjectIndex];
                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.Id == _selectedSoupObject.ObjectId)
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
                                            _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Animal, _randomTrackingTargetIndex, _randomTrackingTargetId);
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

                        if (_selectedSoupObject.ObjectType == SoupObjectType.Plant)
                        {
                            Plant? targetPlant = soup.Plants[_selectedSoupObject.ObjectIndex];

                            if (targetPlant is not null)
                            {
                                if (targetPlant.Id != _selectedSoupObject.ObjectId)
                                {
                                    _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, -1, -1);
                                }
                            }
                            else
                            {
                                _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, -1, -1);
                            }
                        }
                        else if (_selectedSoupObject.ObjectType == SoupObjectType.Animal)
                        {
                            Animal? targetAnimal = soup.Animals[_selectedSoupObject.ObjectIndex];

                            if (targetAnimal is not null)
                            {
                                if (targetAnimal.Id != _selectedSoupObject.ObjectId)
                                {
                                    _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, -1, -1);
                                }
                            }
                            else
                            {
                                _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, -1, -1);
                            }
                        }
                    }
                    catch { }

                    if (SoupView.Width > 0 && SoupView.Height > 0)
                    {
                        Bitmap prevFrameSoupViewImage = _soupViewImage;
                        _soupViewImage = new Bitmap(SoupView.Width, SoupView.Height);

                        try
                        {
                            SoupViewRenderer.DrawSoupView(_soupViewImage, soup, soup.Settings, _cameraPosition, _cameraZoomLevel, _unitPerPixel, new Int2d(_mousePosX, _mousePosY), _selectedSoupObject);
                            SoupObjectOverlayRenderer.DrawSoupObjectOverlayRenderer(_soupViewImage, soup, soup.Settings, _cameraPosition, _cameraZoomLevel, _unitPerPixel, new Int2d(_mousePosX, _mousePosY), _selectedSoupObject, _overlayToggles);
                            if (_isFullScreen) FullScreenOverlayRenderer.DrawFullScreenOverlay(_soupViewImage, frameTime.Elapsed.TotalMilliseconds, _overlayToggles);

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
                    BottomStat_Generation.Text = $"Latest Generation : {soup.LatestGeneration}";
                    BottomStat_TotalBornDie.Text = $"Total Born/Die : {soup.TotalBornCount}/{soup.TotalDieCount}";
                    BottomStat_Tps.Text = $"TPS : {(1d / soup.StepTime).ToString("0.0")} ({(soup.StepTime * 1000d).ToString("0.0000")}ms)";
                    BottomStat_Fps.Text = $"FPS : {(1d / frameTime.Elapsed.TotalSeconds).ToString("0.0")} ({(frameTime.Elapsed.TotalMilliseconds).ToString("0.0000")}ms)";

                    TopMenu_File_Save.Enabled = true;
                    TopMenu_File_SaveAs.Enabled = true;
                    TopMenu_Soup.Enabled = true;
                    TopMenu_Edit.Enabled = true;
                    TopMenu_View.Enabled = true;
                    TopMenu_Window.Enabled = true;

                    if (soup.ThreadCount > 1) TopMenu_Soup_ThreadCountDec.Enabled = true;
                    else TopMenu_Soup_ThreadCountDec.Enabled = false;

                    if (_cameraZoomLevel > 0) TopMenu_View_ZoomOut.Enabled = true;
                    else TopMenu_View_ZoomOut.Enabled = false;
                    if (_cameraZoomLevel < 10) TopMenu_View_ZoomIn.Enabled = true;
                    else TopMenu_View_ZoomIn.Enabled = false;

                    if (_selectedSoupObject.ObjectType == SoupObjectType.Plant || _selectedSoupObject.ObjectType == SoupObjectType.Animal) TopMenu_View_TrackingSelectedCell.Enabled = true;
                    else TopMenu_View_TrackingSelectedCell.Enabled = false;
                    if (soup.Animals.Count > 0 && soup.AnimalPopulation > 0) TopMenu_View_TrackingRandomAnimal.Enabled = true;
                    else TopMenu_View_TrackingRandomAnimal.Enabled = false;

                    if (_isFullScreen) TopMenu_View_ToggleFullScreen.Checked = true;
                    else TopMenu_View_ToggleFullScreen.Checked = false;

                    if (_formAutosaveSettings.IsDisposed) TopMenu_Window_ObjectEditor.Checked = false;
                    else if (_formAutosaveSettings.Visible) TopMenu_Window_ObjectEditor.Checked = true;
                    else TopMenu_Window_ObjectEditor.Checked = false;

                    if (_formObjectEditor.IsDisposed) TopMenu_Window_AutosaveSettings.Checked = false;
                    else if (_formObjectEditor.Visible) TopMenu_Window_ObjectEditor.Checked = true;
                    else TopMenu_Window_ObjectEditor.Checked = false;

                    TopMenu_View_ToggleOverlays_AllOverlays.Checked = (_overlayToggles & OverlayToggles.AllOverlays) == OverlayToggles.AllOverlays;
                    TopMenu_View_ToggleOverlays_SelectedObject.Checked = (_overlayToggles & OverlayToggles.SelectedObject) == OverlayToggles.SelectedObject;
                    TopMenu_View_ToggleOverlays_BrainDiagram.Checked = (_overlayToggles & OverlayToggles.AnimalBrainDiagram) == OverlayToggles.AnimalBrainDiagram;
                    TopMenu_View_ToggleOverlays_BrainInputOutput.Checked = (_overlayToggles & OverlayToggles.AnimalBrainInputOutput) == OverlayToggles.AnimalBrainInputOutput;
                    TopMenu_View_ToggleOverlays_FullScreenOverlay.Checked = (_overlayToggles & OverlayToggles.FullScreenOverlay) == OverlayToggles.FullScreenOverlay;
                }

                frameTime.Restart();

                await Task.Delay(1);
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.N:        // スープの新規作成
                    if (ModifierKeys == Keys.Control) NewSoup();
                    break;
                case Keys.O:        // スープの読み込み
                    if (ModifierKeys == Keys.Control) OpenSoup();
                    break;
                case Keys.F:
                    if (ModifierKeys == Keys.Control) ToggleFullScreen();
                    break;
            }

            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                Double2d mousePositionInSoup = new Double2d((_mousePosX - SoupView.Width / 2d) * _unitPerPixel + _cameraPosition.X, (_mousePosY - SoupView.Height / 2d) * _unitPerPixel + _cameraPosition.Y);
                Int2d mouseTilePosition = soup.GetTilePositionFromPosition(mousePositionInSoup);
                int mouseTileIndex = soup.GetTileIndexFromTilePosition(mouseTilePosition);

                switch (e.KeyCode)
                {
                    case Keys.W:
                        if (ModifierKeys == Keys.None) _cameraPosition = new Double2d(double.Floor(_cameraPosition.X) + 0.5, double.Max(0, double.Floor(_cameraPosition.Y - 1)) + 0.5);                         // カメラを上方向に移動
                        break;
                    case Keys.S:
                        if (ModifierKeys == Keys.None) _cameraPosition = new Double2d(double.Floor(_cameraPosition.X) + 0.5, double.Min(soup.Settings.SoupSizeY - 1, double.Floor(_cameraPosition.Y + 1)) + 0.5);   // カメラを下方向に移動
                        else if ((ModifierKeys & Keys.Control) == Keys.Control && (ModifierKeys & Keys.Shift) == Keys.Shift) SaveAsSoup();  // スープを名前を付けて保存
                        else if ((ModifierKeys & Keys.Control) == Keys.Control) SaveSoup();                                                 // スープを上書き保存
                        break;
                    case Keys.A:
                        if (ModifierKeys == Keys.None) _cameraPosition = new Double2d(double.Max(0, double.Floor(_cameraPosition.X - 1)) + 0.5, double.Floor(_cameraPosition.Y) + 0.5);                         // カメラを左方向に移動
                        else if ((ModifierKeys & Keys.Control) == Keys.Control && (ModifierKeys & Keys.Shift) == Keys.Shift) ShowFormAutosaveSettings();    // オートセーブ設定を表示する
                        break;
                    case Keys.D:
                        if (ModifierKeys == Keys.None) _cameraPosition = new Double2d(double.Min(soup.Settings.SoupSizeX - 1, double.Floor(_cameraPosition.X + 1)) + 0.5, double.Floor(_cameraPosition.Y) + 0.5);   // カメラを右方向に移動
                        else if ((ModifierKeys & Keys.Control) == Keys.Control) KillSelectedCell();     // 選択中のセルを殺す
                        break;
                    case Keys.OemMinus:     // ズームアウト
                        break;
                    case Keys.Space:        // スープの一時停止/再開
                        if (ModifierKeys == Keys.None) SoupRunPause();
                        break;
                    case Keys.OemQuestion:  // スープを1ステップだけ実行
                        if (ModifierKeys == Keys.None) SoupStepRun();
                        break;
                    case Keys.Oemcomma:     // スープのスレッド数を1減らす
                        if (ModifierKeys == Keys.None) SoupThreadCountDec();
                        break;
                    case Keys.OemPeriod:    // スープのスレッド数を1増やす
                        if (ModifierKeys == Keys.None) SoupThreadCountInc();
                        break;
                    case Keys.C:
                        if (ModifierKeys == Keys.None) ResetCameraPosition();   // カメラの位置と倍率をリセットする
                        else if ((ModifierKeys & Keys.Control) == Keys.Control) CopySelectedCell();     // 選択中のセルをコピーする
                        break;
                    case Keys.Z:      // ズームイン
                        if (ModifierKeys == Keys.None) if (_cameraZoomLevel < 10) { _cameraZoomLevel++; UpdateUnitPerPixel(); }
                        break;
                    case Keys.X:
                        if (ModifierKeys == Keys.None) if (_cameraZoomLevel > 0) { _cameraZoomLevel--; UpdateUnitPerPixel(); }  // ズームアウト
                            else if ((ModifierKeys & Keys.Control) == Keys.Control) CutSelectedCell();      // 選択中のセルをカットする
                        break;
                    case Keys.V:
                        if ((ModifierKeys & Keys.Control) == Keys.Control) PasteClipboardCell(mousePositionInSoup); // クリップボードからセルをペーストする
                        break;
                    case Keys.T:
                        if (ModifierKeys == Keys.None)  // カメラの追跡モードを「選択中のセルの追跡」に変更、または追跡を無効化
                        {
                            if (_trackingMode == CameraTrackingMode.TrackingSelectedCell) _trackingMode = CameraTrackingMode.Disabled;
                            else _trackingMode = CameraTrackingMode.TrackingSelectedCell;
                        }
                        else if ((ModifierKeys & Keys.Control) == Keys.Control && (ModifierKeys & Keys.Shift) == Keys.Shift) ShowFormStatistics();  // 統計ウィンドウを表示する
                        break;
                    case Keys.R:
                        if (ModifierKeys == Keys.None)                              // カメラの追跡モードを「ランダムな動物の追跡」に変更、または追跡を無効化
                        {
                            if (_trackingMode == CameraTrackingMode.TrackingRandomAnimal) _trackingMode = CameraTrackingMode.Disabled;
                            else _trackingMode = CameraTrackingMode.TrackingRandomAnimal;
                        }
                        else if ((ModifierKeys & Keys.Shift) == Keys.Shift)         // 追跡中のオブジェクトの自動選択の切り替え
                        {
                            if (!_autoSelectTrackedCell) _autoSelectTrackedCell = true;
                            else _autoSelectTrackedCell = false;
                        }
                        break;
                    case Keys.E:            // オブジェクトエディターを表示する
                        if ((ModifierKeys & Keys.Control) == Keys.Control && (ModifierKeys & Keys.Shift) == Keys.Shift) ShowFormObjectEditor();
                        break;

                    // オーバーレイの表示切替
                    case Keys.D0:           // 全てのオーバーレイ
                        if (ModifierKeys == Keys.None) ChangeOverlayToggles(OverlayToggles.AllOverlays);
                        break;
                    case Keys.D1:           // 選択したオブジェクトの情報
                        if (ModifierKeys == Keys.None) ChangeOverlayToggles(OverlayToggles.SelectedObject);
                        break;
                    case Keys.D2:           // 動物:BrainDiagram
                        if (ModifierKeys == Keys.None) ChangeOverlayToggles(OverlayToggles.AnimalBrainDiagram);
                        break;
                    case Keys.D3:           // 動物:BrainInputOutput
                        if (ModifierKeys == Keys.None) ChangeOverlayToggles(OverlayToggles.AnimalBrainInputOutput);
                        break;
                    case Keys.D9:           // フルスクリーンオーバーレイ
                        if (ModifierKeys == Keys.None) ChangeOverlayToggles(OverlayToggles.FullScreenOverlay);
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
                                MoveSelectedCell(mousePositionInSoup);
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
                                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
                                    {
                                        soup.Tiles[mouseTileIndex].Type = TileType.Wall;
                                    }
                                }
                                break;
                            default:            // オブジェクトを選択する
                                _selectedSoupObject = new SoupObjectPointer();

                                try
                                {
                                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
                                    {
                                        _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, mouseTileIndex, -1);
                                    }

                                    if (_cameraZoomLevel >= 5)
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
                                        {
                                            for (int x = -1; x <= 1; x++)
                                            {
                                                for (int y = -1; y <= 1; y++)
                                                {
                                                    Int2d targetTilePosition = mouseTilePosition + new Int2d(x, y);

                                                    if (targetTilePosition.X >= 0 && targetTilePosition.X < soup.Settings.SoupSizeX && targetTilePosition.Y >= 0 && targetTilePosition.Y < soup.Settings.SoupSizeY)
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
                                                                        _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Plant, targetPlant.Index, targetPlant.Id);
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
                                                                        _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Animal, targetAnimal.Index, targetAnimal.Id);
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
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
                                        {
                                            Tile targetTile = soup.Tiles[mouseTileIndex];

                                            if (targetTile.AnimalPopulation > 0)
                                            {
                                                Animal? targetAnimal = soup.Animals[targetTile.AnimalIndexes[targetTile.AnimalIndexes.Count - 1]];

                                                if (targetAnimal is not null)
                                                {
                                                    _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Animal, targetAnimal.Index, targetAnimal.Id);
                                                }
                                            }
                                            else if (targetTile.PlantPopulation > 0)
                                            {
                                                Plant? targetPlant = soup.Plants[targetTile.PlantIndexes[targetTile.PlantIndexes.Count - 1]];

                                                if (targetPlant is not null)
                                                {
                                                    _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Plant, targetPlant.Index, targetPlant.Id);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }

                                if (!_formObjectEditor.IsDisposed)
                                {
                                    if (_formObjectEditor.Visible)
                                    {
                                        _formObjectEditor.LoadSoupObject(_selectedSoupObject);
                                    }
                                }

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
                                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
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
                Double2d mousePositionInSoup = new Double2d((e.X - SoupView.Width / 2d) * _unitPerPixel + _cameraPosition.X, (e.Y - SoupView.Height / 2d) * _unitPerPixel + _cameraPosition.Y);
                Int2d mouseTilePosition = soup.GetTilePositionFromPosition(mousePositionInSoup);
                int mouseTileIndex = soup.GetTileIndexFromTilePosition(mouseTilePosition);

                if (_selectedSoupObject.ObjectType == SoupObjectType.None || _selectedSoupObject.ObjectType == SoupObjectType.Tile)
                {
                    _selectedSoupObject = new SoupObjectPointer();

                    if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
                    {
                        _selectedSoupObject = new SoupObjectPointer(SoupObjectType.Tile, mouseTileIndex, -1);
                    }
                }

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

                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            switch (ModifierKeys)
                            {
                                case Keys.Shift:    // セルを移動する
                                    MoveSelectedCell(mousePositionInSoup);
                                    break;
                                case Keys.Control:
                                    break;
                                case Keys.Alt:      // 壁を作成する
                                    if (soup.SoupState == SoupState.Pause)
                                    {
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
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
                                        if (mousePositionInSoup.X >= 0d && mousePositionInSoup.X <= soup.Settings.SoupSizeX && mousePositionInSoup.Y >= 0d && mousePositionInSoup.Y <= soup.Settings.SoupSizeY)
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

        private void TopMenu_Edit_CopySelectedCell_Click(object sender, EventArgs e)
        {
            CopySelectedCell();
        }
        private void TopMenu_Edit_CutSelectedCell_Click(object sender, EventArgs e)
        {
            CutSelectedCell();
        }
        private void TopMenu_Edit_PasteSelectedCell_Click(object sender, EventArgs e)
        {
            PasteClipboardCell(_cameraPosition);
        }
        private void TopMenu_Edit_KillSelectedCell_Click(object sender, EventArgs e)
        {
            KillSelectedCell();
        }
        private void TopMenu_Edit_MoveSelectedCell_Click(object sender, EventArgs e)
        {
            MoveSelectedCell(_cameraPosition);
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

        private void TopMenu_View_ToggleOverlays_AllOverlays_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                ChangeOverlayToggles(OverlayToggles.AllOverlays);
            }
        }

        private void TopMenu_View_ToggleOverlays_SelectedObject_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                ChangeOverlayToggles(OverlayToggles.SelectedObject);
            }
        }

        private void TopMenu_View_ToggleOverlays_BrainDiagram_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                ChangeOverlayToggles(OverlayToggles.AnimalBrainDiagram);
            }
        }

        private void TopMenu_View_ToggleOverlays_BrainInputOutput_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                ChangeOverlayToggles(OverlayToggles.AnimalBrainInputOutput);
            }
        }

        private void TopMenu_View_ToggleOverlays_FullScreenOverlay_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                ChangeOverlayToggles(OverlayToggles.FullScreenOverlay);
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

        private void TopMenu_View_ToggleFullScreen_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void TopMenu_Window_AutosaveSettings_Click(object sender, EventArgs e)
        {
            ShowFormAutosaveSettings();
        }

        private void TopMenu_Window_Statistics_Click(object sender, EventArgs e)
        {
            ShowFormStatistics();
        }

        private void TopMenuWindowObjectEditor_Click(object sender, EventArgs e)
        {
            ShowFormObjectEditor();
        }

        private void TopMenu_Help_AboutParamecium_Click(object sender, EventArgs e)
        {
            ShowFormAboutParamecium();
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
                    _selectedSoupObject = new SoupObjectPointer();
                    soup.SetSoupState(SoupState.Stop);
                }

                Globals.Soup = new Soup(soupSettingsSetter.SoupSettings);
                Globals.Soup.StartSoupThread();

                Globals.SoupFilePath = string.Empty;
                Globals.SoupFileName = "untitled.soup";

                _cameraPosition = new Double2d(Globals.Soup.Settings.SoupSizeX / 2d, Globals.Soup.Settings.SoupSizeY / 2d);
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
                        _selectedSoupObject = new SoupObjectPointer();
                        soup.SetSoupState(soupState);
                        return;
                    }
                }
            }

            if (LoadSoupDialog.ShowDialog() == DialogResult.OK)
            {
                Soup? loadedSoup = JsonImportAndExport.FileImport<Soup>(LoadSoupDialog.FileName);

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

                    LoadSoupDialog.FileName = Globals.SoupFileName;
                    SaveSoupDialog.InitialDirectory = Path.GetDirectoryName(Globals.SoupFilePath);
                    SaveSoupDialog.FileName = Globals.SoupFileName;

                    _cameraPosition = new Double2d(Globals.Soup.Settings.SoupSizeX / 2d, Globals.Soup.Settings.SoupSizeY / 2d);
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
                    JsonImportAndExport.FileExport(Globals.SoupFilePath, soup);

                    soup.SetSoupState(soupState);
                    return true;
                }
                else if (SaveSoupDialog.ShowDialog() == DialogResult.OK)
                {
                    soup.Modified = false;
                    JsonImportAndExport.FileExport(SaveSoupDialog.FileName, soup);

                    Globals.SoupFilePath = SaveSoupDialog.FileName;
                    Globals.SoupFileName = Path.GetFileName(SaveSoupDialog.FileName);

                    LoadSoupDialog.InitialDirectory = Path.GetDirectoryName(Globals.SoupFilePath);
                    LoadSoupDialog.FileName = Globals.SoupFileName;
                    SaveSoupDialog.FileName = Globals.SoupFileName;

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
                    soup.Modified = false;
                    JsonImportAndExport.FileExport(SaveSoupDialog.FileName, soup);

                    Globals.SoupFilePath = SaveSoupDialog.FileName;
                    Globals.SoupFileName = Path.GetFileName(SaveSoupDialog.FileName);

                    LoadSoupDialog.InitialDirectory = Path.GetDirectoryName(Globals.SoupFilePath);
                    LoadSoupDialog.FileName = Globals.SoupFileName;
                    SaveSoupDialog.FileName = Globals.SoupFileName;

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

        private void CopySelectedCell()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause)
                {
                    if (_selectedSoupObject.ObjectType == SoupObjectType.Plant || _selectedSoupObject.ObjectType == SoupObjectType.Animal)
                    {
                        object? soupObject = _selectedSoupObject.GetSoupObject();

                        if (soupObject is not null)
                        {
                            Clipboard.SetText($"{_selectedSoupObject.ObjectType.ToString()}/{JsonImportAndExport.JsonExport(soupObject)}");
                        }
                    }
                }
            }
        }
        private void CutSelectedCell()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause)
                {
                    if (_selectedSoupObject.ObjectType == SoupObjectType.Plant || _selectedSoupObject.ObjectType == SoupObjectType.Animal)
                    {
                        object? soupObject = _selectedSoupObject.GetSoupObject();

                        if (soupObject is not null)
                        {
                            Clipboard.SetText($"{_selectedSoupObject.ObjectType.ToString()}/{JsonImportAndExport.JsonExport(soupObject)}");

                            if (_selectedSoupObject.ObjectType == SoupObjectType.Plant) soup.RemovePlant(_selectedSoupObject.ObjectIndex);
                            else if (_selectedSoupObject.ObjectType == SoupObjectType.Animal) soup.RemoveAnimal(_selectedSoupObject.ObjectIndex);

                            soup.Modified = true;
                        }
                    }
                }
            }
        }
        private void PasteClipboardCell(Double2d position)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause)
                {
                    string soupObjectJsonText = Clipboard.GetText();

                    if (soupObjectJsonText.StartsWith($"{SoupObjectType.Plant.ToString()}"))
                    {
                        Plant? soupObject = JsonImportAndExport.JsonImport<Plant>(soupObjectJsonText.Replace($"{SoupObjectType.Plant.ToString()}/", ""));

                        if (soupObject is not null)
                        {
                            soupObject.Position = position;
                            soup.AddPlant(soupObject);

                            soup.Modified = true;
                        }
                    }
                    if (soupObjectJsonText.StartsWith($"{SoupObjectType.Animal.ToString()}"))
                    {
                        Animal? soupObject = JsonImportAndExport.JsonImport<Animal>(soupObjectJsonText.Replace($"{SoupObjectType.Animal.ToString()}/", ""));

                        if (soupObject is not null)
                        {
                            soupObject.Position = position;
                            soup.AddAnimal(soupObject);

                            soup.Modified = true;
                        }
                    }
                }
            }
        }
        private void KillSelectedCell()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause)
                {
                    if (_selectedSoupObject.ObjectType == SoupObjectType.Plant || _selectedSoupObject.ObjectType == SoupObjectType.Animal)
                    {
                        object? soupObject = _selectedSoupObject.GetSoupObject();

                        if (soupObject is not null)
                        {
                            if (_selectedSoupObject.ObjectType == SoupObjectType.Plant)
                            {
                                ((Plant)soupObject).IsAlive = false;
                                ((Plant)soupObject).IsNotAlive(soup, soup.Settings);
                                soup.RemovePlant(_selectedSoupObject.ObjectIndex);

                                soup.Modified = true;
                            }
                            else if (_selectedSoupObject.ObjectType == SoupObjectType.Animal)
                            {
                                ((Animal)soupObject).IsAlive = false;
                                ((Animal)soupObject).IsNotAlive(soup, soup.Settings);
                                soup.RemoveAnimal(_selectedSoupObject.ObjectIndex);

                                soup.Modified = true;
                            }
                        }
                    }
                }
            }
        }
        private void MoveSelectedCell(Double2d position)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (soup.SoupState == SoupState.Pause)
                {
                    if (_selectedSoupObject.ObjectType == SoupObjectType.Plant)
                    {
                        Plant? targetPlant = (Plant?)_selectedSoupObject.GetSoupObject();

                        if (targetPlant is not null)
                        {
                            targetPlant.Position = position;

                            soup.Tiles[targetPlant.TileIndex].PlantIndexes.Remove(targetPlant.Index);
                            targetPlant.TileIndex = soup.GetTileIndexFromPosition(position);
                            soup.Tiles[targetPlant.TileIndex].PlantIndexes.Add(targetPlant.Index);

                            soup.Modified = true;
                        }
                    }
                    if (_selectedSoupObject.ObjectType == SoupObjectType.Animal)
                    {
                        Animal? targetAnimal = (Animal?)_selectedSoupObject.GetSoupObject();

                        if (targetAnimal is not null)
                        {
                            targetAnimal.Position = position;

                            soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Remove(targetAnimal.Index);
                            targetAnimal.TileIndex = soup.GetTileIndexFromPosition(position);
                            soup.Tiles[targetAnimal.TileIndex].AnimalIndexes.Add(targetAnimal.Index);

                            soup.Modified = true;
                        }
                    }
                }
            }
        }

        private void ResetCameraPosition()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                _cameraPosition = new Double2d(soup.Settings.SoupSizeX / 2d, soup.Settings.SoupSizeY / 2d);
                _cameraZoomLevel = 0;
                UpdateUnitPerPixel();
            }
        }

        private void ShowFormAutosaveSettings()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_formAutosaveSettings.IsDisposed) _formAutosaveSettings = new FormAutosaveSettings();

                SoupState soupState = soup.SoupState;
                soup.SetSoupState(SoupState.Pause);

                _formAutosaveSettings.ShowDialog();

                soup.SetSoupState(soupState);
            }
        }

        private void ShowFormStatistics()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_formStatistics.IsDisposed) _formStatistics = new FormStatistics();

                if (!_formStatistics.Visible)
                {
                    _formStatistics.Show(this);
                }
                else
                {
                    _formStatistics.Focus();
                }
            }
        }

        private void ShowFormObjectEditor()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (_formObjectEditor.IsDisposed) _formObjectEditor = new FormObjectEditor();

                if (!_formObjectEditor.Visible)
                {
                    _formObjectEditor.Show(this);
                    _formObjectEditor.LoadSoupObject(_selectedSoupObject);
                }
                else
                {
                    _formObjectEditor.Focus();
                }
            }
        }

        private void ShowFormAboutParamecium()
        {
            if (_formAboutParamecium.IsDisposed) _formAboutParamecium = new FormAboutParamecium();

            if (!_formAboutParamecium.Visible) _formAboutParamecium.Show(this);
            else _formAboutParamecium.Focus();
        }

        private void ToggleFullScreen()
        {
            if (!_isFullScreen)
            {
                _isFullScreen = true;

                TopMenu.Hide();
                BottomStat.Hide();

                SoupView.Location = new Point(0, 0);
                SoupView.Size = ClientSize;

                FormBorderStyle = FormBorderStyle.None;

                _prevWindowState = WindowState;
                _prevClientSize = ClientSize;

                WindowState = FormWindowState.Normal;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                _isFullScreen = false;

                TopMenu.Show();
                BottomStat.Show();

                SoupView.Location = new Point(0, 24);
                SoupView.Size = new Size(ClientSize.Width, ClientSize.Height - 48);

                FormBorderStyle = FormBorderStyle.Sizable;

                WindowState = FormWindowState.Normal;
                WindowState = _prevWindowState;

                ClientSize = _prevClientSize;
            }
        }

        private void ChangeOverlayToggles(OverlayToggles toggles)
        {
            _overlayToggles ^= toggles;
        }

        private void UpdateUnitPerPixel()
        {
            _unitPerPixel = 1d / Math.Pow(2, _cameraZoomLevel);
        }
    }
}
