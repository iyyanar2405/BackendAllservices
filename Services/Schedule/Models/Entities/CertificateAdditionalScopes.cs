using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("CertificateAdditionalScopes")]
    public class CertificateAdditionalScopes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CertificateAdditionalScopeId")]
        public int CertificateAdditionalScopeId { get; set; }

        [Column("CertificateId")]
        public int CertificateId { get; set; }

        [Column("ScopeDescription")]
        public required string ScopeDescription { get; set; }

        [Column("ScopeType")]
        public string? ScopeType { get; set; }

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

        [Column("EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
