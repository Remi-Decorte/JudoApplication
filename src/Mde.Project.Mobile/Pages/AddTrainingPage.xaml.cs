using System.Globalization;
using Microsoft.Maui.Controls;
using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    [QueryProperty(nameof(DateIso), "date")]
    [QueryProperty(nameof(DurationMinutes), "duration")]
    [QueryProperty(nameof(InitialType), "type")]
    public partial class AddTrainingPage : ContentPage
    {
        private readonly AddTrainingViewModel _vm;

        public string? DateIso { get; set; }
        public string? DurationMinutes { get; set; }
        public string? InitialType { get; set; }

        public AddTrainingPage(AddTrainingViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DateIso != null &&
                DateTime.TryParse(DateIso, null, DateTimeStyles.RoundtripKind, out var start))
            {
                _ = int.TryParse(DurationMinutes, out var mins);
                mins = mins == 0 ? 60 : mins;


            }
        }
    }
}
