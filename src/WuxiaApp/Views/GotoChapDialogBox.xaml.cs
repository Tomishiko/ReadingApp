using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Views;
using General.DataModels;
using Microsoft.Maui.Graphics;
using System.Resources;

namespace WuxiaApp.Views;

public partial class GotoChapDialogBox : Popup
{
    int result;
	Style entryInvalid,entryValid;
    public GotoChapDialogBox(Book book)
	{
		InitializeComponent();
        //VisualStateManager.GoToState(grid, "Valid");
        result = book.Readed;
        BindingContext = book;
	}


    private void OK_TapGestureRecognizer(object sender, TappedEventArgs e)
    {
		if (string.IsNullOrEmpty(entry.Text))
			return;
		this.Close(result);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        this.Close();
    }

    private void entry_TextChanged(object sender, TextChangedEventArgs e)
    {
		if (!int.TryParse(e.NewTextValue, out result) || result < 1 || result > (BindingContext as Book).Chapters)
		{

            VisualStateManager.GoToState(grid, "Invalid");
        }
        else
		{
            VisualStateManager.GoToState(grid, "Valid");
            
        }

        
    }
}