using Streaming.Common.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class FFmpegProcessVideoService : IProcessVideoService
    {
        public async Task<TimeSpan> GetVideoLengthAsync(string VideoPath)
        {
            var videoLengthCmd = $"ffprobe -v error -show_entries " + 
                "format=duration -of default=noprint_wrappers=1:nokey=1 '" +
                VideoPath + "'";
            var result = (await videoLengthCmd.ExecuteBashAsync())
                    .Replace("\r\n", String.Empty).Replace("\n", String.Empty);

            return TimeSpan.FromSeconds(double.Parse(result));
        }

        public async Task ProcessVideoAsync(string VideoPath, string OutputDirectoryPath)
        {
            var copyVideoCmd = $"ffmpeg -i '{VideoPath}' -c copy '{OutputDirectoryPath}ts_file.ts'";

            await copyVideoCmd.ExecuteBashAsync();

            var splitVideoIntoPartsCmd = "ffmpeg -i " +
                $"'{VideoPath}' " +
                "-c copy -map 0 -segment_time 5 -bsf:v h264_mp4toannexb -f segment " +
                $"'{OutputDirectoryPath}%03d.ts'";

            await splitVideoIntoPartsCmd.ExecuteBashAsync();
        }

        public async Task GenerateVideoOverviewScreenshots(string VideoPath, string ScreenshotOutputDirectory, TimeSpan ScreenshotInterval)
        {
            double interval = 1 / ScreenshotInterval.TotalSeconds;
            var command = $"ffmpeg -i '{VideoPath}' -filter:v scale=\"140:-1\",fps={interval} '{ScreenshotOutputDirectory}out%d.jpg'";
            await command.ExecuteBashAsync();
        }

        public async Task TakeVideoScreenshot(string VideoPath, string ScreenshotOutputPath, TimeSpan Time)
        {
            var lengthString = $"{Time.Hours}:{Time.Minutes}:{Time.Seconds}";
            var command = $"ffmpeg -ss {lengthString} -i '{VideoPath}' -vframes 1 -q:v 2 '{ScreenshotOutputPath}'";
            await command.ExecuteBashAsync();
        }
    }
}
