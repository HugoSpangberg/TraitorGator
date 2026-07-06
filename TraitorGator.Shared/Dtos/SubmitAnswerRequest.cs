namespace TraitorGator.Shared.Dtos;

public class SubmitAnswerRequest
{
    public Guid PlayerId { get; set; }
    public string PlayerSecret { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
}
