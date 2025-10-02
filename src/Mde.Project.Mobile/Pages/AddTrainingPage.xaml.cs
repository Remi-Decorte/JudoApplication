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
        private void OnColorButtonClicked(object sender, EventArgs e)
        {
            if (BindingContext is AddTrainingViewModel vm)
            {
                var btn = (Button)sender;
                var col = btn.BackgroundColor;
                int r = (int)(col.Red * 255);
                int g = (int)(col.Green * 255);
                int b = (int)(col.Blue * 255);
                vm.SelectedColor = $"#{r:X2}{g:X2}{b:X2}";
            }
        }
    }
}
