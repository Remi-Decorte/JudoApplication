using Mde.Project.Mobile.Pages;

namespace Mde.Project.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddTrainingPage), typeof(AddTrainingPage));
        }
    }
}
