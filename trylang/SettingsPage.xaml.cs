using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
using trylang.Models;

namespace trylang;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Navigate back to CalendarPage
        await Shell.Current.GoToAsync("//CalendarPage");
    }
    private async void OnAccountButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AccountPage");
    }

    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {

        await Shell.Current.GoToAsync("//LoginPage");
    }
}