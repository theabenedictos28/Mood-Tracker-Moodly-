using System;
using Microsoft.Maui.Controls;

namespace trylang
{
    public partial class SpinnerPage : ContentPage
    {
        private bool isSpinning = false;
        private double rotationSpeed = 0; // Adjust rotation speed as necessary

        public SpinnerPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title


            // Attach touch events to the main spinner
            var touchGesture = new PanGestureRecognizer();
            touchGesture.PanUpdated += OnMainSpinnerPanUpdated;
            MainSpinner.GestureRecognizers.Add(touchGesture);
        }


        // Method to switch the main spinner image when an option is selected
        private void OnSpinnerOptionTapped(object sender, EventArgs e)
        {
            if (sender is Image tappedSpinner)
            {
                MainSpinner.Source = tappedSpinner.Source;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false); // Hide the tab bar
        }

        // Handle the pan gesture to start and stop spinning
        private void OnMainSpinnerPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                // Start spinning when touched
                isSpinning = true;
                StartSpinning();
            }
            else if (e.StatusType == GestureStatus.Completed || e.StatusType == GestureStatus.Canceled)
            {
                // Stop spinning when released
                isSpinning = false;
            }
        }

        private async void StartSpinning()
        {
            while (isSpinning)
            {
                rotationSpeed += 5; // Increase rotation speed for smooth spinning
                await MainSpinner.RelRotateTo(rotationSpeed, 50); // Adjust duration for smoother spin
            }

            // Stop the spinner when not spinning
            rotationSpeed = 0;
        }
    }
}
