using System.ComponentModel.DataAnnotations;
using AuthProvider.Models.CustomerPortal;

namespace AuthProvider.DTOs.CustomerPortal
{
    // User DTOs
    /// <summary>
    /// DTO for user response with role and access information
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Related data
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> Cities { get; set; } = new List<string>();
        public IEnumerable<string> Countries { get; set; } = new List<string>();
        public IEnumerable<string> Services { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for creating a new user
    /// </summary>
    public class CreateUserDto
    {
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

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }
    }

    /// <summary>
    /// DTO for updating a user
    /// </summary>
    public class UpdateUserDto
    {
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

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // Role DTOs
    /// <summary>
    /// DTO for role response
    /// </summary>
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsersCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new role
    /// </summary>
    public class CreateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a role
    /// </summary>
    public class UpdateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Training DTOs
    
    /// <summary>
    /// DTO for training response
    /// </summary>
    public class TrainingResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Duration { get; set; }
        public TrainingType TrainingType { get; set; }
        public int? MaxParticipants { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int EnrolledUsersCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new training
    /// </summary>
    public class CreateTrainingDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0.1, 1000)]
        public decimal? Duration { get; set; }

        public TrainingType TrainingType { get; set; } = TrainingType.OnlineModule;

        [Range(1, 1000)]
        public int? MaxParticipants { get; set; }
    }

    /// <summary>
    /// DTO for updating a training
    /// </summary>
    public class UpdateTrainingDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0.1, 1000)]
        public decimal? Duration { get; set; }

        public TrainingType TrainingType { get; set; } = TrainingType.OnlineModule;

        [Range(1, 1000)]
        public int? MaxParticipants { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Categories DTOs
    /// <summary>
    /// DTO for simple category response (used by FindingCategories, FocusAreas, FindingStatuses)
    /// </summary>
    public class SimpleCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for notification category response with user count
    /// </summary>
    public class NotificationCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsersCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a simple category
    /// </summary>
    public class CreateSimpleCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a simple category
    /// </summary>
    public class UpdateSimpleCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // User Access DTOs
    /// <summary>
    /// DTO for user role assignment response
    /// </summary>
    public class UserRoleResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating user role assignment
    /// </summary>
    public class CreateUserRoleDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO for updating user role assignment
    /// </summary>
    public class UpdateUserRoleDto
    {
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for user preferences response
    /// </summary>
    public class UserPreferenceResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating user preference
    /// </summary>
    public class CreateUserPreferenceDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Value { get; set; }
    }

    /// <summary>
    /// DTO for updating user preference
    /// </summary>
    public class UpdateUserPreferenceDto
    {
        [StringLength(500)]
        public string? Value { get; set; }
    }

    /// <summary>
    /// DTO for user access response (generic for city, country, service access)
    /// </summary>
    public class UserAccessResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating user access
    /// </summary>
    public class CreateUserAccessDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int EntityId { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO for updating user access
    /// </summary>
    public class UpdateUserAccessDto
    {
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for user training enrollment response
    /// </summary>
    public class UserTrainingResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TrainingId { get; set; }
        public string TrainingTitle { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TrainingStatus Status { get; set; }
        public decimal? Progress { get; set; }
        public decimal? Score { get; set; }
    }

    /// <summary>
    /// DTO for creating user training enrollment
    /// </summary>
    public class CreateUserTrainingDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TrainingId { get; set; }
    }
}