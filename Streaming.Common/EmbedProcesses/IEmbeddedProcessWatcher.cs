using System.Diagnostics;

namespace Streaming.Common.EmbedProcesses
{
    public interface IEmbeddedProcessWatcher
    {
        void ErrorDataReceived(object sender, DataReceivedEventArgs e);
        void OutputDataReceived(object sender, DataReceivedEventArgs e);
    }
}