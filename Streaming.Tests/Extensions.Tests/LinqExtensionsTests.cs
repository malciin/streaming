using Streaming.Common.Extensions;
using NUnit.Framework;

namespace Streaming.Tests.Extensions.Tests
{
    public class LinqExtensionsTests
    {
        [TestCase(new[] {1, 2, 3, 4}, true)]
        [TestCase(new[] {1, 2, 3, 4}, true)]
        [TestCase(new[] { 1, 1, 2, 2 }, true)]
        [TestCase(new[] { 1, 1, 1, 1 }, true)]
        [TestCase(new[] {3, 2, 3, 4}, false)]
        [TestCase(new[] {4, 3, 2, 1}, false)]
        [TestCase(new[] { 1 }, true)]
        [TestCase(new int[] {}, true)]
        public void IsSortedAscendingWorks(int[] inputArray, bool expectedOutput)
        {
            Assert.AreEqual(expectedOutput, inputArray.IsSortedAscending(x => x));
        }

        [TestCase(new[] { 1, 2, 3, 4 }, false)]
        [TestCase(new[] { 4, 3, 2, 1 }, true)]
        [TestCase(new[] { 4, 3, 3, 3 }, true)]
        [TestCase(new[] { 3, 3, 3, 3 }, true)]
        [TestCase(new[] { 4, 3, 2, 5 }, false)]
        [TestCase(new[] { 1 }, true)]
        [TestCase(new int[] { }, true)]
        public void IsSortedDescendingWorks(int[] inputArray, bool expectedOutput)
        {
            Assert.AreEqual(expectedOutput, inputArray.IsSortedDescending(x => x));
        }
    }
}
