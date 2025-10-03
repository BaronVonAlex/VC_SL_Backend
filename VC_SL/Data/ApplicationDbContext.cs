using VC_SL.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace VC_SL.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Winrate> Winrates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Winrate>()
                .HasIndex(w => new { w.UserId, w.Year, w.Month })
                .IsUnique();
        }
    }
}
