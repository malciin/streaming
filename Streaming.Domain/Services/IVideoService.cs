using Streaming.Domain.Models.DTO;
using Streaming.Domain.Models.DTO.Video;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Domain.Services
{
    public interface IVideoService
    {
        Task<VideoBasicMetadataDTO> GetAsync(Guid VideoId);
        Task<IEnumerable<VideoBasicMetadataDTO>> GetAsync(VideoSearchDTO Search);
        Task<bool> AddAsync(VideoUploadDTO VideoUploadDTO);

        Task<bool> UpdateVideoAfterProcessingAsync(VideoProcessedDataDTO VideoProcessedData);

        Task<string> GetVideoManifestAsync(Guid VideoId);
        Task<byte[]> GetVideoPartAsync(Guid VideoId, int Part);
    }
}
