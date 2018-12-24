using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Command.Bus;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Settings;

namespace Streaming.Application.Command.Handlers.Video
{
	public class UploadVideo : ICommandHandler<Commands.Video.UploadVideo>
	{
		private readonly ICommandBus commandBus;
		private readonly IMongoCollection<Domain.Models.Core.Video> videoCollection;
		private readonly IDirectoriesSettings directoriesSettings;

		public UploadVideo(IMongoCollection<Domain.Models.Core.Video> videoCollection,
			IDirectoriesSettings directoriesSettings)
		{
			this.videoCollection = videoCollection;
		}

		public async Task HandleAsync(Commands.Video.UploadVideo Command)
		{
			var video = new Domain.Models.Core.Video
			{
				CreatedDate = DateTime.Now,
				Title = Command.Title,
				Description = Command.Description,
				VideoId = Guid.NewGuid()
			};

			await videoCollection.InsertOneAsync(video);

			var filePath = String.Format($"{directoriesSettings.ProcessingDirectory}{{0}}{Command.File.FileName}", Path.DirectorySeparatorChar);

			using (var fileStream = File.OpenWrite(filePath))
			{
				await Command.File.CopyToAsync(fileStream);
			}

			commandBus.Push(new Commands.Video.ProcessVideo
			{
				VideoId = video.VideoId,
				VideoPath = filePath
			});
		}
	}
}
