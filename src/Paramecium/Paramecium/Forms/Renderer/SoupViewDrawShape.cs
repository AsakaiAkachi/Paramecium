using Paramecium.Engine;
using static Paramecium.Forms.Renderer.WorldPosViewPosConversion;

namespace Paramecium.Forms.Renderer
{
    public static class SoupViewDrawShape
    {

        public static void DrawEllipse(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d centerPosition, double radius, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawEllipse(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.X - radius),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.Y - radius),
                (float)(radius * 2d * cameraZoomFactor),
                (float)(radius * 2d * cameraZoomFactor)
            );
            colorPen.Dispose();
        }
        public static void FillEllipse(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d centerPosition, double radius, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillEllipse(
                colorBrush,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.X - radius),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.Y - radius),
                (float)(radius * 2d * cameraZoomFactor),
                (float)(radius * 2d * cameraZoomFactor)
            );
            colorBrush.Dispose();
        }

        public static void DrawRectangle(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawRectangle(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)((endPosition.X - startPosition.X) * cameraZoomFactor),
                (float)((endPosition.Y - startPosition.Y) * cameraZoomFactor)
            );
            colorPen.Dispose();
        }
        public static void FillRectangle(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            SolidBrush colorBrush = new SolidBrush(color);
            targetGraphics.FillRectangle(
                colorBrush,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)((endPosition.X - startPosition.X) * cameraZoomFactor),
                (float)((endPosition.Y - startPosition.Y) * cameraZoomFactor)
            );
            colorBrush.Dispose();
        }

        public static void DrawArc(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d centerPosition, double radius, double startAngle, double sweepAngle, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawArc(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.X - radius),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, centerPosition.Y - radius),
                (float)(radius * 2d * cameraZoomFactor),
                (float)(radius * 2d * cameraZoomFactor),
                (float)startAngle,
                (float)sweepAngle
            );
            colorPen.Dispose();
        }

        public static void DrawLine(in Bitmap targetBitmap, in Graphics targetGraphics, Double2d cameraPosition, double cameraZoomFactor, Double2d startPosition, Double2d endPosition, Color color)
        {
            Pen colorPen = new Pen(color);
            targetGraphics.DrawLine(
                colorPen,
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, startPosition.Y),
                (float)WorldPosToViewPosX(targetBitmap, cameraPosition, cameraZoomFactor, endPosition.X),
                (float)WorldPosToViewPosY(targetBitmap, cameraPosition, cameraZoomFactor, endPosition.Y)
            );
            colorPen.Dispose();
        }

        public static double Lerp(double start, double end, double t)
        {
            return (1 - t) * start + t * end;
        }
        public static Color Lerp(Color start, Color end, double t)
        {
            return Color.FromArgb(
                int.Max(0, int.Min(255, (int)Lerp(start.A, end.A, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.R, end.R, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.G, end.G, t))),
                int.Max(0, int.Min(255, (int)Lerp(start.B, end.B, t)))
            );
        }
    }
}
