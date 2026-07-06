namespace TraitorGator.Shared.Dtos;

public class PlayerActionRequest
{
    public Guid PlayerId { get; set; }
    public string PlayerSecret { get; set; } = string.Empty;
}
