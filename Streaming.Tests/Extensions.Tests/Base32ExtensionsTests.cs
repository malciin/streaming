using NUnit.Framework;
using Streaming.Common.Extensions;
using System;

namespace Streaming.Tests.Extensions.Tests
{
    public class Base32ExtensionsTests
    {
        [Test]
        public void Test_That_Conversion_Correctly_Works()
        {
            var exampleBytes = new byte[] { 0x11, 0x22 };
            string base32String = String.Empty;
            byte[] decodedBytes = new byte[] { };

            Assert.DoesNotThrow(() => base32String = exampleBytes.ToBase32String(), $"ToBase32String throws an exception on bytes: {BitConverter.ToString(exampleBytes)}");
            Assert.DoesNotThrow(() => decodedBytes = base32String.ToByteArrayFromBase32String(), $"ToByteArrayFromBase32String throws an exception on string: {base32String}");

            Assert.AreEqual(exampleBytes.Length, decodedBytes.Length);
            for(int i = 0; i<exampleBytes.Length; i++)
            {
                Assert.AreEqual(exampleBytes[i], decodedBytes[i]);
            }
        }
    }
}
