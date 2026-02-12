using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("AuditTeamMembers")]
    public class AuditTeamMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditTeamMemberId")]
        public int AuditTeamMemberId { get; set; }

        [Column("AuditId")]
        public int AuditId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Role")]
        public required string Role { get; set; }

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

        [Column("AssignedDate")]
        public DateTime? AssignedDate { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Specialization")]
        public string? Specialization { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
