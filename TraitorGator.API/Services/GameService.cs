using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services;

public class GameService : IGameService
{
    public const int MinPlayers = 3;
    private const int MaxPlayers = 10;

    private readonly GameDbContext _context;

    public GameService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerSessionDto> CreateGameAsync(CreateGameRequest request)
    {
        var username = NormalizeUsername(request.Username);
        var gameRound = new GameRound
        {
            GameCode = await GenerateUniqueGameCodeAsync(),
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromSeconds(90),
            MaxRounds = Math.Clamp(request.MaxRounds, 1, 5),
            IsComplete = false,
            Started = false
        };

        var host = new Player
        {
            Username = username,
            AccessToken = GeneratePlayerSecret(),
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };

        gameRound.Players.Add(host);
        _context.GameRounds.Add(gameRound);
        await _context.SaveChangesAsync();

        return ToPlayerSession(gameRound, host, isHost: true);
    }

    public async Task<PlayerSessionDto> JoinGameAsync(JoinGameRequest request)
    {
        var gameCode = NormalizeGameCode(request.GameCode);
        var username = NormalizeUsername(request.Username);
        var gameRound = await _context.GameRounds
            .Include(gr => gr.Players)
            .FirstOrDefaultAsync(gr => gr.GameCode == gameCode);

        if (gameRound == null)
        {
            throw new KeyNotFoundException("Spelet hittades inte.");
        }

        if (gameRound.Started || gameRound.IsComplete)
        {
            throw new InvalidOperationException("Spelet har redan startat.");
        }

        if (gameRound.Players.Count >= MaxPlayers)
        {
            throw new InvalidOperationException("Spelet är fullt.");
        }

        if (gameRound.Players.Any(p => string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Användarnamnet är redan taget i den här lobbyn.");
        }

        var player = new Player
        {
            Username = username,
            AccessToken = GeneratePlayerSecret(),
            GameRoundId = gameRound.Id,
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return ToPlayerSession(gameRound, player, isHost: false);
    }

    public async Task<GameStateDto?> GetGameStateAsync(string gameCode)
    {
        var round = await LoadGameForStateAsync(gameCode);
        return round == null ? null : ToGameState(round);
    }

    public async Task<GameRound?> GetGameRoundByCodeAsync(string gameCode)
    {
        var normalizedCode = NormalizeGameCode(gameCode);
        return await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Traitor)
            .FirstOrDefaultAsync(gr => gr.GameCode == normalizedCode);
    }

    public async Task<GameRound?> GetGameRoundByIdAsync(Guid gameRoundId)
    {
        return await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Traitor)
            .FirstOrDefaultAsync(gr => gr.Id == gameRoundId);
    }

    public async Task<GameStateDto> StartGameAsync(string gameCode, PlayerActionRequest request)
    {
        var round = await LoadGameForStateAsync(gameCode)
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        EnsureHost(round, request.PlayerId, request.PlayerSecret);

        if (round.Started)
        {
            throw new InvalidOperationException("Spelet är redan startat.");
        }

        var activePlayers = OrderedPlayers(round).Where(p => p.IsActive).ToList();
        if (activePlayers.Count < MinPlayers)
        {
            throw new InvalidOperationException($"Minst {MinPlayers} spelare behövs för att starta.");
        }

        var traitor = activePlayers[RandomNumberGenerator.GetInt32(activePlayers.Count)];
        foreach (var player in activePlayers)
        {
            player.Role = player.Id == traitor.Id ? PlayerRole.Traitor : PlayerRole.Gator;
        }

        round.TraitorId = traitor.Id;
        round.CurrentRound = 1;
        round.CurrentQuestionId = null;
        round.StartTime = DateTime.UtcNow;
        round.Started = true;
        round.IsComplete = false;
        round.Winner = null;

        await _context.SaveChangesAsync();
        return ToGameState(round);
    }

