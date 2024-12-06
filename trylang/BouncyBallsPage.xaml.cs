using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Storage; // Add this namespace for Preferences


namespace trylang
{
    public partial class BouncyBallsPage : ContentPage
    {
        private Random _random = new Random();
        private int _score = 0;
        private int _timeLeft = 10;
        private int _highScore = 0; // High score variable
        private bool _isGameRunning = false; // Track if the game is running

        public BouncyBallsPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title
            LoadHighScore(); // Load the high score when the page is initialized

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false); // Hide the tab bar
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Check if the game is running
            if (_isGameRunning)
            {
                _isGameRunning = false; // Mark the game as not running

                // Stop any timers or ongoing animations
                Device.BeginInvokeOnMainThread(() =>
                {
                    // Stop any timers or game actions here
                    _timeLeft = 0; // Ensure the timer stops
                    GameArea.Children.Clear(); // Clear game area

                    // Optionally, reset game UI elements like buttons
                    StartGameButton.IsVisible = true; // Make the Start button visible for when the user returns
                });

                // Don't display the alert again when exiting
                // Just make sure game state is reset
            }
        }


        private void LoadHighScore()
        {
            // Load the high score from preferences
            _highScore = Preferences.Get("HighScore", 0); // Default to 0 if not set
            HighScoreLabel.Text = $"High Score: {_highScore}";
        }

        private void OnStartGameClicked(object sender, EventArgs e)
        {
            if (!_isGameRunning)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            _isGameRunning = true;
            _score = 0;
            _timeLeft = 10;
            ScoreLabel.Text = "Score: 0";
            TimerLabel.Text = "Time: 10";
            StartGameButton.IsVisible = false; // Hide the start button
            GameArea.Children.Clear(); // Clear previous bubbles
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), UpdateTimer);
            GenerateBubbles();
        }

        private bool UpdateTimer()
        {
            _timeLeft--;
            TimerLabel.Text = $"Time: {_timeLeft}";

            if (_timeLeft <= 0)
            {
                EndGame();
                return false; // Stop the timer
            }
            return true; // Keep the timer running
        }

        private void GenerateBubbles()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(800), () =>
            {
                if (_timeLeft <= 0) return false;

                // Ensure GameArea is valid
                if (GameArea == null)
                {
                    Console.WriteLine("GameArea is null.");
                    return false; // Stop if GameArea is not available
                }

                double maxX = Math.Max(0, GameArea.Width - 100); // Ensure maxX is at least 0
                double maxY = Math.Max(50, GameArea.Height - 100); // Ensure maxY is at least 50

                var bubbleImage = new Image
                {
                    Source = "cute.png", // Ensure this image exists
                    WidthRequest = 70, // Fixed width
                    HeightRequest = 70, // Fixed height
                    TranslationX = _random.Next(0, (int)maxX),
                    TranslationY = _random.Next(50, (int)maxY),
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => PopBubble(bubbleImage);
                bubbleImage.GestureRecognizers.Add(tapGesture);

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        GameArea.Children.Add(bubbleImage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding bubble: {ex.Message}");
                    }
                });

                bubbleImage.FadeTo(0, 3000).ContinueWith(t =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        GameArea.Children.Remove(bubbleImage);
                    });
                });

                return true; // Keep generating bubbles
            });
        }

        private async void PopBubble(Image bubbleImage)
        {
            await bubbleImage.ScaleTo(2, 150, Easing.SpringIn); // Scale up
            await bubbleImage.FadeTo(0, 300, Easing.CubicOut); // Fade out
            await bubbleImage.RotateTo(720, 300, Easing.CubicInOut); // Rotate the bubble

            // Remove the bubble from the game area
            GameArea.Children.Remove(bubbleImage);

            // Increment the score
            _score += 1;

            // Update the high score if necessary
            if (_score > _highScore)
            {
                _highScore = _score;
                HighScoreLabel.Text = $"High Score: {_highScore}";
            }

            // Animate the score label
            await ScoreLabel.ScaleTo(1.5, 100); // Grow the label slightly
            ScoreLabel.Text = $"Score: {_score}";
            await ScoreLabel.ScaleTo(1, 100); // Return to original size
        }

        private void EndGame()
        {
            // Check if the game is running, and stop if it's still running
            if (!_isGameRunning) return;

            _isGameRunning = false; // Mark the game as not running

            // Save the high score
            Preferences.Set("HighScore", _highScore);

            // Display the game over alert only when the game finishes normally
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Display the alert when the game is over
                await DisplayAlert("Game Over", $"Your final score is {_score}!", "OK");

                // Reset the game UI elements for the next game
                GameArea.Children.Clear();
                StartGameButton.IsVisible = true; // Make the Start button visible again
            });
        }

    }
}
