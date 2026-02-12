using CertificateService.Data.Entities;
using CertificateServiceEntity = CertificateService.Data.Entities.CertificateService;
using Microsoft.EntityFrameworkCore;

namespace CertificateService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Site> Sites => Set<Site>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<CertificateServiceEntity> CertificateServices => Set<CertificateServiceEntity>();
        public DbSet<CertificateSite> CertificateSites => Set<CertificateSite>();
        public DbSet<CertificateAdditionalScope> CertificateAdditionalScopes => Set<CertificateAdditionalScope>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Audit> Audits => Set<Audit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Certificate>().ToTable("Certificates");
            modelBuilder.Entity<Certificate>().HasKey(entity => entity.CertificateId);

            modelBuilder.Entity<Company>().ToTable("Companies");
            modelBuilder.Entity<Company>().HasKey(entity => entity.CompanyId);

            modelBuilder.Entity<Site>().ToTable("Sites");
            modelBuilder.Entity<Site>().HasKey(entity => entity.SiteId);

            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Service>().HasKey(entity => entity.ServiceId);

            modelBuilder.Entity<CertificateServiceEntity>().ToTable("CertificateServices");
            modelBuilder.Entity<CertificateServiceEntity>().HasKey(entity => entity.CertificateServiceId);

            modelBuilder.Entity<CertificateSite>().ToTable("CertificateSites");
            modelBuilder.Entity<CertificateSite>().HasKey(entity => entity.CertificateSiteId);

            modelBuilder.Entity<CertificateAdditionalScope>().ToTable("CertificateAdditionalScopes");
            modelBuilder.Entity<CertificateAdditionalScope>().HasKey(entity => entity.CertificateAdditionalScopeId);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Country>().HasKey(entity => entity.CountryId);

            modelBuilder.Entity<Audit>().ToTable("Audits");
            modelBuilder.Entity<Audit>().HasKey(entity => entity.AuditId);
        }
    }
}
