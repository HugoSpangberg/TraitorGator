using TraitorGator.API.Data;
using TraitorGator.Models.Dto;
using TraitorGator.Models.Models;

namespace TraitorGator.Services.Services;

public class QuestionService
{
    private readonly GameDbContext _context;

    public QuestionService(GameDbContext context)
    {
        _context = context;
    }
    public async Task<Question> CreateQuestionAsync(CreateQuestionRequest request)
    {
        var question = new Question
        {
            Text = request.Text,
            AlteredText = request.AlteredText,
            RoundNumber = request.RoundNumber,
            QuestionType = request.QuestionType
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<Answer> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        // Validera att spelare och fråga finns
        var player = await _context.Players.FindAsync(request.PlayerId);
        if (player == null)
            throw new Exception("Player not found.");

        var question = await _context.Questions.FindAsync(request.QuestionId);
        if (question == null)
            throw new Exception("Question not found.");

        var answer = new Answer
        {
            PlayerId = request.PlayerId,
            QuestionId = request.QuestionId,
            AnswerText = request.AnswerText,
            IsCorrect = false // Här kan du lägga in logik för att avgöra om svaret är korrekt
        };

        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();
        return answer;
    }
}
