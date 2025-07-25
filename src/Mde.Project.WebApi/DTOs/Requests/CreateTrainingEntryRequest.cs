namespace Mde.Project.WebApi.DTOs.Requests
{
    public class CreateTrainingEntryRequest
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<TechniqueScoreRequest> TechniqueScores { get; set; } = new();
    }
}
