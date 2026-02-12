using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Actions")]
    public class Actions
    {
        [Key]
        [Column("id")]
        public int? Id { get; set; }

        [Column("action")]
        public string? Action { get; set; }

        [Column("dueDate")]
        public DateTime? DueDate { get; set; }

        [Column("highPriority")]
        public bool? HighPriority { get; set; }

        [Column("message")]
        public string? Message { get; set; }

        [Column("language")]
        public string? Language { get; set; }

        [Column("service")]
        public string? Service { get; set; }

        [Column("site")]
        public string? Site { get; set; }

        [Column("entityType")]
        public string? EntityType { get; set; }

        [Column("entityId")]
        public int? EntityId { get; set; }

        [Column("subject")]
        public string? Subject { get; set; }

        [Column("snowLink")]
        public string? SnowLink { get; set; }
    }
}
