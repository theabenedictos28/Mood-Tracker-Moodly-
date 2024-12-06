using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Threading.Tasks;

namespace trylang
{
    public partial class RipplesPage : ContentPage
    {
        private readonly Random random = new Random();

        public RipplesPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title


            // Set up the tap gesture recognizer
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnScreenTapped;
            RippleArea.GestureRecognizers.Add(tapGestureRecognizer);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false); // Hide the tab bar
        }
        private async void OnScreenTapped(object sender, TappedEventArgs e)
        {
            // Create the ripple circle as a Frame with no background color, only a border
            var ripple = new Frame
            {
                BorderColor = GetRandomColor(),
                BackgroundColor = Colors.Transparent,
                Opacity = 1,
                CornerRadius = 1000, // Make the frame circular
                WidthRequest = 50,
                HeightRequest = 50,
                HasShadow = false
            };

            // Get the tapped location
            var x = e.GetPosition(RippleArea).Value.X;
            var y = e.GetPosition(RippleArea).Value.Y;

            // Set initial position
            AbsoluteLayout.SetLayoutBounds(ripple, new Rect(x - 25, y - 25, 50, 50));
            AbsoluteLayout.SetLayoutFlags(ripple, AbsoluteLayoutFlags.None);
            RippleArea.Children.Add(ripple);

            // Animate the ripple to expand and fade
            await ripple.ScaleTo(4, 800, Easing.CubicOut);
            await ripple.FadeTo(0, 800, Easing.CubicOut);

            // Remove ripple after animation completes
            RippleArea.Children.Remove(ripple);
        }

        private Color GetRandomColor()
        {
            return Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
        }
    }
}
