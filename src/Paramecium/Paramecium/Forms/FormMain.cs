using Paramecium.Forms.Renderer;
using Paramecium.Engine;
using System.Diagnostics;

namespace Paramecium.Forms
{
    public partial class FormMain : Form
    {
        Bitmap SoupViewCanvas;

        Double2d CameraPosition;
        int ZoomLevel;

        double FramesPerSecond;

        public FormMain()
        {
            InitializeComponent();
            SoupView.MouseWheel += SoupView_MouseWheel;

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

                    SoupViewCanvas = new Bitmap(SoupView.Width, SoupView.Height);
                    SoupViewRenderer.DrawSoupView(ref SoupViewCanvas, CameraPosition, ZoomLevel);

                    SoupView.Image = SoupViewCanvas;

                    sw.Stop();
                    FramesPerSecond *= 0.96d;
                    FramesPerSecond += 1000000d / sw.Elapsed.TotalMicroseconds * 0.04d;
                }

                BottomStat_SoupState.Text = $"Status : {g_Soup.SoupState}";
                BottomStat_ElapsedTimeSteps.Text = $"Time Step : {g_Soup.ElapsedTimeSteps} (T{g_Soup.ThreadCount})";
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
    }
}
