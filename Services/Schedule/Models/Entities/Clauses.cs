using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Clauses")]
    public class Clauses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ClauseId")]
        public int ClauseId { get; set; }

        [Column("ClauseNumber")]
        public required string ClauseNumber { get; set; }

        [Column("ClauseTitle")]
        public required string ClauseTitle { get; set; }

        [Column("ClauseText")]
        public string? ClauseText { get; set; }

        [Column("ChapterId")]
        public int ChapterId { get; set; }

        [Column("ParentClauseId")]
        public int? ParentClauseId { get; set; }

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

        [Column("IsAuditable")]
        public bool IsAuditable { get; set; }
    }
}
