using Microsoft.AspNetCore.Mvc;
using TraitorGator.API.Models;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameRoundsController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameRoundsController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // Skapa en ny spelrunda
    [HttpPost]
    public async Task<ActionResult<GameRound>> CreateGameRound()
    {
        var gameRound = await _gameService.CreateGameRoundAsync();
        return CreatedAtAction(nameof(GetGameRound), new { id = gameRound.Id }, gameRound);
    }

    // Hämta en spelrunda med id
    [HttpGet("{id}")]
    public async Task<ActionResult<GameRound>> GetGameRound(Guid id)
    {
        var gameRound = await _gameService.GetGameRoundByIdAsync(id);
        if (gameRound == null)
        {
            return NotFound();
        }
        return gameRound;
    }

}
