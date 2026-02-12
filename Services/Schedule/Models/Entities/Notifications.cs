using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Notifications")]
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("NotificationId")]
        public int NotificationId { get; set; }

        [Column("Title")]
        public required string Title { get; set; }

        [Column("Message")]
        public required string Message { get; set; }

        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Column("CompanyId")]
        public int? CompanyId { get; set; }

        [Column("SiteId")]
        public int? SiteId { get; set; }

        [Column("ServiceId")]
        public int? ServiceId { get; set; }

        [Column("Priority")]
        public required string Priority { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("ReadBy")]
        public string? ReadBy { get; set; }

        [Column("TargetAudience")]
        public string? TargetAudience { get; set; }

        [Column("ActionRequired")]
        public bool ActionRequired { get; set; }

        [Column("ActionUrl")]
        public string? ActionUrl { get; set; }

        [Column("AttachmentPath")]
        public string? AttachmentPath { get; set; }

        [Column("RelatedEntityType")]
        public string? RelatedEntityType { get; set; }

        [Column("RelatedEntityId")]
        public int? RelatedEntityId { get; set; }
    }
}
