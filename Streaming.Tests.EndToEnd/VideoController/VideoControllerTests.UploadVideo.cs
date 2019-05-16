using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Streaming.Application.DTO;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Video;
using Streaming.Common.Extensions;

namespace Streaming.Tests.EndToEnd.VideoController
{
    public partial class VideoControllerTests : EndToEndTestClass
    {
        private async Task UploadVideoParts(string uploadToken, string mp4Path)
        {
            using (var mp4File = File.OpenRead(mp4Path))
            {
                var onePartByteLength = 1_000_000;
                for (int i = 0; i < mp4File.Length; i+=onePartByteLength)
                {
                    var buffer = new byte[Math.Min(i + onePartByteLength, mp4File.Length)];
                    mp4File.Read(buffer, i, buffer.Length);
                    var partHash = Convert.ToBase64String(MD5.Create().ComputeHash(buffer));
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(uploadToken, Encoding.UTF8), "UploadToken");
                    content.Add(new StringContent(partHash, Encoding.UTF8), "PartMD5Hash");
                    content.Add(new StreamContent(new MemoryStream(buffer)), "PartBytes", "sample.mp4");
                    var response = await Client.PostAsync($"{WebHost.ApiUri}Video/UploadPart", content);
                }
            }
        }
        

        [Test]
        public async Task UploadVideo_Main_Process_Flow()
        {
            WebHost.ConfigureTestUser(Claims.CanUploadVideo)
                   .Start();
            var uploadToken = (await Client.GetAsync($"{WebHost.ApiUri}Video/UploadToken")).Content
                .ReadFromJsonAsObject<TokenDTO>().Token;
            await UploadVideoParts(uploadToken, "_Data/sample.mp4");
            var uploadVideoRequest = await Client.PostAsJsonAsync($"{WebHost.ApiUri}Video", new {
                UploadToken = uploadToken,
                Title = "Test titleeeeeee",
                Description = "Test descriptionnnnnn"
            });
            Assert.IsTrue(uploadVideoRequest.IsSuccessStatusCode, $"Upload video return unsuccessful status code: {uploadVideoRequest.StatusCode}");

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < 60_000)
            {
                var returnedVideos = (await Client.PostAsJsonAsync($"{WebHost.ApiUri}Video/Search", new VideoSearchDTO
                {
                    Keywords = new string[] {},
                    Offset = 0,
                    HowMuch = 1
                })).Content.ReadFromJsonAsObject<Package<VideoMetadataDTO>>();

                if (!returnedVideos.Items.Any())
                {
                    Thread.Sleep(1_000);
                    continue;
                }
                var video = returnedVideos.Items.First();
                Assert.AreEqual("Test titleeeeeee", video.Title, "Uploaded video title is different!");
                Assert.AreEqual("Test descriptionnnnnn", video.Description, "Uploaded video description is different!");
                var manifestRequest = await Client.GetAsync($"{WebHost.ApiUri}Video/Manifest/{video.VideoId}");
                Assert.IsTrue(manifestRequest.IsSuccessStatusCode, $"Getting manifest to freshly uploaded video return unsuccessful status code: {manifestRequest.StatusCode}");
                return;
            }
            Assert.Fail("Video doesn't processed after 60s - propably processing not works - check logs");
        }
    }
}