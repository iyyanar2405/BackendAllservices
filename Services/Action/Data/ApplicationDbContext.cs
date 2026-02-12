using ActionService.Data.Entities;
using ActionService.Data.Queries;
using ActionService.Models;
using Microsoft.EntityFrameworkCore;

namespace ActionService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActionEntity> Actions => Set<ActionEntity>();
        public DbSet<ActionRow> ActionRows => Set<ActionRow>();
        public DbSet<ActionSiteRow> ActionSiteRows => Set<ActionSiteRow>();
        public DbSet<ActionFilterItem> ActionFilterItems => Set<ActionFilterItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ActionEntity>(entity =>
            {
                entity.ToTable("Actions");
                entity.HasKey(action => action.Id);
                entity.Property(action => action.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();
                entity.Property(action => action.Action)
                    .HasColumnName("action")
                    .HasColumnType("nvarchar(255)");
                entity.Property(action => action.DueDate)
                    .HasColumnName("dueDate")
                    .HasColumnType("datetime");
                entity.Property(action => action.HighPriority)
                    .HasColumnName("highPriority")
                    .HasColumnType("bit");
                entity.Property(action => action.Message)
                    .HasColumnName("message")
                    .HasColumnType("nvarchar(max)");
                entity.Property(action => action.Language)
                    .HasColumnName("language")
                    .HasColumnType("nvarchar(50)");
                entity.Property(action => action.Service)
                    .HasColumnName("service")
                    .HasColumnType("nvarchar(100)");
                entity.Property(action => action.Site)
                    .HasColumnName("site")
                    .HasColumnType("nvarchar(100)");
                entity.Property(action => action.EntityType)
                    .HasColumnName("entityType")
                    .HasColumnType("nvarchar(100)");
                entity.Property(action => action.EntityId)
                    .HasColumnName("entityId")
                    .HasColumnType("int");
                entity.Property(action => action.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("nvarchar(255)");
                entity.Property(action => action.SnowLink)
                    .HasColumnName("snowLink")
                    .HasColumnType("nvarchar(255)");
            });

            modelBuilder.Entity<ActionRow>().HasNoKey().ToView(null);
            modelBuilder.Entity<ActionSiteRow>().HasNoKey().ToView(null);
            modelBuilder.Entity<ActionFilterItem>().HasNoKey().ToView(null);
        }
    }
}
