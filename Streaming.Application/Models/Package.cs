using System;
using System.Collections.Generic;
using System.Linq;
using Streaming.Application.Interfaces.Models;

namespace Streaming.Application.Models
{
    public class Package<T> : IPackage<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public PackageDetails Details { get; set; }

        public static Package<T> CreatePackage(IEnumerable<T> result, int totalCount)
            => CreatePackage(result.ToList(), totalCount);

        public static Package<T> CreatePackage(List<T> result, int totalCount)
        {
            var package = new Package<T>();
            package.Items = result;
            package.Details = new PackageDetails
            {
                TotalCount = totalCount,
                Count = result.Count
            };
            return package;
        }

        public IPackage<TOut> Map<TOut>(Func<T, TOut> mapStrategy) where TOut : class
        {
            var package = new Package<TOut>();
            package.Details = Details;
            package.Items = Items.Select(mapStrategy);
            return package;
        }
    }
}