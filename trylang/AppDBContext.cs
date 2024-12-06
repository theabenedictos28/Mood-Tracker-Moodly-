using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Add this for logging
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using trylang.Models;

namespace trylang
{
    public class AppDbContext
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDbContext(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MoodEntry>().Wait();
            _database.CreateTableAsync<User>().Wait(); // Create User table
            _database.CreateTableAsync<FavoriteAffirmation>().Wait(); // Create FavoriteAffirmation table

        }
        public async Task<MoodEntry> GetMoodEntryByDateAsync(string email, string date)
        {
            return await _database.Table<MoodEntry>()
                                  .FirstOrDefaultAsync(m => m.Email == email && m.Date == date);
        }
        public async Task<int> SaveFavoriteAffirmationAsync(FavoriteAffirmation affirmationEntry)
        {
            return await _database.InsertAsync(affirmationEntry);
        }
        public async Task<List<MoodEntry>> GetMoodEntriesAsync(string username)
        {
            var entries = await _database.Table<MoodEntry>()
                                         .Where(m => m.Email == username) // Filter by username
                                         .ToListAsync();
            Debug.WriteLine($"Retrieved Mood Entries Count: {entries.Count}"); // Log count of retrieved entries
            return entries;
        }


        public async Task<int> SaveMoodEntryAsync(MoodEntry moodEntry)
        {
            // Check if an entry already exists for the selected date
            var existingEntry = await _database.Table<MoodEntry>()
                                               .FirstOrDefaultAsync(m => m.Email == moodEntry.Email && m.Date == moodEntry.Date);
            if (existingEntry != null)
            {
                return 0; // Indicate that no new entry was saved
            }

            var result = await _database.InsertAsync(moodEntry);
            Debug.WriteLine($"Saved Mood Entry with Id: {moodEntry.Id}"); // Log saved mood entry Id
            return result;

        }
        public async Task<int> SaveMEntryAsync(MoodEntry moodEntry)
        {
            // Check if an entry already exists for the selected date
            var existingEntry = await _database.Table<MoodEntry>()
                                               .FirstOrDefaultAsync(m => m.Email == moodEntry.Email && m.Date == moodEntry.Date);
            if (existingEntry != null)
            {
                // If an entry exists, update it
                moodEntry.Id = existingEntry.Id; // Set the Id of the existing entry
                return await UpdateMoodEntryAsync(moodEntry); // Call the update method
            }

            // If no existing entry, insert a new one
            var result = await _database.InsertAsync(moodEntry);
            Debug.WriteLine($"Saved Mood Entry with Id: {moodEntry.Id}"); // Log saved mood entry Id
            return result;
        }


        public async Task<User> GetUserAsync(string loginInput)
        {
            // First, try searching by email
            var user = await _database.Table<User>()
                                      .FirstOrDefaultAsync(u => u.Email == loginInput);

            if (user == null)
            {
                // If no user was found by email, try searching by username
                user = await _database.Table<User>()
                                      .FirstOrDefaultAsync(u => u.Username == loginInput);
            }

            return user;
        }


        public async Task<int> SaveUserAsync(User user)
        {
            // Check if the user already exists in the database
            var existingUser = await _database.Table<User>()
                                              .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                // Update the existing user's password
                existingUser.Password = user.Password;
                return await _database.UpdateAsync(existingUser);
            }
            else
            {
                // If the user doesn't exist, insert a new record
                return await _database.InsertAsync(user);
            }
        }


        public Task<int> RemoveFavoriteAffirmationAsync(FavoriteAffirmation affirmation)
        {
            return _database.DeleteAsync(affirmation);
        }
        public async Task<int> DeleteFavoriteAffirmationAsync(string email, string text)
        {
            var affirmation = await _database.Table<FavoriteAffirmation>()
                                             .FirstOrDefaultAsync(a => a.Email == email && a.Text == text);
            return affirmation != null ? await _database.DeleteAsync(affirmation) : 0;
        }

        public Task<List<FavoriteAffirmation>> GetFavoriteAffirmationsAsync(string email)
        {
            return _database.Table<FavoriteAffirmation>()
                            .Where(a => a.Email == email)
                            .ToListAsync();
        }
       
        public async Task<int> UpdateMoodEntryAsync(MoodEntry moodEntry)
        {
            // Ensure the entry exists before updating
            var existingEntry = await _database.Table<MoodEntry>()
                                               .FirstOrDefaultAsync(m => m.Id == moodEntry.Id);
            if (existingEntry != null)
            {
                // Update the existing entry with new values
                existingEntry.Date = moodEntry.Date;
                existingEntry.Mood = moodEntry.Mood;
                existingEntry.MoodName = moodEntry.MoodName;
                existingEntry.MoodColor = moodEntry.MoodColor;
                existingEntry.Notes = moodEntry.Notes;
                existingEntry.Email = moodEntry.Email;
                existingEntry.PhotoPaths = moodEntry.PhotoPaths;
                existingEntry.SelectedEmotions = moodEntry.SelectedEmotions;

                // Perform the update
                return await _database.UpdateAsync(existingEntry);


            }
            return 0; // Indicate that no entry was updated
        }


    }
}