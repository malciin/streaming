using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IAzureBlobClient
    {
        string GetFileLink(string ContainerName, string FileName);
        string GetFileLinkSecuredSAS(string ContainerName, string FileName);
        Task<Stream> GetFileAsync(string ContainerName, string FileName);
        Task UploadFileAsync(string ContainerName, string FileName, Stream InputStream);
    }
}
