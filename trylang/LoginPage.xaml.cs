using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace trylang
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
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

        // Check if the login input is email or username
        public bool IsValidUsernameOrEmail(string input)
        {
            return IsValidEmail(input) || !string.IsNullOrWhiteSpace(input); // Assuming any non-empty string is a valid username.
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var loginInput = EmailEntry.Text; // Using the same field for email or username
            var password = PasswordEntry.Text;

            // Validate input (either email or username)
            if (!IsValidUsernameOrEmail(loginInput))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid email or username.", "OK");
                return;
            }

            // Validate password
            if (!IsValidPassword(password))
            {
                await DisplayAlert("Invalid Password", "Password must be at least 8 characters long.", "OK");
                return;
            }

            // Get user based on email or username
            var user = await App.GetUserAsync(loginInput);

            if (user != null && user.Password == password)
            {
                App.SetCurrentUser(user); // Set the current user here
                await Shell.Current.GoToAsync("MainPage"); // Navigate to Main Page
            }
            else
            {
                await DisplayAlert("Error", "Invalid email/username or password.", "OK");
            }
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SignUpPage"); // Navigate to Sign Up Page
        }
    }
}
