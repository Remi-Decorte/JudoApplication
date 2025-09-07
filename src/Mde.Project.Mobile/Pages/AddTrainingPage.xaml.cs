using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AddTrainingPage : ContentPage
    {
        private readonly AddTrainingViewModel _vm;

        public AddTrainingPage(AddTrainingViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }
    }
}