using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NotificationFilterItem>().HasNoKey();
            modelBuilder.Entity<NotificationRow>().HasNoKey();
            modelBuilder.Entity<NotificationSiteRow>().HasNoKey();
        }
    }
}
