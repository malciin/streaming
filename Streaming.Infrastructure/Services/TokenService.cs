using System;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;

namespace Streaming.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IMessageSignerService messageSigner;

        public TokenService(IMessageSignerService messageSigner)
        {
            this.messageSigner = messageSigner;
        }

        public string GetUploadVideoToken(UploadVideoTokenDataDTO data)
        {
            return Convert.ToBase64String(messageSigner.SignMessage(data.VideoId.ToByteArray()));
        }

        public UploadVideoTokenDataDTO GetDataFromUploadVideoToken(string uploadVideoToken)
        {
            var videoId = new Guid(messageSigner.GetMessage(Convert.FromBase64String(uploadVideoToken)));
            return new UploadVideoTokenDataDTO
            {
                VideoId = videoId
            };
        }
    }
}