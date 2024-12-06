using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trylang.Models;

namespace trylang.View
{
    public partial class InsightsPage : ContentPage
    {
        private DateTime _currentDate; 
        public List<MoodEntry> MoodEntries { get; set; }
        public List<CalendarDay> CalendarDays { get; set; }

        public InsightsPage()
        {
            InitializeComponent();
            this.Title = string.Empty; // Removes the title

            _currentDate = DateTime.Now; // Initialize to the current date
            LoadMoodEntries();

            MessagingCenter.Subscribe<EditMoodPage>(this, "MoodUpdated", (sender) => {
                LoadMoodEntries(); // Refresh mood entries
            });

            MessagingCenter.Subscribe<MainPage>(this, "MoodEntrySaved", (sender) => {
                LoadMoodEntries(); // Refresh mood entries
            });
        }




        private async void LoadMoodEntries()
        {
            MoodEntries = await App.GetMoodEntriesAsync(); // Retrieve the mood entries from the database
            UpdateCalendar();
            UpdateMoodStatistics(); // Ensure statistics are updated after loading entries

        }

        private void UpdateCalendar()
        {
            DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            MonthLabel.Text = firstDayOfMonth.ToString("MMMM yyyy");

            CalendarDays = new List<CalendarDay>();

            // Fill empty days for the first week
            for (int i = 0; i < startDay; i++)
            {
                CalendarDays.Add(new CalendarDay { Day = "", MoodColor = "Black" }); // Default color for empty days
            }

            // Populate days of the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(_currentDate.Year, _currentDate.Month, day);
                var moodColor = GetMoodColorForDate(currentDate);

                CalendarDays.Add(new CalendarDay
                {
                    Day = day.ToString(),
                    MoodColor = moodColor
                });
            }

            CalendarCollectionView.ItemsSource = CalendarDays;

            UpdateMoodStatistics();

            bool hasEntriesForTheMonth = MoodEntries.Any(entry => DateTime.Parse(entry.Date).Month == _currentDate.Month && DateTime.Parse(entry.Date).Year == _currentDate.Year);

            // Show or hide the "No Entries" label
            NoEntriesLabel.IsVisible = !hasEntriesForTheMonth;
        }

        private string GetMoodColorForDate(DateTime date)
        {
            var moodsOnDate = MoodEntries
                .Where(entry => DateTime.Parse(entry.Date) == date)
                .Select(entry => GetMoodColor(entry.MoodName))
                .Distinct()
                .ToList();

            // Return the first mood color found or "Black" if none exists
            return moodsOnDate.FirstOrDefault() ?? "Black";
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            UpdateCalendar(); // Refresh calendar
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            UpdateCalendar(); // Refresh calendar
        }

        private string GetMoodColor(string moodName)
        {
            return moodName switch
            {
                "Angry" => "Red",
                "Happy" => "Blue",
                "Sad" => "Orange",
                "Neutral" => "Violet",
                "Excited" => "Green",
                _ => "Transparent"
            };
        }

        private async void OnFloatingButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void UpdateMoodStatistics()
        {
            // Calculate the number of each mood for the selected month
            var moodCounts = MoodEntries
                .Where(entry => DateTime.Parse(entry.Date).Month == _currentDate.Month && DateTime.Parse(entry.Date).Year == _currentDate.Year)
                .GroupBy(entry => entry.MoodName)
                .Select(group => new { Mood = group.Key, Count = group.Count() })
                .ToList();

            // Calculate total number of entries for the month
            int totalEntries = moodCounts.Sum(mc => mc.Count);

            // Prepare the statistics message
            string statsMessage = "Mood Statistics for " + _currentDate.ToString("MMMM yyyy") + ":\n";
            foreach (var mood in moodCounts)
            {
                // Calculate the percentage of each mood
                double percentage = (double)mood.Count / totalEntries * 100;
                statsMessage += $"{mood.Mood}: {mood.Count} ({percentage:F2}%)\n";
            }

            // Display the statistics
            MoodStatsLabel.Text = statsMessage;
        }
    }

    public class CalendarDay
    {
        public string Day { get; set; }
        public string MoodColor { get; set; } // Color for the day's number
    }
}
