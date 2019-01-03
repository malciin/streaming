using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Repository
{
	public interface IVideoBlobRepository
	{
        string GetVideoUrl(Guid VideoId, int PartNumber);
		Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber);
		Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream);
	}
}
