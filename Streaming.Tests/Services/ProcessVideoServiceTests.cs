using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Streaming.Application.Interfaces.Services;
using Streaming.Common.Extensions;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    class ProcessVideoServiceTests
    {
        private IProcessVideoService processVideoService;
        private DirectoryInfo videoSamplesDir;
        private DirectoryInfo workDir;

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
            processVideoService = builder.Build().Resolve<IProcessVideoService>();
            videoSamplesDir = new DirectoryInfo("_Data/VideoSamples");
            workDir = Directory.CreateDirectory("_Data/Temporary");
        }

        [TearDown]
        public void Teardown()
        {
            workDir.Delete(true);
        }

        [Test]
        public void Conversion_Progress_Callback_Test()
        {
            var bigBuckBunny2_5sFLV = videoSamplesDir.GetFiles().First(x => x.Name.Contains("BigBuckBunny2.5s")).FullName;
            var stopwatch = Stopwatch.StartNew();

            int calledTimes = 0;
            var receivedPercents = new List<double>();
            Assert.DoesNotThrow(() => processVideoService.ConvertVideoToMp4Async(bigBuckBunny2_5sFLV, Path.Combine(workDir.FullName, "output.mp4"),
                progress =>
                {
                    calledTimes++;
                    receivedPercents.Add(progress);
                    Console.WriteLine($"[{DateTime.UtcNow.ToString("HH:mm:ss.fffff")} UTC] So far processed {progress}%");
                }).GetAwaiter().GetResult());
            stopwatch.Stop();
            Console.WriteLine($"Finished processing after {stopwatch.ElapsedMilliseconds}ms");
            
            Assert.Greater(calledTimes, 1, $"Callback called only once! This indicates some " +
                                           $"failure because to process input video " +
                                           $"2 seconds is needed at least");
            Assert.IsTrue(receivedPercents.IsSortedAscending(x => x));
        }
    }
}
