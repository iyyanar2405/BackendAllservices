using Microsoft.EntityFrameworkCore;
using AuthProvider.Models.CustomerPortal;

namespace AuthProvider.Context
{
    /// <summary>
    /// Database context for Customer Portal operations using DB-First approach
    /// </summary>
    public class CustomerPortalContext : DbContext
    {
        public CustomerPortalContext(DbContextOptions<CustomerPortalContext> options) : base(options)
        {
        }

        // All Customer Portal Entities based on existing database tables
        public DbSet<CustomerPortalAction> Actions { get; set; }
        public DbSet<CustomerPortalAudit> Audits { get; set; }
        public DbSet<AuditService> AuditServices { get; set; }
        public DbSet<AuditTeamMember> AuditTeamMembers { get; set; }
        public DbSet<AuditType> AuditTypes { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Clause> Clauses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CustomerPortalService> Services { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<FindingCategory> FindingCategories { get; set; }
        public DbSet<FindingStatus> FindingStatuses { get; set; }
        public DbSet<FocusArea> FocusAreas { get; set; }
        public DbSet<NotificationCategory> NotificationCategories { get; set; }
        public DbSet<CustomerPortalRole> Roles { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<UserCityAccess> UserCityAccess { get; set; }
        public DbSet<UserCountryAccess> UserCountryAccess { get; set; }
        public DbSet<UserNotificationAccess> UserNotificationAccess { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CustomerPortalUser> Users { get; set; }
        public DbSet<UserServiceAccess> UserServiceAccess { get; set; }
        public DbSet<UserTraining> UserTrainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity mappings to match existing database schema
            
            // Actions table mapping
            modelBuilder.Entity<CustomerPortalAction>(entity =>
            {
                entity.ToTable("Actions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Audits table mapping
            modelBuilder.Entity<CustomerPortalAudit>(entity =>
            {
                entity.ToTable("Audits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AuditNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasOne(e => e.AuditType).WithMany(at => at.Audits).HasForeignKey(e => e.AuditTypeId);
                entity.HasOne(e => e.Company).WithMany(c => c.Audits).HasForeignKey(e => e.CompanyId);
                entity.HasOne(e => e.Site).WithMany(s => s.Audits).HasForeignKey(e => e.SiteId);
            });

            // AuditServices table mapping
            modelBuilder.Entity<AuditService>(entity =>
            {
                entity.ToTable("AuditServices");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Audit).WithMany(a => a.AuditServices).HasForeignKey(e => e.AuditId);
                entity.HasOne(e => e.Service).WithMany(s => s.AuditServices).HasForeignKey(e => e.ServiceId);
            });

            // AuditTeamMembers table mapping
            modelBuilder.Entity<AuditTeamMember>(entity =>
            {
                entity.ToTable("AuditTeamMembers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).HasMaxLength(100);
                entity.HasOne(e => e.Audit).WithMany(a => a.AuditTeamMembers).HasForeignKey(e => e.AuditId);
                entity.HasOne(e => e.User).WithMany(u => u.AuditTeamMembers).HasForeignKey(e => e.UserId);
            });

            // AuditTypes table mapping
            modelBuilder.Entity<AuditType>(entity =>
            {
                entity.ToTable("AuditTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Chapters table mapping
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapters");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChapterNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Clauses table mapping
            modelBuilder.Entity<Clause>(entity =>
            {
                entity.ToTable("Clauses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClauseNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.HasOne(e => e.Chapter).WithMany(c => c.Clauses).HasForeignKey(e => e.ChapterId);
            });

            // Cities table mapping
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(10);
                entity.HasOne(e => e.Country).WithMany(c => c.Cities).HasForeignKey(e => e.CountryId);
            });

            // Companies table mapping
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Companies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Website).HasMaxLength(255);
            });

            // Countries table mapping
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(5);
                entity.Property(e => e.ISOCode).HasMaxLength(3);
            });

            // Services table mapping
            modelBuilder.Entity<CustomerPortalService>(entity =>
            {
                entity.ToTable("Services");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Sites table mapping
            modelBuilder.Entity<Site>(entity =>
            {
                entity.ToTable("Sites");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.HasOne(e => e.Company).WithMany(c => c.Sites).HasForeignKey(e => e.CompanyId);
                entity.HasOne(e => e.City).WithMany(c => c.Sites).HasForeignKey(e => e.CityId);
            });

            // ErrorLogs table mapping
            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.ToTable("ErrorLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.StackTrace);
                entity.Property(e => e.Source).HasMaxLength(255);
            });

            // FindingCategories table mapping
            modelBuilder.Entity<FindingCategory>(entity =>
            {
                entity.ToTable("FindingCategories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // FindingStatuses table mapping
            modelBuilder.Entity<FindingStatus>(entity =>
            {
                entity.ToTable("FindingStatuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // FocusAreas table mapping
            modelBuilder.Entity<FocusArea>(entity =>
            {
                entity.ToTable("FocusAreas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // NotificationCategories table mapping
            modelBuilder.Entity<NotificationCategory>(entity =>
            {
                entity.ToTable("NotificationCategories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Roles table mapping
            modelBuilder.Entity<CustomerPortalRole>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Trainings table mapping
            modelBuilder.Entity<Training>(entity =>
            {
                entity.ToTable("Trainings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Duration).HasColumnType("decimal(5,2)");
            });

            // Users table mapping
            modelBuilder.Entity<CustomerPortalUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // User access and relationship tables
            modelBuilder.Entity<UserCityAccess>(entity =>
            {
                entity.ToTable("UserCityAccess");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserCityAccesses).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.City).WithMany(c => c.UserCityAccesses).HasForeignKey(e => e.CityId);
            });

            modelBuilder.Entity<UserCountryAccess>(entity =>
            {
                entity.ToTable("UserCountryAccess");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserCountryAccesses).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Country).WithMany(c => c.UserCountryAccesses).HasForeignKey(e => e.CountryId);
            });

            modelBuilder.Entity<UserNotificationAccess>(entity =>
            {
                entity.ToTable("UserNotificationAccess");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserNotificationAccesses).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.NotificationCategory).WithMany(nc => nc.UserNotificationAccesses).HasForeignKey(e => e.NotificationCategoryId);
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("UserPreferences");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).HasMaxLength(500);
                entity.HasOne(e => e.User).WithMany(u => u.UserPreferences).HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Role).WithMany(r => r.UserRoles).HasForeignKey(e => e.RoleId);
            });

            modelBuilder.Entity<UserServiceAccess>(entity =>
            {
                entity.ToTable("UserServiceAccess");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserServiceAccesses).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Service).WithMany(s => s.UserServiceAccesses).HasForeignKey(e => e.ServiceId);
            });

            modelBuilder.Entity<UserTraining>(entity =>
            {
                entity.ToTable("UserTrainings");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(u => u.UserTrainings).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Training).WithMany(t => t.UserTrainings).HasForeignKey(e => e.TrainingId);
            });
        }
    }
}