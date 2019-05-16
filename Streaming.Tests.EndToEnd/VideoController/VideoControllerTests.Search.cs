using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Streaming.Application.Interfaces.Models;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Models;
using Streaming.Common.Extensions;

namespace Streaming.Tests.EndToEnd.VideoController
{
    public partial class VideoControllerTests : EndToEndTestClass
    {
        [Test]
        public void Search_Package_Works()
        {
            var videos = new List<Video>();
            for (int i = 1; i < 10; i++)
            {
                var videoGuid = new Guid($"00000000-0000-0000-0000-00000000000{i}");
                var video = new Video(videoGuid, $"Test title, Case {i}", $"Test description, Case {i}", new UserDetails
                {
                    UserId = $"{i}",
                    Email = $"user{i}@gmail.com",
                    Nickname = $"user{i}"
                });
                video.SetVideoManifest(VideoManifest
                    .Create(videoGuid)
                    .AddPart(TimeSpan.FromSeconds(i + 1)));
                video.SetThumbnail("thumbnail.jpg");
                videos.Add(video);
            }

            WebHost.SeedDatabase(videos)
                   .Start();

            var response = Client.PostAsJsonAsync($"{WebHost.ApiUri}Video/Search", new VideoSearchDTO
            {
                HowMuch = 10,
                Offset = 0,
                Keywords = new string[] {}
            }).GetAwaiter().GetResult();
            Assert.True(response.IsSuccessStatusCode, $"Not success status code! Status code was: {response.StatusCode}");
            
            var totalVideos = videos.Count;
            IPackage<VideoMetadataDTO> receivedVideos = response.Content.ReadFromJsonAsObject<Package<VideoMetadataDTO>>();
            Assert.AreEqual(videos.Count, receivedVideos.Details.Count);
            Assert.AreEqual(totalVideos, receivedVideos.Details.TotalCount, $"Package Details.TotalCount should return total matching items");

            response = Client.PostAsJsonAsync($"{WebHost.ApiUri}Video/Search", new VideoSearchDTO
            {
                HowMuch = 5,
                Offset = 3,
                Keywords = new string[] { }
            }).GetAwaiter().GetResult();
            Assert.True(response.IsSuccessStatusCode, $"Not success status code! Status code was: {response.StatusCode}");
            receivedVideos = response.Content.ReadFromJsonAsObject<Package<VideoMetadataDTO>>();
            Assert.AreEqual(totalVideos, receivedVideos.Details.TotalCount, $"Package Details.TotalCount should return total matching items");
            Assert.AreEqual("Test title, Case 4", receivedVideos.Items.First().Title);
            Assert.AreEqual(5, receivedVideos.Details.Count);
            
            // TODO: Move this to Query unit tests
            Assert.IsTrue(receivedVideos.Items.IsSortedDescending(x => x.CreatedDate), $"Expected that videos are sorted descending by CreatedDate");
        }
    }
}
