using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Forms.Renderer
{
    public class OverlayInformationRenderer
    {
        public int OffsetX;
        public int OffsetY;

        Graphics Graphics;

        public OverlayInformationRenderer(Graphics graphics)
        {
            Graphics = graphics;
        }

        public void OverlayDrawRectangle(int startX, int startY, int endX, int endY, Color color)
        {
            Pen colorPen = new Pen(color);
            Graphics.DrawRectangle(colorPen, startX + OffsetX, startY + OffsetY, endX - startX, endY - startY);
            colorPen.Dispose();
        }
        public void OverlayFillRectangle(int startX, int startY, int endX, int endY, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            Graphics.FillRectangle(colorBrush, startX + OffsetX, startY + OffsetY, endX - startX, endY - startY);
            colorBrush.Dispose();
        }

        public void OverlayDrawEllipse(int centerX, int centerY, int diameter, Color color)
        {
            Pen colorPen = new Pen(color);
            Graphics.DrawEllipse(colorPen, centerX - diameter / 2 + OffsetX, centerY - diameter / 2 + OffsetY, diameter, diameter);
            colorPen.Dispose();
        }
        public void OverlayFillEllipse(int centerX, int centerY, int diameter, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            Graphics.FillEllipse(colorBrush, centerX - diameter / 2 + OffsetX, centerY - diameter / 2 + OffsetY, diameter, diameter);
            colorBrush.Dispose();
        }

        public void OverlayDrawLine(int startX, int startY, int endX, int endY, Color color)
        {
            Pen colorPen = new Pen(color);
            Graphics.DrawLine(colorPen, startX + OffsetX, startY + OffsetY, endX + OffsetX, endY + OffsetY);
            colorPen.Dispose();
        }

        public void OverlayDrawString(string fontName, int size, string text, int startX, int startY, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            Font fnt = new Font(fontName, size);
            Graphics.DrawString(text, fnt, colorBrush, startX + OffsetX, startY + OffsetY);
            fnt.Dispose();
            colorBrush.Dispose();
        }
        public SizeF OverlayMeasureString(string fontName, int size, string text)
        {
            Font fnt = new Font(fontName, size);
            SizeF result = Graphics.MeasureString(text, fnt);
            fnt.Dispose();

            return result;
        }
    }
}
