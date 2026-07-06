using TraitorGator.Models.Models;

namespace TraitorGator.API.Models;

public class GameRound
{
    public Guid Id { get; set; }
    public GameMode Mode { get; set; } = GameMode.Standard;
    public string GameCode { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(90);
    public bool IsComplete { get; set; }
    public bool Started { get; set; }
    public Guid? CurrentQuestionId { get; set; }
    public Question? CurrentQuestion { get; set; }
    public List<Player> Players { get; set; } = new();
    public int CurrentRound { get; set; } = 1;
    public int MaxRounds { get; set; } = 3;
    public Guid? TraitorId { get; set; }
    public Player? Traitor { get; set; }
    public string? Winner { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
}
