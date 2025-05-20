using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers
{
    [ApiController]
    [Route("api/gameplay")]
    public class GamePlayController : ControllerBase
    {
        private readonly IQuestionService _qs;
        public GamePlayController(IQuestionService qs) => _qs = qs;

        [HttpGet("{gameCode}/question")]
        public async Task<ActionResult<QuestionForPlayerDto>> GetQuestion(
            string gameCode, [FromQuery] Guid playerId)
        {
            var dto = await _qs.GetQuestionForPlayerAsync(gameCode, playerId);
            return Ok(dto);
        }

        [HttpGet("{gameCode}/answers")]
        public async Task<ActionResult<IEnumerable<AnswerResultDto>>> GetAnswers(string gameCode)
        {
            var list = await _qs.GetAnswersForGameAsync(gameCode);
            return Ok(list);
        }
    }

}
