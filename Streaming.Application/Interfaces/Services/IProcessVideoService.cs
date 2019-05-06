﻿using Streaming.Application.Models.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IProcessVideoService
    {
        IEnumerable<VideoCodec> SupportedVideoCodecs();
        Task ConvertVideoToMp4Async(string videoPath, string outputVideoFile, Action<float> progressCallback = null);
        
        /// <summary>
        /// Split mp4 video into TS files path
        /// </summary>
        /// <param name="mp4VideoFilePath">Input mp4 video file path</param>
        /// <param name="outputTsFilesDirectory">Output directory</param>
        /// <returns>Ordered paths for each video part</returns>
        Task<List<string>> SplitMp4FileIntoTSFilesAsync(string mp4VideoFilePath, string outputTsFilesDirectory);
        
        Task TakeVideoScreenshotAsync(string VideoPath, string ScreenshotOutputPath, TimeSpan Time);
        Task GenerateVideoOverviewScreenshotsAsync(string VideoPath, string ScreenshotOutputDirectory, TimeSpan ScreenshotInterval);
    }
}
