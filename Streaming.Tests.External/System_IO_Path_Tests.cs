using NUnit.Framework;
using System.IO;

namespace Streaming.Tests.External
{
    public class System_IO_Path_Tests
    {
        [TestCase("test.mp4", ".mp4")]
        [TestCase("test.", "")]
        [TestCase("some/directory/to/photo.jpg", ".jpg")]
        public void GetExtensions_Should_Return_Path_With_Dot(string input, string expectedOutput)
        {
            var result = Path.GetExtension(input);
            Assert.AreEqual(expectedOutput, result, $"For '{input}' expected '{expectedOutput}' but was {result}");
        }
    }
}