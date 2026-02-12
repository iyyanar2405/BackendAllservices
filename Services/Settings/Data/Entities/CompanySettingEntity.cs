using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Data.Entities
{
    [Table("CompanySettings")]
    public class CompanySettingEntity
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string SettingsJson { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
