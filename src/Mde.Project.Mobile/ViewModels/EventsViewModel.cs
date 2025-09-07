using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using Microsoft.Maui.Devices.Sensors; // Geolocation

namespace Mde.Project.Mobile.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {
        private readonly IEventService _eventService;

        public EventsViewModel(IEventService eventService)
        {
            _eventService = eventService;

            Events = new ObservableCollection<EventModel>();

            // 1) Commands eerst initialiseren (voorkomt NRE)
            RefreshCommand = new Command(async () => await LoadEventsAsync(), () => !IsBusy);
            ToggleAddBoxCommand = new Command(() => IsAdding = !IsAdding);
            _saveNewEventCommand = new Command(async () => await SaveNewEventAsync(), CanSaveNewEvent);
            UseMyLocationCommand = new Command(async () => await FillMyLocationAsync());

            // 2) Standaardwaarden NA de commands
            _newEventDate = DateTime.Today;
        }

        // ================= Data =================
        public ObservableCollection<EventModel> Events { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); (RefreshCommand as Command)?.ChangeCanExecute(); }
        }

        // ================= Add-box state =================
        private bool _isAdding;
        public bool IsAdding
        {
            get => _isAdding;
            set { _isAdding = value; OnPropertyChanged(); }
        }

        private string _newEventTitle = string.Empty;
        public string NewEventTitle
        {
            get => _newEventTitle;
            set { _newEventTitle = value; OnPropertyChanged(); RaiseSaveCanExecuteChanged(); }
        }

        private DateTime _newEventDate;
        public DateTime NewEventDate
        {
            get => _newEventDate;
            set { _newEventDate = value; OnPropertyChanged(); RaiseSaveCanExecuteChanged(); }
        }

        private string _newEventLocation = string.Empty;
        public string NewEventLocation
        {
            get => _newEventLocation;
            set { _newEventLocation = value; OnPropertyChanged(); RaiseSaveCanExecuteChanged(); }
        }

        // ================= Commands =================
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAddBoxCommand { get; }
        public ICommand UseMyLocationCommand { get; }

        private readonly Command _saveNewEventCommand;
        public ICommand SaveNewEventCommand => _saveNewEventCommand;

        private void RaiseSaveCanExecuteChanged() => _saveNewEventCommand?.ChangeCanExecute();

        private bool CanSaveNewEvent() =>
            !string.IsNullOrWhiteSpace(NewEventTitle)
            && NewEventDate != default
            && !string.IsNullOrWhiteSpace(NewEventLocation);

        // ================= Actions =================
        public async Task LoadEventsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Events.Clear();
                var list = await _eventService.GetUpcomingEventsAsync();
                if (list != null)
                    foreach (var e in list) Events.Add(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveNewEventAsync()
        {
            if (!CanSaveNewEvent()) return;

            // Simple local add (geen backend post – hou het simpel)
            Events.Insert(0, new EventModel
            {
                Title = NewEventTitle.Trim(),
                Date = NewEventDate,
                Location = NewEventLocation.Trim()
            });

            // reset form & verberg add-box
            NewEventTitle = string.Empty;
            NewEventDate = DateTime.Today;
            NewEventLocation = string.Empty;
            IsAdding = false;

            await Application.Current?.MainPage?.DisplayAlert("Event", "Event toegevoegd.", "OK");
        }

        private async Task FillMyLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    // Simpel: toon coördinaten. (Reverse geocoding kan later.)
                    NewEventLocation = $"{location.Latitude:0.0000}, {location.Longitude:0.0000}";
                }
                else
                {
                    await Application.Current?.MainPage?.DisplayAlert("Locatie", "Kon je locatie niet bepalen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Locatie", $"Fout: {ex.Message}", "OK");
            }
        }

        // ================= INotifyPropertyChanged =================
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}