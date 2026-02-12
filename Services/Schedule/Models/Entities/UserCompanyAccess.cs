using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("UserCompanyAccess")]
    public class UserCompanyAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserCompanyAccessId")]
        public int UserCompanyAccessId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("AccessLevel")]
        public required string AccessLevel { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("GrantedDate")]
        public DateTime GrantedDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("GrantedBy")]
        public int? GrantedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }
}
