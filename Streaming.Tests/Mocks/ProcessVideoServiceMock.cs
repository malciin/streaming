using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.Enum;

namespace Streaming.Tests.Mocks
{
    public static class ProcessVideoServiceMock
    {
        public static Mock<IProcessVideoService> CreateForData(int howManyTsFilesToGenerate)
        {
            var mock = new Mock<IProcessVideoService>();
            mock.Setup(x => x.SupportedVideoCodecs())
                .Returns(new List<VideoCodec> {VideoCodec.h264});
            mock.Setup(x => x.ConvertVideoToMp4Async(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<double>>()))
                .Returns((string path, string output, Action<double> callback) =>
                {
                    File.Create(output).Dispose();
                    return Task.FromResult(0);
                });
            mock.Setup(x => x.TakeVideoScreenshotAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns((string path, string output, TimeSpan timespan) =>
                {
                    File.Create(output).Dispose();
                    return Task.FromResult(0);
                });
            mock.Setup(x => x.GenerateVideoOverviewScreenshotsAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<Func<ScreenshotGenerationContext, string>>()))
                .Returns((string path, string output, TimeSpan interval) =>
                {
                    return Task.FromResult(new List<string> {});
                });
            mock.Setup(x => x.SplitMp4FileIntoTsFilesAsync(It.IsAny<string>(), It.IsAny<Func<int, string>>()))
                .Returns((string path, Func<int, string> namingStrategy) =>
                {
                    var tsFiles = new List<string>();
                    for (int i = 0; i < howManyTsFilesToGenerate; i++)
                    {
                        var fileName = namingStrategy(i);
                        tsFiles.Add(fileName);
                        File.Create(fileName).Dispose();
                    }

                    return Task.FromResult(tsFiles);
                });
            return mock;
        }
    }
}