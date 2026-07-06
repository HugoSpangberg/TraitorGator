using CreateQuestionRequest = TraitorGator.Models.Dto.CreateQuestionRequest;
using TraitorGator.Models.Models;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Interfaces;

public interface IQuestionService
{
    Task<Question> CreateQuestionAsync(CreateQuestionRequest request);
    Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerRequest request);
    Task<QuestionForPlayerDto> GetQuestionForPlayerAsync(string gameCode, Guid playerId, string playerSecret);
    Task<RoundProgressDto> GetRoundProgressAsync(string gameCode, int? roundNumber = null);
    Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode);
    Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode, int round);
    Task<QuestionDetailDto?> GetQuestionDetailAsync(string gameCode, int round);
}
