using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Audits")]
    public class Audits
    {
        [Key]
        [Column("auditId")]
        public int? AuditId { get; set; }

        [Column("sites")]
        public string? Sites { get; set; }

        [Column("services")]
        public string? Services { get; set; }

        [Column("companyId")]
        public int? CompanyId { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("startDate")]
        public DateTime? StartDate { get; set; }

        [Column("endDate")]
        public DateTime? EndDate { get; set; }

        [Column("leadAuditor")]
        public string? LeadAuditor { get; set; }

        [Column("type")]
        public string? Type { get; set; }
    }
}
