namespace Mde.Project.WebApi.Entities
{
    public class TechniqueScore
    {
        public int Id { get; set; }

        public int TrainingEntryId { get; set; }
        public string Technique { get; set; } = string.Empty; 
        public int ScoreCount { get; set; } = 0;

        public TrainingEntry TrainingEntry { get; set; } = null!;
    }
}
