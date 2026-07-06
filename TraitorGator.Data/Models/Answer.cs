using TraitorGator.API.Models;

namespace TraitorGator.Models.Models;

public class Answer
{
    public Guid Id { get; set; }
    public Guid GameRoundId { get; set; }
    public GameRound GameRound { get; set; } = null!;
    public int RoundNumber { get; set; }
    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
