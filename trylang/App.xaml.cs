using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using trylang.Models;

namespace trylang
{
    public partial class App : Application
    {
        private static AppDbContext _database; // Your private database field

        // Public static property to expose the database
        public static AppDbContext Database => _database;
        public static MoodEntry CurrentMoodEntry { get; set; }
        public static User CurrentUser { get; private set; } // Holds the currently logged-in user

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MoodTracker.db3");
            _database = new AppDbContext(dbPath); // Initialize the database

        }

        public static void SetCurrentUser(User user) // Method to set the current user
        {
            CurrentUser = user;
        }

        public static Task<User> GetUserAsync(string email)
        {
            return _database.GetUserAsync(email);
        }

        public static Task<int> SaveUserAsync(User user)
        {
            return _database.SaveUserAsync(user);
        }

        public static Task<int> SaveMoodEntryAsync(MoodEntry moodEntry)
        {
            return _database.SaveMoodEntryAsync(moodEntry);
        }

        public static async Task<List<MoodEntry>> GetMoodEntriesAsync()
        {
            if (CurrentUser != null)
            {
                return await _database.GetMoodEntriesAsync(CurrentUser.Email); // Pass current user's email
            }
            return new List<MoodEntry>(); // Return empty list if no user is logged in
        }
    }
}
