using Microsoft.AspNetCore.Mvc;
using Mde.Project.WebApi.Data;
using Mde.Project.WebApi.Entities;

namespace Mde.Project.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JudokasController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public JudokasController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: /api/judokas/by-category/-60
    [HttpGet("by-category/{category}")]
    public ActionResult<IEnumerable<Judoka>> GetByCategory(string category)
    {
        var judokas = _dbContext.Judokas
            .Where(j => j.Category == category)
            .ToList();

        if (judokas.Count == 0)
        {
            return NotFound($"Geen judokas gevonden voor categorie: {category}");
        }

        return Ok(judokas);
    }
}
