using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("FindingStatuses")]
    public class FindingStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FindingStatusId")]
        public int FindingStatusId { get; set; }

        [Column("StatusName")]
        public required string StatusName { get; set; }

        [Column("StatusCode")]
        public required string StatusCode { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

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

        [Column("Color")]
        public string? Color { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }

        [Column("IsClosedStatus")]
        public bool IsClosedStatus { get; set; }
    }
}
