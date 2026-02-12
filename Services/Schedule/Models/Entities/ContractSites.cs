using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("ContractSites")]
    public class ContractSites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ContractSiteId")]
        public int ContractSiteId { get; set; }

        [Column("ContractId")]
        public int ContractId { get; set; }

        [Column("SiteId")]
        public int SiteId { get; set; }

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
