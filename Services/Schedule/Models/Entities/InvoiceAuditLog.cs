using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("InvoiceAuditLog")]
    public class InvoiceAuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("InvoiceAuditLogId")]
        public int InvoiceAuditLogId { get; set; }

        [Column("InvoiceId")]
        public int InvoiceId { get; set; }

        [Column("InvoiceNumber")]
        public required string InvoiceNumber { get; set; }

        [Column("Action")]
        public required string Action { get; set; }

        [Column("OldValue")]
        public string? OldValue { get; set; }

        [Column("NewValue")]
        public string? NewValue { get; set; }

        [Column("ChangedFields")]
        public string? ChangedFields { get; set; }

        [Column("Reason")]
        public string? Reason { get; set; }

        [Column("ActionDate")]
        public DateTime ActionDate { get; set; }

        [Column("ActionBy")]
        public int ActionBy { get; set; }

        [Column("IPAddress")]
        public string? IPAddress { get; set; }

        [Column("UserAgent")]
        public string? UserAgent { get; set; }
    }
}
