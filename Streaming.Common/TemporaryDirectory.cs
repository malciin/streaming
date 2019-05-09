using System;
using System.IO;

namespace Streaming.Common
{
    public class TemporaryDirectory : IDisposable
    {
        private DirectoryInfo directoryInfo;
        public string FullName => directoryInfo.FullName;

        public FileInfo[] GetFiles() => directoryInfo.GetFiles();

        public static TemporaryDirectory CreateDirectory(string parentDirectoryPath = "")
        {
            var tempDir = new TemporaryDirectory();
            tempDir.directoryInfo = Directory.CreateDirectory(Path.Combine(parentDirectoryPath, Path.GetRandomFileName()));
            return tempDir;
        }

        public void Dispose()
        {
            directoryInfo.Delete(true);
        }
    }
}