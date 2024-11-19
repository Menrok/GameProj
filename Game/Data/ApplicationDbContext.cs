using Game.Models;
using Microsoft.EntityFrameworkCore;

namespace Game.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hero>()
                .HasOne(h => h.User) 
                .WithMany(u => u.Heroes) 
                .HasForeignKey(h => h.UserId); 
        }
    }
}
