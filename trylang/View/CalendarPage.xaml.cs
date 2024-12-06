using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trylang.Models;
using CommunityToolkit.Maui.Views;

namespace trylang.View
{
    public partial class CalendarPage : ContentPage
    {
        private List<string> quotes = new List<string>
        {
            "Do what you love.",
            "Believe you can and you're halfway there.",
            "The best way to predict the future is to create it.",
            "Make each day your masterpiece",
            "Don’t watch the clock; do what it does. Keep going.",
            "Turn wounds into wisdom."
        };


        public CalendarPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            // Initialize the ListView to be empty at startup
            MoodListView.ItemsSource = null; // Clear any existing entries

            SetRandomQuote();

        }
        private void SetRandomQuote()
        {
            Random random = new Random();
            int index = random.Next(quotes.Count); // Get a random index from the list
            QuoteOfTheDayLabel.Text = quotes[index]; // Set the quote in the Label
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var user = await App.GetUserAsync(App.CurrentUser.Email); // Fetch the user details
            UsernameLabel.Text = $"Hi, {user.Username}!"; // Display the username
        }

        private async void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            MoodListView.ItemsSource = null; // Clear the list
            var filteredEntries = await LoadMoodEntries(e.NewDate);

            if (!filteredEntries.Any())
            {
                await DisplayAlert("No Mood Entries", $"You have not saved any mood entries for {e.NewDate:D}.", "OK");
            }
            else
            {
                // Show the popup with the filtered entries
                var moodPopup = new MoodPopup(filteredEntries);
                this.ShowPopup(moodPopup); // Show the popup
            }
        }

        private async Task<List<MoodEntry>> LoadMoodEntries(DateTime selectedDate)
        {
            var moodEntries = await App.GetMoodEntriesAsync();

            var moodNames = new Dictionary<string, string>
    {
        { "excitedk.png", "Excited" },
        { "happyk.png", "Happy" },
        { "neutralk.png", "Neutral" },
        { "sadk.png", "Sad" },
        { "angryk.png", "Angry" },
        { "hornyk.png", "Horny" }
    };

            var filteredEntries = moodEntries
                .Where(m => DateTime.TryParse(m.Date, out var entryDate) && entryDate.Date == selectedDate.Date)
                .Select(m => new MoodEntry
                {
                    Date = m.Date,
                    Mood = m.Mood,
                    MoodName = moodNames.TryGetValue(m.Mood, out var moodName) ? moodName : "Unknown",
                    Notes = m.Notes,
                    PhotoPaths = m.PhotoPaths,
                    SelectedEmotions = m.SelectedEmotions
                })
                .ToList();

            return filteredEntries;
        }



        private void OnMoodTapped(object sender, EventArgs e)
        {
            // Ensure the sender is an Image (the mood emoji)
            if (sender is Image moodImage)
            {
                // Get the parent StackLayout
                var stackLayout = moodImage.Parent as StackLayout;

                // Find the NotesLabel and MoodPhoto within the StackLayout
                var notesLabel = stackLayout?.Children.OfType<Label>().FirstOrDefault(n => n.StyleId == "NotesLabel");
                var photoImage = stackLayout?.Children.OfType<Image>().FirstOrDefault(img => img.StyleId == "MoodPhoto");

                // Toggle the visibility of the NotesLabel and MoodPhoto
                bool isNotesVisible = false;
                if (notesLabel != null)
                {
                    notesLabel.IsVisible = !notesLabel.IsVisible; // Toggle visibility of the notes
                    isNotesVisible = notesLabel.IsVisible;
                }

                bool isPhotoVisible = false;
                if (photoImage != null)
                {
                    photoImage.IsVisible = !photoImage.IsVisible; // Toggle visibility of the mood photo
                    isPhotoVisible = photoImage.IsVisible;
                }

                // Adjust the position of the QuoteOfTheDayLabel
                if (isNotesVisible || isPhotoVisible)
                {
                    // Move the quote below the mood details (e.g., adding margin)
                    QuoteOfTheDayLabel.Margin = new Thickness(0, 50, 0, 0); // Move down
                }
                else
                {
                    // Reset the quote to its original position
                    QuoteOfTheDayLabel.Margin = new Thickness(0); // Reset to default
                }
            }
        }

        private async void OnSpinnerTapped(object sender, EventArgs e)
        {
            // Navigate to Fidget page or perform any action
            await Shell.Current.GoToAsync("SpinnerPage");
        }

        private async void OnTapItTapped(object sender, EventArgs e)
        {
            // Navigate to Affirmation page or perform any action
            await Shell.Current.GoToAsync("PopItPage");
        }
        private async void OnPopItTapped(object sender, EventArgs e)
        {
            // Navigate to Affirmation page or perform any action
            await Shell.Current.GoToAsync("BouncyBallsPage");
        }
        private async void OnRipplesTapped(object sender, EventArgs e)
        {
            // Navigate to Affirmation page or perform any action
            await Shell.Current.GoToAsync("RipplesPage");
        }
        private async void OnFloatingButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SettingsPage");
        }


    }
}