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
    private readonly ApplicationDbContext _db;

    public TrainingEntriesController(ApplicationDbContext db) => _db = db;

    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // GET: /api/trainingentries/by-user
    [HttpGet("by-user")]
    public async Task<ActionResult<IEnumerable<TrainingEntryResponse>>> GetForLoggedInUser()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized("Gebruiker niet herkend.");

        var entries = await _db.TrainingEntries
            .Include(e => e.TechniqueScores)
            .Where(e => e.UserId == CurrentUserId)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return Ok(entries.Select(Map));
    }

    // GET: /api/trainingentries/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrainingEntryResponse>> GetById(int id)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        var e = await _db.TrainingEntries
            .Include(x => x.TechniqueScores)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);

        if (e is null) return NotFound();

        return Ok(Map(e));
    }

    // POST: /api/trainingentries
    [HttpPost]
    public async Task<ActionResult<TrainingEntryResponse>> CreateTrainingEntry([FromBody] CreateTrainingEntryRequest request)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        // enkel scores bewaren wanneer type "randori" is
        var techniqueScores = string.Equals(request.Type, "randori", StringComparison.OrdinalIgnoreCase)
            ? request.TechniqueScores.Select(s => new TechniqueScore
            {
                Technique = s.Technique,
                ScoreCount = s.ScoreCount
            }).ToList()
            : new List<TechniqueScore>();

        var entry = new TrainingEntry
        {
            UserId = CurrentUserId,
            Date = request.Date,
            Type = request.Type,
            Comment = request.Comment ?? string.Empty,
            TechniqueScores = techniqueScores
        };

        _db.TrainingEntries.Add(entry);
        await _db.SaveChangesAsync();

        var response = Map(entry);
        // Link naar GET /api/trainingentries/{id}
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, response);
    }

    // PUT: /api/trainingentries/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTrainingEntry(int id, [FromBody] CreateTrainingEntryRequest request)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        var entry = await _db.TrainingEntries
            .Include(e => e.TechniqueScores)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);

        if (entry is null) return NotFound();

        entry.Date = request.Date;
        entry.Type = request.Type;
        entry.Comment = request.Comment ?? string.Empty;

        // techniek-scores vervangen indien type randori, anders leegmaken
        if (string.Equals(request.Type, "randori", StringComparison.OrdinalIgnoreCase))
        {
            entry.TechniqueScores.Clear();
            entry.TechniqueScores = request.TechniqueScores.Select(s => new TechniqueScore
            {
                Technique = s.Technique,
                ScoreCount = s.ScoreCount
            }).ToList();
        }
        else
        {
            entry.TechniqueScores.Clear();
        }

        await _db.SaveChangesAsync();
        return Ok(Map(entry));
    }

    // DELETE: /api/trainingentries/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTrainingEntry(int id)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        var entry = await _db.TrainingEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);

        if (entry is null) return NotFound();

        _db.TrainingEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return NoContent(); // 204
    }

    private static TrainingEntryResponse Map(TrainingEntry e) => new()
    {
        Id = e.Id,
        Date = e.Date,
        Type = e.Type,
        Comment = e.Comment,
        TechniqueScores = e.TechniqueScores.Select(ts => new TechniqueScoreResponse
        {
            Id = ts.Id,
            Technique = ts.Technique,
            ScoreCount = ts.ScoreCount
        }).ToList()
    };
}
