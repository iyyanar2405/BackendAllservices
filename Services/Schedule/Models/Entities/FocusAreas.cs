using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("FocusAreas")]
    public class FocusAreas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FocusAreaId")]
        public int FocusAreaId { get; set; }

        [Column("FocusAreaName")]
        public required string FocusAreaName { get; set; }

        [Column("FocusAreaCode")]
        public required string FocusAreaCode { get; set; }

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

        [Column("Category")]
        public string? Category { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
