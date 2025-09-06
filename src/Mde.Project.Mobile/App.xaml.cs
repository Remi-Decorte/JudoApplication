using Mde.Project.Mobile.Pages;

namespace Mde.Project.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Ga direct naar login als eerste pagina
            _ = Shell.Current.GoToAsync("//login");
        }
    }
}
