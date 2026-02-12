using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditServices")]
    public class AuditServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditServiceId")]
        public int AuditServiceId { get; set; }

        [Column("AuditId")]
        public int AuditId { get; set; }

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
    }
}
