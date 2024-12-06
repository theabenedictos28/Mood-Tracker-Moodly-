namespace trylang;

public partial class WelcumPage : ContentPage
{
	public WelcumPage()
	{
		InitializeComponent();
	}
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("MainPage"); // Navigate to Sign Up Page
    }
}