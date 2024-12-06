using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using trylang.Models;

namespace trylang
{
    public partial class MainPage : ContentPage
    {
        private string selectedMood; // To store the selected mood
        private Frame previousMoodFrame; // To keep track of the previous mood frame
        private string selectedPhotoPaths; // To store the selected image path
        private List<string> selectedEmotions = new List<string>();

        public MainPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            selectedMood = ""; // Initialize with a default or leave it empty
            MoodDatePicker.Date = DateTime.Now; // Set the DatePicker to today's date
            MoodDatePicker.MaximumDate = DateTime.Now;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMoodEntries(); // Load existing mood entries when the page appears
        }
        private string GetMoodColor(string mood)
        {
            return mood switch
            {
                "excitedk.png" => "Green",
                "happyk.png" => "Blue",
                "neutralk.png" => "Violet",
                "sadk.png" => "Orange",
                "angryk.png" => "Red",
                _ => "Transparent"
            };
        }


        private async void OnSkipClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CalendarPage"); // Adjust the navigation to your CalendarPage
        }

        private async Task LoadMoodEntries()
        {
            var moodEntries = await App.GetMoodEntriesAsync();
            // Optionally: Update UI to display the mood entries
        }

        private async void OnUploadPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select a mood photo"
                });

                if (result != null)
                {
                    selectedPhotoPaths = result.FullPath;
                    MoodImage.Source = ImageSource.FromFile(selectedPhotoPaths); // Display the selected image
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("File picking failed: " + ex.Message);
            }
        }

        private async void OnSaveMoodClicked(object sender, EventArgs e)
        {
            var moodNames = new Dictionary<string, string>
            {
                { "excitedk.png", "Excited" },
                { "happyk.png", "Happy" },
                { "neutralk.png", "Neutral" },
                { "sadk.png", "Sad" },
                { "angryk.png", "Angry" },
                { "hornyk.png", "Horny" }
            };

            // Get the corresponding mood name
            var moodName = moodNames.TryGetValue(selectedMood, out var name) ? name : "Unknown";
            string moodColor = GetMoodColor(selectedMood); // This line retrieves the mood color based on selectedMood

            // Create a new mood entry
            var moodEntry = new MoodEntry
            {
                Date = MoodDatePicker.Date.ToString("yyyy-MM-dd"), // Store date as ISO 8601
                Mood = selectedMood, // Use the selected mood from emoji
                MoodName = moodName,
                MoodColor = moodColor, // Add this line to store the mood color
                Notes = NotesEditor.Text,
                Email = App.CurrentUser?.Email, // Associate with the current user
                PhotoPaths = selectedPhotoPaths,         // Save the selected photo path
                SelectedEmotions = string.Join(",", selectedEmotions) // Convert the list of selected emotions to a comma-separated string

            };

            // Save the mood entry to the database
            var result = await App.SaveMoodEntryAsync(moodEntry);
            if (result == 0)
            {
                await DisplayAlert("Entry Exists", "You can only add one mood entry per date.", "OK");
                return; // Exit the method if an entry already exists
            }
            var selectedEmotionsString = string.Join(",", selectedEmotions);

            MessagingCenter.Send(this, "MoodEntrySaved");

            await Shell.Current.GoToAsync("//CalendarPage"); // Navigate to Calendar Page
            await LoadMoodEntries(); // Refresh the list of mood entries


            // Clear all fields after saving
            NotesEditor.Text = string.Empty; // Clear the notes editor
            selectedPhotoPaths = null; // Reset the photo path

        }
        private void OnEmotionSelected(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is string emotion)
            {
                if (selectedEmotions.Contains(emotion))
                {
                    selectedEmotions.Remove(emotion); // Deselect emotion
                    button.Source = $"{emotion.ToLower()}.png"; // Default image
                }
                else
                {
                    selectedEmotions.Add(emotion); // Select emotion
                    button.Source = $"{emotion.ToLower()}_selected.png"; // Selected image
                }
            }
        }

        private async void OnMoodTapped(object sender, EventArgs e)
        {
            if (sender is Image moodImage && moodImage.Source is FileImageSource imageSource)
            {
                string mood = imageSource.File; // Get the selected mood image file name
                selectedMood = mood; // Set the selected mood based on the image

                // Zoom in the selected emoji
                if (previousMoodFrame != null)
                {
                    await previousMoodFrame.ScaleTo(1, 50); // Reset the previous mood emoji to original size
                }

                previousMoodFrame = (Frame)moodImage.Parent; // Get the frame of the selected mood image
                await previousMoodFrame.ScaleTo(1.2, 50); // Scale up to 1.2 (zoom in)

                // Show the mood details and expand the section
                MoodDetails.IsVisible = true;

                // Update the label to show selected mood
                NotesEditor.Text = string.Empty; // Clear the notes editor
            }
        }

    }
}
