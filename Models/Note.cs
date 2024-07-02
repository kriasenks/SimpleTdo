namespace SimpleTdo.Models
{
    public class Note
    {
        public Note(string title, string description, Guid userId)
        {
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UserId = userId;
        }
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public Guid UserId { get; init; }  // Добавляем UserId
    }
}