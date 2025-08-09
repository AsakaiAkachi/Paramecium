using Paramecium.Engine;
using Paramecium.Variables;
using System.Drawing;

namespace Paramecium.Rendering
{
    public static partial class SoupViewRenderer
    {
        private class SoupViewOverlayRenderer
        {
            public static readonly SolidBrush OverlayBackgroundBrush = new SolidBrush(Color.FromArgb(127, 63, 63, 63));
            public static readonly SolidBrush OverlayTextBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            public static readonly SolidBrush OverlayGaugeColor1 = new SolidBrush(Color.FromArgb(255, 127, 127, 127));
            public static readonly SolidBrush OverlayGaugeColor2 = new SolidBrush(Color.FromArgb(255, 191, 191, 191));

            public int ColumnWidth;
            public int LineHeight;

            public Int2d Offset;

            public void OverlayDrawInformation(Graphics graphics, string text)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush);
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
                NextLine();
            }

            public void OverlayDrawInformationWithGauge(Graphics graphics, string text, SolidBrush gaugeBrush, double gaugeValue)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush);
                OverlayDrawGauge(graphics, gaugeValue, gaugeBrush);
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
                NextLine();
            }
            public void OverlayDrawInformationWithGauge(Graphics graphics, string text, Color gaugeColor, double gaugeValue)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush);
                OverlayDrawGauge(graphics, gaugeValue, gaugeColor);
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
                NextLine();
            }

            public void OverlayDrawInformationWith2Gauges(Graphics graphics, string text, SolidBrush gauge1Brush, double gauge1Value, SolidBrush gauge2Brush, double gauge2Value)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush);
                OverlayDrawGauge(graphics, gauge1Value, gauge1Brush);
                OverlayDrawGauge(graphics, gauge2Value, gauge2Brush);
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
                NextLine();
            }
            public void OverlayDrawInformationWith2Gauges(Graphics graphics, string text, Color gauge1Color, double gauge1Value, Color gauge2Color, double gauge2Value)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush);
                OverlayDrawGauge(graphics, gauge1Value, gauge1Color);
                OverlayDrawGauge(graphics, gauge2Value, gauge2Color);
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
                NextLine();
            }

            public void OverlayDrawAnimalBrainNodeInfomation(Graphics graphics, string text, Double2d nodePotision, ref int offset)
            {
                OverlayFillRectangle(graphics, OverlayBackgroundBrush, new Int2d((int)(nodePotision.X + 7), (int)(nodePotision.Y + 7) + offset), new Int2d(300, 16));
                OverlayDrawString(graphics, "MS UI Gothic", 12, text, OverlayTextBrush, new Int2d((int)(nodePotision.X + 7), (int)(nodePotision.Y + 7) + offset));

                offset += 16;
            }

            public void OverlayDrawAnimalBrainInOutInfomation(Graphics graphics, string nodeName, double value)
            {
                Double4d color = _brainDiagramNodeNeutralOutputColor;
                if (value < 0) color = Double4d.Lerp(color, _brainDiagramNodeNegativeOutputColor, double.Min(1d, -value));
                if (value > 0) color = Double4d.Lerp(color, _brainDiagramNodePositiveOutputColor, double.Min(1d, value));

                OverlayFillEllipse(graphics, (Color)color, new Int2d(8, 8), 7);
                OverlayDrawEllipse(graphics, _cellOutlinePen, new Int2d(8, 8), 7);
                OverlayDrawString(graphics, "MS UI Gothic", 8, $"{nodeName}", OverlayTextBrush, new Int2d(18, 0));
                OverlayDrawString(graphics, "MS UI Gothic", 8, $"{value.ToString("0.000")}", OverlayTextBrush, new Int2d(18, 9));
                NextLine();
            }

            public void NextLine()
            {
                Offset += new Int2d(0, LineHeight);
            }
            public void NextColumn()
            {
                Offset = new Int2d(Offset.X + ColumnWidth, 0);
            }
            public void ResetOffset()
            {
                Offset = Int2d.Zero;
            }

            public void OverlayFillRectangle(Graphics graphics, SolidBrush brush, Int2d position, Int2d size)
            {
                graphics.FillRectangle(brush, Offset.X + position.X, Offset.Y + position.Y, size.X, size.Y);
            }
            public void OverlayFillRectangle(Graphics graphics, SolidBrush brush, Int2d position)
            {
                graphics.FillRectangle(brush, Offset.X, Offset.Y, ColumnWidth, LineHeight);
            }
            public void OverlayFillRectangle(Graphics graphics, SolidBrush brush)
            {
                OverlayFillRectangle(graphics, brush, Int2d.Zero);
            }
            public void OverlayFillRectangle(Graphics graphics, Color color)
            {
                SolidBrush brush = new SolidBrush(color);
                OverlayFillRectangle(graphics, brush);
                brush.Dispose();
            }

            public void OverlayDrawEllipse(Graphics graphics, Pen pen, Int2d position, int radius)
            {
                graphics.DrawEllipse(pen, Offset.X + position.X - radius, Offset.Y + position.Y - radius, radius * 2 + 1, radius * 2 + 1);
            }
            public void OverlayDrawEllipse(Graphics graphics, Color color, Int2d position, int radius)
            {
                Pen pen = new Pen(color);
                OverlayDrawEllipse(graphics, pen, position, radius);
                pen.Dispose();
            }

            public void OverlayFillEllipse(Graphics graphics, Brush brush, Int2d position, int radius)
            {
                graphics.FillEllipse(brush, Offset.X + position.X - radius, Offset.Y + position.Y - radius, radius * 2 + 1, radius * 2 + 1);
            }
            public void OverlayFillEllipse(Graphics graphics, Color color, Int2d position, int radius)
            {
                SolidBrush brush = new SolidBrush(color);
                OverlayFillEllipse(graphics, brush, position, radius);
                brush.Dispose();
            }

            public void OverlayDrawGauge(Graphics graphics, double value, SolidBrush brush, Int2d position)
            {
                graphics.FillRectangle(brush, Offset.X + position.X, Offset.Y + position.Y, (int)(ColumnWidth * double.Max(0d, double.Min(1d, value))), LineHeight);
            }
            public void OverlayDrawGauge(Graphics graphics, double value, SolidBrush brush)
            {
                OverlayDrawGauge(graphics, value, brush, Int2d.Zero);
            }
            public void OverlayDrawGauge(Graphics graphics, double value, Color color)
            {
                SolidBrush brush = new SolidBrush(color);
                OverlayDrawGauge(graphics, value, brush);
                brush.Dispose();
            }

            public void OverlayDrawLine(Graphics graphics, Pen pen, Int2d startPosition, Int2d endPosition)
            {
                graphics.DrawLine(pen, Offset.X + startPosition.X, Offset.Y + startPosition.Y, Offset.X + endPosition.X, Offset.Y + endPosition.Y);
            }
            public void OverlayDrawLine(Graphics graphics, Color color, Int2d startPosition, Int2d endPosition)
            {
                Pen pen = new Pen(color);
                OverlayDrawLine(graphics, pen, startPosition, endPosition);
                pen.Dispose();
            }

            public void OverlayDrawString(Graphics graphics, string fontName, int size, string text, SolidBrush brush, Int2d position)
            {
                Font fnt = new Font(fontName, size);
                graphics.DrawString(text, fnt, brush, Offset.X + position.X, Offset.Y + position.Y);
                fnt.Dispose();
            }
            public void OverlayDrawString(Graphics graphics, string fontName, int size, string text, SolidBrush brush)
            {
                OverlayDrawString(graphics, fontName, size, text, brush, Int2d.Zero);
            }
            public void OverlayDrawString(Graphics graphics, string fontName, int size, string text, Color color, Int2d position)
            {
                SolidBrush brush = new SolidBrush(color);
                OverlayDrawString(graphics, fontName, size, text, brush, position);
                brush.Dispose();
            }
            public void OverlayDrawString(Graphics graphics, string fontName, int size, string text, Color color)
            {
                SolidBrush brush = new SolidBrush(color);
                OverlayDrawString(graphics, fontName, size, text, brush);
                brush.Dispose();
            }

            public static string StringFromCellId(long cellId)
            {
                string cellIdChars = "0123456789abcdefghijklmnopqrstuvwxyz";

                string result = "";

                for (int i = 0; i < 12; i++)
                {
                    result += cellIdChars[(int)(cellId % 36)];
                    cellId /= 36;
                }

                return result;
            }
        }
    }
}
