using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IProcessVideoService
    {
        IEnumerable<(string Extension, string Codec)> SupportedVideoTypes();
        Task ConvertVideoToMp4(string videoPath, string outputVideoFile);
        Task SplitMp4FileIntoTSFiles(string mp4VideoFilePath, string outputTsFilesDirectory);
        Task TakeVideoScreenshot(string VideoPath, string ScreenshotOutputPath, TimeSpan Time);
        Task GenerateVideoOverviewScreenshots(string VideoPath, string ScreenshotOutputDirectory, TimeSpan ScreenshotInterval);
    }
}
