using CardGame.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Api.Data
{
    public class CardGameContext : DbContext
    {
        public CardGameContext(DbContextOptions<CardGameContext> options) : base(options)
        {
        }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Score> Scores => Set<Score>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithOne(p => p.Game!)
                .HasForeignKey(p => p.GameId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Score)
                .WithOne(c => c.Player!)
                .HasForeignKey<Score>(c => c.PlayerId);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Cards)
                .WithOne(s => s.Player!)
                .HasForeignKey(s => s.PlayerId);
        }
    }
}