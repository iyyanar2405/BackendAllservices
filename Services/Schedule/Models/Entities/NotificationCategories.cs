using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("NotificationCategories")]
    public class NotificationCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

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

        [Column("Color")]
        public string? Color { get; set; }

        [Column("Icon")]
        public string? Icon { get; set; }

        [Column("Priority")]
        public int? Priority { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
