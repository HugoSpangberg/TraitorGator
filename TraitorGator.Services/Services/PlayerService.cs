
using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.Services.Services;

public class PlayerService : IPlayerService
{
    private readonly GameDbContext _context;

    public PlayerService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Player> AddPlayerToGameAsync(string gameCode, string username)
    {
        var gameRound = await _context.GameRounds.FirstOrDefaultAsync(gr => gr.GameCode == gameCode);
        if (gameRound == null)
            throw new Exception("Game round not found.");

        var player = new Player { Username = username, GameRoundId = gameRound.Id };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    public async Task<Player?> GetPlayerByIdAsync(Guid playerId)
    {
        return await _context.Players.FindAsync(playerId);
    }

    public async Task<Player> AssignRoleAsync(Guid playerId, PlayerRole role)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null)
            throw new Exception("Player not found.");

        player.Role = role;
        await _context.SaveChangesAsync();
        return player;
    }
}
