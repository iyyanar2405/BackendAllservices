using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("FindingCategories")]
    public class FindingCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FindingCategoryId")]
        public int FindingCategoryId { get; set; }

        [Column("CategoryName")]
        public required string CategoryName { get; set; }

        [Column("CategoryCode")]
        public required string CategoryCode { get; set; }

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

        [Column("ParentCategoryId")]
        public int? ParentCategoryId { get; set; }

        [Column("Color")]
        public string? Color { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
