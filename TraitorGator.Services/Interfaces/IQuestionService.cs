using TraitorGator.Models.Dto;
using TraitorGator.Models.Models;

namespace TraitorGator.Services.Interfaces;

public interface IQuestionService
{
    Task<Question> CreateQuestionAsync(CreateQuestionRequest request);
    Task<Answer> SubmitAnswerAsync(SubmitAnswerRequest request);
}
