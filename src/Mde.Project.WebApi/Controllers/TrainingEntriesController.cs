using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mde.Project.WebApi.Data;
using Mde.Project.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Mde.Project.WebApi.DTOs.Requests;

namespace Mde.Project.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // enkel ingelogde gebruikers
public class TrainingEntriesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public TrainingEntriesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: /api/trainingentries/by-user
    [HttpGet("by-user")]
    public async Task<ActionResult<IEnumerable<TrainingEntry>>> GetForLoggedInUser()
    {
        string? userId = User?.Identity?.Name;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Gebruiker niet herkend.");

        var entries = await _dbContext.TrainingEntries
            .Include(e => e.TechniqueScores)
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return Ok(entries);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrainingEntry([FromBody] CreateTrainingEntryRequest request)
    {
        string? userId = User?.Identity?.Name;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var entry = new TrainingEntry
        {
            UserId = userId,
            Date = request.Date,
            Type = request.Type,
            TechniqueScores = request.Type.ToLower() == "randori"
                ? request.TechniqueScores.Select(s => new TechniqueScore
                {
                    Technique = s.Technique,
                    ScoreCount = s.ScoreCount
                }).ToList()
                : new List<TechniqueScore>()
        };

        _dbContext.TrainingEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetForLoggedInUser), new { id = entry.Id }, entry);
    }
}

