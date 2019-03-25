using System.Threading.Tasks;

namespace Streaming.Application.Events
{
    public interface IEventReceiver<T> where T : IEvent
    {
        Task Receive(T @event);
    }
}