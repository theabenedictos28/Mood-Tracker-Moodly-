using SQLite;

namespace trylang.Models
{
    public class FavoriteAffirmation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Email { get; set; } // Links to the user
        public string Text { get; set; } // The affirmation text
    }
}
