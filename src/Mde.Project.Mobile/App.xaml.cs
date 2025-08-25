using Mde.Project.Mobile.Pages;

namespace Mde.Project.Mobile
{
    public partial class App : Application
    {
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(loginPage);
        }
    }
}
