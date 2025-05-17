using Mensajitos.Paginas;
using Mensajitos.Servicios;
using Microsoft.Extensions.Logging;

namespace Mensajitos
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

            // Registrar páginas
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MensajesRecientesPage>();
            builder.Services.AddTransient<RegistroPage>();

            // Añadir
            builder.Services.AddTransient<Paginas.ChatPage>();

            // Registrar servicios
            builder.Services.AddSingleton<ServicioSignalR>();
            builder.Services.AddSingleton<ServicioAPI>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}