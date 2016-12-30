using System;

namespace Borg
{
    public static class IntAndLongExrensions
    {
        public static string BytesToString(this long byteCount, string format = "{0:0.##} {1}")
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0) return string.Format(format, 0, suf[0]);
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return string.Format(format, (Math.Sign(byteCount) * num), suf[place]);
        }

        public static int RoundOff(this int i, int round = 10)
        {
            if (round <= 0) throw new ArgumentOutOfRangeException(nameof(round));
            return (int)Math.Round(i / (double)round) * round;
        }
    }
}