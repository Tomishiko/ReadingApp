using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WuxiaApp.Servs;
using WuxiaApp.ViewModels;
using WuxiaApp.Views;

namespace WuxiaApp
{
    //[XamlCompilation(XamlCompilationOptions.Skip)]
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("segoeui.ttf", "SegoeRegular");
                    fonts.AddFont("segoeuib.ttf", "SegoeBold");
                    fonts.AddFont("seguisb.ttf", "SegoeSemiBold");
                    fonts.AddFont("segoeuisl.ttf", "SegoeSemiLight");
                    fonts.AddFont("segoepr.ttf", "SegoePrint");
                    fonts.AddFont("arial.ttf", "Arial");
                    fonts.AddFont("calibri.ttf", "Calibri");
                    fonts.AddFont("Roboto-Regular.ttf", "Roboto");
                    fonts.AddFont("tahoma.ttf", "Tahoma");
                    fonts.AddFont("times.ttf", "TimesNewRoman");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<BaseServices>();
            builder.Services.AddSingleton<PreferenceServices>();
            builder.Services.AddSingleton<LibraryViewModel>();
            builder.Services.AddSingleton<Library>();
            builder.Services.AddSingleton<PopularViewModel>();
            builder.Services.AddSingleton<Popular>();
            builder.Services.AddSingleton<SearchPageViewModel>();
            builder.Services.AddSingleton<SearchPageView>();
            builder.Services.AddTransient<DetailsViewModel>();
            builder.Services.AddTransient<DetailsPage>();
            builder.Services.AddTransient<ReadingView>();
            builder.Services.AddTransient<ReadingViewModel>();
            builder.Services.AddSingleton(Connectivity.Current);
         

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls13;

            return builder.Build();
        }
    }
}