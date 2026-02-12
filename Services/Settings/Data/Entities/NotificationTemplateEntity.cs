using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Data.Entities
{
    [Table("NotificationTemplates")]
    public class NotificationTemplateEntity
    {
        [Key]
        public int NotificationTemplateId { get; set; }

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        public string TemplateType { get; set; } = string.Empty;

        public string? Category { get; set; }
        public string? Subject { get; set; }
        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public string? VariablesJson { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
