using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditLogs")]
    public class AuditLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditLogId")]
        public int AuditLogId { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("UserName")]
        public string? UserName { get; set; }

        [Column("Action")]
        public required string Action { get; set; }

        [Column("EntityType")]
        public string? EntityType { get; set; }

        [Column("EntityId")]
        public int? EntityId { get; set; }

        [Column("EntityName")]
        public string? EntityName { get; set; }

        [Column("OldValues")]
        public string? OldValues { get; set; }

        [Column("NewValues")]
        public string? NewValues { get; set; }

        [Column("ChangedFields")]
        public string? ChangedFields { get; set; }

        [Column("ActionDate")]
        public DateTime ActionDate { get; set; }

        [Column("IPAddress")]
        public string? IPAddress { get; set; }

        [Column("UserAgent")]
        public string? UserAgent { get; set; }

        [Column("SessionId")]
        public string? SessionId { get; set; }

        [Column("RequestUrl")]
        public string? RequestUrl { get; set; }

        [Column("RequestMethod")]
        public string? RequestMethod { get; set; }

        [Column("Reason")]
        public string? Reason { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("Duration")]
        public int? Duration { get; set; }

        [Column("ApplicationName")]
        public string? ApplicationName { get; set; }

        [Column("Environment")]
        public string? Environment { get; set; }

        [Column("CorrelationId")]
        public string? CorrelationId { get; set; }

        [Column("AdditionalData")]
        public string? AdditionalData { get; set; }

        [Column("CompanyId")]
        public int? CompanyId { get; set; }

        [Column("SiteId")]
        public int? SiteId { get; set; }
    }
}
