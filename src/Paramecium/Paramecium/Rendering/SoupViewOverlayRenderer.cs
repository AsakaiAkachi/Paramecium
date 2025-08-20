using Paramecium.Variables;

namespace Paramecium.Rendering
{
    // SoupViewのオーバーレイを描画するクラス
    public static class SoupViewOverlayRenderer
    {
        public static readonly SolidBrush OverlayBackgroundBrush = new SolidBrush(Color.FromArgb(127, 63, 63, 63));
        public static readonly SolidBrush OverlayTextBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        public static readonly SolidBrush OverlayGaugeColor1 = new SolidBrush(Color.FromArgb(255, 127, 127, 127));
        public static readonly SolidBrush OverlayGaugeColor2 = new SolidBrush(Color.FromArgb(255, 191, 191, 191));

        public static void OverlayDrawInformation(OverlayDrawInfo overlayDrawInfo, Graphics graphics, SolidBrush backgroundBrush, SolidBrush textBrush, string text)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, backgroundBrush);
            OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, textBrush);
            overlayDrawInfo.NextLine();
        }
        public static void OverlayDrawInformation(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Color backgroundColor, Color textColor, string text)
        {
            SolidBrush backgroundBrush = new SolidBrush(backgroundColor);
            SolidBrush textBrush = new SolidBrush(textColor);
            OverlayDrawInformation(overlayDrawInfo, graphics, backgroundBrush, textBrush, text);
            backgroundBrush.Dispose();
            textBrush.Dispose();
        }
        public static void OverlayDrawInformation(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text)
        {
            OverlayDrawInformation(overlayDrawInfo, graphics, OverlayBackgroundBrush, OverlayTextBrush, text);
        }

        public static void OverlayDrawInformationWithGauge(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, SolidBrush gaugeBrush, double gaugeValue)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, OverlayBackgroundBrush);
            OverlayDrawGauge(overlayDrawInfo, graphics, gaugeValue, gaugeBrush);
            OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }
        public static void OverlayDrawInformationWithGauge(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, Color gaugeColor, double gaugeValue)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, OverlayBackgroundBrush);
            OverlayDrawGauge(overlayDrawInfo, graphics, gaugeValue, gaugeColor);
            OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }

        public static void OverlayDrawInformationWith2Gauges(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, SolidBrush gauge1Brush, double gauge1Value, SolidBrush gauge2Brush, double gauge2Value)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, OverlayBackgroundBrush);
            OverlayDrawGauge(overlayDrawInfo, graphics, gauge1Value, gauge1Brush);
            OverlayDrawGauge(overlayDrawInfo, graphics, gauge2Value, gauge2Brush);
            OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }
        public static void OverlayDrawInformationWith2Gauges(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string text, Color gauge1Color, double gauge1Value, Color gauge2Color, double gauge2Value)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, OverlayBackgroundBrush);
            OverlayDrawGauge(overlayDrawInfo, graphics, gauge1Value, gauge1Color);
            OverlayDrawGauge(overlayDrawInfo, graphics, gauge2Value, gauge2Color);
            OverlayDrawString(overlayDrawInfo, graphics, "MS UI Gothic", 12, text, OverlayTextBrush);
            overlayDrawInfo.NextLine();
        }



        public static void OverlayFillRectangle(OverlayDrawInfo overlayDrawInfo, Graphics graphics, SolidBrush brush, Int2d position)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            graphics.FillRectangle(brush, itemPosition.X, itemPosition.Y, overlayDrawInfo.ItemSize.X, overlayDrawInfo.ItemSize.Y);
        }
        public static void OverlayFillRectangle(OverlayDrawInfo overlayDrawInfo, Graphics graphics, SolidBrush brush)
        {
            OverlayFillRectangle(overlayDrawInfo, graphics, brush, Int2d.Zero);
        }
        public static void OverlayFillRectangle(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Color color)
        {
            SolidBrush brush = new SolidBrush(color);
            OverlayFillRectangle(overlayDrawInfo, graphics, brush);
            brush.Dispose();
        }

        public static void OverlayDrawEllipse(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Pen pen, Int2d position, int radius)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            graphics.DrawEllipse(pen, itemPosition.X + position.X - radius, itemPosition.Y + position.Y - radius, radius * 2 + 1, radius * 2 + 1);
        }
        public static void OverlayDrawEllipse(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Color color, Int2d position, int radius)
        {
            Pen pen = new Pen(color);
            OverlayDrawEllipse(overlayDrawInfo, graphics, pen, position, radius);
            pen.Dispose();
        }

        public static void OverlayFillEllipse(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Brush brush, Int2d position, int radius)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            graphics.FillEllipse(brush, itemPosition.X + position.X - radius, itemPosition.Y + position.Y - radius, radius * 2 + 1, radius * 2 + 1);
        }
        public static void OverlayFillEllipse(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Color color, Int2d position, int radius)
        {
            SolidBrush brush = new SolidBrush(color);
            OverlayFillEllipse(overlayDrawInfo, graphics, brush, position, radius);
            brush.Dispose();
        }

        public static void OverlayDrawGauge(OverlayDrawInfo overlayDrawInfo, Graphics graphics, double value, SolidBrush brush, Int2d position)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            graphics.FillRectangle(brush, itemPosition.X + position.X, itemPosition.Y + position.Y, (int)(overlayDrawInfo.ItemSize.X * double.Max(0d, double.Min(1d, value))), overlayDrawInfo.ItemSize.Y);
        }
        public static void OverlayDrawGauge(OverlayDrawInfo overlayDrawInfo, Graphics graphics, double value, SolidBrush brush)
        {
            OverlayDrawGauge(overlayDrawInfo, graphics, value, brush, Int2d.Zero);
        }
        public static void OverlayDrawGauge(OverlayDrawInfo overlayDrawInfo, Graphics graphics, double value, Color color)
        {
            SolidBrush brush = new SolidBrush(color);
            OverlayDrawGauge(overlayDrawInfo, graphics, value, brush);
            brush.Dispose();
        }

        public static void OverlayDrawLine(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Pen pen, Int2d startPosition, Int2d endPosition)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            graphics.DrawLine(pen, itemPosition.X + startPosition.X, itemPosition.Y + startPosition.Y, itemPosition.X + endPosition.X, itemPosition.Y + endPosition.Y);
        }
        public static void OverlayDrawLine(OverlayDrawInfo overlayDrawInfo, Graphics graphics, Color color, Int2d startPosition, Int2d endPosition)
        {
            Pen pen = new Pen(color);
            OverlayDrawLine(overlayDrawInfo, graphics, pen, startPosition, endPosition);
            pen.Dispose();
        }

        public static void OverlayDrawString(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string fontName, int size, string text, SolidBrush brush, Int2d position)
        {
            Int2d itemPosition = overlayDrawInfo.GetItemPosition();

            Font fnt = new Font(fontName, size);
            graphics.DrawString(text, fnt, brush, itemPosition.X + position.X, itemPosition.Y + position.Y);
            fnt.Dispose();
        }
        public static void OverlayDrawString(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string fontName, int size, string text, SolidBrush brush)
        {
            OverlayDrawString(overlayDrawInfo, graphics, fontName, size, text, brush, Int2d.Zero);
        }
        public static void OverlayDrawString(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string fontName, int size, string text, Color color, Int2d position)
        {
            SolidBrush brush = new SolidBrush(color);
            OverlayDrawString(overlayDrawInfo, graphics, fontName, size, text, brush, position);
            brush.Dispose();
        }
        public static void OverlayDrawString(OverlayDrawInfo overlayDrawInfo, Graphics graphics, string fontName, int size, string text, Color color)
        {
            SolidBrush brush = new SolidBrush(color);
            OverlayDrawString(overlayDrawInfo, graphics, fontName, size, text, brush);
            brush.Dispose();
        }

        public class OverlayDrawInfo
        {
            public OverlayOrigin Origin;
            public Int2d OriginOffset;

            public Int2d ImageSize;

            public Int2d ItemOffset;
            public Int2d ItemSize;

            public OverlayDrawInfo(OverlayOrigin origin, Int2d originOffset, Int2d imageSize, Int2d itemSize)
            {
                Origin = origin;
                OriginOffset = originOffset;
                ImageSize = imageSize;
                ItemOffset = originOffset;
                ItemSize = itemSize;
            }

            public Int2d GetItemPosition()
            {
                if (Origin == OverlayOrigin.UpperLeft) return OriginOffset + ItemOffset;
                else if (Origin == OverlayOrigin.LowerLeft) return new Int2d(OriginOffset.X + ItemOffset.X, ImageSize.Y - OriginOffset.Y - ItemOffset.Y - ItemSize.Y);
                else if (Origin == OverlayOrigin.UpperRight) return new Int2d(ImageSize.X - OriginOffset.X - ItemOffset.X - ItemSize.X, ItemOffset.Y);
                else if (Origin == OverlayOrigin.LowerRight) return new Int2d(ImageSize.X - OriginOffset.X - ItemOffset.X - ItemSize.X, ImageSize.Y - OriginOffset.Y - ItemOffset.Y - ItemSize.Y);
                else return ItemOffset;
            }

            public void NextLine()
            {
                ItemOffset = new Int2d(ItemOffset.X, ItemOffset.Y + ItemSize.Y);
            }
            public void NextColumn()
            {
                ItemOffset = new Int2d(ItemOffset.X + ItemSize.X, OriginOffset.Y);
            }
            public void ResetOffset()
            {
                ItemOffset = Int2d.Zero;
            }
        }

        public enum OverlayOrigin
        {
            UpperLeft,
            LowerLeft,
            UpperRight,
            LowerRight
        }
    }
}
