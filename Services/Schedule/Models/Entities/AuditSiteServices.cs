using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditSiteServices")]
    public class AuditSiteServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditSiteServiceId")]
        public int AuditSiteServiceId { get; set; }

        [Column("AuditSiteAuditId")]
        public int AuditSiteAuditId { get; set; }

        [Column("ServiceId")]
        public int ServiceId { get; set; }

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

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("Cost")]
        public decimal? Cost { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; }
    }
}
