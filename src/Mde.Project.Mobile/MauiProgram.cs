using Microsoft.Extensions.Logging;
using Mde.Project.Mobile.Services.Interfaces;
using Mde.Project.Mobile.Services.Mocks;  //mock
using Mde.Project.Mobile.Services;    
using Mde.Project.Mobile.ViewModels;
using Mde.Project.Mobile.Pages;

namespace Mde.Project.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            // MOCK:
            builder.Services.AddSingleton<ITrainingService, MockTrainingService>();
            builder.Services.AddSingleton<IAuthService,     MockAuthService>();
            builder.Services.AddSingleton<IEventService,    MockEventService>();
            builder.Services.AddSingleton<IJudokaService,   MockJudokaService>();
            // voor echte API vervang bovenste door
            // builder.Services.AddSingleton<ITrainingService, TrainingService>();

            
            // ViewModels
            builder.Services.AddTransient<AgendaViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<AthletesViewModel>(); 

            // Pages
            builder.Services.AddTransient<AgendaPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<AthletesPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
