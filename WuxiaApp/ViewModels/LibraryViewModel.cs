using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WuxiaApp.Servs;
using General.DataModels;
using Scraper;

namespace WuxiaApp.ViewModels;

public partial class LibraryViewModel : BaseViewModel
{
    public ObservableCollection<Book> Books { get; } = new();
    readonly Services services;
    readonly string libPath;
    public LibraryViewModel(Services services)
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

            var books = await services.GetBooksLocalAsync();
            if (Books.Count != 0)
                Books.Clear();
            foreach (var book in books)
                Books.Add(book);

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

}

