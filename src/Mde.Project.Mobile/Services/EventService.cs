using System.Net.Http.Json;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

public class EventService
{
    private readonly HttpClient _httpClient;

    public EventService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("/") 
        };
    }

    public async Task<List<EventModel>> GetUpcomingEventsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<EventModel>>("api/events/upcoming");
            return response ?? new List<EventModel>();
        }
        catch
        {
            // Eventueel logging of fallback
            return new List<EventModel>();
        }
    }
}
