using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Chapters")]
    public class Chapters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ChapterId")]
        public int ChapterId { get; set; }

        [Column("ChapterNumber")]
        public required string ChapterNumber { get; set; }

        [Column("ChapterTitle")]
        public required string ChapterTitle { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("StandardId")]
        public int? StandardId { get; set; }

        [Column("ParentChapterId")]
        public int? ParentChapterId { get; set; }

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

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }

        [Column("Level")]
        public int? Level { get; set; }
    }
}
