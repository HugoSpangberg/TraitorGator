namespace TraitorGator.Shared.Dtos;

public class SubmitVoteRequest
{
    public Guid VoterPlayerId { get; set; }
    public string PlayerSecret { get; set; } = string.Empty;
    public Guid SuspectPlayerId { get; set; }
    public int RoundNumber { get; set; }
}
