using System;
using System.Collections.Generic;
using System.Linq;

namespace Streaming.Common.Extensions
{
    public static class LinqExtensions
    {
        public static bool IsSortedAscending<T, TY>(this IEnumerable<T> collection, Func<T, TY> sortedBy) where TY : IComparable
        {
            if (collection.Count() <= 1)
            {
                return true;
            }

            var prev = collection.First();
            foreach(var next in collection.Skip(1))
            {
                if (sortedBy(prev).CompareTo(sortedBy(next)) > 0)
                {
                    return false;
                }
                prev = next;
            }
            return true;
        }

        public static bool IsSortedDescending<T, TY>(this IEnumerable<T> collection, Func<T, TY> sortedBy)
            where TY : IComparable
        {
            if (collection.Count() <= 1)
            {
                return true;
            }

            var prev = collection.First();
            foreach (var next in collection.Skip(1))
            {
                if (sortedBy(prev).CompareTo(sortedBy(next)) < 0)
                {
                    return false;
                }
                prev = next;
            }
            return true;
        }
    }
}
