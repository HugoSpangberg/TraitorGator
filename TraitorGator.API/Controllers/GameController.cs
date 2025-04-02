using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.API.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(IGameService gameService, IPlayerService playerService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IPlayerService _playerService = playerService;

    [HttpPost("create")]
    public async Task<IActionResult> CreateGame()
    {
        var gameRound = await _gameService.CreateGameRoundAsync();
        return Ok(gameRound);
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinGame([FromBody] JoinGameRequest request)
    {
        var player = await _playerService.AddPlayerToGameAsync(request.GameCode, request.Username);
        return player is not null ? Ok(player) : NotFound("Game not found");
    }

}

public class JoinGameRequest
{
    public string GameCode { get; set; }
    public string Username { get; set; }
}
