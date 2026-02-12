using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("ContractServices")]
    public class ContractServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ContractServiceId")]
        public int ContractServiceId { get; set; }

        [Column("ContractId")]
        public int ContractId { get; set; }

        [Column("ServiceId")]
        public int ServiceId { get; set; }

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("UnitPrice")]
        public decimal? UnitPrice { get; set; }

        [Column("TotalPrice")]
        public decimal? TotalPrice { get; set; }

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

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Status")]
        public string? Status { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
