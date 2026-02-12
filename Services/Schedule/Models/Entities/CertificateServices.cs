using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("CertificateServices")]
    public class CertificateServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CertificateServiceId")]
        public int CertificateServiceId { get; set; }

        [Column("CertificateId")]
        public int CertificateId { get; set; }

        [Column("ServiceId")]
        public int ServiceId { get; set; }

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

        [Column("Scope")]
        public string? Scope { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
