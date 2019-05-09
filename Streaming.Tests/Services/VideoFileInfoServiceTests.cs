using Autofac;
using NUnit.Framework;
using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using System;
using System.IO;
using System.Linq;
using Streaming.Common.Extensions;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    class VideoFileInfoServiceTests
    {
        private DirectoryInfo videoSamplesPath;
        private IVideoFileInfoService videoFileInfoService;

        [OneTimeSetUp]
        public void AssertFFmpegIsInstalled()
        {
            Assert.DoesNotThrowAsync(() => "ffmpeg -version".ExecuteBashAsync(), $"Ensure that FFmpeg is installed!");
        }

        [SetUp]
        public void Setup()
        {
            var container = new ContainerBuilder();
            container.RegisterModule<ServicesModule>();
            var componentContext = container.Build();
            videoFileInfoService = componentContext.Resolve<IVideoFileInfoService>();
            videoSamplesPath = new DirectoryInfo("_Data/VideoSamples");
        }

        [Test]
        public void File_Not_Found_Exception_Should_Be_Thrown_When_File_Not_Exists() { 
            Assert.ThrowsAsync<FileNotFoundException>(() => videoFileInfoService.GetDetailsAsync("Unexisting/Path"));
            Assert.ThrowsAsync<FileNotFoundException>(() => videoFileInfoService.GetVideoLengthAsync("Unexisting/Path"));
        }

        [Test]
        public void Not_Video_File_Exception_Should_Be_Thrown_When_File_Is_Not_Video()
        {
            Assert.ThrowsAsync<NotVideoFileException>(() => videoFileInfoService.GetDetailsAsync("_Data/VideoSamples/Not_Video_File.dat"));
        }

        private string GetFilePath(string extension) =>
            videoSamplesPath.GetFiles()
                            .First(x => x.Name == $"sample{extension}")
                            .FullName;
            
        private readonly TimeSpan maxDurationError = TimeSpan.FromMilliseconds(250);

        private void TestVideoFormatInfo(string sampleExtension, VideoFileDetailsDTO expected)
        {
            var result = videoFileInfoService.GetDetailsAsync(GetFilePath(sampleExtension)).GetAwaiter().GetResult();
            var durationFromSpecifiedMethod = videoFileInfoService.GetVideoLengthAsync(GetFilePath(sampleExtension)).GetAwaiter().GetResult();
            
            Assert.True(durationFromSpecifiedMethod.EqualWithError(expected.Duration, maxDurationError), 
                $"Wrong duration from duration only method! Expected {expected.Duration.TotalMilliseconds}ms but gets {durationFromSpecifiedMethod.TotalMilliseconds}ms " +
                $"with max allowed duration error of {maxDurationError.TotalMilliseconds}ms");
            
            Assert.True(result.Duration.EqualWithError(expected.Duration, maxDurationError), 
                $"Wrong duration! Expected {expected.Duration.TotalMilliseconds}ms but gets {result.Duration.TotalMilliseconds}ms " + 
                $"with max allowed duration error of {maxDurationError.TotalMilliseconds}ms");
            
            Assert.AreEqual(expected.Video.Resolution, result.Video.Resolution,
                $"Wrong resolution! Expected {expected.Video.Resolution.xResolution}x{expected.Video.Resolution.yResolution}" +
                $" but gets {result.Video.Resolution.xResolution}x{result.Video.Resolution.yResolution}"); 
            
            Assert.AreEqual(expected.Video.Codec, result.Video.Codec, $"Wrong codec! Expected {expected.Video.Codec} but gets {result.Video.Codec}");
        }

        [Test]
        public void Valid_Video_Info_For_3gp_Format() =>
            TestVideoFormatInfo(".3gp", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (352, 288),
                    BitrateKbs = 493,
                    Codec = VideoCodec.h263
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.amr_nb
                },
                Duration = TimeSpan.FromMilliseconds(5_570)
            });

        [Test]
        public void Valid_Video_Info_For_Avi_Format() =>
            TestVideoFormatInfo(".avi", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (320, 240),
                    BitrateKbs = 540,
                    Codec = VideoCodec.mpeg4
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.mp3
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Flv_Format() =>
            TestVideoFormatInfo(".flv", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (320, 240),
                    BitrateKbs = 436,
                    Codec = VideoCodec.flv1
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.mp3
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_M4v_Format() =>
            TestVideoFormatInfo(".m4v", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 256,
                    Codec = VideoCodec.h264
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.aac
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Mkv_Format() =>
            TestVideoFormatInfo(".mkv", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 598,
                    Codec = VideoCodec.h264
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.aac
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Test_Mov_Format() =>
            TestVideoFormatInfo(".mov", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 674,
                    Codec = VideoCodec.mpeg4
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.aac
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Mp4_Format() =>
            TestVideoFormatInfo(".mp4", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 551,
                    Codec = VideoCodec.h264
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.aac
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Mpeg_Format() =>
            TestVideoFormatInfo(".mpeg", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 939,
                    Codec = VideoCodec.mpeg2video
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.mp2
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Mpg_Format() =>
            TestVideoFormatInfo(".mpg", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (352, 240),
                    BitrateKbs = 970,
                    Codec = VideoCodec.mpeg1video
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.mp2
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Ignore("Currently ffmpeg probe have issue with that .rmvb file with error: " +
            "[rm @ 0000019ccd9abe80] Invalid stream index 2 for index at pos 1214774")]
        [Test]
        public void Valid_Video_Info_For_Rmvb_Format() =>
            TestVideoFormatInfo(".rmvb", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (360, 480),
                    BitrateKbs = 871,
                    Codec = VideoCodec.rv40
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.cook
                },
                Duration = TimeSpan.FromMilliseconds(11_150)
            });

        [Test]
        public void Valid_Video_Info_For_Vob_Format() =>
            TestVideoFormatInfo(".vob", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (560, 320),
                    BitrateKbs = 884,
                    Codec = VideoCodec.mpeg2video
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.mp2
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });

        [Test]
        public void Valid_Video_Info_For_Webm_Format() =>
            TestVideoFormatInfo(".webm", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (640, 360),
                    BitrateKbs = 256,
                    Codec = VideoCodec.vp8
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.vorbis
                },
                Duration = TimeSpan.FromMilliseconds(10_500)
            });

        [Test]
        public void Valid_Video_Info_For_Wmv_Format() =>
            TestVideoFormatInfo(".wmv", new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Resolution = (320, 240),
                    BitrateKbs = 793,
                    Codec = VideoCodec.wmv2
                },
                Audio = new VideoFileDetailsDTO.AudioDetailsDTO
                {
                    Codec = AudioCodec.wmav2
                },
                Duration = TimeSpan.FromMilliseconds(5_700)
            });
    }
}
