namespace Paramecium.Engine
{
    public struct Double2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static readonly Double2d Zero = new Double2d(0d, 0d);

        public double LengthSquared { get => MagnitudeSquared(this); }
        public double Length { get => Magnitude(this); }
        public Double2d Normalized { get => Normalize(this); }

        public Double2d()
        {
            X = 0d;
            Y = 0d;
        }
        public Double2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Double2d(Double2d value)
        {
            X = value.X;
            Y = value.Y;
        }

        public static Double2d operator +(Double2d left, Double2d right)
        {
            return new Double2d(left.X + right.X, left.Y + right.Y);
        }
        public static Double2d operator +(double left, Double2d right)
        {
            return new Double2d(left + right.X, left + right.Y);
        }
        public static Double2d operator +(Double2d left, double right)
        {
            return new Double2d(left.X + right, left.Y + right);
        }

        public static Double2d operator -(Double2d value)
        {
            return new Double2d(0d - value.X, 0d - value.Y);
        }
        public static Double2d operator -(Double2d left, Double2d right)
        {
            return new Double2d(left.X - right.X, left.Y - right.Y);
        }
        public static Double2d operator -(double left, Double2d right)
        {
            return new Double2d(left - right.X, left - right.Y);
        }
        public static Double2d operator -(Double2d left, double right)
        {
            return new Double2d(left.X - right, left.Y - right);
        }

        public static Double2d operator *(double left, Double2d right)
        {
            return new Double2d(left * right.X, left * right.Y);
        }
        public static Double2d operator *(Double2d left, double right)
        {
            return new Double2d(left.X * right, left.Y * right);
        }

        public static Double2d operator /(double left, Double2d right)
        {
            return new Double2d(left / right.X, left / right.Y);
        }
        public static Double2d operator /(Double2d left, double right)
        {
            return new Double2d(left.X / right, left.Y / right);
        }

        public override bool Equals(object? value)
        {
            if (value is null || value is not Double2d) return false;
            return X == ((Double2d)value).X && Y == ((Double2d)value).Y;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Double2d left, Double2d right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Double2d left, Double2d right)
        {
            return !left.Equals(right);
        }

        public static double DistanceSquared(Double2d left, Double2d right)
        {
            return (left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y);
        }
        public static double Distance(Double2d left, Double2d right)
        {
            return Math.Sqrt(DistanceSquared(left, right));
        }

        public static double MagnitudeSquared(Double2d value)
        {
            return value.X * value.X + value.Y * value.Y;
        }
        public static double Magnitude(Double2d value)
        {
            return Math.Sqrt(MagnitudeSquared(value));
        }

        public static Double2d Normalize(Double2d value)
        {
            if (Magnitude(value) > 0) return value / Magnitude(value);
            else return Zero;
        }

        public static Double2d FromAngle(double angleNormalized)
        {
            return new Double2d(
                1d * Math.Cos(angleNormalized * Math.Tau),
                1d * Math.Sin(angleNormalized * Math.Tau)
            );
        }
        public static Double2d Rotate(Double2d value, double angleNormalized)
        {
            return new Double2d(
                value.X * Math.Cos(angleNormalized * Math.Tau) - value.Y * Math.Sin(angleNormalized * Math.Tau),
                value.X * Math.Sin(angleNormalized * Math.Tau) + value.Y * Math.Cos(angleNormalized * Math.Tau)
            );
        }
        public static double ToAngle(Double2d value)
        {
            double result = Math.Atan2(value.Y, value.X) / Math.Tau;

            if (result >= 0) return result;
            else return result + 1;
        }
    }
}