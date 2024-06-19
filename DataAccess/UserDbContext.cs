using Microsoft.EntityFrameworkCore;
using SimpleTdo.Models;

namespace SimpleTdo.DataAccess
{
    public class UsersDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public UsersDbContext(IConfiguration configuration, DbContextOptions<UsersDbContext> options) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
            }
        }
    }
}