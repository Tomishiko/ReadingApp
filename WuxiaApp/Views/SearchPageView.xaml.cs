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
        double translation;
        bool visibility;


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
        //if (e.VerticalDelta > 15 && searchBar.IsVisible == true)
        //{
        //    translation = -70;
        //    visibility = false;
        //}
        //else if (e.VerticalDelta < -15 && searchBar.IsVisible == false)
        //{
        //    translation = 0;
        //    visibility = true;
        //}
        //else
        //{
        //    return;
        //}

        //await searchBar.TranslateTo(searchBar.TranslationX, translation, 300);
        ////await Coll.TranslateTo(Coll.TranslationX, translation, 300);

        //await Task.Delay(100);

        //searchBar.IsVisible = visibility;


    }
}