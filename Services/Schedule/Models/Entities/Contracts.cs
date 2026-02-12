using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Contracts")]
    public class Contracts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ContractId")]
        public int ContractId { get; set; }

        [Column("ContractNumber")]
        public required string ContractNumber { get; set; }

        [Column("ContractName")]
        public required string ContractName { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("ContractType")]
        public string? ContractType { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("TotalValue")]
        public decimal? TotalValue { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; }

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

        [Column("SignedDate")]
        public DateTime? SignedDate { get; set; }

        [Column("SignedByClient")]
        public string? SignedByClient { get; set; }

        [Column("SignedByDNV")]
        public string? SignedByDNV { get; set; }

        [Column("ContractPath")]
        public string? ContractPath { get; set; }

        [Column("Terms")]
        public string? Terms { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("RenewalDate")]
        public DateTime? RenewalDate { get; set; }

        [Column("AutoRenewal")]
        public bool AutoRenewal { get; set; }
    }
}
