using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        private DateTime _displayMonth;
        private DateTime? _selectedDate;

        public AgendaViewModel()
        {
            var today = DateTime.Today;
            DisplayMonth = new DateTime(today.Year, today.Month, 1);
            SelectedDate = today;

            PreviousMonthCommand = new Command(() =>
            {
                DisplayMonth = DisplayMonth.AddMonths(-1);
            });

            NextMonthCommand = new Command(() =>
            {
                DisplayMonth = DisplayMonth.AddMonths(1);
            });

            GoToTodayCommand = new Command(() =>
            {
                var t = DateTime.Today;
                DisplayMonth = new DateTime(t.Year, t.Month, 1);
                SelectedDate = t;
            });

            NewEntryCommand = new Command(async () =>
            {
                // Navigeer naar AddTrainingPage (route geregistreerd in AppShell)
                await Shell.Current.GoToAsync(nameof(Pages.AddTrainingPage));
            });
        }

        //Properties
        public DateTime DisplayMonth
        {
            get => _displayMonth;
            set
            {
                if (Set(ref _displayMonth, value))
                {
                    OnPropertyChanged(nameof(CurrentMonthTitle));
                }
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => Set(ref _selectedDate, value);
        }

        // Titel boven de kalender, bv. "September 2025"
        public string CurrentMonthTitle =>
            DisplayMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

        //Commands
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand GoToTodayCommand { get; }
        public ICommand NewEntryCommand { get; }

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
