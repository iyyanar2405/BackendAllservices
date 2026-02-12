using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Data.Entities
{
    [Table("UserPreferenceProfiles")]
    public class UserPreferenceProfileEntity
    {
        [Key]
        public int UserPreferenceProfileId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string PreferencesJson { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
