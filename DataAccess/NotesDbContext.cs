using Microsoft.EntityFrameworkCore;
using SimpleTdo.Models;


namespace SimpleTdo.DataAccess
{
    public class NotesDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public NotesDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Note> Notes => Set<Note>();
        public DbSet<User> Users => Set<User>(); // Добавляем пользователей в NotesDbContext

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Note>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(n => n.UserId);
        }
    }
}
