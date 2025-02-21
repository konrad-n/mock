using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.App;

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

		// Rejestracja serwisów
		builder.Services.AddSingleton<IDataService, SQLiteDataService>();

		// Rejestracja ViewModels
		builder.Services.AddTransient<MainViewModel>();

        // Rejestracja stron
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
