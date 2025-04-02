using TraitorGator.API.Models;
using TraitorGator.Models.Models;

namespace TraitorGator.Services.Interfaces;

public interface IPlayerService
{
    Task<Player> AddPlayerToGameAsync(string gameCode, string username);
    Task<Player?> GetPlayerByIdAsync(Guid playerId);
    Task<Player> AssignRoleAsync(Guid playerId, PlayerRole role);
}
