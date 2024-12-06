using SQLite;
using System;

namespace trylang.Models
{
    public class MoodEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Mood { get; set; }
        public string MoodName { get; set; }
        public string MoodColor { get; set; }

        public string SelectedEmotions { get; set; } 
        public string Notes { get; set; }
        public string PhotoPaths { get; set; }
        public string Date { get; set; } 



    }
}
