using CommunityToolkit.Maui.Views;
using WuxiaApp.Servs;
using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class ReadingView : ContentPage
{
    Stack<visualLvls> _uiStack;
    enum visualLvls
    {
        Reading,
        Dock,
        options,
        fontopts,
        fontpicker,
        backgroundColor,
        Init
    }

    public PreferenceServices preference { get; private set; }
    public List<string> Fonts { get { return PreferenceServices.Fonts; } }
    public List<Color> Backgrounds { get { return PreferenceServices.Backgrounds; } }
    
    public ReadingView(ReadingViewModel viewModel, PreferenceServices services)
    {
        preference = services;
        Shell.SetTabBarIsVisible(this, false);
        InitializeComponent();
        fontpicker.BindingContext = this;
        colorCollection.BindingContext = this;
        UI_Groups(visualLvls.Init, 0);
        _uiStack = new Stack<visualLvls>();
        //colorCollection.ItemsSource = services.Backgrounds;
        //fontpicker.ItemsSource = services.Fonts;
        colorCollection.SelectedItem = services.BackColor;
        colorCollection.ScaleTo(0);
        this.Disappearing += ReadingView_Disappearing;
        BindingContext = viewModel;
    }

    private void ReadingView_Disappearing(object sender, EventArgs e)
    {

        var currentTheme = Application.Current.RequestedTheme;
        if(currentTheme == AppTheme.Dark)
        {
            StatusBar.StatusBarColor = Color.FromRgb(0, 0, 0);
            StatusBar.StatusBarStyle = CommunityToolkit.Maui.Core.StatusBarStyle.LightContent;
        }
        else
        {
            StatusBar.StatusBarColor = Color.FromRgb(255, 255, 255);
            StatusBar.StatusBarStyle = CommunityToolkit.Maui.Core.StatusBarStyle.DarkContent;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

        if (_uiStack.TryPop(out visualLvls toClose))
            UI_Groups(toClose, 0);
        else
        {
            _uiStack.Push(visualLvls.Dock);
            UI_Groups(visualLvls.Dock, 1);
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
        UI_Groups_Swithcer(visualLvls.options);
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
        UI_Groups_Swithcer(visualLvls.fontopts);
    }
    private void UI_Groups(visualLvls lvl, int opacity)
    {
        switch (lvl)
        {
            case visualLvls.Dock:
                {
                    DockBot.FadeTo(opacity);
                    DockTop.FadeTo(opacity);
                    break;
                }
            case visualLvls.options:
                {
                    options.ScaleYTo(opacity);
                    break;
                }
            case visualLvls.fontopts:
                {
                    fontopts.ScaleTo(opacity);
                    break;
                }
            case visualLvls.fontpicker:
                {
                    fontpicker.ScaleYTo(opacity);
                    break;
                }
            case visualLvls.backgroundColor:
                {
                    colorCollection.ScaleTo(opacity);
                    break;
                }
            default:
                {
                    DockBot.FadeTo(0);
                    DockTop.FadeTo(0);
                    options.ScaleYTo(0);
                    fontopts.ScaleTo(0);
                    fontpicker.ScaleYTo(0);
                    break;
                }

        }
    }

    private void UI_Groups_Swithcer(visualLvls lvl)
    {
        var initialState = _uiStack.Peek();
        if (initialState == visualLvls.Dock)
        {
            _uiStack.Push(lvl);
            UI_Groups(lvl, 1);
            return;
        }

        do
        {
            var toClose = _uiStack.Pop();
            UI_Groups(toClose, 0);

        } while (_uiStack.Peek() != visualLvls.Dock);

        if (initialState != lvl)
        {
            UI_Groups(lvl, 1);
            _uiStack.Push(lvl);
        }

    }

    private async void FontSelection_Tapped(object sender, TappedEventArgs e)
    {
        _uiStack.Push(visualLvls.fontpicker);
        UI_Groups(visualLvls.fontpicker, 1);

    }

    private async void Fontpicker_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var toClose = _uiStack.Pop();
        preference.Font = e.Item as string;
        UI_Groups(toClose, 0);
    }


    private void ColorOptionsTapped(object sender, TappedEventArgs e)
    {
        UI_Groups_Swithcer(visualLvls.backgroundColor);
    }

    private void colorCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        preference.BackColor = e.CurrentSelection[0] as Color;
    }
}
