using Microsoft.AspNetCore.Mvc;
using TraitorGator.API.Models;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(IGameService gameService, IPlayerService playerService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IPlayerService _playerService = playerService;

    [HttpPost("create")]
    public async Task<ActionResult<GameRound>> CreateGameRound()
    {
        var round = await _gameService.CreateGameRoundAsync();
        return Ok(round);
    }

    [HttpPost("join")]
    public async Task<ActionResult<Player>> JoinGame([FromBody] JoinGameRequest request)
    {
        var player = await _playerService.AddPlayerToGameAsync(request.GameCode, request.Username);
        return player != null ? Ok(player) : NotFound("Game not found");
    }

    [HttpGet("{gameCode}")]
    public async Task<ActionResult<GameRound>> GetGameByCode(string gameCode)
    {
        var round = await _gameService.GetGameRoundByCodeAsync(gameCode);
        return round != null? Ok(round) : NotFound("Game not found");
    }

    [HttpPost("{gameCode}/start")]
    public async Task<IActionResult> StartGame(string gameCode)
    {
        var ok = await _gameService.StartGameAsync(gameCode);
        return ok ? Ok() : BadRequest("För få spelare för att starta");
    }
}


