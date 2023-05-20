using CommunityToolkit.Maui.Views;
using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class ReadingView : ContentPage
{
	public ReadingView(ReadingViewModel viewModel)
	{

        Shell.SetTabBarIsVisible(this, false);
        InitializeComponent();
        options.ScaleYTo(0);
        BindingContext = viewModel;

    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		//Shell.SetNavBarIsVisible(this, !Shell.GetNavBarIsVisible(this));
		//DockTop.IsVisible = !DockTop.IsVisible;
		//Dock.IsVisible = !Dock.IsVisible;
		if (Dock.IsVisible)
        {
            if (options.ScaleY != 0)
                await options.ScaleYTo(0,100);

            Dock.FadeTo(0,250);
            BottomBtns.FadeTo(1, 250);

            await DockTop.FadeTo(0,250);
            Dock.IsVisible = !Dock.IsVisible;
            DockTop.IsVisible = !DockTop.IsVisible;
        }
		else
		{
            Dock.IsVisible = !Dock.IsVisible;
            DockTop.IsVisible = !DockTop.IsVisible;
            Dock.FadeTo(1, 250);
            DockTop.FadeTo(1, 250);
            BottomBtns.FadeTo(0, 250);

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
        await Shell.Current.GoToAsync("..",true);
    }


    private async void OptionsButton_clicked(object sender, EventArgs e)
    {
        if (options.ScaleY != 0d && options.ScaleY != 1d)
            return;
        
        var scale = 1 - options.ScaleY;
        options.AnchorY = 0;
        await options.ScaleYTo(scale);
        
        //options.TranslateTo(0, -60);

    }

    private async void GoToChap_Tapped(object sender, EventArgs e)
    {
        var vm = (BindingContext as ReadingViewModel);
        var dialogbox = new GotoChapDialogBox(vm.CurrentBook);
        var chapto = await this.ShowPopupAsync(dialogbox);
        if (chapto is null) { return; }
        TapGestureRecognizer_Tapped(this, new TappedEventArgs(e));
        await vm.GoToChapAsync(chapto as int?);
        //await Navigation.PushModalAsync(new GotoChapDialogBox(vm.CurrentBook));

    }
}