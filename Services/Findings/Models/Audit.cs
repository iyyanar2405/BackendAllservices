using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindingsAPI.Gateway
{
    [Table("Audits")]
    public class Audit
    {
        [Key]
        public int AuditId { get; set; }
        public int CompanyId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LeadAuditor { get; set; }
        public string? Type { get; set; }
    }
}
