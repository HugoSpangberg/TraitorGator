using TraitorGator.API.Models;

namespace TraitorGator.Models.Models;

public class Answer
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Player Player { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }

    public string AnswerText { get; set; }

    // (Eventuellt) En flagga för om svaret är korrekt – kan sättas senare
    public bool IsCorrect { get; set; }
}
