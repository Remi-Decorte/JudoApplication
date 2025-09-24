using Microsoft.Extensions.Logging;
using Mde.Project.Mobile.ViewModels;
using Mde.Project.Mobile.Pages;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Services.Mock;
using CommunityToolkit.Maui;
using Mde.Project.Mobile.Services;
using Syncfusion.Maui.Core.Hosting;
using Mde.Project.Mobile.Pages.Popups;

namespace Mde.Project.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            // MOCK:
            builder.Services.AddSingleton<ITrainingService, TrainingService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<IJudokaService, JudokasService>();
            // voor echte API vervang bovenste door
            // builder.Services.AddSingleton<ITrainingService, TrainingService>();


            // ViewModels
            builder.Services.AddTransient<AgendaViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<AthletesViewModel>();
            builder.Services.AddTransient<AddTrainingViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<EventsViewModel>();

            // Pages
            builder.Services.AddTransient<AgendaPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<AthletesPage>();
            builder.Services.AddTransient<AddTrainingPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<EventsPage>();
            builder.Services.AddTransient<AddQuickTrainingPopup>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
