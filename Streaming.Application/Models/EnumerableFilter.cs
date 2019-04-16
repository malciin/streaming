using System.Collections.Generic;

namespace Streaming.Application.Models
{
    public delegate IEnumerable<T> EnumerableFilter<T>(IEnumerable<T> input);
}