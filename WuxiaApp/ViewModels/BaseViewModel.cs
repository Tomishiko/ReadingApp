using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using WuxiaApp.Views;

namespace WuxiaApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;
    public bool IsNotBusy => !IsBusy;
    [RelayCommand]
    async Task NavigateToDetails(Book book)
    {
        
        if (book == null)
            return;
      
        var query = new Dictionary<string, object>
        {
            { "slug", book.Slug }
        };
        await Shell.Current.GoToAsync(nameof(DetailsPage), query);

        
    }
}

