
using TraitorGator.Models.Enums;

namespace TraitorGator.Models.Dto
{
    public class CreateQuestionRequest
    {
        public string Text { get; set; } = string.Empty;
        public string AlteredText { get; set; } = string.Empty;
        public int? RoundNumber { get; set; }
        public QuestionType QuestionType { get; set; }
    }
}
