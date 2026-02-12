using System.Text.Json.Serialization;

namespace SettingsService.Models
{
    public class UserPreferencesPayload
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("preferences")]
        public UserPreferenceDetails? Preferences { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
    }

    public class UserPreferencesUpdateRequest
    {
        [JsonPropertyName("userId")]
        public int? UserId { get; set; }

        [JsonPropertyName("preferences")]
        public UserPreferenceDetails? Preferences { get; set; }

        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; set; }
    }

    public class UserPreferencesUpdateResponse
    {
        public UpdateResult Result { get; set; } = new();
    }

    public class UserPreferenceDetails
    {
        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }

        [JsonPropertyName("dateFormat")]
        public string? DateFormat { get; set; }

        [JsonPropertyName("timeFormat")]
        public string? TimeFormat { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("notifications")]
        public UserNotificationPreferences? Notifications { get; set; }

        [JsonPropertyName("dashboard")]
        public UserDashboardPreferences? Dashboard { get; set; }

        [JsonPropertyName("display")]
        public UserDisplayPreferences? Display { get; set; }
    }

    public class UserNotificationPreferences
    {
        [JsonPropertyName("email")]
        public bool? Email { get; set; }

        [JsonPropertyName("browser")]
        public bool? Browser { get; set; }

        [JsonPropertyName("mobile")]
        public bool? Mobile { get; set; }

        [JsonPropertyName("auditReminders")]
        public bool? AuditReminders { get; set; }

        [JsonPropertyName("certificateExpiry")]
        public bool? CertificateExpiry { get; set; }

        [JsonPropertyName("findingUpdates")]
        public bool? FindingUpdates { get; set; }

        [JsonPropertyName("invoiceAlerts")]
        public bool? InvoiceAlerts { get; set; }
    }

    public class UserDashboardPreferences
    {
        [JsonPropertyName("defaultView")]
        public string? DefaultView { get; set; }

        [JsonPropertyName("widgets")]
        public List<string> Widgets { get; set; } = new();

        [JsonPropertyName("refreshInterval")]
        public int? RefreshInterval { get; set; }
    }

    public class UserDisplayPreferences
    {
        [JsonPropertyName("theme")]
        public string? Theme { get; set; }

        [JsonPropertyName("compactMode")]
        public bool? CompactMode { get; set; }

        [JsonPropertyName("showHelpTips")]
        public bool? ShowHelpTips { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int? ItemsPerPage { get; set; }
    }
}
