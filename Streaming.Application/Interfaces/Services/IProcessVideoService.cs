using Streaming.Application.Models.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IProcessVideoService
    {
        IEnumerable<VideoCodec> SupportedVideoCodecs();
        Task ConvertVideoToMp4Async(string videoPath, string outputVideoFile, Action<double> progressCallback = null);
        
        /// <summary>
        /// Split mp4 video into TS files path
        /// </summary>
        /// <param name="mp4VideoFilePath">Input mp4 video file path</param>
        /// <param name="tsFilesPathStrategy">Strategy for naming tsFileParts. Input int arg is a part number started from 0</param>
        /// <returns>Ordered paths for each video part</returns>
        Task<List<string>> SplitMp4FileIntoTsFilesAsync(string mp4VideoFilePath, Func<int, string> tsFilesPathStrategy);
        
        Task TakeVideoScreenshotAsync(string videoPath, string screenshotOutputPath, TimeSpan time);

        Task TakeVideoGifAsync(string videoPath, string gifOutputPath, TimeSpan startTime, TimeSpan gifLength);
        
        /// <summary>
        /// Generate video overview screenshots
        /// </summary>
        /// <param name="mp4VideoFilePath">Input mp4 video file path</param>
        /// <param name="screenshotInterval">Interval by which screenshot is generated</param>
        /// <param name="screenshotFilesPathStrategy">Strategy for naming screenshots.</param>
        /// <returns>Ordered paths for each video part</returns>
        /// TODO: Change 'TimeSpan screenshotInterval' to 'int howManyScreenshot' to be independend from video length (for example for long video we still want to have const number of overview screenshots)
        Task<List<string>> GenerateVideoOverviewScreenshotsAsync(string mp4VideoFilePath, TimeSpan screenshotInterval, Func<ScreenshotGenerationContext, string> screenshotFilesPathStrategy);
    }
    public class ScreenshotGenerationContext
    {
        /// <summary>
        /// Started from 0
        /// </summary>
        public int ScreenshotNumber { get; set; }
        public TimeSpan ScreenshotTime { get; set; }
    }
}
