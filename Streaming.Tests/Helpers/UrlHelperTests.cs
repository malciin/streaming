using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Streaming.Common.Helpers;

namespace Streaming.Tests.Helpers
{
    public class UrlHelperTests
    {
        private static IEnumerable<(HttpContext context, string expectedOutput)> GetHostUrlProducesCorrectUrlData
        {
            get
            {
                var contexts = new List<HttpContext>();
                
                var context = new DefaultHttpContext();
                context.Request.Path = "/some/uri";
                context.Request.Host = new HostString("localhost:80");
                context.Request.Scheme = "http";
                contexts.Add(context);
                
                context = new DefaultHttpContext();
                context.Request.Path = "/watch?v=qYS0EeaAUMw";
                context.Request.Host = new HostString("youtube.com");
                context.Request.Scheme = "https";
                contexts.Add(context);
                
                yield return (contexts[0], "http://localhost:80");
                yield return (contexts[1], "https://youtube.com");
            }
        } 
        
        [TestCaseSource(nameof(GetHostUrlProducesCorrectUrlData))]
        public void GetHostUrl_Produces_Correct_Url((HttpContext context, string expectedOutput) input)
        {
            var returnedUrl = UrlHelper.GetHostUrl(input.context);
            Assert.AreEqual(input.expectedOutput, returnedUrl);
        }
    }
}