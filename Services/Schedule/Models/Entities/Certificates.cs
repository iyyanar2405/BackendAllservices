using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Certificates")]
    public class Certificates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CertificateId")]
        public int CertificateId { get; set; }

        [Column("CertificateNumber")]
        public required string CertificateNumber { get; set; }

        [Column("CertificateName")]
        public required string CertificateName { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("SiteId")]
        public int? SiteId { get; set; }

        [Column("ServiceId")]
        public int ServiceId { get; set; }

        [Column("IssueDate")]
        public DateTime IssueDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("CertificateType")]
        public string? CertificateType { get; set; }

        [Column("Scope")]
        public string? Scope { get; set; }

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

        [Column("IssuedBy")]
        public int? IssuedBy { get; set; }

        [Column("RevisionNumber")]
        public int RevisionNumber { get; set; }

        [Column("PreviousCertificateId")]
        public int? PreviousCertificateId { get; set; }

        [Column("CertificatePath")]
        public string? CertificatePath { get; set; }

        [Column("AuditId")]
        public int? AuditId { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
