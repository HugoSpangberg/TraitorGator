namespace TraitorGator.Shared.Dtos;

public class GameStateDto
{
    public string GameCode { get; set; } = string.Empty;
    public bool Started { get; set; }
    public bool IsComplete { get; set; }
    public int CurrentRound { get; set; }
    public int MaxRounds { get; set; }
    public int DurationSeconds { get; set; }
    public int MinPlayers { get; set; } = 3;
    public string? Winner { get; set; }
    public List<PlayerSummaryDto> Players { get; set; } = new();
}
