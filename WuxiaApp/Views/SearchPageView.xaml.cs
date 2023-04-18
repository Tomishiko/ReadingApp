using Microsoft.Maui;
using System.ComponentModel;
using System.Transactions;
using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class SearchPageView : ContentPage
{
	public SearchPageView(SearchPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext= viewModel;
	}
    
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.VerticalDelta > 15 && searchBar.IsVisible == true)
        {
            await searchBar.TranslateTo(searchBar.TranslationX, -70, 300);
            await Task.Delay(100);
            searchBar.IsVisible = false;


        }
        else if (e.VerticalDelta < -15 && searchBar.IsVisible == false)
        {
            searchBar.IsVisible = true;
            await Task.Delay(100);
            await searchBar.TranslateTo(searchBar.TranslationX, 0, 300);


        }

    }

    private void Coll_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
       
        (sender as CollectionView).SelectedItem = null;
    }
}