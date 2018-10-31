using Streaming.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Domain.Services
{
    public interface IVideoService
    {
        Task<IEnumerable<VideoBasicMetadataDTO>> GetRangeAsync(int Offset, int HowMuch);
        Task<bool> AddAsync(VideoUploadDTO VideoUploadDTO);
        Task<string> GetVideoManifestAsync(Guid VideoId);
        Task<byte[]> GetVideoPartAsync(Guid VideoId, int Part);
    }
}
