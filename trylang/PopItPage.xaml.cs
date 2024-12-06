using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace trylang
{
    public partial class PopItPage : ContentPage
    {
        private int score;
        private int timeLeft;
        private int highScore;
        private System.Timers.Timer timer;
        private const string HighScoreKey = "HighScore";

        // Store the selected image
        private string selectedImageSource;

        private List<string> imageSources = new List<string>
        {
            "one.png",
            "two.png",
            "three.png",
            "four.png",
            "five.png"
        };

        public PopItPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            LoadHighScore();
            ResetGame();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false); // Hide the tab bar
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Stop and dispose of the timer
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            // Reset game state (optional, depending on your desired behavior)
            ResetGame();
        }

        private void LoadHighScore()
        {
            highScore = Preferences.Get(HighScoreKey, 0);
            highScoreLabel.Text = $"High Score: {highScore}";
        }

        private void ResetGame()
        {
            score = 0;
            timeLeft = 10;
            scoreLabel.IsVisible = false;
            highScoreLabel.IsVisible = false;
            timerLabel.IsVisible = false;
            selectedImageDisplay.IsVisible = false;
            startButton.IsVisible = false; // Hide the start button initially
        }

        private void OnImageSelected(object sender, EventArgs e)
        {
            // Get the source of the selected image button
            var selectedButton = sender as ImageButton;
            selectedImageSource = (selectedButton.Source as FileImageSource)?.File; // Safely cast to FileImageSource

            // Show the selected image in the display
            selectedImageDisplay.Source = selectedImageSource; // Set the source of the image display
            selectedImageDisplay.IsVisible = true; // Make the display visible

            titleLabel.IsVisible = false; // Hide the title label

            scoreLabel.IsVisible = false;
            highScoreLabel.IsVisible = false;
            timerLabel.IsVisible = false;
            // Hide the image grid after an image is selected
            imageGrid.IsVisible = false;


            // Show the start button after an image is selected
            startButton.IsVisible = true;
            score = 0; // Reset score to 0 or keep it based on your game logic
            scoreLabel.Text = $"Score: {score}";
        }

        private void StartGame()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            score = 0;
            timeLeft = 10;
            scoreLabel.Text = $"Score: {score}";
            timerLabel.Text = $"Time: {timeLeft}";

            // Hide the start button when the game starts
            startButton.IsVisible = false;

            // Show the selected image in the display
            selectedImageDisplay.Source = selectedImageSource; // Set the selected image as the source for the display
            selectedImageDisplay.IsVisible = true; // Ensure the selected image is visible

            // Hide the image grid during the game
            imageGrid.IsVisible = false; // Hide the image grid

            scoreLabel.IsVisible = true;
            highScoreLabel.IsVisible = true;
            timerLabel.IsVisible = true;
            timer = new System.Timers.Timer(1000); // Set the timer to tick every second
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timeLeft--;

            // Update the UI on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                timerLabel.Text = $"Time: {timeLeft}";
                if (timeLeft <= 0)
                {
                    EndGame();
                }
            });
        }

        private void EndGame()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null; // Set timer to null
            }

            // Update high score if current score is greater
            if (score > highScore)
            {
                highScore = score;
                highScoreLabel.Text = $"High Score: {highScore}";
                Preferences.Set(HighScoreKey, highScore); // Save the new high score
            }

            // Display an alert with the final score
            DisplayAlert("Game Over", $"Your final score is: {score}", "OK");

            // Show the start button to allow restarting the game
            startButton.IsVisible = false;

            // Reset the selected image display
            selectedImageDisplay.IsVisible = false; // Hide the selected image display after the game ends

            // Make the image grid visible again to allow the user to choose a new image
            imageGrid.IsVisible = true; // Show the image grid for selection
        }
        private async void OnSelectedImageClicked(object sender, EventArgs e)
        {

            score++; // Increase the score by 1 or however you want to score

            // Update the score label
            scoreLabel.Text = $"Score: {score}";

            await AnimateImage(selectedImageDisplay);

        }

        private async Task AnimateImage(ImageButton imageButton)
        {
            // Scale down and fade out
            await Task.WhenAll(
                imageButton.ScaleTo(0.9, 100, Easing.CubicIn), // Scale down
                imageButton.FadeTo(0.5, 100) // Fade out
            );

            // Scale back up and fade in
            await Task.WhenAll(
                imageButton.ScaleTo(1.0, 100, Easing.CubicOut), // Scale back up
                imageButton.FadeTo(1.0, 100) // Fade in
            );
        }
        
        private void OnStartGameClicked(object sender, EventArgs e)
        {
            StartGame(); // Call the StartGame method when the start button is clicked
        }
    }
}