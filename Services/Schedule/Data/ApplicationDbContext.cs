using Microsoft.EntityFrameworkCore;
using ScheduleService.Models.Entities;

namespace ScheduleService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Actions> Actions { get; set; } = null!;
        public DbSet<AuditLogs> AuditLogs { get; set; } = null!;
        public DbSet<Audits> Audits { get; set; } = null!;
        public DbSet<AuditServices> AuditServices { get; set; } = null!;
        public DbSet<AuditSiteAudits> AuditSiteAudits { get; set; } = null!;
        public DbSet<AuditSiteRepresentatives> AuditSiteRepresentatives { get; set; } = null!;
        public DbSet<AuditSites> AuditSites { get; set; } = null!;
        public DbSet<AuditSiteServices> AuditSiteServices { get; set; } = null!;
        public DbSet<AuditTeamMembers> AuditTeamMembers { get; set; } = null!;
        public DbSet<AuditTypes> AuditTypes { get; set; } = null!;
        public DbSet<CertificateAdditionalScopes> CertificateAdditionalScopes { get; set; } = null!;
        public DbSet<Certificates> Certificates { get; set; } = null!;
        public DbSet<CertificateServices> CertificateServices { get; set; } = null!;
        public DbSet<CertificateSites> CertificateSites { get; set; } = null!;
        public DbSet<Chapters> Chapters { get; set; } = null!;
        public DbSet<Cities> Cities { get; set; } = null!;
        public DbSet<Clauses> Clauses { get; set; } = null!;
        public DbSet<Companies> Companies { get; set; } = null!;
        public DbSet<Contracts> Contracts { get; set; } = null!;
        public DbSet<ContractServices> ContractServices { get; set; } = null!;
        public DbSet<ContractSites> ContractSites { get; set; } = null!;
        public DbSet<Countries> Countries { get; set; } = null!;
        public DbSet<ErrorLogs> ErrorLogs { get; set; } = null!;
        public DbSet<Financials> Financials { get; set; } = null!;
        public DbSet<FindingCategories> FindingCategories { get; set; } = null!;
        public DbSet<FindingClauses> FindingClauses { get; set; } = null!;
        public DbSet<FindingFocusAreas> FindingFocusAreas { get; set; } = null!;
        public DbSet<FindingResponses> FindingResponses { get; set; } = null!;
        public DbSet<Findings> Findings { get; set; } = null!;
        public DbSet<FindingStatuses> FindingStatuses { get; set; } = null!;
        public DbSet<FocusAreas> FocusAreas { get; set; } = null!;
        public DbSet<InvoiceAuditLog> InvoiceAuditLog { get; set; } = null!;
        public DbSet<Invoices> Invoices { get; set; } = null!;
        public DbSet<NotificationCategories> NotificationCategories { get; set; } = null!;
        public DbSet<Notifications> Notifications { get; set; } = null!;
        public DbSet<Roles> Roles { get; set; } = null!;
        public DbSet<ServiceEntity> ServiceEntity { get; set; } = null!;
        public DbSet<Sites> Sites { get; set; } = null!;
        public DbSet<Trainings> Trainings { get; set; } = null!;
        public DbSet<UserCityAccess> UserCityAccess { get; set; } = null!;
        public DbSet<UserCompanyAccess> UserCompanyAccess { get; set; } = null!;
        public DbSet<UserCountryAccess> UserCountryAccess { get; set; } = null!;
        public DbSet<UserNotificationAccess> UserNotificationAccess { get; set; } = null!;
        public DbSet<UserPreferences> UserPreferences { get; set; } = null!;
        public DbSet<UserRoles> UserRoles { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<UserServiceAccess> UserServiceAccess { get; set; } = null!;
        public DbSet<UserSiteAccess> UserSiteAccess { get; set; } = null!;
        public DbSet<UserTrainings> UserTrainings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Actions>().ToTable("Actions");
            modelBuilder.Entity<AuditLogs>().ToTable("AuditLogs");
            modelBuilder.Entity<Audits>().ToTable("Audits");
            modelBuilder.Entity<AuditServices>().ToTable("AuditServices");
            modelBuilder.Entity<AuditSiteAudits>().ToTable("AuditSiteAudits");
            modelBuilder.Entity<AuditSiteRepresentatives>().ToTable("AuditSiteRepresentatives");
            modelBuilder.Entity<AuditSites>().ToTable("AuditSites");
            modelBuilder.Entity<AuditSiteServices>().ToTable("AuditSiteServices");
            modelBuilder.Entity<AuditTeamMembers>().ToTable("AuditTeamMembers");
            modelBuilder.Entity<AuditTypes>().ToTable("AuditTypes");
            modelBuilder.Entity<CertificateAdditionalScopes>().ToTable("CertificateAdditionalScopes");
            modelBuilder.Entity<Certificates>().ToTable("Certificates");
            modelBuilder.Entity<CertificateServices>().ToTable("CertificateServices");
            modelBuilder.Entity<CertificateSites>().ToTable("CertificateSites");
            modelBuilder.Entity<Chapters>().ToTable("Chapters");
            modelBuilder.Entity<Cities>().ToTable("Cities");
            modelBuilder.Entity<Clauses>().ToTable("Clauses");
            modelBuilder.Entity<Companies>().ToTable("Companies");
            modelBuilder.Entity<Contracts>().ToTable("Contracts");
            modelBuilder.Entity<ContractServices>().ToTable("ContractServices");
            modelBuilder.Entity<ContractSites>().ToTable("ContractSites");
            modelBuilder.Entity<Countries>().ToTable("Countries");
            modelBuilder.Entity<ErrorLogs>().ToTable("ErrorLogs");
            modelBuilder.Entity<Financials>().ToTable("Financials");
            modelBuilder.Entity<FindingCategories>().ToTable("FindingCategories");
            modelBuilder.Entity<FindingClauses>().ToTable("FindingClauses");
            modelBuilder.Entity<FindingFocusAreas>().ToTable("FindingFocusAreas");
            modelBuilder.Entity<FindingResponses>().ToTable("FindingResponses");
            modelBuilder.Entity<Findings>().ToTable("Findings");
            modelBuilder.Entity<FindingStatuses>().ToTable("FindingStatuses");
            modelBuilder.Entity<FocusAreas>().ToTable("FocusAreas");
            modelBuilder.Entity<InvoiceAuditLog>().ToTable("InvoiceAuditLog");
            modelBuilder.Entity<Invoices>().ToTable("Invoices");
            modelBuilder.Entity<NotificationCategories>().ToTable("NotificationCategories");
            modelBuilder.Entity<Notifications>().ToTable("Notifications");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<ServiceEntity>().ToTable("Services");
            modelBuilder.Entity<Sites>().ToTable("Sites");
            modelBuilder.Entity<Trainings>().ToTable("Trainings");
            modelBuilder.Entity<UserCityAccess>().ToTable("UserCityAccess");
            modelBuilder.Entity<UserCompanyAccess>().ToTable("UserCompanyAccess");
            modelBuilder.Entity<UserCountryAccess>().ToTable("UserCountryAccess");
            modelBuilder.Entity<UserNotificationAccess>().ToTable("UserNotificationAccess");
            modelBuilder.Entity<UserPreferences>().ToTable("UserPreferences");
            modelBuilder.Entity<UserRoles>().ToTable("UserRoles");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<UserServiceAccess>().ToTable("UserServiceAccess");
            modelBuilder.Entity<UserSiteAccess>().ToTable("UserSiteAccess");
            modelBuilder.Entity<UserTrainings>().ToTable("UserTrainings");
        }
    }
}
