using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services;

namespace Mde.Project.Mobile.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<EventModel> Events { get; private set; } = new();
        private readonly EventService _eventService = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadEventsAsync()
        {
            IsLoading = true;
            var events = await _eventService.GetUpcomingEventsAsync();
            Events.Clear();

            foreach (var ev in events)
            {
                Events.Add(ev);
            }

            IsLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
