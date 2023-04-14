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

    private void Button_Clicked(object sender, EventArgs e)
    {
		var btn = sender as Button;
		btn.Text = "Done!";
		btn.IsEnabled = false;
    }
}