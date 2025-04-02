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
    public List<Player> Players { get; set; } = new List<Player>();
    public int CurrentRound { get; set; } = 1;
    public List<Question> Questions { get; set; } = new();
    public Guid? TraitorId { get; set; }
    public Player? Traitor { get; set; }
}
