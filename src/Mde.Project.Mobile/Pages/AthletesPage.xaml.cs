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

        private void OnGoHome(object sender, EventArgs e)
        {
            // optioneel: Shell navigation of Navigation.PopToRootAsync();
        }

        private void OnGoAgenda(object sender, EventArgs e)
        {
            // optioneel: Shell navigation naar Agenda
        }

        // 🔹 aangeroepen door SelectionChanged="OnJudokaSelected" in XAML
        private async void OnJudokaSelected(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as JudokaModel;
            if (selected == null) return;

            // navigeer naar detailpagina
            var vm = new AthleteDetailViewModel(selected);
            var page = new AthleteDetailPage(vm);
            await Navigation.PushAsync(page);

            // selectie wissen zodat opnieuw klikken kan
            if (sender is CollectionView cv) cv.SelectedItem = null;
        }
    }
}

