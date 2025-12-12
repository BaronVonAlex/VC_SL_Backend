using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VC_SL.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace VC_SL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Winrate> Winrates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<IdentityRoleClaim<string>>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<IdentityUserRole<string>>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<IdentityUserClaim<string>>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<IdentityUserLogin<string>>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<IdentityUserToken<string>>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<ApplicationUser>()
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<User>()
            .ToTable("Users")
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<Winrate>()
            .ToTable("Winrates")
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<Winrate>()
            .HasIndex(w => new { w.UserId, w.Year, w.Month })
            .IsUnique();
    }
}