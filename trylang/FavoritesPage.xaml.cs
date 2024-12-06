using System.Collections.ObjectModel;

namespace trylang
{
    public partial class FavoritesPage : ContentPage
    {
        private ObservableCollection<string> _favoritedAffirmations;

        public FavoritesPage(ObservableCollection<string> favoritedAffirmations)
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            _favoritedAffirmations = favoritedAffirmations;
            FavoritesCollection.ItemsSource = _favoritedAffirmations;
        }

        private async void OnRemoveButtonClicked(object sender, EventArgs e)
        {
            // Get the button that was clicked
            if (sender is ImageButton removeButton) // Change to ImageButton
            {
                // Get the affirmation associated with this button
                var affirmation = removeButton.BindingContext as string;
                if (affirmation != null && _favoritedAffirmations.Contains(affirmation))
                {
                    // Remove the affirmation from the favorites list (UI update)
                    _favoritedAffirmations.Remove(affirmation);

                    // Assuming CurrentUser is set and holds the logged-in user
                    if (App.CurrentUser != null)
                    {
                        // Remove the affirmation from the database
                        await RemoveAffirmationFromDatabaseAsync(App.CurrentUser.Email, affirmation);
                    }
                }
            }
        }
        private async Task RemoveAffirmationFromDatabaseAsync(string email, string affirmation)
        {
            var result = await App.Database.DeleteFavoriteAffirmationAsync(email, affirmation);

            if (result > 0)
            {
                // Successfully deleted from the database
                Console.WriteLine($"Affirmation '{affirmation}' deleted from the database.");
            }
            else
            {
                // Handle any failure or log it
                Console.WriteLine($"Failed to delete affirmation '{affirmation}' from the database.");
            }
        }
    }
}