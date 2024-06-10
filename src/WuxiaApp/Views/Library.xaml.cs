using WuxiaApp.ViewModels;
using Scraper;
using General.DataModels;

namespace WuxiaApp.Views
{
    public partial class Library : ContentPage
    {
       
       
        public Library(LibraryViewModel libraryViewModel)
        {

            InitializeComponent();
            BindingContext = libraryViewModel;
        }

        private void SwipeItem_Invoked(object sender, EventArgs e)
        {
            var swipe = sender as SwipeItemView;
            var book = swipe.BindingContext as Book;
            var vm = BindingContext as LibraryViewModel;
            vm.DeleteLastBookCommand.Execute(book);
            
            
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as CollectionView).SelectedItem = null;
        }
    }
}