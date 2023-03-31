using CommunityToolkit.Mvvm.ComponentModel;

namespace WuxiaApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;
    protected Dictionary<string, string> ImageParams;
    public bool IsNotBusy => !IsBusy;
}

