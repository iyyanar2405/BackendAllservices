using FinanceService.Data.Entities;
using FinanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<InvoiceListItem> InvoiceListItems => Set<InvoiceListItem>();
        public DbSet<JsonResponseRecord> JsonResponses => Set<JsonResponseRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceListItem>().HasNoKey();
            modelBuilder.Entity<JsonResponseRecord>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}
