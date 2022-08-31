using SkiaSharp;

namespace StampImages.App.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp App { get; set; }
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("NotoSansJP-Regular.otf", "NotoSansJPRegular");

                });

            App = builder.Build();
            return App;
        }
    }
}