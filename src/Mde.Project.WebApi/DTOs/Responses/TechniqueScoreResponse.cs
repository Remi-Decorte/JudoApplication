namespace Mde.Project.WebApi.DTOs.Responses
{
    public class TechniqueScoreResponse
    {
        public int Id { get; set; }
        public string Technique { get; set; } = string.Empty;
        public int ScoreCount { get; set; }
    }
}
