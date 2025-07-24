using Microsoft.AspNetCore.Mvc;
using Mde.Project.WebApi.Data;
using Mde.Project.WebApi.DTOs;

namespace Mde.Project.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /api/events/upcoming
    [HttpGet("upcoming")]
    public ActionResult<IEnumerable<EventDto>> GetUpcomingEvents()
    {
        var today = DateTime.Today;

        var events = _context.Events
            .Where(e => e.Date >= today)
            .OrderBy(e => e.Date)
            .Take(3)
            .Select(e => new EventDto
            {
                Title = e.Title,
                Location = e.Location,
                Date = e.Date
            })
            .ToList();

        return Ok(events);
    }
}