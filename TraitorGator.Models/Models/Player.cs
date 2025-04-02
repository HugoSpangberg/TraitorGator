using TraitorGator.Models.Models;

namespace TraitorGator.API.Models;

public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public PlayerRole Role { get; set; }
    public bool IsActive { get; set; }
    public Guid GameRoundId { get; set; }
    public GameRound GameRound { get; set; }
}
