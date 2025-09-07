namespace Mde.Project.WebApi.DTOs.Responses
{
    public class TrainingEntryResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public List<TechniqueScoreResponse> TechniqueScores { get; set; } = new();
    }
}
