using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class EventService : BaseApiService, IEventService
    {
        public Task<List<EventModel>?> GetUpcomingEventsAsync() =>
            ExecuteApiCallAsync<List<EventModel>>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/Events/upcoming");
                System.Diagnostics.Debug.WriteLine("GET " + url);
                return _httpClient.GetAsync(url);
            });
    }
}
