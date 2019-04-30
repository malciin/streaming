using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using Streaming.Common.Extensions;

namespace Streaming.Tests.EndToEnd.VideoController
{
    public partial class VideoControllerTests : EndToEndTestClass
    {
        [Test]
        public void PaginationWorks()
        {
            var videos = new List<Video>();
            for (int i = 1; i < 10; i++)
            {
                var videoGuid = new Guid($"00000000-0000-0000-0000-00000000000{i}");
                videos.Add(new Video
                {
                    VideoId = videoGuid,
                    CreatedDate = DateTime.UtcNow,
                    Title = $"Test title, Case {i}",
                    Description = $"Test description, Case {i}",
                    State = VideoState.Processed | VideoState.MainThumbnailGenerated | VideoState.ManifestGenerated,
                    Length = TimeSpan.FromSeconds(i),
                    FinishedProcessingDate = DateTime.UtcNow.AddSeconds(1),
                    Owner = new UserDetails
                    {
                        UserId = $"{i}",
                        Email = $"user{i}@gmail.com",
                        Nickname = $"user{i}"
                    },
                    VideoManifest = VideoManifest
                            .Create(videoGuid)
                            .AddPart(TimeSpan.FromSeconds(i + 1))
                });
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

            var receivedVideos = response.Content.ReadFromJsonAsObject<IEnumerable<VideoMetadataDTO>>();
            Assert.AreEqual(videos.Count, receivedVideos.Count());

            response = Client.PostAsJsonAsync($"{WebHost.ApiUri}Video/Search", new VideoSearchDTO
            {
                HowMuch = 5,
                Offset = 3,
                Keywords = new string[] { }
            }).GetAwaiter().GetResult();
            Assert.True(response.IsSuccessStatusCode, $"Not success status code! Status code was: {response.StatusCode}");
            receivedVideos = response.Content.ReadFromJsonAsObject<IEnumerable<VideoMetadataDTO>>();
            Assert.AreEqual("Test title, Case 4", receivedVideos.First().Title);
            Assert.AreEqual(5, receivedVideos.Count());

            Assert.IsTrue(receivedVideos.IsSortedDescending(x => x.CreatedDate), $"Expected that videos are sorted descending by CreatedDate");
        }
    }
}
