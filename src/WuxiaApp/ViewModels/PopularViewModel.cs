using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Collections.ObjectModel;
using WuxiaApp.Servs;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WuxiaClassLib.DataModels;
using System.Windows.Input;

namespace WuxiaApp.ViewModels;

public partial class UICollectionElement : ObservableObject
{
    
    internal Uri nextDataChunk;

    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    [ObservableProperty]
    bool isLoading;
    public bool IsNotLoading => !IsLoading;
    [ObservableProperty]
    bool isLoadingNewData;
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
    readonly BaseServices services;
    public ObservableCollection<UICollectionElement> Data { get; } = new();
    public PopularViewModel(BaseServices services,IConnectivity connectivity)
    {
        this.services = services;
        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            Shell.Current.DisplayAlert("No internet acces!", "Please check your internet acces and try again.", "ok");
            connectivity.ConnectivityChanged += ConnectivityChanged;
            return;
        }
        Task.Run(async () => await GetBooksAsync());
    }

    void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        
        if (e.NetworkAccess == NetworkAccess.Internet && Data.Count==0)
            Task.Run(async () => await GetBooksAsync());
       
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

    async Task populateBooks(UICollectionElement currentElement, string searchPattern = "",
        string order = "-total_views")
    {
        //List<Book> books = new();
        if(currentElement.Books.Count !=0)
            currentElement.Books.Clear();
        var searchresult = await services.SearchBookAsync(category: searchPattern, ordering: order, limit: "4");
        if(searchresult is null)
            return;

        currentElement.nextDataChunk = new Uri(searchresult.next);
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
            book.PicturePath = services.FormPicturePath(result.image,result.slug);
            currentElement.Books.Add(book);
        }

        currentElement.IsLoading = false;

    }

    [RelayCommand]
    async Task LoadNextData(UICollectionElement currentElement)
    {
        if (IsBusy || !hasInternet)
            return;

        currentElement.IsLoadingNewData = true;
        try
        {
            var searchResult = await services.LoadNextDataAsync(currentElement.nextDataChunk);
            currentElement.nextDataChunk = new Uri(searchResult.next);
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
                book.PicturePath = services.FormPicturePath(result.image,result.slug);
                currentElement.Books.Add(book);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");


        }
        finally { currentElement.IsLoadingNewData = false; }

    }
}
