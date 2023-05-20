using WuxiaApp.Views;

namespace WuxiaApp
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(ReadingView), typeof(ReadingView));
        }
    }
}