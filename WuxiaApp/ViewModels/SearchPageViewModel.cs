using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using General.DataModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WuxiaApp.Servs;
using WuxiaApp.Views;
using CommunityToolkit.Maui.Core;
#if ANDROID
using Microsoft.Maui.Platform;
#endif


namespace WuxiaApp.ViewModels;

public partial class SearchPageViewModel : BaseViewModel
{
    public ObservableCollection<Book> Books { get; } = new();
    [ObservableProperty]
    bool isLoadingNewData=false;
    [ObservableProperty]
    uint footer;
    
    readonly Services services;
    Uri nextData;
    IConnectivity connectivity;
    IToast connectionToast;
    public SearchPageViewModel(Services services)
    {
        //this.connectivity= connectivity;
        this.services = services;
        Title = "Search";
    }


    [RelayCommand]
    async Task SearchBooksAsync(string searchParam)
    {

        if (IsBusy)
            return;
        if (!hasInternet)
        {
            await Shell.Current.DisplayAlert("No internet acces!", "Please check your internet acces and try again.", "ok");
            return;
        }
        if (Books.Count != 0)
            Books.Clear();
        try
        {

            IsBusy = true;
            Footer = 100;
#if ANDROID

        if (Platform.CurrentActivity.CurrentFocus != null)
        {
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
            Platform.CurrentActivity.CurrentFocus.ClearFocus();
        }
#endif
            var searchresult = await services.SearchBookAsync(searchParam);
            nextData = new Uri(searchresult.next);
            if (Books.Count != 0)
                Books.Clear();
            foreach (var result in searchresult.results)
            {
                var book = new Book()
                {
                    Chapters = result.chapters,
                    Description = result.description,
                    Ratings = result.rating,
                    Title = result.name,
                    Views = result.views,
                    Slug = result.slug
                };
                if (result.image == null)
                    book.PicturePath = "unloaded_image.png";
                else
                    book.PicturePath = services.FormPicturePath(result.slug);
                Books.Add(book);
            }
                



        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        finally { IsBusy = false; }
    }

    //[RelayCommand]
    //async Task NavigateToDetails(string bookSlug)
    //{

    //    if (bookSlug == null)
    //        return;

    //    var query = new Dictionary<string, object>
    //    {
    //        { "slug", bookSlug }
    //    };
    //    await Shell.Current.GoToAsync(nameof(DetailsPage), query);

    
    [RelayCommand]
    async Task LoadNextDataChunkAsync()
    {
        if (IsBusy || nextData==null || IsLoadingNewData)
        {
            Footer = 0;
            return;
        }
            
        
        try
        {
            IsLoadingNewData = true;
            if (!hasInternet)
                return;
            var searchResult = await services.LoadNextDataAsync(nextData);
            nextData = searchResult.next == null ? null : new Uri(searchResult.next);
            foreach (var result in searchResult.results)
            {
                var book = new Book()
                {
                    Chapters = result.chapters,
                    Description = result.description,
                    Ratings = result.rating,
                    Title = result.name,
                    Views = result.views,
                    Slug = result.slug
                };
                if (result.image == null)
                    book.PicturePath = "unloaded_image.png";
                else
                    book.PicturePath = services.FormPicturePath(result.slug);
                Books.Add(book);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }finally { IsLoadingNewData = false; Footer = 60; }

        
    }
}

