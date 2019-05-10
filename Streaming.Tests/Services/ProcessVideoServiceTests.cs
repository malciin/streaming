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
        private DirectoryInfo workDir;
        private DirectoryInfo tsFilesDir;

        private InputFiles inputFiles;
        
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
            inputFiles = new InputFiles(new DirectoryInfo("_Data/VideoSamples"));
            workDir = Directory.CreateDirectory("_Data/Temporary");
            tsFilesDir = Directory.CreateDirectory(Path.Combine(workDir.FullName, "TSFiles"));
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
            await processVideoService.TakeVideoScreenshotAsync(inputFiles.SampleMp4, screenshotPath, TimeSpan.FromSeconds(2.5));
            Assert.IsTrue(File.Exists(screenshotPath));
            Assert.DoesNotThrow(() => {
                var img = System.Drawing.Image.FromFile(screenshotPath);
                img.Dispose();
            }, "System.Drawing.Image cannot load generated thumbnail");
        }

        [Test]
        public async Task Convert_To_Mp4_With_Progress_Support()
        {
            int calledTimes = 0;

            var outputMp4 = Path.Combine(workDir.FullName, "output.mp4");
            var receivedPercents = new List<double>();
            Assert.DoesNotThrowAsync(() => processVideoService.ConvertVideoToMp4Async(inputFiles.BigBuckBunnyFlv2_5s, outputMp4,
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
            string Strategy(int partNumber) => Path.Combine(tsFilesDir.FullName, $"{partNumber}.ts");
            var orderedTsFiles = await processVideoService.SplitMp4FileIntoTsFilesAsync(inputFiles.BigBuckBunnyAudioOnly120s, 
                Strategy);
            
            int i = 0;
            TimeSpan totalTimeSpan = TimeSpan.Zero;
            foreach (var tsFile in orderedTsFiles)
            {
                var expectedPath = Strategy(i++);
                Assert.AreEqual(tsFile, expectedPath, $"Inproper path! Expected {expectedPath} but was {tsFile}");
                Assert.IsTrue(File.Exists(tsFile), $"{tsFile} not exists!");
                totalTimeSpan = totalTimeSpan.Add(await videoFileInfoService.GetVideoLengthAsync(tsFile));
                
            }
            Assert.IsTrue(totalTimeSpan.EqualWithError(inputFiles.BigBuckBunnyAudioOnly120sLength, maxDurationError), 
                $"Expected {inputFiles.BigBuckBunnyAudioOnly120sLength.TotalMilliseconds}ms += {maxDurationError.TotalMilliseconds}ms but was {totalTimeSpan.TotalMilliseconds}ms");
        }

        [Test]
        public async Task Create_Gif_Test()
        {
            var gifPath = Path.Combine(workDir.FullName, "test.gif");
            await processVideoService.TakeVideoGifAsync(inputFiles.SampleMp4, gifPath, TimeSpan.Zero,
                TimeSpan.FromSeconds(2.5));

            Assert.DoesNotThrow(() => {
                    var img = System.Drawing.Image.FromFile(gifPath);
                    img.Dispose();
                }, "System.Drawing.Image cannot load generated gif");
        }

        private class InputFiles
        {
            private DirectoryInfo samplesDir;

            public string SampleMp4 => samplesDir.GetFiles().First(x => x.Name == "sample.mp4").FullName;
            public string BigBuckBunnyFlv2_5s => samplesDir.GetFiles().First(x => x.Name.Contains("BigBuckBunny2.5s")).FullName;
            public string BigBuckBunnyAudioOnly120s => samplesDir.GetFiles().First(x => x.Name.Contains("BigBuckBunnyAudioOnly120s")).FullName;
            public TimeSpan BigBuckBunnyAudioOnly120sLength => TimeSpan.FromSeconds(120.5);
            
            public InputFiles(DirectoryInfo samplesDir)
            {
                this.samplesDir = samplesDir;
            }
        }
    }
}
