namespace TraitorGator.Shared.Dtos
{
    public class AnswerResultDto
    {
        public Guid PlayerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
    }
}
