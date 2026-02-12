using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Services")]
    public class ServiceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ServiceId")]
        public int ServiceId { get; set; }

        [Column("ServiceName")]
        public required string ServiceName { get; set; }

        [Column("ServiceCode")]
        public required string ServiceCode { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("ServiceType")]
        public string? ServiceType { get; set; }

        [Column("Category")]
        public string? Category { get; set; }

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

        [Column("Duration")]
        public int? Duration { get; set; }

        [Column("Cost")]
        public decimal? Cost { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; }

        [Column("Prerequisites")]
        public string? Prerequisites { get; set; }

        [Column("ValidityPeriod")]
        public int? ValidityPeriod { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
