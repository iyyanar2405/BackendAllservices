using AuditService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditEntity> Audits => Set<AuditEntity>();
        public DbSet<AuditServiceEntity> AuditServices => Set<AuditServiceEntity>();
        public DbSet<AuditSiteEntity> AuditSites => Set<AuditSiteEntity>();
        public DbSet<AuditSiteAuditEntity> AuditSiteAudits => Set<AuditSiteAuditEntity>();
        public DbSet<AuditTeamMemberEntity> AuditTeamMembers => Set<AuditTeamMemberEntity>();
        public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
        public DbSet<SiteEntity> Sites => Set<SiteEntity>();
        public DbSet<CityEntity> Cities => Set<CityEntity>();
        public DbSet<CountryEntity> Countries => Set<CountryEntity>();
        public DbSet<FindingEntity> Findings => Set<FindingEntity>();
        public DbSet<FindingCategoryEntity> FindingCategories => Set<FindingCategoryEntity>();
        public DbSet<FindingStatusEntity> FindingStatuses => Set<FindingStatusEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEntity>().ToTable("Audits").HasKey(item => item.AuditId);
            modelBuilder.Entity<AuditServiceEntity>().ToTable("AuditServices").HasKey(item => item.AuditServiceId);
            modelBuilder.Entity<AuditSiteEntity>().ToTable("AuditSites").HasKey(item => item.AuditSiteId);
            modelBuilder.Entity<AuditSiteAuditEntity>().ToTable("AuditSiteAudits").HasKey(item => item.AuditSiteAuditId);
            modelBuilder.Entity<AuditTeamMemberEntity>().ToTable("AuditTeamMembers").HasKey(item => item.AuditTeamMemberId);
            modelBuilder.Entity<ServiceEntity>().ToTable("Services").HasKey(item => item.ServiceId);
            modelBuilder.Entity<SiteEntity>().ToTable("Sites").HasKey(item => item.SiteId);
            modelBuilder.Entity<CityEntity>().ToTable("Cities").HasKey(item => item.CityId);
            modelBuilder.Entity<CountryEntity>().ToTable("Countries").HasKey(item => item.CountryId);
            modelBuilder.Entity<FindingEntity>().ToTable("Findings").HasKey(item => item.FindingId);
            modelBuilder.Entity<FindingCategoryEntity>().ToTable("FindingCategories").HasKey(item => item.FindingCategoryId);
            modelBuilder.Entity<FindingStatusEntity>().ToTable("FindingStatuses").HasKey(item => item.FindingStatusId);
        }
    }
}
