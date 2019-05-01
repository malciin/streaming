using System;
using System.Collections.Generic;
using NUnit.Framework;
using Streaming.Application.Models.DTO.Video;
using Streaming.Common.Extensions;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;

namespace Streaming.Tests.EndToEnd.VideoController
{
    public partial class VideoControllerTests : EndToEndTestClass
    {
        [Test]
        public void GetSingleVideo()
        {
            var videoGuid = new Guid("00000000-0000-0000-0000-000000000001");
            WebHost
                .SeedDatabase(new List<Video>
                {
                    new Video
                    {
                        VideoId = videoGuid,
                        CreatedDate = DateTime.UtcNow,
                        Title = "Test title",
                        Description = "Test description",
                        State = VideoState.Processed | VideoState.MainThumbnailGenerated | VideoState.ManifestGenerated,
                        Length = TimeSpan.FromSeconds(5),
                        FinishedProcessingDate = DateTime.UtcNow.AddSeconds(1),
                        Owner = new UserDetails
                        {
                            UserId = "id",
                            Email = "email@gmail.com",
                            Nickname = "nick"
                        },
                        VideoManifest = VideoManifest
                            .Create(videoGuid)
                            .AddPart(TimeSpan.FromSeconds(5))
                    }
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
