using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Streaming.Common.EmbedProcesses;

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

        public Task GenerateVideoOverviewScreenshotsAsync(string videoPath, string screenshotOutputDirectory, TimeSpan screenshotInterval)
        {
            throwIfFileNotExists(videoPath);
            double interval = 1 / screenshotInterval.TotalSeconds;
            var command = $"ffmpeg -i '{videoPath}' -filter:v scale=\"140:-1\",fps={interval} '{screenshotOutputDirectory}out%d.jpg'";
            return command.ExecuteBashAsync();
        }

        public Task TakeVideoScreenshotAsync(string videoPath, string screenshotOutputPath, TimeSpan timeSpan)
        {
            throwIfFileNotExists(videoPath);
            var lengthString = $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
            var command = $"ffmpeg -ss {lengthString} -i '{videoPath}' -vframes 1 -q:v 2 '{screenshotOutputPath}'";
            return command.ExecuteBashAsync();
        }

        public async Task ConvertVideoToMp4Async(string videoPath, string outputVideoFile, Action<double> progressCallback = null)
        {
            var details = await GetDetailsAsync(videoPath);
            if (!SupportedVideoCodecs.Contains(details.Video.Codec))
                throw new NotSupportedVideoFileException();
            
            string ffmpegArgs = $"-i {videoPath} -f mp4 ";
            
            if (details.Video.Codec != VideoCodec.h264)
                ffmpegArgs += "-vcodec libx264 -acodec aac";
            else if (details.Audio.Codec != AudioCodec.aac)
                ffmpegArgs += "-vcodec copy -acodec aac";
            else
                ffmpegArgs += "-vcodec copy -acodec copy";
            ffmpegArgs += $" {outputVideoFile}";
            
            using (var process = EmbeddedProcess.Create("ffmpeg", ffmpegArgs))
            {
                if (progressCallback != null)
                    process.AddWatcher(new FFmpegConversionProgressWatcher(progressCallback));
                await process.HandleAsync();
            }
        }

        public async Task<List<string>> SplitMp4FileIntoTsFilesAsync(string mp4VideoFilePath, string outputTsFilesDirectory)
        {
            throwIfFileNotExists(mp4VideoFilePath);
            var uniqueIdentifier = Guid.NewGuid().ToByteArray().ToBase32String();
            var splitCommand = $"ffmpeg -i '{mp4VideoFilePath}' -c copy -map 0 -segment_time 5 -f segment '{outputTsFilesDirectory}{uniqueIdentifier}_%08d.ts'";
            await splitCommand.ExecuteBashAsync();
            return new DirectoryInfo(outputTsFilesDirectory).GetFiles(outputTsFilesDirectory)
                .Where(x => Regex.IsMatch(x.Name, uniqueIdentifier + @"_\d{8}\.ts"))
                .OrderBy(x => x.Name).Select(x => x.FullName).ToList();
        }

        private VideoFileDetailsDTO getVideoDetailsFromOutputString(string output)
        {
            var videoDetails = new VideoFileDetailsDTO();

            // We removing new lines because sometimes ffmpeg can add accidentally newline and as a consequence
            // regex will throw an exception - we only save newlines before "Stream" to correctly recognize multiple streams
            output = output.Replace(Environment.NewLine, String.Empty).Replace("Stream #", $"{Environment.NewLine}Stream #");

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

        private static readonly IEnumerable<VideoCodec> SupportedVideoCodecs = new List<VideoCodec>
        {
            VideoCodec.h264,
            VideoCodec.flv1
        };

        IEnumerable<VideoCodec> IProcessVideoService.SupportedVideoCodecs()
            => SupportedVideoCodecs;

        private void throwIfFileNotExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }
        }
    }
}
