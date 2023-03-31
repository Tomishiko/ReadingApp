using WuxiaApp.ViewModels;
using Scraper;

namespace WuxiaApp.Views
{
    public partial class Library : ContentPage
    {
       
       
        public Library(LibraryViewModel libraryViewModel)
        {

            InitializeComponent();

            BindingContext = libraryViewModel;
        }




    }
}