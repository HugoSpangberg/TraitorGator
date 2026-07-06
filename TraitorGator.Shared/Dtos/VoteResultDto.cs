namespace TraitorGator.Shared.Dtos;

public class VoteResultDto
{
    public Guid VoterPlayerId { get; set; }
    public string VoterUsername { get; set; } = string.Empty;
    public Guid SuspectPlayerId { get; set; }
    public string SuspectUsername { get; set; } = string.Empty;
}
