using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("UserRoles")]
    public class UserRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserRoleId")]
        public int UserRoleId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("RoleId")]
        public int RoleId { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("AssignedDate")]
        public DateTime AssignedDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("AssignedBy")]
        public int? AssignedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }
}
