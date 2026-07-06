using TraitorGator.API.Models;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Interfaces;

public interface IPlayerService
{
    Task<Player?> GetPlayerByIdAsync(Guid playerId);
    Task<PlayerRoleDto?> GetPlayerRoleAsync(Guid playerId, string playerSecret);
    Task<bool> IsValidPlayerSecretAsync(Guid playerId, string playerSecret);
}
