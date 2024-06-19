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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
        }
    }
}
