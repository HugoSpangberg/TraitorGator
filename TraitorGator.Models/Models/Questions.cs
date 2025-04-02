using TraitorGator.Models.Enums;

namespace TraitorGator.Models.Models;

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public string AlteredText { get; set; }
    public int? RoundNumber { get; set; }
    public QuestionType QuestionType { get; set; }
}
