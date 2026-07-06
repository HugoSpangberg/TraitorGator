namespace TraitorGator.Shared.Dtos;

public class PlayerSummaryDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsHost { get; set; }
    public bool HasAnswered { get; set; }
    public bool HasVoted { get; set; }
}
