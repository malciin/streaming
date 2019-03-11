﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;

namespace Streaming.Application.Commands.Video
{
	public class UploadVideoHandler : ICommandHandler<Commands.Video.UploadVideoCommand>
	{
		private readonly ICommandBus commandBus;
        private readonly IMessageSignerService messageSigner;
        private readonly IVideoRepository videoRepo;
		private readonly IDirectoriesSettings directoriesSettings;

		public UploadVideoHandler(IVideoRepository videoRepo,
			IDirectoriesSettings directoriesSettings,
            ICommandBus commandBus,
            IMessageSignerService messageSigner)
		{
			this.videoRepo = videoRepo;
            this.directoriesSettings = directoriesSettings;
            this.commandBus = commandBus;
            this.messageSigner = messageSigner;
		}

        public Guid getVideoIdFromUploadToken(string uploadToken)
        {
            var signedMessage = Convert.FromBase64String(uploadToken);
            var message = messageSigner.GetMessage(signedMessage);
            return new Guid(message);
        }

        public async Task HandleAsync(Commands.Video.UploadVideoCommand Command)
		{
			var video = new Domain.Models.Video
            {
				CreatedDate = DateTime.Now,
				Title = Command.Title,
				Description = Command.Description,
				VideoId = getVideoIdFromUploadToken(Command.UploadToken),
                Owner = new Domain.Models.UserDetails
                {
                    Identifier = Command.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = Command.User.FindFirst(ClaimTypes.Email).Value,
                    Nickname = Command.User.FindFirst(x => x.Type == "nickname")?.Value
                }
			};

			await videoRepo.AddAsync(video);
            await videoRepo.CommitAsync();

            commandBus.Push(new Commands.Video.ProcessVideoCommand
			{
				VideoId = video.VideoId
            });
		}
	}
}
