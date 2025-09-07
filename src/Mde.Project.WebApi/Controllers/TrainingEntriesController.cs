using Mde.Project.WebApi.Data;
using Mde.Project.WebApi.DTOs.Requests;
using Mde.Project.WebApi.DTOs.Responses;
using Mde.Project.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Gebruiker niet herkend.");

        var entries = await _dbContext.TrainingEntries
            .Include(e => e.TechniqueScores)
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Date)
            .ToListAsync();

        var response = entries.Select(entry => new TrainingEntryResponse
        {
            Id = entry.Id,
            Date = entry.Date,
            Type = entry.Type,
            Comment = entry.Comment,
            TechniqueScores = entry.TechniqueScores.Select(ts => new TechniqueScoreResponse
            {
                Id = ts.Id,
                Technique = ts.Technique,
                ScoreCount = ts.ScoreCount
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrainingEntry([FromBody] CreateTrainingEntryRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var techniqueScores = request.Type.ToLower() == "randori"
                ? request.TechniqueScores.Select(s => new TechniqueScore
                {
                    Technique = s.Technique,
                    ScoreCount = s.ScoreCount
                }).ToList()
                : new List<TechniqueScore>();

        var entry = new TrainingEntry
        {
            UserId = userId,
            Date = request.Date,
            Type = request.Type,
            Comment = request.Comment ?? string.Empty,
            TechniqueScores = techniqueScores
        };

        _dbContext.TrainingEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        // Mapping naar response DTO
        var response = new TrainingEntryResponse
        {
            Id = entry.Id,
            Date = entry.Date,
            Type = entry.Type,
            Comment = entry.Comment,
            TechniqueScores = entry.TechniqueScores.Select(ts => new TechniqueScoreResponse
            {
                Id = ts.Id,
                Technique = ts.Technique,
                ScoreCount = ts.ScoreCount
            }).ToList()
        };

        return CreatedAtAction(nameof(GetForLoggedInUser), new { id = entry.Id }, response);
    }
}

