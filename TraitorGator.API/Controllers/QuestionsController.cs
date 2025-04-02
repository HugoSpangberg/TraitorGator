using Microsoft.AspNetCore.Mvc;
using TraitorGator.Models.Dto;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : Controller
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        try
        {
            var question = await _questionService.CreateQuestionAsync(request);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestion(Guid id)
    {
        // Du kan implementera en metod för att hämta en fråga om du vill
        return Ok("Endpoint för att hämta en fråga ej implementerad än.");
    }
}
