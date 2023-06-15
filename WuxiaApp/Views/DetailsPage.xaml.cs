using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Platform;
using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class DetailsPage : ContentPage
{
	DetailsViewModel vm;

	public DetailsPage(DetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm=vm;
		ReadingButton.Bind(
			Button.TextProperty,
			static (DetailsViewModel vm)=>vm.IsInLibrary,
			convert: (bool? isInLib) => (bool)isInLib ? "Continue reading ch. "+vm.Book.Readed : "Start reading");
		AddButton.Bind(
			Button.TextProperty,
			static (DetailsViewModel vm) => vm.IsInLibrary,
			convert: (bool? isInLib) => (bool)isInLib ? "Already in the library" : "Add to library");
		AddButton.Bind(
			Button.IsEnabledProperty,
			static (DetailsViewModel vm) => vm.IsInLibrary,
			convert: (bool? isInLib) => !isInLib);
	}
/// <summary>
/// Add button behavior when book is added to the lib
/// </summary>
    private void Button_Clicked(object sender, EventArgs e)
    {
		var btn = sender as Button;
		btn.Text = "Done!";
		btn.IsEnabled = false;

    }
}