using System;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IProcessVideoService
    {
        Task ProcessVideoAsync(string VideoPath, string OutputDirectory);
        Task TakeVideoScreenshot(string VideoPath, string ScreenshotOutputPath, TimeSpan Time);
        Task GenerateVideoOverviewScreenshots(string VideoPath, string ScreenshotOutputDirectory, TimeSpan ScreenshotInterval);
        Task<TimeSpan> GetVideoLengthAsync(string VideoPath);
    }
}
