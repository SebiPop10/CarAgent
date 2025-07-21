using CarAgent_BE.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarAgent_BE.Data
{
    public class ApplicationUser : IdentityUser
    {
        // câmpuri suplimentare (ex: Nume, Prenume)
        public string? FullName { get; set; }
        // Flag dacă userul a venit prin OAuth
        public bool IsExternal { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        // extensii role dacă ai nevoie
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabel Refresh Tokens (îl facem mai jos)
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Index unic pe Email normalizat (deja există în general)
            builder.Entity<ApplicationUser>()
                   .HasIndex(u => u.Email)
                   .IsUnique(false); // Identity oricum gestionează NormalizedEmail

            builder.Entity<RefreshToken>()
                   .HasIndex(r => r.Token)
                   .IsUnique();
        }
    }
}