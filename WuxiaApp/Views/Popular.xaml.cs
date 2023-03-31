using WuxiaApp.ViewModels;

namespace WuxiaApp.Views;

public partial class Popular : ContentPage
{
	public Popular(PopularViewModel popularViewModel)
	{
		InitializeComponent();
		BindingContext = popularViewModel;
	}

}