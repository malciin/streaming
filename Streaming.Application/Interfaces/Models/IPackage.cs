using System;
using System.Collections.Generic;

namespace Streaming.Application.Interfaces.Models
{
    public interface IPackage<T> where T : class
    {
        IEnumerable<T> Items { get; }
        PackageDetails Details { get; }

        IPackage<TOut> Map<TOut>(Func<T, TOut> mapStrategy) where TOut : class;
    }

    public class PackageDetails
    {
        public int TotalCount { get; set; }
        public int Count { get; set; }
    }
}