using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WuxiaApp.Servs;
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
        
        ImageParams = new Dictionary<string, string>
        {
            ["preview"] = ".webp?width=450&quality=100",
            ["source"] = "https://wuxiaworldeu.b-cdn.net/original/"
        };
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
            var searchresult = await services.SearchBook(searchParam); 
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
                    Views = result.views
                };
                if (result.image == null)
                    book.PicturePath = "unloaded_image.png";
                else
                    book.PicturePath = ImageParams["source"] + result.slug + ImageParams["preview"];
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
}

