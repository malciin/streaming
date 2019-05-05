using System;

namespace Streaming.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Return true if given dateTime contains within certain dates
        /// </summary>
        public static bool IsWithin(this DateTime value, DateTime first, DateTime second)
        {
            return first.Ticks <= value.Ticks && value.Ticks <= second.Ticks;
        }
    }
}