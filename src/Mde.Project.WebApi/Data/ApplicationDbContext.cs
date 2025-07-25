using Mde.Project.WebApi.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Mde.Project.WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Judoka> Judokas { get; set; }
        public DbSet<TrainingEntry> TrainingEntries { get; set; }
        public DbSet<TechniqueScore> TechniqueScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TechniqueScore>()
                .HasOne(t => t.TrainingEntry)
                .WithMany(e => e.TechniqueScores)
                .HasForeignKey(t => t.TrainingEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

