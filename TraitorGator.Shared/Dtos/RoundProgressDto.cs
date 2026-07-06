namespace TraitorGator.Shared.Dtos;

public class RoundProgressDto
{
    public string GameCode { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public int PlayerCount { get; set; }
    public int AnswersSubmitted { get; set; }
    public bool AllAnswered { get; set; }
}
