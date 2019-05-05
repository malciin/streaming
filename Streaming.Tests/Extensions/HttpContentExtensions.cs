using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.Extensions
{
    public class HttpContentExtensions
    {
        private class TestClass
        {
            public readonly List<int> ListInt = new List<int>();
            public string SomeField;
        }
        
        [Test]
        public void ReadFromJsonAsObject_Test()
        {
            var testClass = new TestClass();
            testClass.ListInt.Add(5);
            testClass.ListInt.Add(6);
            testClass.SomeField = "TestVal";
            
            HttpContent content = new StringContent(JsonConvert.SerializeObject(testClass));
            var result = content.ReadFromJsonAsObject<TestClass>();
            
            Assert.IsTrue(result.ListInt.Count == 2 && result.ListInt.Contains(5) && result.ListInt.Contains(6));
            Assert.AreEqual("TestVal", result.SomeField);
        }
    }
}