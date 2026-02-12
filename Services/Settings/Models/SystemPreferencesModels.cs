using System.Text.Json.Serialization;

namespace SettingsService.Models
{
    public class SystemPreferencesPayload
    {
        [JsonPropertyName("generalSettings")]
        public GeneralSettings? GeneralSettings { get; set; }

        [JsonPropertyName("auditSettings")]
        public AuditSettings? AuditSettings { get; set; }

        [JsonPropertyName("certificateSettings")]
        public CertificateSettings? CertificateSettings { get; set; }

        [JsonPropertyName("financialSettings")]
        public FinancialSettings? FinancialSettings { get; set; }

        [JsonPropertyName("communicationSettings")]
        public CommunicationSettings? CommunicationSettings { get; set; }
    }

    public class SystemPreferencesUpdateRequest : SystemPreferencesPayload
    {
        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; set; }
    }

    public class SystemPreferencesUpdateResponse
    {
        public UpdateResult Result { get; set; } = new();
    }

    public class GeneralSettings
    {
        [JsonPropertyName("systemName")]
        public string? SystemName { get; set; }

        [JsonPropertyName("systemVersion")]
        public string? SystemVersion { get; set; }

        [JsonPropertyName("maintenanceMode")]
        public bool? MaintenanceMode { get; set; }

        [JsonPropertyName("maxFileUploadSize")]
        public int? MaxFileUploadSize { get; set; }

        [JsonPropertyName("sessionTimeout")]
        public int? SessionTimeout { get; set; }

        [JsonPropertyName("passwordPolicy")]
        public PasswordPolicy? PasswordPolicy { get; set; }
    }

    public class PasswordPolicy
    {
        [JsonPropertyName("minLength")]
        public int? MinLength { get; set; }

        [JsonPropertyName("requireUppercase")]
        public bool? RequireUppercase { get; set; }

        [JsonPropertyName("requireLowercase")]
        public bool? RequireLowercase { get; set; }

        [JsonPropertyName("requireNumbers")]
        public bool? RequireNumbers { get; set; }

        [JsonPropertyName("requireSpecialChars")]
        public bool? RequireSpecialChars { get; set; }

        [JsonPropertyName("expirationDays")]
        public int? ExpirationDays { get; set; }
    }

    public class AuditSettings
    {
        [JsonPropertyName("auditReminders")]
        public bool? AuditReminders { get; set; }

        [JsonPropertyName("reminderDaysBefore")]
        public int? ReminderDaysBefore { get; set; }

        [JsonPropertyName("autoScheduling")]
        public bool? AutoScheduling { get; set; }

        [JsonPropertyName("defaultAuditDuration")]
        public int? DefaultAuditDuration { get; set; }
    }

    public class CertificateSettings
    {
        [JsonPropertyName("expiryNotificationDays")]
        public int? ExpiryNotificationDays { get; set; }

        [JsonPropertyName("autoRenewalReminders")]
        public bool? AutoRenewalReminders { get; set; }

        [JsonPropertyName("digitalSignature")]
        public bool? DigitalSignature { get; set; }

        [JsonPropertyName("qrCodeGeneration")]
        public bool? QrCodeGeneration { get; set; }
    }

    public class FinancialSettings
    {
        [JsonPropertyName("defaultCurrency")]
        public string? DefaultCurrency { get; set; }

        [JsonPropertyName("taxCalculation")]
        public string? TaxCalculation { get; set; }

        [JsonPropertyName("invoiceTemplateId")]
        public int? InvoiceTemplateId { get; set; }

        [JsonPropertyName("paymentReminderDays")]
        public int? PaymentReminderDays { get; set; }
    }

    public class CommunicationSettings
    {
        [JsonPropertyName("emailServerConfig")]
        public EmailServerConfig? EmailServerConfig { get; set; }

        [JsonPropertyName("notificationTemplates")]
        public List<NotificationTemplateSummary> NotificationTemplates { get; set; } = new();
    }

    public class EmailServerConfig
    {
        [JsonPropertyName("smtpServer")]
        public string? SmtpServer { get; set; }

        [JsonPropertyName("smtpPort")]
        public int? SmtpPort { get; set; }

        [JsonPropertyName("useSSL")]
        public bool? UseSSL { get; set; }
    }

    public class NotificationTemplateSummary
    {
        [JsonPropertyName("templateId")]
        public int? TemplateId { get; set; }

        [JsonPropertyName("templateName")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("templateType")]
        public string? TemplateType { get; set; }
    }
}
