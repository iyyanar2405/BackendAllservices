using System.ComponentModel.DataAnnotations;
using AuthProvider.Models.CustomerPortal;

namespace AuthProvider.DTOs.CustomerPortal
{
    // Actions DTOs
    public class ActionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime DueDate { get; set; }
        public ActionStatus Status { get; set; }
        public ActionPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateActionDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public int? AssignedToUserId { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        public ActionPriority Priority { get; set; } = ActionPriority.Medium;
    }

    public class UpdateActionDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public int? AssignedToUserId { get; set; }
        public DateTime DueDate { get; set; }
        public ActionStatus Status { get; set; }
        public ActionPriority Priority { get; set; }
    }

    // Audit DTOs
    public class AuditResponseDto
    {
        public int Id { get; set; }
        public string AuditNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AuditTypeId { get; set; }
        public string AuditTypeName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public int? LeadAuditorId { get; set; }
        public string? LeadAuditorName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public AuditStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TeamMembersCount { get; set; }
        public int ServicesCount { get; set; }
    }

    public class CreateAuditDto
    {
        [Required(ErrorMessage = "Audit number is required")]
        [StringLength(100, ErrorMessage = "Audit number cannot exceed 100 characters")]
        public string AuditNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Audit type is required")]
        public int AuditTypeId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        public int SiteId { get; set; }

        public int? LeadAuditorId { get; set; }

        [Required(ErrorMessage = "Planned start date is required")]
        public DateTime PlannedStartDate { get; set; }

        [Required(ErrorMessage = "Planned end date is required")]
        public DateTime PlannedEndDate { get; set; }
    }

    public class UpdateAuditDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Audit type is required")]
        public int AuditTypeId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        public int SiteId { get; set; }

        public int? LeadAuditorId { get; set; }

        [Required(ErrorMessage = "Planned start date is required")]
        public DateTime PlannedStartDate { get; set; }

        [Required(ErrorMessage = "Planned end date is required")]
        public DateTime PlannedEndDate { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public AuditStatus Status { get; set; }
    }

    // AuditType DTOs
    public class AuditTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuditsCount { get; set; }
    }

    public class CreateAuditTypeDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    public class UpdateAuditTypeDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Audit Service DTOs
    public class AuditServiceResponseDto
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public string AuditNumber { get; set; } = string.Empty;
        public string AuditTitle { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ServiceCode { get; set; }
    }

    public class CreateAuditServiceDto
    {
        [Required(ErrorMessage = "Audit ID is required")]
        public int AuditId { get; set; }

        [Required(ErrorMessage = "Service ID is required")]
        public int ServiceId { get; set; }
    }

    // Audit Team Member DTOs
    public class AuditTeamMemberResponseDto
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public string AuditNumber { get; set; } = string.Empty;
        public string AuditTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }

    public class CreateAuditTeamMemberDto
    {
        [Required(ErrorMessage = "Audit ID is required")]
        public int AuditId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters")]
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateAuditTeamMemberDto
    {
        [Required(ErrorMessage = "Role is required")]
        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters")]
        public string Role { get; set; } = string.Empty;
    }

    // Chapter DTOs
    public class ChapterResponseDto
    {
        public int Id { get; set; }
        public string ChapterNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<ClauseResponseDto>? Clauses { get; set; }
    }

    public class CreateChapterDto
    {
        [Required(ErrorMessage = "Chapter number is required")]
        [StringLength(10, ErrorMessage = "Chapter number cannot exceed 10 characters")]
        public string ChapterNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }

    public class UpdateChapterDto
    {
        [Required(ErrorMessage = "Chapter number is required")]
        [StringLength(10, ErrorMessage = "Chapter number cannot exceed 10 characters")]
        public string ChapterNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Clause DTOs
    public class ClauseResponseDto
    {
        public int Id { get; set; }
        public string ClauseNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ChapterId { get; set; }
        public string ChapterTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateClauseDto
    {
        [Required(ErrorMessage = "Clause number is required")]
        [StringLength(20, ErrorMessage = "Clause number cannot exceed 20 characters")]
        public string ClauseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Chapter is required")]
        public int ChapterId { get; set; }
    }

    public class UpdateClauseDto
    {
        [Required(ErrorMessage = "Clause number is required")]
        [StringLength(20, ErrorMessage = "Clause number cannot exceed 20 characters")]
        public string ClauseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Chapter is required")]
        public int ChapterId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Error Log DTOs
    public class ErrorLogResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateErrorLogDto
    {
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        [StringLength(255, ErrorMessage = "Source cannot exceed 255 characters")]
        public string? Source { get; set; }

        public int? UserId { get; set; }
    }
}