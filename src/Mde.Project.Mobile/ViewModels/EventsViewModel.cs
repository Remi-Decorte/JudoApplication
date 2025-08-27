using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using Microsoft.Maui;                     // MainThread, Application
using Microsoft.Maui.Storage;            // FileSystem
using Microsoft.Maui.Devices.Sensors;    // Geolocation

namespace Mde.Project.Mobile.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {
        private readonly IEventService _eventService;

        private const string LocalFileName = "events_local.json";
        private string LocalPath => Path.Combine(FileSystem.AppDataDirectory, LocalFileName);

        public EventsViewModel(IEventService eventService)
        {
            _eventService = eventService;

            Events = new ObservableCollection<EventModel>();
            RefreshCommand = new Command(async () => await LoadEventsAsync());

            // add-box defaults
            NewEventDate = DateTime.Today.AddDays(1);

            // add-box commands
            ToggleAddBoxCommand = new Command(() => IsAdding = !IsAdding);
            UseMyLocationCommand = new Command(async () => await FillMyLocationAsync());
            SaveNewEventCommand = new Command(async () => await SaveNewEventAsync(), CanSaveNewEvent);
        }

        // ========== lijst ==========
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

                // 1) van API
                var fromApi = await _eventService.GetUpcomingEventsAsync();
                if (fromApi != null)
                    foreach (var e in fromApi) Events.Add(e);

                // 2) + lokaal toegevoegde events
                var local = await LoadLocalAsync();
                if (local != null)
                {
                    // alleen toekomstige events tonen
                    foreach (var e in local.Where(x => x.Date >= DateTime.Today))
                        Events.Add(e);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ========== add-box state ==========
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
            set { _newEventTitle = value; OnPropertyChanged(); ((Command)SaveNewEventCommand).ChangeCanExecute(); }
        }

        private DateTime _newEventDate;
        public DateTime NewEventDate
        {
            get => _newEventDate;
            set { _newEventDate = value; OnPropertyChanged(); ((Command)SaveNewEventCommand).ChangeCanExecute(); }
        }

        private string _newEventLocation = string.Empty;
        public string NewEventLocation
        {
            get => _newEventLocation;
            set { _newEventLocation = value; OnPropertyChanged(); ((Command)SaveNewEventCommand).ChangeCanExecute(); }
        }

        public ICommand ToggleAddBoxCommand { get; }
        public ICommand UseMyLocationCommand { get; }
        public ICommand SaveNewEventCommand { get; }

        private bool CanSaveNewEvent()
            => !string.IsNullOrWhiteSpace(NewEventTitle)
               && NewEventDate != default;

        private async Task FillMyLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    NewEventLocation = $"{location.Latitude:0.00000},{location.Longitude:0.00000}";
                }
                else
                {
                    await Application.Current?.MainPage?.DisplayAlert("Locatie", "Kon je locatie niet bepalen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Locatie", $"Geen toegang of niet beschikbaar: {ex.Message}", "OK");
            }
        }

        private async Task SaveNewEventAsync()
        {
            if (!CanSaveNewEvent()) return;

            // 1) voeg toe aan UI
            var model = new EventModel
            {
                Title = NewEventTitle.Trim(),
                Date = NewEventDate,
                Location = string.IsNullOrWhiteSpace(NewEventLocation) ? "Onbekend" : NewEventLocation.Trim()
            };
            Events.Add(model);

            // 2) append lokaal
            var existing = await LoadLocalAsync() ?? new List<EventModel>();
            existing.Add(model);
            await SaveLocalAsync(existing);

            // 3) box resetten & feedback
            NewEventTitle = string.Empty;
            NewEventLocation = string.Empty;
            NewEventDate = DateTime.Today.AddDays(1);
            IsAdding = false;

            await Application.Current?.MainPage?.DisplayAlert("Event", "Event opgeslagen (lokaal).", "OK");
        }

        // ========== local storage ==========
        private async Task<List<EventModel>?> LoadLocalAsync()
        {
            try
            {
                if (!File.Exists(LocalPath)) return null;
                var json = await File.ReadAllTextAsync(LocalPath);
                return JsonSerializer.Deserialize<List<EventModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        private async Task SaveLocalAsync(List<EventModel> list)
        {
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = false });
            await File.WriteAllTextAsync(LocalPath, json);
        }

        // ========== INotifyPropertyChanged ==========
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
