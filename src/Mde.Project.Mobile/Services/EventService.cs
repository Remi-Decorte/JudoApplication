using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

public class EventService : BaseApiService, IEventService
{
    public async Task<List<EventModel>?> GetUpcomingEventsAsync()
    {
        return await ExecuteApiCallAsync<List<EventModel>>(() =>
            _httpClient.GetAsync("events/upcoming"));
    }
}
