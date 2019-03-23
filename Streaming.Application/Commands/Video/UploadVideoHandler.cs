using System;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Interfaces.Strategies;

namespace Streaming.Application.Commands.Video
{
	public class UploadVideoHandler : ICommandHandler<UploadVideoCommand>
	{
		private readonly ICommandBus commandBus;
        private readonly IMessageSignerService messageSigner;
        private readonly IVideoRepository videoRepo;
        private readonly IPathStrategy pathStrategy;
        private readonly IVideoFileInfoService videoFileInfoService;
        private readonly IProcessVideoService processVideoService;

		public UploadVideoHandler(IVideoRepository videoRepo,
			IDirectoriesSettings directoriesSettings,
            ICommandBus commandBus,
            IMessageSignerService messageSigner,
            IPathStrategy pathStrategy,
            IVideoFileInfoService videoFileInfoService,
            IProcessVideoService processVideoService)
		{
			this.videoRepo = videoRepo;
            this.commandBus = commandBus;
            this.messageSigner = messageSigner;
            this.pathStrategy = pathStrategy;
            this.processVideoService = processVideoService;
            this.videoFileInfoService = videoFileInfoService;
		}

        public Guid getVideoIdFromUploadToken(string uploadToken)
        {
            var signedMessage = Convert.FromBase64String(uploadToken);
            var message = messageSigner.GetMessage(signedMessage);
            return new Guid(message);
        }

        public async Task HandleAsync(UploadVideoCommand Command)
		{
            var videoId = getVideoIdFromUploadToken(Command.UploadToken);
            var inputFilePath = pathStrategy.VideoProcessingFilePath(videoId);

            var videoFileInfo = await videoFileInfoService.GetDetailsAsync(inputFilePath);
            if (!processVideoService.SupportedVideoTypes().Select(x => x.Codec).Contains(videoFileInfo.Video.Codec))
            {
                throw new NotSupportedException("Video file format not supported!");
            }

			var video = new Domain.Models.Video
            {
				CreatedDate = DateTime.Now,
				Title = Command.Title,
				Description = Command.Description,
				VideoId = videoId,
                Owner = new Domain.Models.UserDetails
                {
                    UserId = Command.User.UserId,
                    Email = Command.User.Email,
                    Nickname = Command.User.Nickname
                }
			};

			await videoRepo.AddAsync(video);
            await videoRepo.CommitAsync();

            commandBus.Push(new ProcessVideoCommand
            {
				VideoId = video.VideoId,
                InputFilePath = inputFilePath,
                InputFileInfo = videoFileInfo,
                UserId = Command.User.UserId
            });
		}
	}
}
