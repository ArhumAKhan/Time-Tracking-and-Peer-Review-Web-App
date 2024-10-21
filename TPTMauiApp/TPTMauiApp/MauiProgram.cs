using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TPTMauiApp.Data;
using TPTMauiApp.ViewModels;
using TPTMauiApp.Views;

namespace TPTMauiApp;

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

        var connectionString =
            "Server=localhost;Database=maui;User=root;Password=root;";

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DataDisplayPage>();
        builder.Services.AddSingleton<ApplicationDbContext>();
        builder.Services.AddTransient<DataDisplayViewModel>();


        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
