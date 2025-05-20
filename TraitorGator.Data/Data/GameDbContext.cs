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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.GameRound)
                .WithMany(gr => gr.Players)
                .HasForeignKey(p => p.GameRoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameRound>()
                .HasOne(gr => gr.Traitor)
                .WithMany()
                .HasForeignKey(gr => gr.TraitorId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
