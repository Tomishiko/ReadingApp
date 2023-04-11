using CommunityToolkit.Mvvm.ComponentModel;
using General.DataModels;
using WuxiaApp.Servs;

namespace WuxiaApp.ViewModels;

public partial class DetailsViewModel : BaseViewModel
{
    Services services;
    [ObservableProperty]
    public Book book;
    public DetailsViewModel(Services services)
    {

        this.services = services;
        book = new Book()
        {
            Chapters = 120,
            Description = "blah blah blah this is discription",
            PicturePath = "https://wuxiaworldeu.b-cdn.net/original/versatile-mage.webp?width=400&quality=80",
            Ratings = "5.0",
            Readed = 120,
            Title = "Some Book Is Here",
            Views = "120k"
        };
        this.Title = book.Title;
    }
}


