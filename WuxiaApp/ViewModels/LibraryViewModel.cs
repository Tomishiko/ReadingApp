using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WuxiaApp.Servs;
using General.DataModels;
using Scraper;
using WuxiaApp.Views;

namespace WuxiaApp.ViewModels;

public partial class LibraryViewModel : BaseViewModel
{
    readonly BaseServices services;
    readonly string libPath;
    public ObservableCollection<Book> Books { get; } = new();
    public LibraryViewModel(BaseServices services,IConnectivity connectivity):base(connectivity)
    {
        libPath = Path.Combine(FileSystem.Current.AppDataDirectory, "library.dat");
        this.services = services;
        if (!File.Exists(libPath))
        {
            Task t = Task.Run(async () => await CopyLocals());
            t.Wait();
        }

        Task.Run(async () => await GetBooksAsync());
        Title = "Library";

        
    }


    private async Task CopyLocals()
    {

        try
        {
            await services.CopyLibraryFileAsync("library", "library.dat");

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("error :", ex.Message, "ok");
        }

    }

    [RelayCommand]
    async Task GetBooksAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var books = await services.GetBooksLocalAsync(FileSystem.Current);
            if (Books.Count != 0)
                Books.Clear();
            foreach (var book in books)
                Books.Add(book.Value);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get books: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    async Task DeleteLastBookAsync(Book bookToDelete)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            services.DeleteBook(bookToDelete);
            Books.Remove(bookToDelete);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to delete book: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");

        }
        finally { IsBusy = false; }
    }
    [RelayCommand]
    async Task ButtonClickedAsync(Book book)
    {
        if (book == null)
            return;

        var query = new Dictionary<string, object>
        {
            { "SelectedBook", book }
        };
        await Shell.Current.GoToAsync(nameof(ReadingView), query);


    }

}

