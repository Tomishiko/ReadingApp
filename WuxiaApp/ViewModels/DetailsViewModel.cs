using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.Diagnostics;
using WuxiaApp.Servs;
using WuxiaClassLib.DataModels;


namespace WuxiaApp.ViewModels;
//[QueryProperty(nameof(Name),"Book")]
public partial class DetailsViewModel : BaseViewModel, IQueryAttributable
{
    Services services;
    LibraryViewModel libraryViewModel;

    [ObservableProperty]
    Book book;
   
    public DetailsViewModel(Services services,LibraryViewModel libVm)
    {
        libraryViewModel = libVm;
        this.services = services;
        ImageParams = new Dictionary<string, string>
        {
            ["preview"] = ".webp",   //?width=150&quality=60",
            ["source"] = "https://wuxiaworldeu.b-cdn.net/original/"
        };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (IsBusy)
            return;

        var slug = query["slug"] as String;
        if (slug == null)
            return;
        await GetBookDataAsync(slug);
    }

    async Task GetBookDataAsync(string name)
    {
        try
        {

            IsBusy = true;
            var bookinfo = await services.GetBookInfoAsync(name);

            var book = new Book()
            {
                Chapters = bookinfo.chapters,
                Description = bookinfo.description,
                Ratings = bookinfo.rating,
                Title = bookinfo.name,
                Views = bookinfo.human_views,
                Author = bookinfo.author,
                Categories = bookinfo.category,
                FirstChapter = bookinfo.first_chapter,
                LastUpdate = bookinfo.last_chap_updated,
                Status = bookinfo.novelStatus,
                Ranking = bookinfo.ranking
            };
            if (bookinfo.image == null)
                book.PicturePath = "unloaded_image.png";
            else
                book.PicturePath = ImageParams["source"] + bookinfo.slug + ImageParams["preview"];
            Book = book;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        { IsBusy = false; }

    }
    void AddNewBook(Book book)
    {
        services.AddNewBook(book);
        libraryViewModel.Books.Add(book);
    }


}


