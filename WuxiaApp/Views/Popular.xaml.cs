using CommunityToolkit.Maui.Animations;
using Microsoft.Maui.Controls.Shapes;
using WuxiaApp.ViewModels;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace WuxiaApp.Views;

public partial class Popular : ContentPage
{
   
	public Popular(PopularViewModel popularViewModel)
	{
		InitializeComponent();
		BindingContext = popularViewModel;
	}

    private async void CollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
		var currentItem = (VisualElement)sender;

        if (e.PropertyName == nameof(IsVisible) && currentItem.IsVisible)
			await currentItem.FadeTo(1, 1500, Easing.CubicIn);
    }
}
