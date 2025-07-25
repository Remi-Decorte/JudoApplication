using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mde.Project.WebApi.Data;
using Mde.Project.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

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
}

