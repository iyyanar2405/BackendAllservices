using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Findings")]
    public class Findings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FindingId")]
        public int FindingId { get; set; }

        [Column("FindingNumber")]
        public required string FindingNumber { get; set; }

        [Column("AuditId")]
        public int AuditId { get; set; }

        [Column("SiteId")]
        public int? SiteId { get; set; }

        [Column("Title")]
        public required string Title { get; set; }

        [Column("Description")]
        public required string Description { get; set; }

        [Column("FindingType")]
        public required string FindingType { get; set; }

        [Column("Severity")]
        public string? Severity { get; set; }

        [Column("FindingStatusId")]
        public int FindingStatusId { get; set; }

        [Column("FindingCategoryId")]
        public int? FindingCategoryId { get; set; }

        [Column("IdentifiedDate")]
        public DateTime IdentifiedDate { get; set; }

        [Column("DueDate")]
        public DateTime? DueDate { get; set; }

        [Column("ClosedDate")]
        public DateTime? ClosedDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("IdentifiedBy")]
        public int? IdentifiedBy { get; set; }

        [Column("AssignedTo")]
        public int? AssignedTo { get; set; }

        [Column("Evidence")]
        public string? Evidence { get; set; }

        [Column("RootCause")]
        public string? RootCause { get; set; }

        [Column("CorrectiveAction")]
        public string? CorrectiveAction { get; set; }

        [Column("PreventiveAction")]
        public string? PreventiveAction { get; set; }

        [Column("VerificationMethod")]
        public string? VerificationMethod { get; set; }

        [Column("CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Column("VerificationDate")]
        public DateTime? VerificationDate { get; set; }

        [Column("VerifiedBy")]
        public int? VerifiedBy { get; set; }
    }
}
