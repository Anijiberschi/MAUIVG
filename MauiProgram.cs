using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using MyApp.Converters;
using System.Xml;

namespace MyApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Views et ViewModels
            builder.Services.AddSingleton<MainView>();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddTransient<DetailsView>();
            builder.Services.AddTransient<DetailsViewModel>();

            builder.Services.AddTransient<GraphView>();
            builder.Services.AddTransient<GraphViewModel>();

            // Services
            builder.Services.AddSingleton<BarcodeScannerService>();
            builder.Services.AddSingleton<JSONServices>();
            builder.Services.AddSingleton<CSVServices>();

            // Resource Dictionaries
            builder.Services.AddSingleton<ResourceDictionary>(new ResourceDictionary
            {
                // Ajouter les convertisseurs
                { "BoolToColorConverter", new BoolToColorConverter() },
                { "BoolToStatusConverter", new BoolToStatusConverter() },
                { "StringNotEmptyConverter", new StringNotEmptyConverter() }
            });

            return builder.Build();
        }
    }
}