using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
	public interface IVideoPartsFileService
	{
        string GetVideoUrl(Guid videoId, int partNumber);
		Task<Stream> GetVideoAsync(Guid videoId, int partNumber);
		Task UploadAsync(Guid videoId, int partNumber, Stream stream);
	}
}
