using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Collections.ObjectModel;
using WuxiaApp.Servs;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WuxiaClassLib.DataModels;
using System.Windows.Input;
using WuxiaApp.Views;

namespace WuxiaApp.ViewModels;

public partial class UICollectionElement : ObservableObject
{
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    [ObservableProperty]
    bool isLoading;
    public bool IsNotLoading => !IsLoading;
    public ObservableCollection<Book> Books { get; }
    [ObservableProperty]
    string categoryName;
    [ObservableProperty]
    int opacity;
    public ICommand TriggerAnimationCommand { get; set; }

    public UICollectionElement(string category)
    {
        Books = new();
        IsLoading = true;
        CategoryName = category;
        opacity= 1;
    }
}

public partial class PopularViewModel : BaseViewModel
{
    readonly Services services;

    public ObservableCollection<UICollectionElement> Data { get; } = new();
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
        List<Task> tasks = new();
        if (IsBusy)
            return;
        Data.Add(new UICollectionElement("Hot novels"));
        Data.Add(new UICollectionElement("Top viewed Novels"));
        try
        {
            
            IsBusy = true;
            tasks.Add(populateBooks(Data[0], order: "-weekly_views"));
            tasks.Add(populateBooks(Data[1]));

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        finally 
        {
            await Task.WhenAll(tasks);
            IsBusy = false;

        }
    }

    async Task populateBooks(UICollectionElement currentElement,string searchPattern = "", 
        string order = "-total_views")
    {
        //List<Book> books = new();
        searchResult searchresult;
        searchresult = await services.SearchBookAsync(category:searchPattern,ordering:order);
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
                book.PicturePath = ImageParams["source"] + result.slug + ImageParams["preview"];
            currentElement.Books.Add(book);
        }
        
        //currentElement.TriggerAnimationCommand.Execute(null);
        currentElement.IsLoading = false;


        //return books;

    }

    [RelayCommand]
    async Task NavigateToDetails(string bookSlug)
    {
        
        if (bookSlug == null)
            return;

        var query = new Dictionary<string, object>
        {
            { "slug", bookSlug }
        };
        Shell.Current.GoToAsync(nameof(DetailsPage), true, query);
        
        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage,false);
    }

}
