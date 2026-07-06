namespace TraitorGator.Shared.Dtos;

public class CreateGameRequest
{
    public string Username { get; set; } = string.Empty;
    public int MaxRounds { get; set; } = 3;
}
