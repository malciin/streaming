using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Streaming.Application.Interfaces.Services;
using Streaming.Common.Extensions;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    class ProcessVideoServiceTests
    {
        #region TestSetup

        private IProcessVideoService processVideoService;
        private IVideoFileInfoService videoFileInfoService;

        private TimeSpan maxDurationError = TimeSpan.FromMilliseconds(500);
        private DirectoryInfo videoSamplesDir;
        private DirectoryInfo workDir;

        private string SampleMp4 => videoSamplesDir.GetFiles().Where(x => x.Name == "sample.mp4").First().FullName;
        private readonly TimeSpan SampleMp4Duration = TimeSpan.FromMilliseconds(5_700);


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            try {
                Directory.Delete("_Data/Temporary", true);
            }
            catch (DirectoryNotFoundException) { }
        }

        [SetUp]
        public void Setup() 
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ServicesModule>();
            var container = builder.Build();
            processVideoService = container.Resolve<IProcessVideoService>();
            videoFileInfoService = container.Resolve<IVideoFileInfoService>();
            videoSamplesDir = new DirectoryInfo("_Data/VideoSamples");
            workDir = Directory.CreateDirectory("_Data/Temporary");
        }

        [TearDown]
        public void Teardown()
        {
            workDir.Delete(true);
        }

        #endregion

        [Test]
        public async Task Thumbnail_Generate()
        {
            var screenshotPath = Path.Combine(workDir.FullName, "screen.jpg");
            await processVideoService.TakeVideoScreenshotAsync(SampleMp4, screenshotPath, TimeSpan.FromSeconds(2.5));
            Assert.IsTrue(File.Exists(screenshotPath));
        }

        [Test]
        public async Task Convert_To_Mp4_With_Progress_Support()
        {
            var bigBuckBunny2_5sFLV = videoSamplesDir.GetFiles().First(x => x.Name.Contains("BigBuckBunny2.5s")).FullName;
            int calledTimes = 0;

            var outputMp4 = Path.Combine(workDir.FullName, "output.mp4");
            var receivedPercents = new List<double>();
            Assert.DoesNotThrowAsync(() => processVideoService.ConvertVideoToMp4Async(bigBuckBunny2_5sFLV, outputMp4,
                progress =>
                {
                    calledTimes++;
                    receivedPercents.Add(progress);
                    Console.WriteLine($"[{DateTime.UtcNow.ToString("HH:mm:ss.fffff")} UTC] So far processed {progress}%");
                }));
            
            Assert.IsTrue(File.Exists(outputMp4), $"{outputMp4} not exists!");
            Assert.IsTrue((await videoFileInfoService.GetVideoLengthAsync(outputMp4)).EqualWithError(TimeSpan.FromSeconds(2.5), maxDurationError));
            Assert.Greater(calledTimes, 1, $"Callback called only once! This indicates some " +
                                           $"failure because to process input video " +
                                           $"2 seconds is needed at least");
            Assert.IsTrue(receivedPercents.IsSortedAscending(x => x));
        }

        [Test]
        public async Task Convert_To_TSFiles()
        {
            var tsFilesDir = Directory.CreateDirectory(Path.Combine(workDir.FullName, "TSFiles"));
            string NameStrategy(int part) => Path.Combine(tsFilesDir.FullName, $"{part}.ts");
            
            // TODO: Add another test that checks if returned orderedTsFiles is correctly ordered (9 -> 10 -> 11) not in lexical sort but numeric
            var orderedTsFiles = await processVideoService.SplitMp4FileIntoTsFilesAsync(SampleMp4, NameStrategy);
            int i = 0;
            
            TimeSpan totalTimeSpan = TimeSpan.Zero;
            foreach (var tsFile in orderedTsFiles)
            {
                totalTimeSpan = totalTimeSpan.Add(await videoFileInfoService.GetVideoLengthAsync(tsFile));
                var expectedPath = NameStrategy(i++);
                Assert.AreEqual(tsFile, expectedPath, $"Inproper path! Expected {expectedPath} but was {tsFile}");
            }
            Assert.IsTrue(totalTimeSpan.EqualWithError(SampleMp4Duration, maxDurationError), 
                $"Expected {SampleMp4Duration.TotalMilliseconds}ms += {maxDurationError.TotalMilliseconds}ms but was {totalTimeSpan.TotalMilliseconds}ms");
        }
    }
}
