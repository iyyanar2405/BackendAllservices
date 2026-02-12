using Microsoft.EntityFrameworkCore;
using SettingsService.Data.Entities;

namespace SettingsService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SystemPreferenceEntity> SystemPreferences => Set<SystemPreferenceEntity>();
        public DbSet<UserPreferenceProfileEntity> UserPreferenceProfiles => Set<UserPreferenceProfileEntity>();
        public DbSet<NotificationTemplateEntity> NotificationTemplates => Set<NotificationTemplateEntity>();
        public DbSet<CompanySettingEntity> CompanySettings => Set<CompanySettingEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemPreferenceEntity>()
                .HasKey(entity => entity.SystemPreferenceId);

            modelBuilder.Entity<UserPreferenceProfileEntity>()
                .HasKey(entity => entity.UserPreferenceProfileId);

            modelBuilder.Entity<NotificationTemplateEntity>()
                .HasKey(entity => entity.NotificationTemplateId);

            modelBuilder.Entity<CompanySettingEntity>()
                .HasKey(entity => entity.CompanyId);

            modelBuilder.Entity<CompanySettingEntity>()
                .Property(entity => entity.CompanyId)
                .ValueGeneratedNever();

            modelBuilder.Entity<UserEntity>()
                .HasKey(entity => entity.UserId);

            modelBuilder.Entity<UserEntity>()
                .ToTable("Users", tableBuilder => tableBuilder.ExcludeFromMigrations());

            base.OnModelCreating(modelBuilder);
        }
    }
}
