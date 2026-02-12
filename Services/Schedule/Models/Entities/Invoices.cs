using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Invoices")]
    public class Invoices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("InvoiceId")]
        public int InvoiceId { get; set; }

        [Column("InvoiceNumber")]
        public required string InvoiceNumber { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("ContractId")]
        public int? ContractId { get; set; }

        [Column("InvoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [Column("DueDate")]
        public DateTime DueDate { get; set; }

        [Column("PlannedPaymentDate")]
        public DateTime? PlannedPaymentDate { get; set; }

        [Column("PaidDate")]
        public DateTime? PaidDate { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("TaxAmount")]
        public decimal? TaxAmount { get; set; }

        [Column("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [Column("Currency")]
        public required string Currency { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

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

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Terms")]
        public string? Terms { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("InvoicePath")]
        public string? InvoicePath { get; set; }

        [Column("PaymentMethod")]
        public string? PaymentMethod { get; set; }

        [Column("PaymentReference")]
        public string? PaymentReference { get; set; }

        [Column("DiscountAmount")]
        public decimal? DiscountAmount { get; set; }

        [Column("LateFee")]
        public decimal? LateFee { get; set; }
    }
}
