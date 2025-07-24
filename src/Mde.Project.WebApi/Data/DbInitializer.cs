using Mde.Project.WebApi.Entities;

namespace Mde.Project.WebApi.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext db)
        {
            // Voeg testdata toe als er nog geen events zijn
            if (!db.Events.Any())
            {
                db.Events.AddRange(new[]
                {
                new Event { Title = "Grand Slam Tokyo", Location = "Tokyo, Japan", Date = DateTime.Today.AddDays(10) },
                new Event { Title = "World Masters", Location = "Budapest, Hungary", Date = DateTime.Today.AddDays(20) },
                new Event { Title = "Paris Grand Slam", Location = "Paris, France", Date = DateTime.Today.AddDays(30) }
            });

                db.SaveChanges();
            }
        }
    }
}