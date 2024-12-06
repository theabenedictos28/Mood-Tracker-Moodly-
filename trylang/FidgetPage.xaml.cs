using System;
using Microsoft.Maui.Controls;

namespace trylang
{
    public partial class FidgetPage : ContentPage
    {
        public FidgetPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to CalendarPage
            await Shell.Current.GoToAsync("//RelaxPage");
        }
        // Handle the tap event for Spinner
        private async void OnSpinnerTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SpinnerPage());

        }

        // Handle the tap event for Pop It
        private async void OnPopItTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PopItPage());

        }

        // Handle the tap event for Bouncy Balls
        private async void OnBouncyBallsTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BouncyBallsPage());
        }

        // Handle the tap event for Ripples
        private async void OnRipplesTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RipplesPage());
        }
    }
}
