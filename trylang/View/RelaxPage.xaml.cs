using System;
using Microsoft.Maui.Controls;

namespace trylang.View
{
    public partial class RelaxPage : ContentPage
    {
        public RelaxPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

        }
      

        private async void OnFidgetTapped(object sender, EventArgs e)
        {
            // Navigate to Fidget page or perform any action
            await Shell.Current.GoToAsync("//FidgetPage");
        }

        private async void OnAffirmationTapped(object sender, EventArgs e)
        {
            // Navigate to Affirmation page or perform any action
            await Shell.Current.GoToAsync("//AffirmationPage");
        }
        private async void OnFloatingButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

    }
}
