namespace Paramecium.Libs
{
    public class Int2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Int2D()
        {
            X = 0;
            Y = 0;
        }
        public Int2D(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Int2D(Int2D value)
        {
            X = value.X;
            Y = value.Y;
        }

        public static Int2D operator +(Int2D left, Int2D right)
        {
            return new Int2D(left.X + right.X, left.Y + right.Y);
        }
        public static Int2D operator /(Int2D left, int right)
        {
            return new Int2D(left.X / right, left.Y / right);
        }

        public static bool operator ==(Int2D left, Int2D right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return (left.X == right.X) && (left.Y == right.Y);
        }
        public static bool operator !=(Int2D left, Int2D right)
        {
            return !(left == right);
        }
        public override bool Equals(object? value)
        {
            if (value == null || this.GetType() != value.GetType())
            {
                return false;
            }

            return (this == (Int2D)value);
        }
        public override int GetHashCode()
        {
            return X + Y;
        }
    }
}
