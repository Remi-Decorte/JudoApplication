using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockEventService : IEventService
    {
        public Task<List<EventModel>> GetUpcomingEventsAsync()
        {
            var today = DateTime.Today;
            return Task.FromResult(new List<EventModel>
            {
                new() { Title = "Local Randori", Date = today.AddDays(7) },
                new() { Title = "Strength Camp", Date = today.AddDays(14) },
                new() { Title = "Technique Workshop", Date = today.AddDays(21) },
            });
        }
    }
}

