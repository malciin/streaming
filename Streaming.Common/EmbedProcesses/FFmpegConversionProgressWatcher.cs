using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Streaming.Common.EmbedProcesses
{
    public class FFmpegConversionProgressWatcher : IEmbeddedProcessWatcher
    {
        private TimeSpan? videoLength;
        private readonly Action<double> progressCallback;

        public FFmpegConversionProgressWatcher(Action<double> progressCallback)
        {
            this.progressCallback = progressCallback;
        }
        
        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            if (videoLength == null)
            {
                var match = Regex.Match(e.Data, @"  Duration: (\d{2}:\d{2}:\d{2}.\d{2})");
                if (match.Success)
                {
                    videoLength = TimeSpan.ParseExact(match.Groups[1].Value, "hh':'mm':'ss'.'ff", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                var match = Regex.Match(e.Data, @"frame.*time=(\d{2}:\d{2}:\d{2}.\d{2})");
                if (match.Success)
                {
                    var currentProgress = TimeSpan.ParseExact(match.Groups[1].Value, "hh':'mm':'ss'.'ff", CultureInfo.InvariantCulture);
                    var percent = currentProgress.TotalSeconds / videoLength.Value.TotalSeconds * 100;
                    if (percent > 100) // it can happend (surprisingly) because ffmpeg duration of original file
                        percent = 100; // can differ by some milliseconds than converted video to new videoformat
                        
                    progressCallback(percent);
                }
            }
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
        }
    }
}