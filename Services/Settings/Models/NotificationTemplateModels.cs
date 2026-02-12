using System.Text.Json.Serialization;

namespace SettingsService.Models
{
    public class NotificationTemplateResponse
    {
        [JsonPropertyName("templateId")]
        public int TemplateId { get; set; }

        [JsonPropertyName("templateName")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("templateType")]
        public string? TemplateType { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("bodyHtml")]
        public string? BodyHtml { get; set; }

        [JsonPropertyName("bodyText")]
        public string? BodyText { get; set; }

        [JsonPropertyName("variables")]
        public List<string> Variables { get; set; } = new();

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }
    }

    public class NotificationTemplatesResponse
    {
        [JsonPropertyName("templates")]
        public List<NotificationTemplateResponse> Templates { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }

    public class NotificationTemplateUpdateRequest
    {
        [JsonPropertyName("templateId")]
        public int TemplateId { get; set; }

        [JsonPropertyName("templateName")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("templateType")]
        public string? TemplateType { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("bodyHtml")]
        public string? BodyHtml { get; set; }

        [JsonPropertyName("bodyText")]
        public string? BodyText { get; set; }

        [JsonPropertyName("variables")]
        public List<string>? Variables { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; set; }
    }

    public class NotificationTemplateUpdateResponse
    {
        [JsonPropertyName("templateId")]
        public int TemplateId { get; set; }

        [JsonPropertyName("result")]
        public UpdateResult Result { get; set; } = new();
    }
}