    public async Task<GameStateDto> AdvanceRoundAsync(string gameCode, PlayerActionRequest request)
    {
        var round = await LoadGameForStateAsync(gameCode)
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        EnsureHost(round, request.PlayerId, request.PlayerSecret);

        if (!round.Started)
        {
            throw new InvalidOperationException("Spelet har inte startat.");
        }

        if (round.IsComplete)
        {
            throw new InvalidOperationException("Spelet är redan avslutat.");
        }

        var playerCount = round.Players.Count(p => p.IsActive);
        var voteCount = round.Votes.Count(v => v.RoundNumber == round.CurrentRound);
        if (voteCount < playerCount)
        {
            throw new InvalidOperationException("Alla spelare måste rösta innan nästa runda.");
        }

        if (round.CurrentRound >= round.MaxRounds)
        {
            round.IsComplete = true;
            round.Winner = "Traitor";
        }
        else
        {
            round.CurrentRound++;
            round.CurrentQuestionId = null;
            round.StartTime = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return ToGameState(round);
    }

    public async Task<GameRoundResultDto?> GetRoundResultAsync(string gameCode, int roundNumber)
    {
        var normalizedCode = NormalizeGameCode(gameCode);
        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Traitor)
            .FirstOrDefaultAsync(gr => gr.GameCode == normalizedCode);

        if (round == null)
        {
            return null;
        }

        var answers = await _context.Answers
            .Include(a => a.Player)
            .Include(a => a.Question)
            .Where(a => a.GameRoundId == round.Id && a.RoundNumber == roundNumber)
            .ToListAsync();

        var votes = await _context.Votes
            .Include(v => v.VoterPlayer)
            .Include(v => v.SuspectPlayer)
            .Where(v => v.GameRoundId == round.Id && v.RoundNumber == roundNumber)
            .ToListAsync();

        var question = answers.FirstOrDefault()?.Question;
        if (question == null && roundNumber == round.CurrentRound && round.CurrentQuestionId.HasValue)
        {
            question = await _context.Questions.FindAsync(round.CurrentQuestionId.Value);
        }

        var playerCount = round.Players.Count(p => p.IsActive);
        var votingComplete = playerCount > 0 && votes.Count >= playerCount;
        var accused = votingComplete ? GetTopSuspects(round, votes) : [];
        var accusedWasTraitor = votingComplete && round.TraitorId.HasValue
            ? accused.Any(p => p.Id == round.TraitorId.Value)
            : (bool?)null;

        var orderedPlayers = OrderedPlayers(round).ToList();
        return new GameRoundResultDto
        {
            GameCode = round.GameCode,
            RoundNumber = roundNumber,
            NormalQuestion = question?.Text ?? string.Empty,
            TraitorQuestion = question?.AlteredText ?? string.Empty,
            Answers = answers
                .OrderBy(a => orderedPlayers.FindIndex(p => p.Id == a.PlayerId))
                .Select(a => new AnswerResultDto
                {
                    PlayerId = a.PlayerId,
                    Username = a.Player.Username,
                    AnswerText = a.AnswerText
                })
                .ToList(),
            Votes = votes
                .OrderBy(v => orderedPlayers.FindIndex(p => p.Id == v.VoterPlayerId))
                .Select(v => new VoteResultDto
                {
                    VoterPlayerId = v.VoterPlayerId,
                    VoterUsername = v.VoterPlayer.Username,
                    SuspectPlayerId = v.SuspectPlayerId,
                    SuspectUsername = v.SuspectPlayer.Username
                })
                .ToList(),
            PlayerCount = playerCount,
            VotesSubmitted = votes.Count,
            VotingComplete = votingComplete,
            AccusedUsername = accused.Count == 0 ? null : string.Join(", ", accused.Select(p => p.Username)),
            AccusedWasTraitor = accusedWasTraitor,
            GameComplete = round.IsComplete,
            Winner = round.Winner,
            TraitorUsername = round.IsComplete ? round.Traitor?.Username : null,
            CanAdvance = votingComplete && !round.IsComplete && roundNumber == round.CurrentRound
        };
    }

