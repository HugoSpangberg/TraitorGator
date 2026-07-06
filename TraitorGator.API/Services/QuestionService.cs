using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Enums;
using TraitorGator.Models.Models;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;
using CreateQuestionRequest = TraitorGator.Models.Dto.CreateQuestionRequest;

namespace TraitorGator.Services.Services;

public class QuestionService : IQuestionService
{
    private readonly GameDbContext _context;

    public QuestionService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Question> CreateQuestionAsync(CreateQuestionRequest request)
    {
        var question = new Question
        {
            Text = request.Text.Trim(),
            AlteredText = request.AlteredText.Trim(),
            RoundNumber = request.RoundNumber,
            QuestionType = request.QuestionType
        };

        if (string.IsNullOrWhiteSpace(question.Text) || string.IsNullOrWhiteSpace(question.AlteredText))
        {
            throw new ArgumentException("Både vanlig fråga och traitor-fråga krävs.");
        }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        var player = await _context.Players.FindAsync(request.PlayerId)
            ?? throw new KeyNotFoundException("Spelaren hittades inte.");

        if (!GameService.SecretMatches(player.AccessToken, request.PlayerSecret))
        {
            throw new UnauthorizedAccessException("Spelarsessionen är ogiltig.");
        }

        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .FirstOrDefaultAsync(gr => gr.Id == player.GameRoundId)
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        if (!round.Started || round.IsComplete)
        {
            throw new InvalidOperationException("Rundan tar inte emot svar.");
        }

        if (round.CurrentQuestionId != request.QuestionId)
        {
            throw new InvalidOperationException("Frågan matchar inte den aktiva rundan.");
        }

        var answerText = string.IsNullOrWhiteSpace(request.AnswerText)
            ? "(inget svar)"
            : request.AnswerText.Trim();
        if (answerText.Length > 500)
        {
            throw new ArgumentException("Svaret får vara max 500 tecken.");
        }

        var answer = await _context.Answers.FirstOrDefaultAsync(a =>
            a.GameRoundId == round.Id &&
            a.RoundNumber == round.CurrentRound &&
            a.PlayerId == player.Id);

        if (answer == null)
        {
            answer = new Answer
            {
                GameRoundId = round.Id,
                RoundNumber = round.CurrentRound,
                PlayerId = player.Id,
                QuestionId = request.QuestionId
            };
            _context.Answers.Add(answer);
        }

        answer.AnswerText = answerText;
        answer.SubmittedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new AnswerResultDto
        {
            PlayerId = player.Id,
            Username = player.Username,
            AnswerText = answer.AnswerText
        };
    }

    public async Task<QuestionForPlayerDto> GetQuestionForPlayerAsync(string gameCode, Guid playerId, string playerSecret)
    {
        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .Include(gr => gr.CurrentQuestion)
            .FirstOrDefaultAsync(gr => gr.GameCode == gameCode.Trim().ToUpperInvariant())
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        if (!round.Started || round.IsComplete)
        {
            throw new InvalidOperationException("Spelet är inte i en aktiv frågerunda.");
        }

        var player = round.Players.FirstOrDefault(p => p.Id == playerId && p.IsActive);
        if (player == null || !GameService.SecretMatches(player.AccessToken, playerSecret))
        {
            throw new UnauthorizedAccessException("Spelarsessionen är ogiltig.");
        }

        var question = round.CurrentQuestion;
        if (question == null)
        {
            question = await PickQuestionAsync(round);
            round.CurrentQuestionId = question.Id;
            await _context.SaveChangesAsync();
        }

        var text = player.Role == PlayerRole.Traitor ? question.AlteredText : question.Text;
        return new QuestionForPlayerDto
        {
            QuestionId = question.Id,
            RoundNumber = round.CurrentRound,
            DurationSeconds = (int)round.Duration.TotalSeconds,
            Text = text
        };
    }

    public async Task<RoundProgressDto> GetRoundProgressAsync(string gameCode, int? roundNumber = null)
    {
        var round = await _context.GameRounds
            .Include(gr => gr.Players)
            .FirstOrDefaultAsync(gr => gr.GameCode == gameCode.Trim().ToUpperInvariant())
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        var selectedRound = roundNumber ?? round.CurrentRound;
        var playerCount = round.Players.Count(p => p.IsActive);
        var answersSubmitted = await _context.Answers
            .CountAsync(a => a.GameRoundId == round.Id && a.RoundNumber == selectedRound);

        return new RoundProgressDto
        {
            GameCode = round.GameCode,
            RoundNumber = selectedRound,
            PlayerCount = playerCount,
            AnswersSubmitted = answersSubmitted,
            AllAnswered = playerCount > 0 && answersSubmitted >= playerCount
        };
    }

    public async Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode, int roundNumber)
    {
        var gameRound = await _context.GameRounds
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GameCode == gameCode.Trim().ToUpperInvariant())
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        var answers = await _context.Answers
            .AsNoTracking()
            .Include(a => a.Player)
            .Where(a => a.GameRoundId == gameRound.Id && a.RoundNumber == roundNumber)
            .OrderBy(a => a.Player.JoinedAt)
            .ToListAsync();

        return answers.Select(a => new AnswerResultDto
        {
            PlayerId = a.PlayerId,
            Username = a.Player.Username,
            AnswerText = a.AnswerText
        });
    }

    public async Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode)
    {
        var gameRound = await _context.GameRounds
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GameCode == gameCode.Trim().ToUpperInvariant())
            ?? throw new KeyNotFoundException("Spelet hittades inte.");

        return await GetAnswersForGameAsync(gameCode, gameRound.CurrentRound);
    }

    public async Task<QuestionDetailDto?> GetQuestionDetailAsync(string gameCode, int roundNumber)
    {
        var gameRound = await _context.GameRounds
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GameCode == gameCode.Trim().ToUpperInvariant());

        if (gameRound == null)
        {
            return null;
        }

        Question? question = null;
        if (roundNumber == gameRound.CurrentRound && gameRound.CurrentQuestionId.HasValue)
        {
            question = await _context.Questions.FindAsync(gameRound.CurrentQuestionId.Value);
        }

        question ??= await _context.Answers
            .AsNoTracking()
            .Include(a => a.Question)
            .Where(a => a.GameRoundId == gameRound.Id && a.RoundNumber == roundNumber)
            .Select(a => a.Question)
            .FirstOrDefaultAsync();

        return question == null
            ? null
            : new QuestionDetailDto
            {
                Text = question.Text,
                AlteredText = question.AlteredText
            };
    }

    private async Task<Question> PickQuestionAsync(GameRound round)
    {
        var wantedType = round.Mode switch
        {
            GameMode.Standard => QuestionType.Quiz,
            GameMode.Advanced => QuestionType.OtherWords,
            _ => QuestionType.Quiz
        };

        var candidates = await _context.Questions
            .Where(q => q.QuestionType == wantedType && (q.RoundNumber == null || q.RoundNumber == round.CurrentRound))
            .ToListAsync();

        if (candidates.Count == 0)
        {
            candidates = await _context.Questions
                .Where(q => q.QuestionType == wantedType)
                .ToListAsync();
        }

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException("Inga frågor finns för spelläget.");
        }

        return candidates[Random.Shared.Next(candidates.Count)];
    }
}
