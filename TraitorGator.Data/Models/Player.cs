using TraitorGator.Models.Models;

namespace TraitorGator.API.Models;

public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public PlayerRole Role { get; set; } = PlayerRole.Gator;
    public bool IsActive { get; set; } = true;
    public string AccessToken { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public Guid GameRoundId { get; set; }
    public GameRound GameRound { get; set; } = null!;
}
