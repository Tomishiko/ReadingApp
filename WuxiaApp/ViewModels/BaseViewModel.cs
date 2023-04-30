using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Networking;

namespace WuxiaApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;
    static protected bool hasInternet;
    [ObservableProperty]
    string title;
    public BaseViewModel() { }
    public BaseViewModel(IConnectivity connectivity)
    {
        connectivity.ConnectivityChanged += ConnectivityChanged;
        if(connectivity.NetworkAccess!=NetworkAccess.Internet)
            hasInternet = false;
        else
            hasInternet = true;
    }
    public bool IsNotBusy => !IsBusy;
    private async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if(e.NetworkAccess == NetworkAccess.Internet)
        {
            hasInternet = true;
            await Toast.Make("Connected.", ToastDuration.Short, 16).Show();

        }
        else
        {
            hasInternet = false;
            await Toast.Make("Connection lost!\n Check your internet!", ToastDuration.Long, 16).Show();
        }
    }

}



