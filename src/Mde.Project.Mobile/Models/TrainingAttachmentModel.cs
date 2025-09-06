using System.Text.Json.Serialization;

namespace Mde.Project.Mobile.Models
{
    public class TrainingAttachmentModel
    {
        // "photo" (later kan bv. "audio" ook)
        [JsonPropertyName("type")]
        public string Type { get; set; } = "photo";

        // Lokaal bestandspad of (later) een URL
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }
    }
}
