using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
	public interface IVideoFilesService
	{
        string GetVideoUrl(Guid VideoId, int PartNumber);
		Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber);
		Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream);
	}
}
