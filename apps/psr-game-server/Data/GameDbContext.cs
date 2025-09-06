using Microsoft.EntityFrameworkCore;
using PsrGameServer.Models;

namespace PsrGameServer.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<TournamentHistory> TournamentHistories { get; set; }
    public DbSet<PlayerHistory> PlayerHistories { get; set; }
    public DbSet<RoundHistory> RoundHistories { get; set; }
    public DbSet<PlayerRoundHistory> PlayerRoundHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<PlayerHistory>()
            .HasOne(p => p.Tournament)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TournamentHistoryId);

        modelBuilder.Entity<RoundHistory>()
            .HasOne(r => r.Tournament)
            .WithMany(t => t.Rounds)
            .HasForeignKey(r => r.TournamentHistoryId);

        modelBuilder.Entity<PlayerRoundHistory>()
            .HasOne(pr => pr.Round)
            .WithMany(r => r.PlayerResults)
            .HasForeignKey(pr => pr.RoundHistoryId);

        // Configure enums
        modelBuilder.Entity<TournamentHistory>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<RoundHistory>()
            .Property(r => r.ServerMove)
            .HasConversion<string>();

        modelBuilder.Entity<PlayerRoundHistory>()
            .Property(pr => pr.Move)
            .HasConversion<string>();
    }
}