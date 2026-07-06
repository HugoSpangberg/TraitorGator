using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Services;

public class PlayerService : IPlayerService
{
    private readonly GameDbContext _context;

    public PlayerService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetPlayerByIdAsync(Guid playerId)
    {
        return await _context.Players.FindAsync(playerId);
    }

    public async Task<PlayerRoleDto?> GetPlayerRoleAsync(Guid playerId, string playerSecret)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null || !GameService.SecretMatches(player.AccessToken, playerSecret))
        {
            return null;
        }

        return new PlayerRoleDto
        {
            PlayerId = player.Id,
            Username = player.Username,
            Role = player.Role.ToString(),
            IsTraitor = player.Role == PlayerRole.Traitor
        };
    }

    public async Task<bool> IsValidPlayerSecretAsync(Guid playerId, string playerSecret)
    {
        var player = await _context.Players.FindAsync(playerId);
        return player != null && GameService.SecretMatches(player.AccessToken, playerSecret);
    }
}
