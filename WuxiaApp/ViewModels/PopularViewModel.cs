using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Collections.ObjectModel;
using WuxiaApp.Servs;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WuxiaClassLib.DataModels;

namespace WuxiaApp.ViewModels;

public partial class PopularViewModel : BaseViewModel
{
    //public ObservableCollection<Book> Books { get; } = new();
    readonly Services services;

    public ObservableCollection<PopularBooksModel> Data { get; } = new();
    public PopularViewModel(Services services)
    {
        this.services = services;
        ImageParams = new Dictionary<string, string>
        {
            ["preview"] = ".webp?width=150&quality=60",
            ["source"] = "https://wuxiaworldeu.b-cdn.net/original/"
        };

        Task.Run(async () => await GetBooksAsync());
        Title = "Popular";
    }

    [RelayCommand]
    async Task GetBooksAsync()
    {
        if (IsBusy)
            return;
        try
        {
         
            IsBusy = true;

            var books = await populateBooks();
            Data.Add(new PopularBooksModel("Top Viewed Novels", books));
            Data[0].isLoading= false;
            IsBusy = false;

            books = await populateBooks( order:"-weekly_views");
            Data.Add(new PopularBooksModel("Hot Novels", books));
            Data[1].isLoading = false;

            books = await populateBooks(searchPattern:"xuanhuan");
            Data.Add(new PopularBooksModel("Most Viewed Xuanhuan", books));
            Data[2].isLoading = false;

            books = await populateBooks(searchPattern: "mature");
            Data.Add(new PopularBooksModel("Most Viewed Mature", books));
            Data[3].isLoading = false;

            books = await populateBooks(searchPattern: "mecha");
            Data.Add(new PopularBooksModel("Most Viewed Mecha", books));
            Data[4].isLoading = false;

            books = await populateBooks(searchPattern: "sci-fi");
            Data.Add(new PopularBooksModel("Most Viewed Sci-fi", books));
            Data[5].isLoading = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        finally 
        { 
            IsBusy = false;

        }
    }

    async Task<List<Book>> populateBooks(string searchPattern = "", 
        string order = "-total_views")
    {
        List<Book> books = new();
        searchResult searchresult;
        searchresult = await services.SearchBook(category:searchPattern,ordering:order);
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
            books.Add(book);
        }
        return books;

    }



}
