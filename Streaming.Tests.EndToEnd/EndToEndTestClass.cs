using System.Net.Http;
using NUnit.Framework;

namespace Streaming.Tests.EndToEnd
{
    public abstract class EndToEndTestClass
    {
        protected ITestWebhost WebHost { get; private set; }
        protected HttpClient Client { get; private set; }

        [SetUp]
        public void StartUp()
        {
            WebHost = new TestWebhost();
            Client = new HttpClient();
        }

        [TearDown]
        public void CloseServer()
        {
            WebHost.Stop();
        }
    }
}