    public async Task<GameRoundResultDto> SubmitVoteAsync(string gameCode, SubmitVoteRequest request)
    {
        var normalizedCode = NormalizeGameCode(gameCode);
        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Votes)
            .FirstOrDefaultAsync(gr => gr.GameCode == normalizedCode)
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        if (!round.Started || round.IsComplete)
        {
            throw new InvalidOperationException("Röstning är inte öppen.");
        }

        if (request.RoundNumber != round.CurrentRound)
        {
            throw new InvalidOperationException("Den rundan är inte aktiv längre.");
        }

        var voter = FindPlayerWithSecret(round, request.VoterPlayerId, request.PlayerSecret);
        if (voter == null)
        {
            throw new UnauthorizedAccessException("Spelarsessionen är ogiltig.");
        }

        if (request.SuspectPlayerId == request.VoterPlayerId)
        {
            throw new InvalidOperationException("Du kan inte rösta på dig själv.");
        }

        if (!round.Players.Any(p => p.Id == request.SuspectPlayerId && p.IsActive))
        {
            throw new InvalidOperationException("Den valda spelaren finns inte i spelet.");
        }

        var playerCount = round.Players.Count(p => p.IsActive);
        var answerCount = await _context.Answers
            .CountAsync(a => a.GameRoundId == round.Id && a.RoundNumber == round.CurrentRound);
        if (answerCount < playerCount)
        {
            throw new InvalidOperationException("Alla spelare måste svara innan röstning.");
        }

        var vote = await _context.Votes.FirstOrDefaultAsync(v =>
            v.GameRoundId == round.Id &&
            v.RoundNumber == round.CurrentRound &&
            v.VoterPlayerId == request.VoterPlayerId);

        if (vote == null)
        {
            vote = new Vote
            {
                GameRoundId = round.Id,
                RoundNumber = round.CurrentRound,
                VoterPlayerId = request.VoterPlayerId
            };
            _context.Votes.Add(vote);
        }

