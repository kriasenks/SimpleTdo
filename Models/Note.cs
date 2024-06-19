

using System;

namespace SimpleTdo.Models
{
    public class Note
    {
        public Note()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public Note(string title, string description) 
        {
            Title = title; Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
