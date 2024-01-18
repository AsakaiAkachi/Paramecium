namespace Paramecium.Libs
{
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D()
        {
            X = 0;
            Y = 0;
        }
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Vector2D(Random random, double xMin, double yMin, double xMax, double yMax)
        {
            X = random.NextDouble() * (xMax - xMin) + xMin;
            Y = random.NextDouble() * (yMax - xMin) + yMin;
        }

        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X - right.X, left.Y - right.Y);
        }
        public static Vector2D operator *(Vector2D left, double right)
        {
            return new Vector2D(left.X * right, left.Y * right);
        }
        public static Vector2D operator /(Vector2D left, double right)
        {
            return new Vector2D(left.X / right, left.Y / right);
        }

        public static bool operator ==(Vector2D left, Vector2D right)
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
        public static bool operator !=(Vector2D left, Vector2D right)
        {
            return !(left == right);
        }
        public override bool Equals(object? value)
        {
            if (value == null || this.GetType() != value.GetType())
            {
                return false;
            }

            return (this == (Vector2D)value);
        }
        public override int GetHashCode()
        {
            return (int)X + (int)Y;
        }

        public static double Distance(Vector2D left, Vector2D right)
        {
            return Math.Sqrt(Math.Pow(left.X - right.X, 2) + Math.Pow(left.Y - right.Y, 2));
        }
        public static double Size(Vector2D value)
        {
            return Math.Sqrt(Math.Pow(0 - value.X, 2) + Math.Pow(0 - value.Y, 2));
        }
        public static Vector2D Normalization(Vector2D value)
        {
            double valueSize = Vector2D.Size(value);
            if (valueSize != 0d)
            {
                return value /= valueSize;
            }
            else
            {
                return new Vector2D();
            }
        }

        public static Vector2D FromAngle(double angle)
        {
            return new Vector2D(
                    1d * Math.Cos(angle * (Math.PI / 180d)),
                    1d * Math.Sin(angle * (Math.PI / 180d))
                );
        }
        public static Vector2D Rotate(Vector2D value, double angle)
        {
            return new Vector2D(
                    value.X * Math.Cos(angle * (Math.PI / 180d)) - value.Y * Math.Sin(angle * (Math.PI / 180d)),
                    value.X * Math.Sin(angle * (Math.PI / 180d)) + value.Y * Math.Cos(angle * (Math.PI / 180d))
                );
        }

        public static double ToAngle(Vector2D value)
        {
            double result = Math.Atan2(value.Y, value.X) * (180d / Math.PI);
            if (result < 0) result = 360d + result;
            return result;
        }

        public static Int2D ToGridPosition(Vector2D value)
        {
            Int2D result = new Int2D((int)value.X, (int)value.Y);

            if (result.X < 0) result.X = 0;
            if (result.X >= Variables.SoupInstance.env_SizeX) result.X = Variables.SoupInstance.env_SizeX - 1;
            if (result.Y < 0) result.Y = 0;
            if (result.Y >= Variables.SoupInstance.env_SizeY) result.Y = Variables.SoupInstance.env_SizeY - 1;

            return result;
        }
    }
}
