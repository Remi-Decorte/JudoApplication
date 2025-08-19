using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AthletesPage : ContentPage
    {
        private readonly AthletesViewModel _vm;

        // ✅ DI levert het ViewModel aan
        public AthletesPage(AthletesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // laad categorieën + judoka’s (VM regelt SelectedCategory)
            await _vm.LoadCategoriesAsync();
        }

        private void OnGoHome(object sender, EventArgs e)
        {

        }

        private void OnGoAgenda(object sender, EventArgs e)
        {

        }
    }
}

