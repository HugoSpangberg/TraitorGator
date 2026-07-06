using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<PlayerSessionDto>> CreateGame([FromBody] CreateGameRequest request)
    {
        try
        {
            return Ok(await _gameService.CreateGameAsync(request));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpPost("join")]
    public async Task<ActionResult<PlayerSessionDto>> JoinGame([FromBody] JoinGameRequest request)
    {
        try
        {
            return Ok(await _gameService.JoinGameAsync(request));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpGet("{gameCode}")]
    public async Task<ActionResult<GameStateDto>> GetGameByCode(string gameCode)
    {
        var state = await _gameService.GetGameStateAsync(gameCode);
        return state == null ? NotFound("Spelet hittades inte.") : Ok(state);
    }

    [HttpPost("{gameCode}/start")]
    public async Task<ActionResult<GameStateDto>> StartGame(string gameCode, [FromBody] PlayerActionRequest request)
    {
        try
        {
            return Ok(await _gameService.StartGameAsync(gameCode, request));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpPost("{gameCode}/next")]
    public async Task<ActionResult<GameStateDto>> AdvanceRound(string gameCode, [FromBody] PlayerActionRequest request)
    {
        try
        {
            return Ok(await _gameService.AdvanceRoundAsync(gameCode, request));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    private ActionResult ToErrorResult(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => NotFound(ex.Message),
            UnauthorizedAccessException => Unauthorized(ex.Message),
            ArgumentException or InvalidOperationException => BadRequest(ex.Message),
            _ => Problem(ex.Message)
        };
    }
}
