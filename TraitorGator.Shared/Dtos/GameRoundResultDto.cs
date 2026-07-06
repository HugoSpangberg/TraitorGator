namespace TraitorGator.Shared.Dtos;

public class GameRoundResultDto
{
    public string GameCode { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public string NormalQuestion { get; set; } = string.Empty;
    public string TraitorQuestion { get; set; } = string.Empty;
    public List<AnswerResultDto> Answers { get; set; } = new();
    public List<VoteResultDto> Votes { get; set; } = new();
    public int PlayerCount { get; set; }
    public int VotesSubmitted { get; set; }
    public bool VotingComplete { get; set; }
    public string? AccusedUsername { get; set; }
    public bool? AccusedWasTraitor { get; set; }
    public bool GameComplete { get; set; }
    public string? Winner { get; set; }
    public string? TraitorUsername { get; set; }
    public bool CanAdvance { get; set; }
}
