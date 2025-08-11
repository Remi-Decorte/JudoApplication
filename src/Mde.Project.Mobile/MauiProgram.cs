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
            // voor echte API vervang bovenste door
            // builder.Services.AddSingleton<ITrainingService, TrainingService>();

            //ViewModels 
            builder.Services.AddTransient<AgendaViewModel>();

            //Pages
            builder.Services.AddTransient<AgendaPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
