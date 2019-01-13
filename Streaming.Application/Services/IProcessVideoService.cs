using Streaming.Domain.Models.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IProcessVideoService
    {
        Task ProcessVideoAsync(string VideoPath, string OutputDirectory);
        Task<TimeSpan> GetVideoLengthAsync(string VideoPath);
    }
}
