using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Companies")]
    public class Companies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("CompanyName")]
        public required string CompanyName { get; set; }

        [Column("CompanyCode")]
        public required string CompanyCode { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("CityId")]
        public int? CityId { get; set; }

        [Column("CountryId")]
        public int? CountryId { get; set; }

        [Column("PostalCode")]
        public string? PostalCode { get; set; }

        [Column("Phone")]
        public string? Phone { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("Website")]
        public string? Website { get; set; }

        [Column("ContactPerson")]
        public string? ContactPerson { get; set; }

        [Column("ContactEmail")]
        public string? ContactEmail { get; set; }

        [Column("ContactPhone")]
        public string? ContactPhone { get; set; }

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

        [Column("Industry")]
        public string? Industry { get; set; }

        [Column("EmployeeCount")]
        public int? EmployeeCount { get; set; }

        [Column("TaxId")]
        public string? TaxId { get; set; }

        [Column("RegistrationNumber")]
        public string? RegistrationNumber { get; set; }

        [Column("LogoUrl")]
        public string? LogoUrl { get; set; }
    }
}
