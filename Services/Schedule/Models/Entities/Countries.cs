using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Countries")]
    public class Countries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CountryId")]
        public int CountryId { get; set; }

        [Column("CountryName")]
        public required string CountryName { get; set; }

        [Column("CountryCode")]
        public required string CountryCode { get; set; }

        [Column("CountryCodeAlpha2")]
        public required string CountryCodeAlpha2 { get; set; }

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

        [Column("Region")]
        public string? Region { get; set; }

        [Column("Continent")]
        public string? Continent { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; }

        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
    }
}
