using TraitorGator.API.Models;

namespace TraitorGator.Services.Interfaces;

public interface IGameService
{
    Task<GameRound> CreateGameRoundAsync();
    Task<GameRound?> GetGameRoundByIdAsync(Guid gameRoundId);
    Task<GameRound?> GetGameRoundByCodeAsync(string gameCode);
}
