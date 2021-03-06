﻿using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    public class FFmpegProcessVideoService : IProcessVideoService, IVideoFileInfoService
    {
        public async Task<TimeSpan> GetVideoLengthAsync(string videoPath)
        {
            throwIfFileNotExists(videoPath);
            var videoLengthCmd = $"ffprobe -v error -show_entries " + 
                "format=duration -of default=noprint_wrappers=1:nokey=1 '" +
                videoPath + "'";
            var result = (await videoLengthCmd.ExecuteBashAsync())
                    .Replace("\r\n", String.Empty).Replace("\n", String.Empty);

            return TimeSpan.FromSeconds(double.Parse(result));
        }

        public Task GenerateVideoOverviewScreenshots(string videoPath, string screenshotOutputDirectory, TimeSpan screenshotInterval)
        {
            throwIfFileNotExists(videoPath);
            double interval = 1 / screenshotInterval.TotalSeconds;
            var command = $"ffmpeg -i '{videoPath}' -filter:v scale=\"140:-1\",fps={interval} '{screenshotOutputDirectory}out%d.jpg'";
            return command.ExecuteBashAsync(BashExtensions.DefaultOutput.ErrorOutput);
        }

        public Task TakeVideoScreenshot(string videoPath, string screenshotOutputPath, TimeSpan timeSpan)
        {
            throwIfFileNotExists(videoPath);
            var lengthString = $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
            var command = $"ffmpeg -ss {lengthString} -i '{videoPath}' -vframes 1 -q:v 2 '{screenshotOutputPath}'";
            return command.ExecuteBashAsync(BashExtensions.DefaultOutput.ErrorOutput);
        }

        public Task ConvertVideoToMp4(string videoPath, string outputVideoFile, Action<string> commandLineOutputCallback = null)
        {
            throwIfFileNotExists(videoPath);
            var convertMp4Command = $"ffmpeg -i '{videoPath}' -f mp4 -vcodec libx264 -acodec aac '{outputVideoFile}'";
            return convertMp4Command.ExecuteBashAsync(BashExtensions.DefaultOutput.ErrorOutput, commandLineOutputCallback);
        }

        public Task SplitMp4FileIntoTSFiles(string mp4VideoFilePath, string outputTsFilesDirectory)
        {
            throwIfFileNotExists(mp4VideoFilePath);
            var splitCommand = $"ffmpeg -i '{mp4VideoFilePath}' -c copy -map 0 -segment_time 5 -f segment '{outputTsFilesDirectory}%03d.ts'";
            return splitCommand.ExecuteBashAsync(BashExtensions.DefaultOutput.ErrorOutput);
        }

        private VideoFileDetailsDTO getVideoDetailsFromOutputString(string output)
        {
            var videoDetails = new VideoFileDetailsDTO();

            var durationAndBitrateRegex = Regex.Match(output, @"Duration: (?<duration>(\d+[:\.]?)+).*bitrate: (?<bitrate>\d+) kb\/s");
            var duration = durationAndBitrateRegex.Groups["duration"].Value;
            videoDetails.Duration = TimeSpan.ParseExact(duration, @"hh\:mm\:ss\.ff", CultureInfo.InvariantCulture);
            videoDetails.Video.BitrateKbs = int.Parse(durationAndBitrateRegex.Groups["bitrate"].Value);

            var videoTypeRegex = Regex.Match(output, @"Stream #\d:\d+.*Video\: (?<codec>[^ ,]+).* (?<xResolution>\d+)x(?<yResolution>\d+)");
            videoDetails.Video.Codec = (VideoCodec)Enum.Parse(typeof(VideoCodec), videoTypeRegex.Groups["codec"].Value);
            videoDetails.Video.Resolution = (int.Parse(videoTypeRegex.Groups["xResolution"].Value), int.Parse(videoTypeRegex.Groups["yResolution"].Value));

            var audioTypeRegex = Regex.Match(output, @"Stream #\d:\d+.*Audio\: (?<codec>[^ ,]+)");
            videoDetails.Audio.Codec = (AudioCodec)Enum.Parse(typeof(AudioCodec), audioTypeRegex.Groups["codec"].Value);

            return videoDetails;
        }

        public async Task<VideoFileDetailsDTO> GetDetailsAsync(string videoPath)
        {
            throwIfFileNotExists(videoPath);
            using (var process = $"ffmpeg -i '{videoPath}'".StartBashExecution())
            {
                // We manually create process and read from StandardError, because ffmpeg returns
                // an error and '-1' error code when we don't specify the output file
                var output = await process.StandardError.ReadToEndAsync();

                try
                {
                    return getVideoDetailsFromOutputString(output);
                }
                catch (Exception inner)
                {
                    throw new NotVideoFileException(videoPath, inner);
                }
            }
        }

        IEnumerable<VideoCodec> IProcessVideoService.SupportedVideoCodecs()
            => new List<VideoCodec>
            {
                VideoCodec.h264
            };

        private void throwIfFileNotExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }
        }
    }
}
