using System;
using System.Collections.Generic;
using NUnit.Framework;
using Streaming.Application.Models.DTO.Video;
using Streaming.Common.Extensions;
using Streaming.Domain.Models;

namespace Streaming.Tests.EndToEnd.VideoController
{
    public partial class VideoControllerTests : EndToEndTestClass
    {
        [Test]
        public void GetSingleVideo()
        {
            var videoGuid = new Guid("00000000-0000-0000-0000-000000000001");
            
            var video = new Video(videoGuid, "Test title", "Test description", new UserDetails
            {
                UserId = "id",
                Email = "email@gmail.com",
                Nickname = "nick"
            });
            video.SetVideoManifest(VideoManifest
                .Create(videoGuid)
                .AddPart(TimeSpan.FromSeconds(5)));
            video.SetThumbnail("thumbnail.jpg");
            
            WebHost
                .SeedDatabase(new List<Video>
                {
                    video
                })
                .Start();

            var response = Client.GetAsync($"{WebHost.ApiUri}Video/{videoGuid}").GetAwaiter().GetResult();
            Assert.True(response.IsSuccessStatusCode, $"Not success status code! Status code was: {response.StatusCode}");

            var metadata = response.Content.ReadFromJsonAsObject<VideoMetadataDTO>();
            Assert.AreEqual("Test title", metadata.Title);
            Assert.AreEqual("Test description", metadata.Description);
            Assert.AreEqual("nick", metadata.OwnerNickname);
        }
    }
}
