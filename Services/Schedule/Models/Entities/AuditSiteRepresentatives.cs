using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditSiteRepresentatives")]
    public class AuditSiteRepresentatives
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditSiteRepresentativeId")]
        public int AuditSiteRepresentativeId { get; set; }

        [Column("AuditSiteAuditId")]
        public int AuditSiteAuditId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Role")]
        public string? Role { get; set; }

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

        [Column("ContactPhone")]
        public string? ContactPhone { get; set; }

        [Column("ContactEmail")]
        public string? ContactEmail { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
