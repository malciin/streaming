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
                "format=duration -of default=noprint_wrappers=1:nokey=1 " +
                VideoPath;
            var result = (await videoLengthCmd.ExecuteBashAsync())
                    .Replace("\r\n", String.Empty).Replace("\n", String.Empty);

            return TimeSpan.FromSeconds(double.Parse(result));
        }

        public async Task ProcessVideoAsync(string VideoPath, string OutputDirectoryPath)
        {
            var copyVideoCmd = $"ffmpeg -i {VideoPath} -c copy {OutputDirectoryPath}ts_file.ts";

            await copyVideoCmd.ExecuteBashAsync();

            var splitVideoIntoPartsCmd = String.Format("ffmpeg -i " +
                $"{VideoPath} " +
                "-c copy -map 0 -segment_time 5 -f segment " +
                $"{OutputDirectoryPath}{{0}}%03d.ts", Path.DirectorySeparatorChar);

            await splitVideoIntoPartsCmd.ExecuteBashAsync();
        }
    }
}
