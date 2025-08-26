using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AgendaPage : ContentPage
    {
        private readonly AgendaViewModel _vm;
        private readonly AddTrainingPage _addTrainingPage;

        // ViewModel en AddTrainingPage via DI
        public AgendaPage(AgendaViewModel vm, AddTrainingPage addTrainingPage)
        {
            InitializeComponent();
            _vm = vm;
            _addTrainingPage = addTrainingPage;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadTrainingsAsync();
        }

        // <- DIT is de handler die XAML verwacht
        private async void OnAddTrainingClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(_addTrainingPage);
        }
    }
}