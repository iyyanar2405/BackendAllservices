using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditSites")]
    public class AuditSites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditSiteId")]
        public int AuditSiteId { get; set; }

        [Column("AuditId")]
        public int AuditId { get; set; }

        [Column("SiteId")]
        public int SiteId { get; set; }

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

        [Column("Status")]
        public string? Status { get; set; }

        [Column("ScheduledDate")]
        public DateTime? ScheduledDate { get; set; }

        [Column("CompletedDate")]
        public DateTime? CompletedDate { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
