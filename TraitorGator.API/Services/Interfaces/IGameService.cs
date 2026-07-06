using TraitorGator.API.Models;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Interfaces;

public interface IGameService
{
    Task<PlayerSessionDto> CreateGameAsync(CreateGameRequest request);
    Task<PlayerSessionDto> JoinGameAsync(JoinGameRequest request);
    Task<GameStateDto?> GetGameStateAsync(string gameCode);
    Task<GameRound?> GetGameRoundByIdAsync(Guid gameRoundId);
    Task<GameRound?> GetGameRoundByCodeAsync(string gameCode);
    Task<GameStateDto> StartGameAsync(string gameCode, PlayerActionRequest request);
    Task<GameStateDto> AdvanceRoundAsync(string gameCode, PlayerActionRequest request);
    Task<GameRoundResultDto?> GetRoundResultAsync(string gameCode, int roundNumber);
    Task<GameRoundResultDto> SubmitVoteAsync(string gameCode, SubmitVoteRequest request);
}
