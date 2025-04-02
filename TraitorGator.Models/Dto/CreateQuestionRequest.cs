
using TraitorGator.Models.Enums;

namespace TraitorGator.Models.Dto
{
    public class CreateQuestionRequest
    {
        public string Text { get; set; }
        public string AlteredText { get; set; }
        public int? RoundNumber { get; set; }
        public QuestionType QuestionType { get; set; }
    }
}
