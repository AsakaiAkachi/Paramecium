using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paramecium.Variables
{
    public struct Double4d
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public static readonly Double4d Zero = new Double4d();

        [JsonIgnore]
        public double MagnitudeSquared { get => LengthSquared(this); }
        [JsonIgnore]
        public double Magnitude { get => Length(this); }
        [JsonIgnore]
        public Double4d Normalized { get => Normalize(this); }

        public Double4d()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }
        public Double4d(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Double4d(double value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }
        public Double4d(Double4d value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        public static Double4d operator +(Double4d left, Double4d right)
        {
            return new Double4d(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        public static Double4d operator -(Double4d value)
        {
            return new Double4d(-value.X, -value.Y, -value.Z, -value.W);
        }
        public static Double4d operator -(Double4d left, Double4d right)
        {
            return new Double4d(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        public static Double4d operator *(Double4d left, double right)
        {
            return new Double4d(left.X * right, left.Y * right, left.Z * right, left.W * right);
        }

        public static Double4d operator /(Double4d left, double right)
        {
            return new Double4d(left.X / right, left.Y / right, left.Z / right, left.W / right);
        }

        public static bool operator ==(Double4d left, Double4d right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Double4d left, Double4d right)
        {
            return !left.Equals(right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || !(obj is Double4d)) return false;
            else return X == ((Double4d)obj).X && Y == ((Double4d)obj).Y && Z == ((Double4d)obj).Z && W == ((Double4d)obj).W;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static double DistanceSquared(Double4d value1, Double4d value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y) + (value1.Z - value2.Z) * (value1.Z - value2.Z) + (value1.W - value2.W) * (value1.W - value2.W);
        }
        public static double Distance(Double4d value1, Double4d value2)
        {
            return Math.Sqrt(DistanceSquared(value1, value2));
        }

        public static double LengthSquared(Double4d value)
        {
            return DistanceSquared(Zero, value);
        }
        public static double Length(Double4d value)
        {
            return Distance(Zero, value);
        }

        public static Double4d Normalize(Double4d value)
        {
            return value / Length(value);
        }

        public static Double4d Lerp(Double4d a, Double4d b, double t)
        {
            return new Double4d(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y), a.Z + t * (b.Z - a.Z), a.W + t * (b.W - a.W));
        }

        public static explicit operator uint(Double4d value)
        {
            return (uint)Math.Max(0, Math.Min(255, value.X * 255d)) * 16777216u + (uint)Math.Max(0, Math.Min(255, value.Y * 255d)) * 65536u + (uint)Math.Max(0, Math.Min(255, value.Z * 255d)) * 256u + (uint)Math.Max(0, Math.Min(255, value.W * 255d));
        }
        public static explicit operator Color(Double4d value)
        {
            return Color.FromArgb((int)Math.Max(0, Math.Min(255, value.X * 255d)), (int)Math.Max(0, Math.Min(255, value.Y * 255d)), (int)Math.Max(0, Math.Min(255, value.Z * 255d)), (int)Math.Max(0, Math.Min(255, value.W * 255d)));
        }
    }
}
