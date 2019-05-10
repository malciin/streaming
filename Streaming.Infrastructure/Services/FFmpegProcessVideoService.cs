using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Streaming.Common;
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

        public async Task TakeVideoGifAsync(string videoPath, string gifOutputPath, TimeSpan startTime, TimeSpan gifLength)
        {
            var pallete = $"{Path.GetTempFileName()}.png";
            var generatePalleteProcess = EmbeddedProcess.Create("ffmpeg", 
                $"-y -ss {startTime.TotalSeconds} -t {gifLength.TotalSeconds} -i \"{videoPath}\" " +
                $"-vf fps=10,scale=120:-1:flags=lanczos,palettegen {pallete}");
            var generateGifProcess = EmbeddedProcess.Create("ffmpeg",
                $"-ss {startTime.TotalSeconds} -t {gifLength.TotalSeconds} -i \"{videoPath}\" -i \"{pallete}\" " +
                $"-filter_complex \"fps=10,scale=120:-1:flags=lanczos[x];[x][1:v]paletteuse\" {gifOutputPath}");

            try
            {
                await generatePalleteProcess.GetResultAsync();
                await generateGifProcess.GetResultAsync();
            }
            finally
            {
                generatePalleteProcess.Dispose();
                generateGifProcess.Dispose();
                File.Delete(pallete);
            }
        }

        public async Task<List<string>> GenerateVideoOverviewScreenshotsAsync(string mp4VideoFilePath, TimeSpan screenshotInterval,
            Func<ScreenshotGenerationContext, string> screenshotFilesPathStrategy)
        {
            throwIfFileNotExists(mp4VideoFilePath);
            double interval = 1 / screenshotInterval.TotalSeconds;
            
            var uniqueIdentifier = Guid.NewGuid().ToByteArray().ToBase32String();
            using (var tempDirectory = TemporaryDirectory.CreateDirectory())
            {
                var command = $"ffmpeg -i '{mp4VideoFilePath}' -filter:v scale=\"140:-1\",fps={interval} '{Path.Combine(tempDirectory.FullName, $"{uniqueIdentifier}_%08d.jpg")}'";
                await command.ExecuteBashAsync();
            
                var files = tempDirectory.GetFiles()
                    .Where(x => Regex.IsMatch(x.Name, uniqueIdentifier + @"_\d{8}\.ts"))
                    .OrderBy(x => x.Name).Select(x => x.FullName).ToList();
            
                for (int i = 0; i < files.Count; i++)
                {
                    var newPath = screenshotFilesPathStrategy(new ScreenshotGenerationContext
                    {
                        ScreenshotNumber = i,
                        ScreenshotTime = TimeSpan.FromSeconds(screenshotInterval.TotalSeconds * i)
                    });
                    File.Move(files[i], newPath);
                    files[i] = newPath;
                }

                return files;
            }
        }

        public async Task TakeVideoScreenshotAsync(string videoPath, string screenshotOutputPath, TimeSpan timeSpan)
        {
            throwIfFileNotExists(videoPath);
            var lengthString = $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
            using (var process = EmbeddedProcess.Create("ffmpeg", $"-ss {lengthString} -i \"{videoPath}\" -vframes 1 -q:v 2 \"{screenshotOutputPath}\""))
            {
                await process.GetResultAsync();
            }
        }

        public async Task ConvertVideoToMp4Async(string videoPath, string outputVideoFile, Action<double> progressCallback = null)
        {
            var details = await GetDetailsAsync(videoPath);
            if (!SupportedVideoCodecs.Contains(details.Video.Codec))
                throw new NotSupportedVideoFileException();
            
            string ffmpegArgs = $"-i \"{videoPath}\" -f mp4 ";
            
            if (details.Video.Codec != VideoCodec.h264)
                ffmpegArgs += "-vcodec libx264 -acodec aac";
            else if (details.Audio.Codec != AudioCodec.aac)
                ffmpegArgs += "-vcodec copy -acodec aac";
            else
                ffmpegArgs += "-vcodec copy -acodec copy";
            ffmpegArgs += $" \"{outputVideoFile}\"";
            
            using (var process = EmbeddedProcess.Create("ffmpeg", ffmpegArgs))
            {
                if (progressCallback != null)
                    process.AddWatcher(new FFmpegConversionProgressWatcher(progressCallback));
                await process.GetResultAsync();
            }
        }

        public async Task<List<string>> SplitMp4FileIntoTsFilesAsync(string mp4VideoFilePath, Func<int, string> tsFilesPathStrategy)
        {
            throwIfFileNotExists(mp4VideoFilePath);
            var uniqueIdentifier = Guid.NewGuid().ToByteArray().ToBase32String();
            using (var tempDirectory = TemporaryDirectory.CreateDirectory())
            {
                using (var splitProcess = EmbeddedProcess.Create("ffmpeg",
                    $"-i \"{mp4VideoFilePath}\" -c copy -map 0 -segment_time 4 -f segment \"" +
                    Path.Combine(tempDirectory.FullName, $"{uniqueIdentifier}_%08d.ts") + "\""))
                {
                    await splitProcess.GetResultAsync();
                }

                var files = tempDirectory.GetFiles()
                    .Where(x => Regex.IsMatch(x.Name, uniqueIdentifier + @"_\d{8}\.ts"))
                    .OrderBy(x => x.Name).Select(x => x.FullName).ToList();
            
                for (int i = 0; i < files.Count; i++)
                {
                    var newPath = tsFilesPathStrategy(i);
                    File.Move(files[i], newPath);
                    files[i] = newPath;
                }
                return files;
            }
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
