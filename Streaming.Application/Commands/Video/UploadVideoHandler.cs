using System;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Domain.Models;

namespace Streaming.Application.Commands.Video
{
	public class UploadVideoHandler : ICommandHandler<UploadVideoCommand>
	{
		private readonly ICommandBus commandBus;
		private readonly ITokenService tokenService;
		private readonly IVideoRepository videoRepo;
        private readonly IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy;

        public UploadVideoHandler(IVideoRepository videoRepo,
            ICommandBus commandBus,
            IVideoProcessingFilesPathStrategy videoProcessingFilesPathStrategy, ITokenService tokenService)
		{
			this.videoRepo = videoRepo;
            this.commandBus = commandBus;
            this.videoProcessingFilesPathStrategy = videoProcessingFilesPathStrategy;
            this.tokenService = tokenService;
		}

        public async Task HandleAsync(UploadVideoCommand command)
        {
	        Guid videoId = tokenService.GetDataFromUploadVideoToken(command.UploadToken).VideoId;
            var inputFilePath = videoProcessingFilesPathStrategy.RawUploadedVideoFilePath(videoId);

			var video = new Domain.Models.Video(videoId, command.Title, command.Description, new UserDetails{
				Email = command.User.Email,
				Nickname = command.User.Nickname,
				UserId = command.User.UserId
			});
			await videoRepo.AddAsync(video);

            commandBus.Push(new ProcessVideoCommand
            {
				Video = video,
                InputFilePath = inputFilePath,
            });
		}
	}
}