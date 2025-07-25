namespace Mde.Project.WebApi.Entities
{
    public class TrainingEntry
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty; 
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty; 

       // public ICollection<TechniqueScore> TechniqueScores { get; set; } = new List<TechniqueScore>();
    }
}
