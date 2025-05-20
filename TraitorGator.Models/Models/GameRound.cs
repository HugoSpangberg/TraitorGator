using TraitorGator.Models.Models;

namespace TraitorGator.API.Models;

public class GameRound
{
    public Guid Id { get; set; }
    public GameMode Mode { get; set; }
    public string GameCode { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsComplete { get; set; }
    public bool Started { get; set; }
    public Guid? CurrentQuestionId { get; set; }
    public Question? CurrentQuestion { get; set; }
    public List<Player> Players { get; set; } = new();
    public int CurrentRound { get; set; } = 1;
    public Guid? TraitorId { get; set; }
    public Player? Traitor { get; set; }
}
