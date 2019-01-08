using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
	public interface IVideoBlobService
	{
        string GetVideoUrl(Guid VideoId, int PartNumber);
		Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber);
		Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream);
	}
}
