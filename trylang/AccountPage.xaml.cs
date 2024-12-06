using Microsoft.Maui.Controls;
using System;

namespace trylang
{
    public partial class AccountPage : ContentPage
    {
        private bool _isNewPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public AccountPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            LoadUserDetails();
        }
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to CalendarPage
            await Shell.Current.GoToAsync("//SettingsPage");
        }
        private void LoadUserDetails()
        {
            if (App.CurrentUser != null)
            {
                EmailEntry.Text = App.CurrentUser.Email;
                UsernameEntry.Text = App.CurrentUser.Username;
            }
        }

        private void OnToggleNewPasswordVisibility(object sender, EventArgs e)
        {
            _isNewPasswordVisible = !_isNewPasswordVisible;
            NewPasswordEntry.IsPassword = !_isNewPasswordVisible;
            ((Button)sender).Text = _isNewPasswordVisible ? "Hide" : "Show";
        }

        private void OnToggleConfirmPasswordVisibility(object sender, EventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            ConfirmPasswordEntry.IsPassword = !_isConfirmPasswordVisible;
            ((Button)sender).Text = _isConfirmPasswordVisible ? "Hide" : "Show";
        }

        private async void OnUpdatePasswordClicked(object sender, EventArgs e)
        {
            var currentPassword = CurrentPasswordEntry.Text;
            var newPassword = NewPasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(currentPassword) ||
                string.IsNullOrEmpty(newPassword) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (App.CurrentUser.Password != currentPassword)
            {
                await DisplayAlert("Error", "Current password is incorrect.", "OK");
                return;
            }

            if (newPassword != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (newPassword.Length < 8)
            {
                await DisplayAlert("Error", "New password must be at least 8 characters long.", "OK");
                return;
            }

            // Update the password for the current user
            App.CurrentUser.Password = newPassword;
            await App.Database.SaveUserAsync(App.CurrentUser);

            // Clear the password fields
            CurrentPasswordEntry.Text = string.Empty;
            NewPasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await DisplayAlert("Success", "Password updated successfully.", "OK");
        }
    }
}