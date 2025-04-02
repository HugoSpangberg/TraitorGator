using Microsoft.AspNetCore.Mvc;
using TraitorGator.Models.Dto;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController : Controller
{
    private readonly IQuestionService _questionService;

    public AnswersController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        try
        {
            var answer = await _questionService.SubmitAnswerAsync(request);
            return Ok(answer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
