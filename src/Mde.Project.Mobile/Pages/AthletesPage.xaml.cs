using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AthletesPage : ContentPage
    {
        private readonly AthletesViewModel _vm;

        public AthletesPage(AthletesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadAsync();
        }

        private void OnGoHome(object sender, EventArgs e) { /* ... */ }
        private void OnGoAgenda(object sender, EventArgs e) { /* ... */ }

        // 🔹 Tapped op het hele kaartje
        private async void OnJudokaTapped(object sender, TappedEventArgs e)
        {
            if (sender is not Frame frame) return;
            if (frame.BindingContext is not JudokaModel judoka) return;

            var vm = new AthleteDetailViewModel(judoka);
            var page = new AthleteDetailPage(vm);
            await Navigation.PushAsync(page);
        }
    }
}
