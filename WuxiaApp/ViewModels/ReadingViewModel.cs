
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DataModels;
using System.ComponentModel;
using WuxiaApp.Servs;

namespace WuxiaApp.ViewModels;

public partial class ReadingViewModel:BaseViewModel,IQueryAttributable, INotifyPropertyChanged
{
    Services services;
    string currentChapterPath;
    [ObservableProperty]
    string nextChapterPath;
    [ObservableProperty]
    string prevChapterPath;
    [ObservableProperty]
    string text;
    [ObservableProperty]
    string chapName;
    [ObservableProperty]
    Book currentBook;

    //ChapterData chapterData;
    //public event PropertyChangedEventHandler PropertyChanged;
    public ReadingViewModel(Services services)
    {
        this.services = services;

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (IsBusy)
            return;
        if (query["SelectedBook"] is not Book book)
            throw new ArgumentException(
                $"Can not convert provided value type {query["SelectedBook"].GetType()} associated with 'SelectedBook' key to Book type", 
                nameof(query));
        CurrentBook = book;
        if (CurrentBook.Readed != 1)
            currentChapterPath = CurrentBook.Slug + $"-{CurrentBook.Readed}";
        else
            currentChapterPath = CurrentBook.FirstChapter;

        Task.Run(async () => await LoadText(currentChapterPath));
    }
    [RelayCommand]
    async Task LoadText(string chapterPath)
    {
        if (chapterPath[chapterPath.Length - 1] == '-')
            return;
        ArgumentNullException.ThrowIfNull(chapterPath, nameof(chapterPath));
        if (IsBusy)
            return;
        IsBusy = true;
        
        try
        {
            var chapterData = await services.FetchChapterAsync(chapterPath);
            ChapName = chapterData.title;
            Text = chapterData.text;//ParagraphParser(chapterData.text);
            NextChapterPath = string.Concat(CurrentBook.Slug ,"-",chapterData.nextChap);
            PrevChapterPath = string.Concat(CurrentBook.Slug ,"-",chapterData.prevChap);
            
            CurrentBook.Readed = chapterData.index;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Unexpected error!", $"{ex.Message}", "ok");
            throw new Exception(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    public async Task GoToChapAsync(int? chapNumber)
    {
        currentChapterPath = CurrentBook.Slug + $"-{chapNumber}";
        await LoadText(currentChapterPath);

    }

}

