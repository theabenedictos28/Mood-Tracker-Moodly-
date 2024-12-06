using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using System.Collections.Generic;
using trylang.Models;

namespace trylang
{
    public partial class MoodPopup : Popup
    {
        public MoodPopup(List<MoodEntry> moodEntries)
        {
            InitializeComponent();
            PopupMoodListView.ItemsSource = moodEntries;
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            // Get the MoodEntry from the CommandParameter
            var button = sender as Button;
            if (button?.CommandParameter is MoodEntry selectedMoodEntry)
            {
                App.CurrentMoodEntry = selectedMoodEntry; // Set the current mood entry to be edited
                await Shell.Current.GoToAsync("EditMoodPage"); // Navigate to the edit page using relative route
                Close();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Unable to edit the mood entry.", "OK");
            }
        }


    }
}
