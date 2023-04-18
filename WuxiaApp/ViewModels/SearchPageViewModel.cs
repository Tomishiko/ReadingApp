using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WuxiaApp.Servs;
using WuxiaApp.Views;
#if ANDROID
using Microsoft.Maui.Platform;
#endif


namespace WuxiaApp.ViewModels;

public partial class SearchPageViewModel : BaseViewModel
{
    public ObservableCollection<Book> Books { get; } = new();
    
    readonly Services services;
    public SearchPageViewModel(Services services)
    {
        this.services = services;
        Title = "Search";
    }



    [RelayCommand]
    async Task SearchBooksAsync(string searchParam)
    {

        if (IsBusy)
            return;
        if (Books.Count != 0)
            Books.Clear();
        try
        {

            IsBusy = true;

#if ANDROID

        if (Platform.CurrentActivity.CurrentFocus != null)
        {
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
            Platform.CurrentActivity.CurrentFocus.ClearFocus();
        }
#endif
            var searchresult = await services.SearchBookAsync(searchParam); 
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

    //}
}

