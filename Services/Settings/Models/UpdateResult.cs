using System.Text.Json.Serialization;

namespace SettingsService.Models
{
    public class UpdateResult
    {
        [JsonPropertyName("updatedFields")]
        public List<string> UpdatedFields { get; set; } = new();

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonPropertyName("updatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
