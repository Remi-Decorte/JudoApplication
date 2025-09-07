using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AthleteDetailPage : ContentPage
    {
        public AthleteDetailPage(AthleteDetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
