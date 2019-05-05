using System;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;

namespace Streaming.Application.Commands.Video
{
	public class UploadVideoHandler : ICommandHandler<UploadVideoCommand>
	{
		private readonly ICommandBus commandBus;
        private readonly IMessageSignerService messageSigner;
        private readonly IVideoRepository videoRepo;
        private readonly IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy;

        public UploadVideoHandler(IVideoRepository videoRepo,
            ICommandBus commandBus,
            IMessageSignerService messageSigner,
            IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy)
		{
			this.videoRepo = videoRepo;
            this.commandBus = commandBus;
            this.messageSigner = messageSigner;
            this.videoProcessingFilesPathStrategy = videoProcessingFilesPathStrategy;
        }

        private Guid GetVideoIdFromUploadToken(string uploadToken)
        {
            var signedMessage = Convert.FromBase64String(uploadToken);
            var message = messageSigner.GetMessage(signedMessage);
            return new Guid(message);
        }

        public async Task HandleAsync(UploadVideoCommand command)
		{
            var videoId = GetVideoIdFromUploadToken(command.UploadToken);
            var inputFilePath = videoProcessingFilesPathStrategy.RawUploadedVideoFilePath(videoId);

			var video = new Domain.Models.Video
            {
				CreatedDate = DateTime.Now,
				Title = command.Title,
				Description = command.Description,
				VideoId = videoId,
                Owner = command.User.Details
			};

			await videoRepo.AddAsync(video);

            commandBus.Push(new ProcessVideoCommand
            {
				VideoId = video.VideoId,
                InputFilePath = inputFilePath,
                UserId = command.User.Details.UserId
            });
		}
	}
}