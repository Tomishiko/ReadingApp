using WuxiaApp.Servs;
using WuxiaApp.Views;

namespace WuxiaApp
{
    public partial class App : Application
    {
        Services services;
        public App(Services services)
        {
            InitializeComponent();
            MainPage = new AppShell();
            this.services = services;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Deactivated += async (sender, eventArgs) => {
                await Task.Run(() => services.Save());
            };
            //window.Stopped += async (sender, eventArgs) => {
            //    await services.Save();
            //};
            return window;
        }


    }
}