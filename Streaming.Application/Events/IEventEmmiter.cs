using System.Threading.Tasks;

namespace Streaming.Application.Events
{
    public interface IEventEmmiter
    {
        Task Emit<T>(T @event) where T : IEvent;
    }
}
