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
using Newtonsoft.Json.Linq;
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
                await process.GetResultAsync();
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

        public async Task<VideoFileDetailsDTO> GetDetailsAsync(string videoPath)
        {
            throwIfFileNotExists(videoPath);
            var commandResult = await EmbeddedProcess.Create("ffprobe", $"-v quiet -print_format json -show_format -show_streams {videoPath}").GetExecutionResultAsync();
            return ParseJsonOutput(videoPath, commandResult.StandardOutput);
        }

        private static VideoFileDetailsDTO ParseJsonOutput(string videoPath, string commandResult)
        {
            var videoFileInfoJson = JObject.Parse(commandResult);
            var videoInfo = videoFileInfoJson["streams"]?.FirstOrDefault(x => x["codec_type"].Value<string>() == "video");
            var audioInfo = videoFileInfoJson["streams"]?.FirstOrDefault(x => x["codec_type"].Value<string>() == "audio");
            if (videoInfo == null)
            {
                throw new NotVideoFileException(videoPath);
            }

            var format = videoFileInfoJson["format"];

            var details = new VideoFileDetailsDTO()
            {
                Video =
                {
                    Codec = (VideoCodec) Enum.Parse(typeof(VideoCodec), videoInfo["codec_name"].Value<string>()),
                    Resolution = (videoInfo["width"].Value<int>(), videoInfo["height"].Value<int>()),
                    BitrateKbs = videoInfo["bit_rate"]?.Value<int>()
                },
                Duration = TimeSpan.FromSeconds(format["duration"].Value<double>()),
                SizeBytes = format["size"].Value<int>()
            };
            if (audioInfo != null)
            {
                details.Audio.Codec = (AudioCodec) Enum.Parse(typeof(AudioCodec), audioInfo["codec_name"].Value<string>());
            }

            return details;
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
