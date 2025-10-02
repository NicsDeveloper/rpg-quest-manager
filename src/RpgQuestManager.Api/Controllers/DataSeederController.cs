using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DataSeederController : ControllerBase
{
    private readonly QuestDataSeeder _dataSeeder;

    public DataSeederController(QuestDataSeeder dataSeeder)
    {
        _dataSeeder = dataSeeder;
    }

    [HttpPost("seed-quests")]
    public async Task<IActionResult> SeedQuests()
    {
        try
        {
            await _dataSeeder.SeedAsync();
            return Ok(new { message = "Dados de missões e monstros populados com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Erro ao popular dados: {ex.Message}" });
        }
    }
}
