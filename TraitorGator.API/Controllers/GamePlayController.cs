using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Services.Services;
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

        // Hämtar frågedetaljer: /api/gameplay/questionDetail?round=1&gameCode=XYZ
        [HttpGet("questionDetail")]
        public async Task<ActionResult<QuestionDetailDto>> QuestionDetail(
            [FromQuery] string gameCode,
            [FromQuery] int round)
        {
            var dto = await _qs.GetQuestionDetailAsync(gameCode, round);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // Hämtar svaren för rundan: /api/gameplay/results?gameCode=XYZ&round=1
        [HttpGet("results")]
        public async Task<ActionResult<IEnumerable<AnswerResultDto>>> Results(
            [FromQuery] string gameCode,
            [FromQuery] int round)
        {
            var list = await _qs.GetAnswersForGameAsync(gameCode, round);
            return Ok(list);
        }
    }

}
