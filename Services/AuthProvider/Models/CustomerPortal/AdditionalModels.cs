using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthProvider.Models.CustomerPortal
{
    /// <summary>
    /// Error Logs entity
    /// </summary>
    public class ErrorLog
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        [StringLength(255)]
        public string? Source { get; set; }

        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser? User { get; set; }
    }

    /// <summary>
    /// Finding Categories entity
    /// </summary>
    public class FindingCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Finding Statuses entity
    /// </summary>
    public class FindingStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Focus Areas entity
    /// </summary>
    public class FocusArea
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Notification Categories entity
    /// </summary>
    public class NotificationCategory
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
        public virtual ICollection<UserNotificationAccess> UserNotificationAccesses { get; set; } = new List<UserNotificationAccess>();
    }

    /// <summary>
    /// Roles entity
    /// </summary>
    public class CustomerPortalRole
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
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    /// <summary>
    /// Trainings entity
    /// </summary>
    public class Training
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Duration { get; set; } // Duration in hours

        public TrainingType TrainingType { get; set; } = TrainingType.OnlineModule;

        public int? MaxParticipants { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(100)]
        public string? Instructor { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserTraining> UserTrainings { get; set; } = new List<UserTraining>();
    }

    /// <summary>
    /// Users entity
    /// </summary>
    public class CustomerPortalUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserCityAccess> UserCityAccesses { get; set; } = new List<UserCityAccess>();
        public virtual ICollection<UserCountryAccess> UserCountryAccesses { get; set; } = new List<UserCountryAccess>();
        public virtual ICollection<UserNotificationAccess> UserNotificationAccesses { get; set; } = new List<UserNotificationAccess>();
        public virtual ICollection<UserPreference> UserPreferences { get; set; } = new List<UserPreference>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserServiceAccess> UserServiceAccesses { get; set; } = new List<UserServiceAccess>();
        public virtual ICollection<UserTraining> UserTrainings { get; set; } = new List<UserTraining>();
        public virtual ICollection<AuditTeamMember> AuditTeamMembers { get; set; } = new List<AuditTeamMember>();
        public virtual ICollection<CustomerPortalAction> AssignedActions { get; set; } = new List<CustomerPortalAction>();
        public virtual ICollection<CustomerPortalAction> CreatedActions { get; set; } = new List<CustomerPortalAction>();
    }

    /// <summary>
    /// User City Access entity
    /// </summary>
    public class UserCityAccess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CityId { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual City City { get; set; } = null!;
    }

    /// <summary>
    /// User Country Access entity
    /// </summary>
    public class UserCountryAccess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CountryId { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual Country Country { get; set; } = null!;
    }

    /// <summary>
    /// User Notification Access entity
    /// </summary>
    public class UserNotificationAccess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationCategoryId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual NotificationCategory NotificationCategory { get; set; } = null!;
    }

    /// <summary>
    /// User Preferences entity
    /// </summary>
    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
    }

    /// <summary>
    /// User Roles entity
    /// </summary>
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual CustomerPortalRole Role { get; set; } = null!;
    }

    /// <summary>
    /// User Service Access entity
    /// </summary>
    public class UserServiceAccess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual CustomerPortalService Service { get; set; } = null!;
    }

    /// <summary>
    /// User Trainings entity
    /// </summary>
    public class UserTraining
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainingId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public TrainingStatus Status { get; set; } = TrainingStatus.Enrolled;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Progress { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; }

        // Navigation properties
        public virtual CustomerPortalUser User { get; set; } = null!;
        public virtual Training Training { get; set; } = null!;
    }

    // Additional Enums
    
    /// <summary>
    /// Training type enumeration
    /// </summary>
    public enum TrainingType
    {
        OnlineModule = 1,
        InPersonTraining = 2,
        Workshop = 3,
        Seminar = 4,
        Certification = 5,
        Webinar = 6
    }

    /// <summary>
    /// Training enrollment status enumeration
    /// </summary>
    public enum TrainingStatus
    {
        Enrolled = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        Failed = 5
    }
}