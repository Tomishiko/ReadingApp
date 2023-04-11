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
		
	}

}