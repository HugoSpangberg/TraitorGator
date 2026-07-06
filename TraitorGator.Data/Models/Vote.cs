using TraitorGator.API.Models;

namespace TraitorGator.Models.Models;

public class Vote
{
    public Guid Id { get; set; }
    public Guid GameRoundId { get; set; }
    public GameRound GameRound { get; set; } = null!;
    public int RoundNumber { get; set; }
    public Guid VoterPlayerId { get; set; }
    public Player VoterPlayer { get; set; } = null!;
    public Guid SuspectPlayerId { get; set; }
    public Player SuspectPlayer { get; set; } = null!;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
