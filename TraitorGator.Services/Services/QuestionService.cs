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

            // 1) Bestäm vilken QuestionType vi ska plocka från
            QuestionType wantedType = round.Mode switch
            {
                GameMode.Standard => QuestionType.Quiz,
                GameMode.Advanced => QuestionType.OtherWords,
                _ => throw new Exception("Okänt spelläge")
            };

            // 2) Hämta alla frågor av rätt typ
            var candidates = await _context.Questions
                .Where(q => q.QuestionType == wantedType)
                .ToListAsync();

            if (!candidates.Any())
                throw new Exception("Inga frågor definierade för detta spelläge");

            // 3) Slumpa en enda fråga (om det inte redan finns en sparad)
            Question q;
            if (!round.CurrentQuestionId.HasValue)
            {
                var rnd = new Random();
                q = candidates[rnd.Next(candidates.Count)];
                round.CurrentQuestionId = q.Id;
                await _context.SaveChangesAsync();
            }
            else
            {
                q = await _context.Questions.FindAsync(round.CurrentQuestionId.Value)
                    ?? throw new Exception("Sparad fråga saknas i databasen");
            }

            // 4) Returnera alteredText för traitor, text för gator
            var text = player.Role == PlayerRole.Traitor
                ? q.AlteredText
                : q.Text;

            return new QuestionForPlayerDto
            {
                QuestionId = q.Id,
                Text = text
            };
        }


        public async Task<IEnumerable<AnswerResultDto>> GetAnswersForGameAsync(string gameCode)
        {
            var currentRound = await _context.GameRounds
                .Where(gr => gr.GameCode == gameCode)
                .Select(gr => gr.CurrentRound)
                .FirstOrDefaultAsync();

            var answers = await _context.Answers
                .Include(a => a.Player)
                .Where(a => a.Question.RoundNumber == currentRound)
                .ToListAsync();

            return answers.Select(a => new AnswerResultDto
            {
                Username = a.Player.Username,
                AnswerText = a.AnswerText
            });
        }
    }
}