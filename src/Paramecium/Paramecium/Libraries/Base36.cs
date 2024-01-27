global using static Paramecium.Libraries.Base36;

namespace Paramecium.Libraries
{
    public static class Base36
    {
        static string Base36Str = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string LongToBase36(long value)
        {
            if (value >= 0)
            {
                if (value < 36)
                {
                    return $"{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 1296)
                {
                    return $"{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 46656)
                {
                    return $"{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 1679616)
                {
                    return $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 60466176)
                {
                    return $"{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 2176782336)
                {
                    return $"{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 78364164096)
                {
                    return $"{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 2821109907456)
                {
                    return $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 101559956668416)
                {
                    return $"{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 3656158440062976)
                {
                    return $"{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 131621703842267136)
                {
                    return $"{Base36Str[(int)(value / 3656158440062976 % 36)]}{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else
                {
                    return $"{Base36Str[(int)(value / 131621703842267136 % 36)]}{Base36Str[(int)(value / 3656158440062976 % 36)]}{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
            }
            else return String.Empty;
        }

        public static string LongToBase36(long value, int length)
        {
            if (value >= 0)
            {
                if (value < 36 && length == 1)
                {
                    return $"{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 1296 && length == 2)
                {
                    return $"{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 46656 && length == 3)
                {
                    return $"{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 1679616 && length == 4)
                {
                    return $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 60466176 && length == 5)
                {
                    return $"{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 2176782336 && length == 6)
                {
                    return $"{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 78364164096 && length == 7)
                {
                    return $"{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 2821109907456 && length == 8)
                {
                    return $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 101559956668416 && length == 9)
                {
                    return $"{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 3656158440062976 && length == 10)
                {
                    return $"{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (value < 131621703842267136 && length == 11)
                {
                    return $"{Base36Str[(int)(value / 3656158440062976 % 36)]}{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else if (length == 12)
                {
                    return $"{Base36Str[(int)(value / 131621703842267136 % 36)]}{Base36Str[(int)(value / 3656158440062976 % 36)]}{Base36Str[(int)(value / 101559956668416 % 36)]}{Base36Str[(int)(value / 2821109907456 % 36)]}" +
                        $"{Base36Str[(int)(value / 78364164096 % 36)]}{Base36Str[(int)(value / 2176782336 % 36)]}{Base36Str[(int)(value / 60466176 % 36)]}{Base36Str[(int)(value / 1679616 % 36)]}" +
                        $"{Base36Str[(int)(value / 46656 % 36)]}{Base36Str[(int)(value / 1296 % 36)]}{Base36Str[(int)(value / 36 % 36)]}{Base36Str[(int)(value % 36)]}";
                }
                else return String.Empty;
            }
            else return String.Empty;
        }
    }
}
