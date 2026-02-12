using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthProvider.Models.CustomerPortal
{
    /// <summary>
    /// Actions entity
    /// </summary>
    public class CustomerPortalAction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? AssignedToUserId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime DueDate { get; set; }
        public ActionStatus Status { get; set; } = ActionStatus.Open;
        public ActionPriority Priority { get; set; } = ActionPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser? AssignedToUser { get; set; }
        public virtual CustomerPortalUser? CreatedByUser { get; set; }
    }

    /// <summary>
    /// Audits entity
    /// </summary>
    public class CustomerPortalAudit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AuditNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int AuditTypeId { get; set; }
        public int CompanyId { get; set; }
        public int SiteId { get; set; }
        public int? LeadAuditorId { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public AuditStatus Status { get; set; } = AuditStatus.Planned;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual AuditType AuditType { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual Site Site { get; set; } = null!;
        public virtual CustomerPortalUser? LeadAuditor { get; set; }
        public virtual ICollection<AuditTeamMember> AuditTeamMembers { get; set; } = new List<AuditTeamMember>();
        public virtual ICollection<AuditService> AuditServices { get; set; } = new List<AuditService>();
    }

    /// <summary>
    /// Audit Services entity
    /// </summary>
    public class AuditService
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public int ServiceId { get; set; }

        // Navigation properties
        public virtual CustomerPortalAudit Audit { get; set; } = null!;
        public virtual CustomerPortalService Service { get; set; } = null!;
    }

    /// <summary>
    /// Audit Team Members entity
    /// </summary>
    public class AuditTeamMember
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public int UserId { get; set; }

        [StringLength(100)]
        public string Role { get; set; } = string.Empty;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalAudit Audit { get; set; } = null!;
        public virtual CustomerPortalUser User { get; set; } = null!;
    }

    /// <summary>
    /// Audit Types entity
    /// </summary>
    public class AuditType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<CustomerPortalAudit> Audits { get; set; } = new List<CustomerPortalAudit>();
    }

    /// <summary>
    /// Chapters entity
    /// </summary>
    public class Chapter
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string ChapterNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Clause> Clauses { get; set; } = new List<Clause>();
    }

    /// <summary>
    /// Clauses entity
    /// </summary>
    public class Clause
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string ClauseNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public int ChapterId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Chapter Chapter { get; set; } = null!;
    }

    /// <summary>
    /// Cities entity
    /// </summary>
    public class City
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Code { get; set; }

        public int CountryId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
        public virtual ICollection<UserCityAccess> UserCityAccesses { get; set; } = new List<UserCityAccess>();
    }

    /// <summary>
    /// Companies entity
    /// </summary>
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
        public virtual ICollection<CustomerPortalAudit> Audits { get; set; } = new List<CustomerPortalAudit>();
    }

    /// <summary>
    /// Countries entity
    /// </summary>
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(5)]
        public string? Code { get; set; }

        [StringLength(3)]
        public string? ISOCode { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        public virtual ICollection<UserCountryAccess> UserCountryAccesses { get; set; } = new List<UserCountryAccess>();
    }

    /// <summary>
    /// Services entity
    /// </summary>
    public class CustomerPortalService
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<AuditService> AuditServices { get; set; } = new List<AuditService>();
        public virtual ICollection<UserServiceAccess> UserServiceAccesses { get; set; } = new List<UserServiceAccess>();
    }

    /// <summary>
    /// Sites entity
    /// </summary>
    public class Site
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public int CompanyId { get; set; }
        public int CityId { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual City City { get; set; } = null!;
        public virtual ICollection<CustomerPortalAudit> Audits { get; set; } = new List<CustomerPortalAudit>();
    }

    // Enums
    public enum ActionStatus
    {
        Open,
        InProgress,
        Completed,
        Cancelled,
        OnHold
    }

    public enum ActionPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum AuditStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled,
        OnHold
    }
}