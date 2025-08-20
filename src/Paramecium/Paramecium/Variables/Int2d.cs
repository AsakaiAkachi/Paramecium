using System.Diagnostics.CodeAnalysis;

namespace Paramecium.Variables
{
    // Int型2次元ベクトル、主にタイルの位置の表現や画像の描画に使用
    public struct Int2d
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly Int2d Zero = new Int2d(0, 0);

        public Int2d()
        {
            X = 0;
            Y = 0;
        }
        public Int2d(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Int2d(Int2d value)
        {
            X = value.X;
            Y = value.Y;
        }

        public static Int2d operator +(Int2d left, Int2d right)
        {
            return new Int2d(left.X + right.X, left.Y + right.Y);
        }

        public static Int2d operator -(Int2d value)
        {
            return new Int2d(-value.X, -value.Y);
        }
        public static Int2d operator -(Int2d left, Int2d right)
        {
            return new Int2d(left.X - right.X, left.Y - right.Y);
        }

        public static Int2d operator *(Int2d left, int right)
        {
            return new Int2d(left.X * right, left.Y * right);
        }

        public static Int2d operator /(Int2d left, int right)
        {
            return new Int2d(left.X / right, left.Y / right);
        }

        public static bool operator ==(Int2d left, Int2d right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Int2d left, Int2d right)
        {
            return !left.Equals(right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || !(obj is Int2d)) return false;
            else return X == ((Int2d)obj).X && Y == ((Int2d)obj).Y;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
