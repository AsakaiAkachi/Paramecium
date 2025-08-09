using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paramecium.Variables
{
    public struct Double2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static readonly Double2d Zero = new Double2d(0d, 0d);

        [JsonIgnore]
        public double MagnitudeSquared { get => LengthSquared(this); }
        [JsonIgnore]
        public double Magnitude { get => Length(this); }
        [JsonIgnore]
        public Double2d Normalized { get => Normalize(this); }
        [JsonIgnore]
        public double Angle { get => ToAngle(this); }
        [JsonIgnore]
        public double AngleDegree { get => ToAngleDegree(this); }
        [JsonIgnore]
        public double Angle01 { get => ToAngle01(this); }

        public Double2d()
        {
            X = 0;
            Y = 0;
        }
        public Double2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Double2d(double value)
        {
            X = value;
            Y = value;
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

        public static Double2d operator -(Double2d value)
        {
            return new Double2d(-value.X, -value.Y);
        }
        public static Double2d operator -(Double2d left, Double2d right)
        {
            return new Double2d(left.X - right.X, left.Y - right.Y);
        }

        public static Double2d operator *(Double2d left, double right)
        {
            return new Double2d(left.X * right, left.Y * right);
        }

        public static Double2d operator /(Double2d left, double right)
        {
            return new Double2d(left.X / right, left.Y / right);
        }

        public static bool operator ==(Double2d left, Double2d right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Double2d left, Double2d right)
        {
            return !left.Equals(right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || !(obj is Double2d)) return false;
            else return X == ((Double2d)obj).X && Y == ((Double2d)obj).Y;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static double DistanceSquared(Double2d value1, Double2d value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y);
        }
        public static double Distance(Double2d value1, Double2d value2)
        {
            return Math.Sqrt(DistanceSquared(value1, value2));
        }

        public static double LengthSquared(Double2d value)
        {
            return DistanceSquared(Zero, value);
        }
        public static double Length(Double2d value)
        {
            return Distance(Zero, value);
        }
        
        public static Double2d Normalize(Double2d value)
        {
            return value / Length(value);
        }

        public static Double2d FromAngle(double angle)
        {
            return new Double2d(
                1d * Math.Cos(angle),
                1d * Math.Sin(angle)
            );
        }
        public static Double2d FromAngleDegree(double angle)
        {
            return FromAngle(angle * Math.Tau / 360d);
        }
        public static Double2d FromAngle01(double angle)
        {
            return FromAngle(angle * Math.Tau);
        }

        public static Double2d Rotate(Double2d value, double angle)
        {
            return new Double2d(
                value.X * Math.Cos(angle) - value.Y * Math.Sin(angle),
                value.X * Math.Sin(angle) + value.Y * Math.Cos(angle)
            );
        }
        public static Double2d RotateDegree(Double2d value, double angle)
        {
            return Rotate(value, angle * Math.Tau / 360d);
        }
        public static Double2d Rotate01(Double2d value, double angle)
        {
            return Rotate(value, angle * Math.Tau);
        }

        public static double ToAngle(Double2d value)
        {
            return Math.Atan2(value.Y, value.X);
        }
        public static double ToAngleDegree(Double2d value)
        {
            return ToAngle(value) * 360d / Math.Tau;
        }
        public static double ToAngle01(Double2d value)
        {
            return ToAngle(value) / Math.Tau;
        }


        public static Double2d Lerp(Double2d a, Double2d b, double t)
        {
            return new Double2d(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
        }
    }
}
