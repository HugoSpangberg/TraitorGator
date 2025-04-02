using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Services.Interfaces;

namespace TraitorGator.Services;

public class GameService : IGameService
{
    private readonly GameDbContext _context;

    public GameService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<GameRound> CreateGameRoundAsync()
    {
        var gameRound = new GameRound
        {
            GameCode = GenerateGameCode(),
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromMinutes(30),
            IsComplete = false
        };

        _context.GameRounds.Add(gameRound);
        await _context.SaveChangesAsync();
        return gameRound;
    }

    public async Task<GameRound?> GetGameRoundByIdAsync(Guid gameRoundId)
    {
        return await _context.GameRounds.FindAsync(gameRoundId);
    }

    public async Task<GameRound?> GetGameRoundByCodeAsync(string gameCode)
    {
        return await _context.GameRounds.FirstOrDefaultAsync(gr => gr.GameCode == gameCode);
    }



    private static string GenerateGameCode()
    {
        var rnd = new Random();
        return rnd.Next(1000, 9999).ToString();
    }
}
