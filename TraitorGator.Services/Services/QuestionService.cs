using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.API.Models;
using TraitorGator.Models.Dto;
using TraitorGator.Models.Enums;
using TraitorGator.Models.Models;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.Services.Services
{
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
                Text = request.Text,
                AlteredText = request.AlteredText,
                RoundNumber = request.RoundNumber,
                QuestionType = request.QuestionType
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Answer> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var player = await _context.Players.FindAsync(request.PlayerId);
            if (player == null)
                throw new Exception("Player not found.");

            var question = await _context.Questions.FindAsync(request.QuestionId);
            if (question == null)
                throw new Exception("Question not found.");

            var answer = new Answer
            {
                PlayerId = request.PlayerId,
                QuestionId = request.QuestionId,
                AnswerText = request.AnswerText,
                IsCorrect = false
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }


        public async Task<QuestionForPlayerDto> GetQuestionForPlayerAsync(string gameCode, Guid playerId)
        {
            var round = await _context.GameRounds
                .Include(gr => gr.Players)
                .FirstOrDefaultAsync(gr => gr.GameCode == gameCode);
            if (round == null)
                throw new Exception("Game not found");

            var player = round.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                throw new Exception("Player not in game");

            QuestionType wantedType = round.Mode switch
            {
                GameMode.Standard => QuestionType.Quiz,
                GameMode.Advanced => QuestionType.OtherWords,
                _ => throw new Exception("Okänt spelläge")
            };

            var candidates = await _context.Questions.Where(q => q.QuestionType == wantedType).ToListAsync();

            if (!candidates.Any())
                throw new Exception("Inga frågor definierade för detta spelläge");

            Question question;
            if (!round.CurrentQuestionId.HasValue)
            {
                var random = new Random();
                question = candidates[random.Next(candidates.Count)];
                round.CurrentQuestionId = question.Id;
                await _context.SaveChangesAsync();
            }
            else
            {
                question = await _context.Questions.FindAsync(round.CurrentQuestionId.Value) ?? throw new Exception("Sparad fråga saknas i databasen");
            }

            var text = player.Role == PlayerRole.Traitor ? question.AlteredText : question.Text;

            return new QuestionForPlayerDto
            {
                QuestionId = question.Id,
                Text = text
            };
        }


        public async Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode, int round)
        {
            var gr = await _context.GameRounds
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (gr == null)
                throw new Exception("Game round not found");

            if (!gr.CurrentQuestionId.HasValue)
                throw new Exception("No question selected for this round");

            var questionId = gr.CurrentQuestionId.Value;

            var answers = await _context.Answers
                .Include(a => a.Player)
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();

            return answers.Select(a => new AnswerResultDto
            {
                Username   = a.Player.Username,
                AnswerText = a.AnswerText
            });
        }
        public async Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode)
        {
            var gr = await _context.GameRounds
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GameCode == gameCode);

            if (gr == null)
                throw new Exception("Game round not found");

            return await GetAnswersForGameAsync(gameCode, gr.CurrentRound);
        }

        public async Task<QuestionDetailDto?> GetQuestionDetailAsync(string gameCode, int round)
        {
            var gr = await _context.GameRounds
                .Include(g => g.CurrentQuestion)
                .FirstOrDefaultAsync(g => g.GameCode == gameCode);

            if (gr == null || gr.CurrentQuestion == null)
                return null;

            if (gr.CurrentRound != round)
                return null;

            return new QuestionDetailDto
            {
                Text = gr.CurrentQuestion.Text,
                AlteredText = gr.CurrentQuestion.AlteredText
            };
        }
    }
}