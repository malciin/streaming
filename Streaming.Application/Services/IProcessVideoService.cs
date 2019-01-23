using Streaming.Domain.Models.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IProcessVideoService
    {
        Task ProcessVideoAsync(string VideoPath, string OutputDirectory);
        Task TakeVideoScreenshot(string VideoPath, string ScreenshotOutputPath, TimeSpan Time);
        Task GenerateVideoOverviewScreenshots(string VideoPath, string ScreenshotOutputDirectory, TimeSpan ScreenshotInterval);
        Task<TimeSpan> GetVideoLengthAsync(string VideoPath);
    }
}
