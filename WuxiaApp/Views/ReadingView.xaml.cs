using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics;
using WuxiaApp.Servs;
using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class ReadingView : ContentPage
{
    enum visualLvls
    {
        Reading,
        Dock,
        Menu1,
        Menu2,
        Menu3,
        Init
    }
    Stack<visualLvls> _uiActivity;
    string _currentFont;

    public string CurrentFont
    {
        get => _currentFont;
        set
        {
            _currentFont = value;
            OnPropertyChanged();
        }
    }
    List<string> fonts;
    Services services;
    public ReadingView(ReadingViewModel viewModel,Services services)
    {
        Shell.SetTabBarIsVisible(this, false);
        InitializeComponent();
        UI_Level_Switcher(visualLvls.Init, 0);
        _uiActivity = new Stack<visualLvls>();
        fonts = new List<string>
        {
            "OpenSansRegular",
            "SegoeRegular",
            "SegoePrint",
            "Arial",
            "Calibri",
            "Roboto",
            "Tahoma",
            "TimesNewRoman"
        };
        fontpicker.ItemsSource = fonts;
        this.services = services;
        if (services.UserProfileSet)
        {
            CurrentFont = services.UserFont;
            slider.Value = services.UserFontSize;
        }
        else
        {
            CurrentFont = fonts[0];
            slider.Value = 14;
        }
        
        Disappearing += ReadingView_Disappearing;
        BindingContext = viewModel;

    }

    private void ReadingView_Disappearing(object sender, EventArgs e)
    {
        services.SetUserPreferences(CurrentFont, slider.Value,Color.FromRgb(0,0,0));
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

        if (_uiActivity.TryPop(out visualLvls toClose))
            UI_Level_Switcher(toClose, 0);
        else
        {
            _uiActivity.Push(visualLvls.Dock);
            UI_Level_Switcher(visualLvls.Dock, 1);
        }

    }

    private async void LabelTextChanged(object sender, TappedEventArgs e)
    {
        Scroll?.ScrollToAsync(0, 0, true);

    }

    private void ButtonPrev_Clicked(object sender, EventArgs e)
    {
        var vm = (BindingContext as ReadingViewModel);
        vm.LoadTextCommand.Execute(vm.PrevChapterPath);
    }

    private void ButtonNext_Clicked(object sender, EventArgs e)
    {
        var vm = (BindingContext as ReadingViewModel);
        vm.LoadTextCommand.Execute(vm.NextChapterPath);
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }


    private async void OptionsButton_clicked(object sender, EventArgs e)
    {
        var toClose = _uiActivity.Peek();
        if (toClose == visualLvls.Dock)
        {
            _uiActivity.Push(visualLvls.Menu1);
            await UI_Level_Switcher(visualLvls.Menu1, 1);
            return;
        }

        do
        {
            var temp = _uiActivity.Pop();
            UI_Level_Switcher(temp, 0);
        }
        while (_uiActivity.Peek() != visualLvls.Dock);
        if (toClose != visualLvls.Menu1)
        {
            UI_Level_Switcher(visualLvls.Menu1, 1);
            _uiActivity.Push(visualLvls.Menu1);
        }

    }

    private async void GoToChap_Tapped(object sender, EventArgs e)
    {
        var vm = (BindingContext as ReadingViewModel);
        var dialogbox = new GotoChapDialogBox(vm.CurrentBook);
        var chapto = await this.ShowPopupAsync(dialogbox);
        if (chapto is null) { return; }
        TapGestureRecognizer_Tapped(this, new TappedEventArgs(e));
        vm.GoToChapAsync(chapto as int?);
        Scroll?.ScrollToAsync(0, 0, true);
        //await Navigation.PushModalAsync(new GotoChapDialogBox(vm.CurrentBook));

    }



    private async void slider_DragStarted(object sender, EventArgs e)
    {
        fontSizeIndicator.IsEnabled = true;
        fontSizeIndicator.IsVisible = true;
    }

    private void slider_DragCompleted(object sender, EventArgs e)
    {
        fontSizeIndicator.IsEnabled = false;
        fontSizeIndicator.IsVisible = false;
    }

    private async void FontButton_clicked(object sender, EventArgs e)
    {

        var toClose = _uiActivity.Peek();
        if (toClose == visualLvls.Dock)
        {
            _uiActivity.Push(visualLvls.Menu2);
            await UI_Level_Switcher(visualLvls.Menu2, 1);
            return;
        }
        if (toClose != visualLvls.Menu2)
            UI_Level_Switcher(visualLvls.Menu2, 1);
        do
        {
            toClose = _uiActivity.Pop();
            UI_Level_Switcher(toClose, 0);
        }
        while (_uiActivity.Peek() != visualLvls.Dock);

        //font options button
    }
    async Task UI_Level_Switcher(visualLvls lvl, int opacity)
    {
        switch (lvl)
        {
            //case visualLvls.Reading:
            //    {
            //        DockBot.FadeTo(opacity);
            //        await DockTop.FadeTo(opacity);
            //        _currentLvl = lvl;
            //        break;
            //    }
            case visualLvls.Dock:
                {
                    DockBot.FadeTo(opacity);
                    DockTop.FadeTo(opacity);
                    break;
                }
            case visualLvls.Menu1:
                {
                    options.ScaleYTo(opacity);
                    break;
                }
            case visualLvls.Menu2:
                {
                    fontopts.ScaleTo(opacity);
                    break;
                }
            case visualLvls.Menu3:
                {
                    fontpicker.ScaleYTo(opacity);
                    break;
                }
            default:
                {
                    DockBot.FadeTo(0);
                    DockTop.FadeTo(0);
                    options.ScaleYTo(0);
                    fontopts.ScaleTo(0);
                    fontpicker.ScaleYTo(0);
                    //await fontopts.FadeTo(0);
                    break;
                }

        }
    }

    private async void FontSelection_Tapped(object sender, TappedEventArgs e)
    {
        _uiActivity.Push(visualLvls.Menu3);
        await UI_Level_Switcher(visualLvls.Menu3, 1);

    }

    private async void Fontpicker_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var toClose = _uiActivity.Pop();
        CurrentFont = e.Item as string;
        await UI_Level_Switcher(toClose, 0);
    }
}
