using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;
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

    public async Task<GameRound?> GetGameRoundByCodeAsync(string gameCode)
    {
        return await _context.GameRounds
            .Include(gr => gr.Players)       // Ladda spelare
            .Include(gr => gr.Traitor)       // Om du vill visa Traitor-fältet
            .Include(gr => gr.Questions)     // Om du visar frågor i lobbyn
            .FirstOrDefaultAsync(gr => gr.GameCode == gameCode);
    }

    public async Task<GameRound?> GetGameRoundByIdAsync(Guid gameRoundId)
    {
        return await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Traitor)
            .Include(gr => gr.Questions)
            .FirstOrDefaultAsync(gr => gr.Id == gameRoundId);
    }


    private static string GenerateGameCode()
    {
        var rnd = new Random();
        return rnd.Next(1000, 9999).ToString();
    }

    public async Task<bool> StartGameAsync(string gameCode)
    {
        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .FirstOrDefaultAsync(gr => gr.GameCode == gameCode);

        if (round == null || round.Players.Count < 3)
            return false;

        // 1 traitor för varje 3 spelare
        int traitorCount = round.Players.Count / 3;
        var rnd = new Random();
        var traitors = round.Players
            .OrderBy(_ => rnd.Next())
            .Take(traitorCount)
            .ToList();

        // Sätt roller
        foreach (var p in round.Players)
            p.Role = traitors.Contains(p)
                ? PlayerRole.Traitor
                : PlayerRole.Normal;

        await _context.SaveChangesAsync();
        return true;
    }
}
