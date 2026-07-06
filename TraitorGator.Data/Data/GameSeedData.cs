using Microsoft.EntityFrameworkCore;
using TraitorGator.Models.Enums;
using TraitorGator.Models.Models;

namespace TraitorGator.API.Data;

public static class GameSeedData
{
    public static async Task EnsureSeededAsync(GameDbContext db)
    {
        if (await db.Questions.AnyAsync())
        {
            return;
        }

        db.Questions.AddRange(
            new Question
            {
                Text = "Vilken plats hade varit värst att fastna på över natten?",
                AlteredText = "Vilken plats hade varit bäst att gömma sig på över natten?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Vilken vardagssak skulle du ta med till en öde ö?",
                AlteredText = "Vilken vardagssak skulle vara mest misstänkt att ta med till en öde ö?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Vilken superkraft vore mest användbar i skolan eller på jobbet?",
                AlteredText = "Vilken superkraft vore enklast att använda utan att bli upptäckt?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Vilken maträtt beskriver din personlighet bäst?",
                AlteredText = "Vilken maträtt skulle vara svårast att förklara på en fin middag?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Vilket djur skulle vara den bästa lagkamraten i en tävling?",
                AlteredText = "Vilket djur skulle vara bäst på att sabotera en tävling?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Vilken app skulle du klara dig sämst utan en vecka?",
                AlteredText = "Vilken app skulle avslöja mest om någon försökte hålla sig anonym?",
                QuestionType = QuestionType.Quiz
            },
            new Question
            {
                Text = "Beskriv en semester som nästan alla i gruppen skulle gilla.",
                AlteredText = "Beskriv en semester där någon lätt skulle kunna smita från gruppen.",
                QuestionType = QuestionType.OtherWords
            },
            new Question
            {
                Text = "Beskriv ett perfekt fredagsmys utan att nämna film eller snacks.",
                AlteredText = "Beskriv ett perfekt alibi för en fredagskväll utan att nämna film eller snacks.",
                QuestionType = QuestionType.OtherWords
            });

        await db.SaveChangesAsync();
    }
}
