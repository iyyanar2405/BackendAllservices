using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditTypes")]
    public class AuditTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditTypeId")]
        public int AuditTypeId { get; set; }

        [Column("AuditTypeName")]
        public required string AuditTypeName { get; set; }

        [Column("AuditTypeCode")]
        public required string AuditTypeCode { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Duration")]
        public int? Duration { get; set; }

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

        [Column("RequiredCertifications")]
        public string? RequiredCertifications { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
