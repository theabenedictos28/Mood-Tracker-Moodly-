using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using trylang.Models;

namespace trylang
{
    public partial class EditMoodPage : ContentPage
    {
        private string selectedMood;
        private Frame previousMoodFrame;
        private string selectedPhotoPaths;
        private List<string> selectedEmotions = new List<string>();
        private bool isEditing = false;
        private MoodEntry currentMoodEntry;

        public EditMoodPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            selectedMood = string.Empty;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (App.CurrentMoodEntry != null)
            {
                PopulateMoodEntry(App.CurrentMoodEntry);
            }
        }

        private async Task LoadMoodEntries()
        {
            var moodEntries = await App.GetMoodEntriesAsync();
            // Optionally, update UI with mood entries if needed
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
                    MoodImage.Source = ImageSource.FromFile(selectedPhotoPaths); // Show the selected image
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("File picking failed: " + ex.Message);
            }
        }

        public void StartEdit(MoodEntry moodEntry)
        {
            currentMoodEntry = moodEntry;
            isEditing = true;

            PopulateMoodEntry(moodEntry);
        }

        private void PopulateMoodEntry(MoodEntry moodEntry)
        {
            MoodDatePicker.Date = DateTime.TryParse(moodEntry.Date, out var parsedDate) ? parsedDate : DateTime.Now;
            selectedMood = moodEntry.Mood;
            selectedPhotoPaths = moodEntry.PhotoPaths;
            MoodImage.Source = !string.IsNullOrEmpty(moodEntry.PhotoPaths)
                ? ImageSource.FromFile(moodEntry.PhotoPaths)
                : null;

            NotesEditor.Text = moodEntry.Notes;



            // Populate selectedEmotions correctly from the mood entry
            selectedEmotions = moodEntry.SelectedEmotions?.Split(',').Where(emotion => !string.IsNullOrWhiteSpace(emotion)).ToList() ?? new List<string>();


            // Update the UI to reflect the selected emotions
            UpdateEmotionButtons();

            ScaleSelectedMoodButton(selectedMood);

        }
        private void ScaleSelectedMoodButton(string mood)
        {
            // Reset scale for all mood buttons
            ResetAllMoodButtonsScale();

            // Find the corresponding Frame for the selected mood
            var frame = FindMoodFrame(mood);
            if (frame != null)
            {
                frame.Scale = 1.2; // Scale up the selected mood
            }
        }


        private void ResetAllMoodButtonsScale()
        {
            var allFrames = new List<Frame> { excitedk, happyk, neutralk, sadk, angryk };

            foreach (var frame in allFrames)
            {
                frame.Scale = 1.0; // Reset scale to normal
            }
        }


        private Frame FindMoodFrame(string mood)
        {
            return mood switch
            {
                "excitedk.png" => excitedk,
                "happyk.png" => happyk,
                "neutralk.png" => neutralk,
                "sadk.png" => sadk,
                "angryk.png" => angryk,
                _ => null
            };
        }


        private void UpdateEmotionButtons()
        {
            // List of all possible emotions
            var allEmotions = new List<string>
    {
        "Bored", "Excited", "Frustrated", "Happy", "Sad",
        "Angry", "Surprised", "Hopeful", "Relax", "Loved",
        "Energized", "Calm", "Stress", "Curious", "Alone"
    };

            foreach (var emotion in allEmotions)
            {
                var button = FindEmotionButton(emotion);
                if (button != null)
                {
                    // Check if the emotion is selected
                    if (selectedEmotions.Contains(emotion))
                    {
                        button.Source = $"{emotion.ToLower()}_selected.png"; // Set to selected image
                    }
                    else
                    {
                        button.Source = $"{emotion.ToLower()}.png"; // Set to default image
                    }
                }
            }
        }

        private ImageButton FindEmotionButton(string emotion)
        {
            // Find the button by its name
            return this.FindByName<ImageButton>(emotion.ToLower());
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

            var moodEntry = new MoodEntry
            {
                Id = isEditing ? currentMoodEntry.Id : 0,
                Date = MoodDatePicker.Date.ToString("yyyy-MM-dd"),
                Mood = selectedMood,
                MoodName = moodName,
                MoodColor = moodColor,
                Notes = NotesEditor.Text,
                Email = App.CurrentUser?.Email,
                PhotoPaths = selectedPhotoPaths,
                SelectedEmotions = string.Join(",", selectedEmotions) // Ensure this is a comma-separated string
            };

            if (isEditing)
            {
                await App.Database.UpdateMoodEntryAsync(moodEntry);
                await DisplayAlert("Success", "Mood entry updated successfully.", "OK");
                isEditing = false;


            }
            else
            {
                await App.Database.SaveMEntryAsync(moodEntry);
                await DisplayAlert("Success", "Mood entry saved successfully.", "OK");

                MessagingCenter.Send(this, "MoodUpdated");

            }

            ClearMoodEntryFields();
            await Shell.Current.GoToAsync("//CalendarPage");
        }
        private void ClearMoodEntryFields()
        {
            selectedMood = string.Empty;
            NotesEditor.Text = string.Empty;
            selectedEmotions.Clear();
            MoodImage.Source = null;
            selectedPhotoPaths = null;
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

        private async void OnMoodTapped(object sender, EventArgs e)
        {
            if (sender is Image moodImage && moodImage.Source is FileImageSource imageSource)
            {
                selectedMood = imageSource.File;

                // Reset scale for all mood frames in the MoodStack
                foreach (var child in MoodStack.Children)
                {
                    if (child is Frame moodFrame)
                    {
                        await moodFrame.ScaleTo(1, 50);
                    }
                }

                // Scale the currently selected mood
                if (moodImage.Parent is Frame selectedMoodFrame)
                {
                    await selectedMoodFrame.ScaleTo(1.2, 50);
                    previousMoodFrame = selectedMoodFrame; // Update the reference to the currently selected mood frame
                }
            }
        }



        private void OnEmotionSelected(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is string emotion)
            {
                if (selectedEmotions.Contains(emotion))
                {
                    selectedEmotions.Remove(emotion);
                    button.Source = $"{emotion.ToLower()}.png"; // Set to unselected image
                    Debug.WriteLine($"Removed emotion: {emotion}");
                }
                else
                {
                    selectedEmotions.Add(emotion);
                    button.Source = $"{emotion.ToLower()}_selected.png"; // Set to selected image
                    Debug.WriteLine($"Added emotion: {emotion}");
                }
            }
        }
    }
}
