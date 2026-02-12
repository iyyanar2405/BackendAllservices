using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Financials")]
    public class Financials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FinancialId")]
        public int FinancialId { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("Quarter")]
        public int? Quarter { get; set; }

        [Column("Month")]
        public int? Month { get; set; }

        [Column("Revenue")]
        public decimal? Revenue { get; set; }

        [Column("Expenses")]
        public decimal? Expenses { get; set; }

        [Column("Profit")]
        public decimal? Profit { get; set; }

        [Column("OutstandingAmount")]
        public decimal? OutstandingAmount { get; set; }

        [Column("PaidAmount")]
        public decimal? PaidAmount { get; set; }

        [Column("OverdueAmount")]
        public decimal? OverdueAmount { get; set; }

        [Column("Currency")]
        public required string Currency { get; set; }

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

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("DataSource")]
        public string? DataSource { get; set; }
    }
}
