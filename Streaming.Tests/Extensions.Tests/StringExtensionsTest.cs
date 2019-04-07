using NUnit.Framework;
using Streaming.Common.Extensions;
using System;
using System.IO;

namespace Streaming.Tests.Extensions.Tests
{
    class StringExtensionsTest
    {
        [Test]
        public void Replace_CharArray_Test()
        {

        }

        // Cannot use TestCase for this... (Path.DirectorySeparatorChar will differ)
        [Test]
        public void NormalizePathForOs_Test()
        {
            var firstCase = "hello/somewhere\\else\\now/";
            var secondCase = "hello\\somewhere/else/now";

            var convertedFirst = firstCase.NormalizePathForOS();
            var convertedSecond = secondCase.NormalizePathForOS();

            var firstExpected = String.Format("hello{0}somewhere{0}else{0}now{0}", Path.DirectorySeparatorChar);
            var secondExpected = String.Format("hello{0}somewhere{0}else{0}now", Path.DirectorySeparatorChar);

            Assert.AreEqual(firstExpected, convertedFirst, $"Expected {firstExpected} but was {convertedFirst}");
            Assert.AreEqual(secondExpected, convertedSecond, $"Expected {secondExpected} but was {convertedSecond}");
        }

        [TestCase("hello/somewhere/else/now", "hello/somewhere/else")]
        [TestCase("hello/somewhere/else/now/", "hello/somewhere/else/now")]
        [TestCase("hello", "hello")]
        public void SubstringToLastOccurence_Test(string input, string expectedOutput)
        {
            var output = input.SubstringToLastOccurence('/');

            Assert.AreEqual(expectedOutput, output, $"For '{input}' expected '{expectedOutput}' but was '{output}'");
        }
    }
}
