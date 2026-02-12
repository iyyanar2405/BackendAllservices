using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("FindingFocusAreas")]
    public class FindingFocusAreas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FindingFocusAreaId")]
        public int FindingFocusAreaId { get; set; }

        [Column("FindingId")]
        public int FindingId { get; set; }

        [Column("FocusAreaId")]
        public int FocusAreaId { get; set; }

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

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
