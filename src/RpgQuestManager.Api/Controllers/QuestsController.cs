using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IQuestService _quests;
    public QuestsController(ApplicationDbContext db, IQuestService quests) { _db = db; _quests = quests; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Quests.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        var q = await _quests.StartAsync(id);
        return Ok(q);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var q = await _quests.CompleteAsync(id);
        return Ok(q);
    }
}


