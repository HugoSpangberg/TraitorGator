using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public AnswersController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<AnswerResultDto>> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        try
        {
            return Ok(await _questionService.SubmitAnswerAsync(request));
        }
        catch (Exception ex)
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
}
