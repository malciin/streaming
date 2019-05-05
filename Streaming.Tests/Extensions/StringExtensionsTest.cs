using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.Extensions
{
    class StringExtensionsTest
    {
        [Test]
        public void Replace_CharArray_Test()
        {

        }

        private static IEnumerable<(string input, string output)> NormalizePathForOs_DataProvider
        {
            get
            {
                yield return ("hello/somewhere\\else\\now/",
                    String.Format("hello{0}somewhere{0}else{0}now{0}", Path.DirectorySeparatorChar));
                yield return ("hello\\somewhere/else/now",
                    String.Format("hello{0}somewhere{0}else{0}now", Path.DirectorySeparatorChar));
            }
        }

        [TestCaseSource(nameof(NormalizePathForOs_DataProvider))]
        [Test]
        public void NormalizePathForOs_Test((string input, string output) data)
        {
            var returnedOutput = data.input.NormalizePathForOS();
            Assert.AreEqual(returnedOutput, data.output, $"Expected {data.output} but was {returnedOutput}");
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
