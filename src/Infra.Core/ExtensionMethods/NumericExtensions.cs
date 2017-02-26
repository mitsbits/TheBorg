using System;

namespace Borg
{
    public static class NumericExtensions
    {
        public static string ToFileSizeDisplay(this int i)
        {
            return ToFileSizeDisplay((long)i, 2);
        }

        public static string ToFileSizeDisplay(this int i, int decimals)
        {
            return ToFileSizeDisplay((long)i, decimals);
        }

        public static string ToFileSizeDisplay(this long i)
        {
            return ToFileSizeDisplay(i, 2);
        }

        public static string ToFileSizeDisplay(this long i, int decimals)
        {
            if (i < 1024 * 1024) // 1 MB
            {
                string value = Math.Round((decimal)i / 1024m, decimals).ToString("N" + decimals);
                if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                    value = value.Substring(0, value.Length - decimals - 1);

                return String.Concat(value, " B");
            }
            else
            {

                if (i < 1024 * 1024 * 1024) // 1 GB
                {
                    string value = Math.Round((decimal)i / 1024m / 1024m, decimals).ToString("N" + decimals);
                    if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                        value = value.Substring(0, value.Length - decimals - 1);

                    return String.Concat(value, " MB");
                }
                else
                {
                    string value = Math.Round((decimal)i / 1024m / 1024m / 1024m, decimals).ToString("N" + decimals);
                    if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                        value = value.Substring(0, value.Length - decimals - 1);

                    return String.Concat(value, " GB");
                }
            }
        }

        public static string ToOrdinal(this int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString("#,###0") + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num.ToString("#,###0") + "st";

                case 2:
                    return num.ToString("#,###0") + "nd";

                case 3:
                    return num.ToString("#,###0") + "rd";

                default:
                    return num.ToString("#,###0") + "th";
            }
        }
        public static string BytesDisplay(this long byteCount, string format = "{0:0.##} {1}")
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0) return string.Format(format, 0, suf[0]);
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return string.Format(format, (Math.Sign(byteCount) * num), suf[place]);
        }

        public static string BytesDisplay(this int byteCount, string format = "{0:0.##} {1}")
        {
            return ((long)byteCount).BytesDisplay(format);
        }

        public static int RoundOff(this int i, int round = 10)
        {
            if (round <= 0) throw new ArgumentOutOfRangeException(nameof(round));
            return (int)Math.Round(i / (double)round) * round;
        }
    }
}