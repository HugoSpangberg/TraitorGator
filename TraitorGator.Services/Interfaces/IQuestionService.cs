using TraitorGator.Models.Dto;
using TraitorGator.Models.Models;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Interfaces;

public interface IQuestionService
{
    Task<Question> CreateQuestionAsync(CreateQuestionRequest request);
    Task<Answer> SubmitAnswerAsync(SubmitAnswerRequest request);
    Task<QuestionForPlayerDto> GetQuestionForPlayerAsync(string gameCode, Guid playerId);
    Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode);
}
