using Paramecium.Engine;

namespace Paramecium.Forms.Renderer
{
    public static class WorldPosViewPosConversion
    {
        public static double WorldPosToViewPosX(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, double worldPosX)
        {
            return (worldPosX - cameraPosition.X) * cameraZoomFactor + targetBitmap.Width / 2d;
        }
        public static double WorldPosToViewPosX(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, double worldPosX)
        {
            return (worldPosX - cameraPosition.X) * cameraZoomFactor + targetWidth / 2d;
        }
        public static double WorldPosToViewPosY(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, double worldPosY)
        {
            return (worldPosY - cameraPosition.Y) * cameraZoomFactor + targetBitmap.Height / 2d;
        }
        public static double WorldPosToViewPosY(int targetHeight, Double2d cameraPosition, double cameraZoomFactor, double worldPosY)
        {
            return (worldPosY - cameraPosition.Y) * cameraZoomFactor + targetHeight / 2d;
        }

        public static double ViewPosToWorldPosX(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, int viewPosX)
        {
            return (viewPosX - targetBitmap.Width / 2d) / cameraZoomFactor + cameraPosition.X;
        }
        public static double ViewPosToWorldPosX(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, int viewPosX)
        {
            return (viewPosX - targetWidth / 2d) / cameraZoomFactor + cameraPosition.X;
        }
        public static double ViewPosToWorldPosY(in Bitmap targetBitmap, Double2d cameraPosition, double cameraZoomFactor, int viewPosY)
        {
            return (viewPosY - targetBitmap.Height / 2d) / cameraZoomFactor + cameraPosition.Y;
        }
        public static double ViewPosToWorldPosY(int targetWidth, Double2d cameraPosition, double cameraZoomFactor, int viewPosY)
        {
            return (viewPosY - targetWidth / 2d) / cameraZoomFactor + cameraPosition.Y;
        }
    }
}
