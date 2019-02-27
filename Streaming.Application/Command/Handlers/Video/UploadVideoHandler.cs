using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Command.Bus;
using Streaming.Application.Services;
using Streaming.Application.Settings;

namespace Streaming.Application.Command.Handlers.Video
{
	public class UploadVideoHandler : ICommandHandler<Commands.Video.UploadVideoCommand>
	{
		private readonly ICommandBus commandBus;
        private readonly IMessageSignerService messageSigner;
        private readonly IMongoCollection<Domain.Models.Video> videoCollection;
		private readonly IDirectoriesSettings directoriesSettings;

		public UploadVideoHandler(IMongoCollection<Domain.Models.Video> videoCollection,
			IDirectoriesSettings directoriesSettings,
            ICommandBus commandBus,
            IMessageSignerService messageSigner)
		{
			this.videoCollection = videoCollection;
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
				VideoId = getVideoIdFromUploadToken(Command.UploadToken)
			};

			await videoCollection.InsertOneAsync(video);

            commandBus.Push(new Commands.Video.ProcessVideoCommand
			{
				VideoId = video.VideoId
            });
		}
	}
}
