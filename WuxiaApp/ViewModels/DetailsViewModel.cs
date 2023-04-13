using CommunityToolkit.Mvvm.ComponentModel;
using General.DataModels;
using System.Diagnostics;
using WuxiaApp.Servs;
using WuxiaClassLib.DataModels;


namespace WuxiaApp.ViewModels;
//[QueryProperty(nameof(Name),"Book")]
public partial class DetailsViewModel : BaseViewModel ,IQueryAttributable
{
    Services services;

    [ObservableProperty]
    public Book book;

    public DetailsViewModel(Services services)
    {
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
                Views = bookinfo.views,
                Slug = bookinfo.slug
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
        { IsBusy = false;}

    }
}


