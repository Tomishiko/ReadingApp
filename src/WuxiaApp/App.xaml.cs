using WuxiaApp.Servs;
using WuxiaApp.Views;

namespace WuxiaApp
{
    public partial class App : Application
    {
        BaseServices services;
        public App(BaseServices services)
        {
            InitializeComponent();
            MainPage = new AppShell();
            this.services = services;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Deactivated += async (sender, eventArgs) => {
                await Task.Run(() => services.Save(FileSystem.Current));
            };
            //window.Stopped += async (sender, eventArgs) => {
            //    await services.Save();
            //};
            return window;
        }


    }
}