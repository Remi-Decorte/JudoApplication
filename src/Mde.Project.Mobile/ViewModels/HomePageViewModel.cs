using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services.Interfaces;

namespace Mde.Project.Mobile.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private readonly IEventService _eventService;

        public HomePageViewModel(IEventService eventService)
        {
            _eventService = eventService;
        }

        public ObservableCollection<EventModel> Events { get; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public async Task LoadEventsAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                Events.Clear();
                var events = await _eventService.GetUpcomingEventsAsync();
                foreach (var ev in events) Events.Add(ev);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
