using trylang.Models;
using Microsoft.Maui.Controls;
using System;

namespace trylang;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
        this.Title = string.Empty; // Removes the title

    }

    // Email validation method
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Simplified password validation method (minimum 6 characters)
    public bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text; // New field for confirming password

        // Validate email
        if (!IsValidEmail(email))
        {
            await DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        // Validate password
        if (!IsValidPassword(password))
        {
            await DisplayAlert("Invalid Password", "Password must be at least 8 characters long.", "OK");
            return;
        }

        // Check if passwords match
        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        var existingUser = await App.GetUserAsync(email);
        if (existingUser != null)
        {
            await DisplayAlert("Error", "Email already exists.", "OK");
            return;
        }

        var newUser = new User { Username = username, Email = email, Password = password };
        await App.SaveUserAsync(newUser);

        await DisplayAlert("Success", "Account created successfully. You can now log in.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
