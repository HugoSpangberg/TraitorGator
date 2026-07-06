using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;

namespace TraitorGator.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<GameRound> GameRounds { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameRound>()
                .HasIndex(gr => gr.GameCode)
                .IsUnique();

            modelBuilder.Entity<GameRound>()
                .Property(gr => gr.GameCode)
                .HasMaxLength(8);

            modelBuilder.Entity<GameRound>()
                .Property(gr => gr.Winner)
                .HasMaxLength(16);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.GameRound)
                .WithMany(gr => gr.Players)
                .HasForeignKey(p => p.GameRoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .Property(p => p.Username)
                .HasMaxLength(32);

            modelBuilder.Entity<Player>()
                .Property(p => p.AccessToken)
                .HasMaxLength(128);

            modelBuilder.Entity<Player>()
                .HasIndex(p => new { p.GameRoundId, p.Username })
                .IsUnique();

            modelBuilder.Entity<GameRound>()
                .HasOne(gr => gr.Traitor)
                .WithMany()
                .HasForeignKey(gr => gr.TraitorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.GameRound)
                .WithMany(gr => gr.Answers)
                .HasForeignKey(a => a.GameRoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Player)
                .WithMany()
                .HasForeignKey(a => a.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasIndex(a => new { a.GameRoundId, a.RoundNumber, a.PlayerId })
                .IsUnique();

            modelBuilder.Entity<Answer>()
                .Property(a => a.AnswerText)
                .HasMaxLength(500);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.GameRound)
                .WithMany(gr => gr.Votes)
                .HasForeignKey(v => v.GameRoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.VoterPlayer)
                .WithMany()
                .HasForeignKey(v => v.VoterPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.SuspectPlayer)
                .WithMany()
                .HasForeignKey(v => v.SuspectPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.GameRoundId, v.RoundNumber, v.VoterPlayerId })
                .IsUnique();
        }
    }
}
