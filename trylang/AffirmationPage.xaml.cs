using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
using trylang.Models;

namespace trylang
{
    public partial class AffirmationPage : ContentPage
    {

        private ObservableCollection<Affirmation> affirmations;
        private int currentIndex;

        public AffirmationPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            InitializeAffirmations();
            UpdateCard(); // Set the initial card text
        }

        private void InitializeAffirmations()
        {
            affirmations = new ObservableCollection<Affirmation>
            {
                new Affirmation { Text = "I am capable of achieving my goals." },
                new Affirmation { Text = "Every day, I am becoming a better version of myself." },
                new Affirmation { Text = "I am worthy of love and respect." },
                new Affirmation { Text = "I attract positivity and good energy." },
                new Affirmation { Text = "I am resilient and can overcome challenges." },
                new Affirmation { Text = "I believe in my abilities and express my true self." },
                new Affirmation { Text = "I am grateful for the abundance in my life." }
            };
            currentIndex = 0;
        }
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to CalendarPage
            await Shell.Current.GoToAsync("//RelaxPage");
        }
        private async void OnCardTapped(object sender, EventArgs e)
        {
            // Fade out the current card
            await CurrentCard.FadeTo(0, 200);

            // Move to the next affirmation
            currentIndex++;
            if (currentIndex >= affirmations.Count)
            {
                currentIndex = 0; // Loop back to the first card
            }

            // Update the text of the current card
            UpdateCard();

            // Fade in the new card
            await CurrentCard.FadeTo(1, 200);
        }
        


        private void UpdateCard()
        {
            AffirmationLabel.Text = affirmations[currentIndex].Text;

            // Ensure the HeartButton reflects the current favorite state
            HeartButton.Text = affirmations[currentIndex].IsFavorite ? "❤️" : "🤍";
        }

        private async void OnViewFavoritesClicked(object sender, EventArgs e)
        {
            var favoriteAffirmations = await App.Database.GetFavoriteAffirmationsAsync(App.CurrentUser.Email);
            await Navigation.PushAsync(new FavoritesPage(new ObservableCollection<string>(favoriteAffirmations.Select(a => a.Text))));
        }

        private async void OnHeartButtonClicked(object sender, EventArgs e)
        {
            string currentAffirmation = AffirmationLabel.Text;

            // Toggle favorite status
            if (affirmations[currentIndex].IsFavorite)
            {
                // Remove from favorites
                await App.Database.DeleteFavoriteAffirmationAsync(App.CurrentUser.Email, currentAffirmation);
                affirmations[currentIndex].IsFavorite = false;
                HeartButton.Text = "🤍";
            }
            else
            {
                // Add to favorites
                var affirmationEntry = new FavoriteAffirmation
                {
                    Email = App.CurrentUser.Email,
                    Text = currentAffirmation
                };
                await App.Database.SaveFavoriteAffirmationAsync(affirmationEntry);
                affirmations[currentIndex].IsFavorite = true;
                HeartButton.Text = "❤️";
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            // Ensure the currentIndex is within the range of affirmations
            if (currentIndex >= 0 && currentIndex < affirmations.Count)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = affirmations[currentIndex].Text, // Get the affirmation text to share
                    Title = "Share Affirmation"               // Title for the share dialog
                });
            }
            else
            {
                // Optional: Handle the case where currentIndex is out of range
                await Application.Current.MainPage.DisplayAlert("Error", "No affirmation to share.", "OK");
            }
        }
    }


    public class Affirmation
    {
        public string Text { get; set; }
        public bool IsFavorite { get; set; } = false; // Default not favorite
    }
}