        vote.SuspectPlayerId = request.SuspectPlayerId;
        vote.SubmittedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await ResolveVoteIfCompleteAsync(round);
        return await GetRoundResultAsync(gameCode, request.RoundNumber)
            ?? throw new KeyNotFoundException("Spelet hittades inte.");
    }

    internal static bool SecretMatches(string expected, string actual)
    {
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(actual))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        return expectedBytes.Length == actualBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }

    private async Task ResolveVoteIfCompleteAsync(GameRound round)
    {
        var playerCount = round.Players.Count(p => p.IsActive);
        var votes = await _context.Votes
            .Where(v => v.GameRoundId == round.Id && v.RoundNumber == round.CurrentRound)
            .ToListAsync();

        if (votes.Count < playerCount)
        {
            return;
        }

        var topSuspects = GetTopSuspects(round, votes);
        var traitorCaught = round.TraitorId.HasValue && topSuspects.Any(p => p.Id == round.TraitorId.Value);
        if (traitorCaught)
        {
            round.IsComplete = true;
            round.Winner = "Gators";
        }
        else if (round.CurrentRound >= round.MaxRounds)
        {
            round.IsComplete = true;
            round.Winner = "Traitor";
        }

        await _context.SaveChangesAsync();
    }

    private static List<Player> GetTopSuspects(GameRound round, IEnumerable<Vote> votes)
    {
        var groupedVotes = votes
            .GroupBy(v => v.SuspectPlayerId)
            .Select(group => new { PlayerId = group.Key, Count = group.Count() })
            .ToList();

        if (groupedVotes.Count == 0)
        {
            return [];
        }

        var maxVotes = groupedVotes.Max(group => group.Count);
        var topIds = groupedVotes
            .Where(group => group.Count == maxVotes)
            .Select(group => group.PlayerId)
            .ToHashSet();

        return OrderedPlayers(round)
            .Where(p => topIds.Contains(p.Id))
            .ToList();
    }

    private async Task<GameRound?> LoadGameForStateAsync(string gameCode)
    {
        var normalizedCode = NormalizeGameCode(gameCode);
        return await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.Answers)
            .Include(gr => gr.Votes)
            .FirstOrDefaultAsync(gr => gr.GameCode == normalizedCode);
    }

    private static GameStateDto ToGameState(GameRound round)
    {
        var orderedPlayers = OrderedPlayers(round).ToList();
        var hostId = orderedPlayers.FirstOrDefault()?.Id;
        var answeredPlayerIds = round.Answers
            .Where(a => a.RoundNumber == round.CurrentRound)
            .Select(a => a.PlayerId)
            .ToHashSet();
        var votedPlayerIds = round.Votes
            .Where(v => v.RoundNumber == round.CurrentRound)
            .Select(v => v.VoterPlayerId)
            .ToHashSet();

        return new GameStateDto
        {
            GameCode = round.GameCode,
            Started = round.Started,
            IsComplete = round.IsComplete,
            CurrentRound = round.CurrentRound,
            MaxRounds = round.MaxRounds,
            DurationSeconds = (int)round.Duration.TotalSeconds,
            MinPlayers = MinPlayers,
            Winner = round.Winner,
            Players = orderedPlayers
                .Select(p => new PlayerSummaryDto
                {
                    Id = p.Id,
                    Username = p.Username,
                    IsActive = p.IsActive,
                    IsHost = hostId == p.Id,
                    HasAnswered = answeredPlayerIds.Contains(p.Id),
                    HasVoted = votedPlayerIds.Contains(p.Id)
                })
                .ToList()
        };
    }

    private static PlayerSessionDto ToPlayerSession(GameRound round, Player player, bool isHost)
    {
        return new PlayerSessionDto
        {
            GameCode = round.GameCode,
            PlayerId = player.Id,
            Username = player.Username,
            PlayerSecret = player.AccessToken,
            IsHost = isHost
        };
    }

    private static void EnsureHost(GameRound round, Guid playerId, string playerSecret)
    {
        var host = OrderedPlayers(round).FirstOrDefault()
            ?? throw new InvalidOperationException("Spelet saknar host.");

        if (host.Id != playerId || !SecretMatches(host.AccessToken, playerSecret))
        {
            throw new UnauthorizedAccessException("Bara hosten kan göra detta.");
        }
    }

    private static Player? FindPlayerWithSecret(GameRound round, Guid playerId, string playerSecret)
    {
        var player = round.Players.FirstOrDefault(p => p.Id == playerId && p.IsActive);
        return player != null && SecretMatches(player.AccessToken, playerSecret) ? player : null;
    }

    private static IEnumerable<Player> OrderedPlayers(GameRound round)
    {
        return round.Players.OrderBy(p => p.JoinedAt).ThenBy(p => p.Id);
    }

    private async Task<string> GenerateUniqueGameCodeAsync()
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var code = RandomNumberGenerator.GetInt32(0, 10_000).ToString("0000");
            if (!await _context.GameRounds.AnyAsync(gr => gr.GameCode == code))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Kunde inte skapa en unik spelkod.");
    }

    private static string GeneratePlayerSecret()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    private static string NormalizeGameCode(string gameCode)
    {
        var normalized = (gameCode ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Spelkod krävs.");
        }

        return normalized;
    }

    private static string NormalizeUsername(string username)
    {
        var normalized = (username ?? string.Empty).Trim();
        if (normalized.Length < 2)
        {
            throw new ArgumentException("Användarnamnet måste vara minst två tecken.");
        }

        if (normalized.Length > 24)
        {
            throw new ArgumentException("Användarnamnet får vara max 24 tecken.");
        }

        return normalized;
    }
}
