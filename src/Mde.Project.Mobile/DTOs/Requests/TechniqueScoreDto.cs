using System.Text.Json.Serialization;

namespace Mde.Project.WebApi.DTOs.Requests
{
    public class TechniqueScoreDto
    {
        [JsonPropertyName("technique")] public string Technique { get; set; } = "";
        [JsonPropertyName("count")] public int Count { get; set; }
    }
}
