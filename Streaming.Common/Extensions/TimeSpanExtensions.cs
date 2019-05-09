using System;

namespace Streaming.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static bool IsWithin(this TimeSpan timespan, TimeSpan from, TimeSpan to)
        {
            return from.TotalSeconds <= timespan.TotalSeconds && timespan.TotalSeconds <= to.TotalSeconds;
        }

        /// <summary>
        /// Return true when timespan == expected +- maxError
        /// </summary>
        public static bool EqualWithError(this TimeSpan timespan, TimeSpan expected, TimeSpan maxError)
        {
            return IsWithin(timespan, expected.Subtract(maxError), expected.Add(maxError));
        }
    }
}