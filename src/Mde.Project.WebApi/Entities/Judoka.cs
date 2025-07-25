namespace Mde.Project.WebApi.Entities
{
    public class Judoka
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; 
    }
}
