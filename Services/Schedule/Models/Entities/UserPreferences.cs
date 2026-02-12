using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("UserPreferences")]
    public class UserPreferences
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserPreferenceId")]
        public int UserPreferenceId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("PreferenceKey")]
        public required string PreferenceKey { get; set; }

        [Column("PreferenceValue")]
        public string? PreferenceValue { get; set; }

        [Column("PreferenceType")]
        public required string PreferenceType { get; set; }

        [Column("Category")]
        public string? Category { get; set; }

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
    }
}
