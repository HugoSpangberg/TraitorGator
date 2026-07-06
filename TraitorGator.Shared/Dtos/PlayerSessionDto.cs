namespace TraitorGator.Shared.Dtos;

public class PlayerSessionDto
{
    public string GameCode { get; set; } = string.Empty;
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PlayerSecret { get; set; } = string.Empty;
    public bool IsHost { get; set; }
}
