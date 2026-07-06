using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers;

[ApiController]
[Route("api/gameplay")]
public class GamePlayController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IQuestionService _questionService;

    public GamePlayController(IGameService gameService, IQuestionService questionService)
    {
        _gameService = gameService;
        _questionService = questionService;
    }

    [HttpGet("{gameCode}/question")]
    public async Task<ActionResult<QuestionForPlayerDto>> GetQuestion(string gameCode, [FromQuery] Guid playerId)
    {
        try
        {
            var secret = Request.Headers["X-Player-Secret"].FirstOrDefault() ?? string.Empty;
            return Ok(await _questionService.GetQuestionForPlayerAsync(gameCode, playerId, secret));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpGet("{gameCode}/progress")]
    public async Task<ActionResult<RoundProgressDto>> GetProgress(string gameCode, [FromQuery] int? round)
    {
        try
        {
            return Ok(await _questionService.GetRoundProgressAsync(gameCode, round));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpGet("{gameCode}/answers")]
    public async Task<ActionResult<IEnumerable<AnswerResultDto>>> GetAnswers(string gameCode)
    {
        try
        {
            return Ok(await _questionService.GetAnswersForGameAsync(gameCode));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpGet("questionDetail")]
    public async Task<ActionResult<QuestionDetailDto>> QuestionDetail([FromQuery] string gameCode, [FromQuery] int round)
    {
        var dto = await _questionService.GetQuestionDetailAsync(gameCode, round);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("results")]
    public async Task<ActionResult<GameRoundResultDto>> Results([FromQuery] string gameCode, [FromQuery] int round)
    {
        var result = await _gameService.GetRoundResultAsync(gameCode, round);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("{gameCode}/vote")]
    public async Task<ActionResult<GameRoundResultDto>> SubmitVote(string gameCode, [FromBody] SubmitVoteRequest request)
    {
        try
        {
            return Ok(await _gameService.SubmitVoteAsync(gameCode, request));
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
