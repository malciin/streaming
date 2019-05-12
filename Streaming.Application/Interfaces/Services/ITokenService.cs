using Streaming.Application.Models.DTO.Video;

namespace Streaming.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GetUploadVideoToken(UploadVideoTokenDataDTO data);
        UploadVideoTokenDataDTO GetDataFromUploadVideoToken(string uploadVideoToken);
    }
}