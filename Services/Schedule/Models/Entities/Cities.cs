using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Cities")]
    public class Cities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CityId")]
        public int CityId { get; set; }

        [Column("CityName")]
        public required string CityName { get; set; }

        [Column("CountryId")]
        public int CountryId { get; set; }

        [Column("StateProvince")]
        public string? StateProvince { get; set; }

        [Column("PostalCode")]
        public string? PostalCode { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("TimeZone")]
        public string? TimeZone { get; set; }

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
    }
}
