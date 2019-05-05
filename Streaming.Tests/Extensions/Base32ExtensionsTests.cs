using System;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.Extensions
{
    public class Base32ExtensionsTests
    {
        [TestCase(new byte[] {}, TestName = "Base32 extensions method should also works on empty byte array")]
        [TestCase(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 })]
        public void Test_That_Base32_Extensions_Correctly_Works(byte[] input)
        {
            string base32String = String.Empty;
            byte[] decodedBytes = new byte[] { };
            
            Assert.DoesNotThrow(() => base32String = input.ToBase32String(), $"ToBase32String throws an exception on bytes: {BitConverter.ToString(input)}");
            Assert.DoesNotThrow(() => decodedBytes = base32String.ToByteArrayFromBase32String(), $"ToByteArrayFromBase32String throws an exception on string: {base32String}");

            Assert.AreEqual(input.Length, decodedBytes.Length);
            for(int i = 0; i<input.Length; i++)
            {
                Assert.AreEqual(input[i], decodedBytes[i]);
            }
        }
    }
}
