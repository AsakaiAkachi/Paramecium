namespace Paramecium.Libs
{
    public class ColorInt3
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public ColorInt3()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
        }
        public ColorInt3(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
        public ColorInt3(Random random)
        {
            Red = random.Next(0, 256);
            Green = random.Next(0, 256);
            Blue = random.Next(0, 256);
        }


        public static implicit operator Color(ColorInt3 value)
        {
            return Color.FromArgb((byte)value.Red, (byte)value.Green, (byte)value.Blue);
        }
    }
}
