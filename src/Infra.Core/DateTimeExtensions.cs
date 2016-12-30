using System;

namespace Borg
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Subtract(d).Ticks + d.Ticks - 1) / d.Ticks * d.Ticks);
        }
    }
}