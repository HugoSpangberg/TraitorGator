namespace TraitorGator.Shared.Dtos;

public class QuestionForPlayerDto
{
    public Guid QuestionId { get; set; }
    public int RoundNumber { get; set; }
    public int DurationSeconds { get; set; }
    public string Text { get; set; } = string.Empty;
}
