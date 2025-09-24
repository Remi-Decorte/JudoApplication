using CommunityToolkit.Maui.Views;
using Mde.Project.Mobile.ViewModels;
using Syncfusion.Maui.Scheduler;
using Mde.Project.Mobile.Pages.Popups;

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
            await _vm.LoadAsync();
        }

        // <- DIT is de handler die XAML verwacht
        private async void OnAddTrainingClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(_addTrainingPage);
        }

        private async void Scheduler_Tapped(object sender, SchedulerTappedEventArgs e)
        {
            if (e.Element is not SchedulerElement.SchedulerCell and not SchedulerElement.Appointment)
                return;

            var suggested = e.Date ?? DateTime.Now;

            var popup = new AddQuickTrainingPopup(suggested);
            var result = await this.ShowPopupAsync(popup) as AddQuickTrainingPopup.Result;
            if (result is null) return;

            var start = result.Start;
            var end = start.AddHours(1);
            var type = result.Type;
            var color = result.Color;

            try
            {
                await _vm.CreateAndAddTrainingAsync(start, end, type, color);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Opslaan mislukt", ex.Message, "OK");
            }
        }
    }
}