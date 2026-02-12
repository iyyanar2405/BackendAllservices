using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditSiteAudits")]
    public class AuditSiteAudits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditSiteAuditId")]
        public int AuditSiteAuditId { get; set; }

        [Column("AuditId")]
        public int AuditId { get; set; }

        [Column("SiteId")]
        public int SiteId { get; set; }

        [Column("AuditTypeId")]
        public int AuditTypeId { get; set; }

        [Column("AuditNumber")]
        public required string AuditNumber { get; set; }

        [Column("ScheduledDate")]
        public DateTime? ScheduledDate { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("CompletedDate")]
        public DateTime? CompletedDate { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("LeadAuditorId")]
        public int? LeadAuditorId { get; set; }

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

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("ReportPath")]
        public string? ReportPath { get; set; }

        [Column("CertificateIssued")]
        public bool CertificateIssued { get; set; }

        [Column("CertificateNumber")]
        public string? CertificateNumber { get; set; }
    }
}
