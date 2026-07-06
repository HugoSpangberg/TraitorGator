namespace TraitorGator.Shared.Dtos;

public class PlayerRoleDto
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsTraitor { get; set; }
}
