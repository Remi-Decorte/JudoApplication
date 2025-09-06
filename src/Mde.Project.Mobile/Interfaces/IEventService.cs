using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Interfaces
{
    public interface IEventService
    {
        Task<List<EventModel>> GetUpcomingEventsAsync();
    }
}
