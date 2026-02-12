using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("ErrorLogs")]
    public class ErrorLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ErrorLogId")]
        public int ErrorLogId { get; set; }

        [Column("ErrorMessage")]
        public required string ErrorMessage { get; set; }

        [Column("ErrorType")]
        public string? ErrorType { get; set; }

        [Column("Severity")]
        public required string Severity { get; set; }

        [Column("Source")]
        public string? Source { get; set; }

        [Column("StackTrace")]
        public string? StackTrace { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("SessionId")]
        public string? SessionId { get; set; }

        [Column("IPAddress")]
        public string? IPAddress { get; set; }

        [Column("UserAgent")]
        public string? UserAgent { get; set; }

        [Column("RequestUrl")]
        public string? RequestUrl { get; set; }

        [Column("RequestMethod")]
        public string? RequestMethod { get; set; }

        [Column("RequestBody")]
        public string? RequestBody { get; set; }

        [Column("ErrorCode")]
        public string? ErrorCode { get; set; }

        [Column("InnerException")]
        public string? InnerException { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("MachineName")]
        public string? MachineName { get; set; }

        [Column("ProcessId")]
        public int? ProcessId { get; set; }

        [Column("ThreadId")]
        public int? ThreadId { get; set; }

        [Column("ApplicationName")]
        public string? ApplicationName { get; set; }

        [Column("Environment")]
        public string? Environment { get; set; }

        [Column("CorrelationId")]
        public string? CorrelationId { get; set; }

        [Column("AdditionalData")]
        public string? AdditionalData { get; set; }
    }
}
