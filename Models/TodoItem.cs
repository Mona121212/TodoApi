using System.Data;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public bool IsCompleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        // every TodoItem belongs to  a User
        // UserId defines the foreign key relationship in the database, while User is the navigation property that allows us to access the related entity in code. Without UserId, the relationship cannot be stored properly. Without User, we cannot easily access the related object in C#.
        public int UserId { get; set; }
        // Navigation property to User
        public User? User { get; set; }
    }
}
