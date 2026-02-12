// Data/ApplicationDbContext.cs
using FindingsAPI.Gateway;
using Microsoft.EntityFrameworkCore;

namespace FindingsAPI.Gateway.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Finding> Findings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Site> Sites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Finding>()
                .HasOne(f => f.Company)
                .WithMany()
                .HasForeignKey(f => f.CompanyId);

            modelBuilder.Entity<Finding>()
                .HasOne(f => f.Site)
                .WithMany()
                .HasForeignKey(f => f.SiteId);

            // Configure indexes
            modelBuilder.Entity<Finding>()
                .HasIndex(f => f.FindingNumber)
                .IsUnique();

            modelBuilder.Entity<Finding>()
                .HasIndex(f => new { f.Status, f.Category });

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.CompanyName);

            modelBuilder.Entity<Site>()
                .HasIndex(s => s.SiteName);
        }
    }
}