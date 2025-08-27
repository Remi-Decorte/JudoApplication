using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {
        private readonly IEventService _eventService;

        public EventsViewModel(IEventService eventService)
        {
            _eventService = eventService;
            Events = new ObservableCollection<EventModel>();
            RefreshCommand = new Command(async () => await LoadEventsAsync());
        }

        public ObservableCollection<EventModel> Events { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }

        public async Task LoadEventsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Events.Clear();
                var list = await _eventService.GetUpcomingEventsAsync();
                if (list != null)
                {
                    foreach (var e in list)
                        Events.Add(e);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
