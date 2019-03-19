using Streaming.Application.Models.DTO.Video;
using System;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IVideoFileInfoService
    {
        Task<VideoFileDetailsDTO> GetDetailsAsync(string videoPath);
        Task<TimeSpan> GetVideoLengthAsync(string videoPath);
    }
}
