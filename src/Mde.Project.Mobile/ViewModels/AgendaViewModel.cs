using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Scheduler;

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        private readonly ITrainingService _trainingService;

        public AgendaViewModel(ITrainingService trainingService)
        {
            _trainingService = trainingService;

            var today = DateTime.Today;
            DisplayDate = today;
            SelectedDate = today;

            // Start in maand
            _selectedViewMode = "Maand";
            SchedulerView = SchedulerView.Month;

            Appointments = new ObservableCollection<SchedulerAppointment>();

            PreviousPeriodCommand = new Command(() =>
            {
                DisplayDate = IsWeekView ? DisplayDate.AddDays(-7) : DisplayDate.AddMonths(-1);
                OnPropertyChanged(nameof(CurrentPeriodTitle));
            });

            NextPeriodCommand = new Command(() =>
            {
                DisplayDate = IsWeekView ? DisplayDate.AddDays(7) : DisplayDate.AddMonths(1);
                OnPropertyChanged(nameof(CurrentPeriodTitle));
            });

            GoToTodayCommand = new Command(() =>
            {
                var t = DateTime.Today;
                DisplayDate = t;
                SelectedDate = t;
                OnPropertyChanged(nameof(CurrentPeriodTitle));
            });

            NewEntryCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(Pages.AddTrainingPage));
            });
        }

        public ObservableCollection<SchedulerAppointment> Appointments { get; }

        private DateTime _displayDate;
        public DateTime DisplayDate { get => _displayDate; set => Set(ref _displayDate, value); }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate { get => _selectedDate; set => Set(ref _selectedDate, value); }

        private SchedulerView _schedulerView;
        public SchedulerView SchedulerView { get => _schedulerView; set => Set(ref _schedulerView, value); }

        public IList<string> ViewModes { get; } = new[] { "Maand", "Week" };

        private string _selectedViewMode;
        public string SelectedViewMode
        {
            get => _selectedViewMode;
            set
            {
                if (Set(ref _selectedViewMode, value))
                {
                    if (string.Equals(value, "Week", StringComparison.OrdinalIgnoreCase))
                    {
                        SchedulerView = SchedulerView.Week;
                        IsWeekView = true;
                    }
                    else
                    {
                        SchedulerView = SchedulerView.Month;
                        IsWeekView = false;
                    }
                    OnPropertyChanged(nameof(CurrentPeriodTitle));
                }
            }
        }

        public async Task CreateAndAddTrainingAsync(DateTime start, DateTime end, string type, Color color)
        {
            var dto = new TrainingEntryModel
            {
                Type = type,
                Date = start,
                Comment = string.Empty,
                TechniqueScores = new(),
            };

            await _trainingService.CreateTrainingEntryAsync(dto);

            Appointments.Add(new SchedulerAppointment
            {
                Subject = type,
                StartTime = start,
                EndTime = end,
                Background = new SolidColorBrush(color)
            });
        }

        private bool _isWeekView;
        public bool IsWeekView { get => _isWeekView; set => Set(ref _isWeekView, value); }

        public string CurrentPeriodTitle =>
            IsWeekView ? WeekRangeText(DisplayDate)
                       : DisplayDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

        public ICommand PreviousPeriodCommand { get; }
        public ICommand NextPeriodCommand { get; }
        public ICommand GoToTodayCommand { get; }
        public ICommand NewEntryCommand { get; }

        private static string WeekRangeText(DateTime anchor)
        {
            int diff = (7 + (anchor.DayOfWeek - DayOfWeek.Monday)) % 7;
            var start = anchor.AddDays(-diff).Date;
            var end = start.AddDays(6);
            string left = start.ToString("d MMM", CultureInfo.CurrentCulture);
            string right = end.ToString("d MMM yyyy", CultureInfo.CurrentCulture);
            return $"{left} – {right}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
