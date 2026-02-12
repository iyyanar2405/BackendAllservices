using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Sites")]
    public class Sites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("SiteId")]
        public int SiteId { get; set; }

        [Column("SiteName")]
        public required string SiteName { get; set; }

        [Column("SiteCode")]
        public required string SiteCode { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("CityId")]
        public int? CityId { get; set; }

        [Column("CountryId")]
        public int? CountryId { get; set; }

        [Column("PostalCode")]
        public string? PostalCode { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("Phone")]
        public string? Phone { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("ContactPerson")]
        public string? ContactPerson { get; set; }

        [Column("ContactEmail")]
        public string? ContactEmail { get; set; }

        [Column("ContactPhone")]
        public string? ContactPhone { get; set; }

        [Column("SiteType")]
        public string? SiteType { get; set; }

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

        [Column("EmployeeCount")]
        public int? EmployeeCount { get; set; }

        [Column("Area")]
        public decimal? Area { get; set; }

        [Column("TimeZone")]
        public string? TimeZone { get; set; }
    }
}
