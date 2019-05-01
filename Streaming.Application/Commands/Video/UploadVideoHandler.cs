using System;
using System.Linq;
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
        private readonly IVideoFileInfoService videoFileInfoService;
        private readonly IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy;
        private readonly IProcessVideoService processVideoService;

		public UploadVideoHandler(IVideoRepository videoRepo,
            ICommandBus commandBus,
            IMessageSignerService messageSigner,
            IVideoFileInfoService videoFileInfoService,
            IProcessVideoService processVideoService,
            IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy)
		{
			this.videoRepo = videoRepo;
            this.commandBus = commandBus;
            this.messageSigner = messageSigner;
            this.processVideoService = processVideoService;
            this.videoFileInfoService = videoFileInfoService;
            this.videoProcessingFilesPathStrategy = videoProcessingFilesPathStrategy;
        }

        private Guid getVideoIdFromUploadToken(string uploadToken)
        {
            var signedMessage = Convert.FromBase64String(uploadToken);
            var message = messageSigner.GetMessage(signedMessage);
            return new Guid(message);
        }

        public async Task HandleAsync(UploadVideoCommand command)
		{
            var videoId = getVideoIdFromUploadToken(command.UploadToken);
            var inputFilePath = videoProcessingFilesPathStrategy.RawUploadedVideoFilePath(videoId);

            var videoFileInfo = await videoFileInfoService.GetDetailsAsync(inputFilePath);
            if (!processVideoService.SupportedVideoCodecs().Contains(videoFileInfo.Video.Codec))
            {
                throw new NotSupportedException("Video file format not supported!");
            }

			var video = new Domain.Models.Video
            {
				CreatedDate = DateTime.Now,
				Title = command.Title,
				Description = command.Description,
				VideoId = videoId,
                Owner = new Domain.Models.UserDetails
                {
                    UserId = command.User.UserId,
                    Email = command.User.Email,
                    Nickname = command.User.Nickname
                }
			};

			await videoRepo.AddAsync(video);

            commandBus.Push(new ProcessVideoCommand
            {
				VideoId = video.VideoId,
                InputFilePath = inputFilePath,
                InputFileInfo = videoFileInfo,
                UserId = command.User.UserId
            });
		}
	}
}
