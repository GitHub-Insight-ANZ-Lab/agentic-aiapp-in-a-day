using Microsoft.EntityFrameworkCore;
using PsrGameServer.Data;
using PsrGameServer.Models;

namespace PsrGameServer.Services;

public class TournamentHistoryService : ITournamentHistoryService
{
    private readonly GameDbContext _context;
    private readonly ILogger<TournamentHistoryService> _logger;

    public TournamentHistoryService(GameDbContext context, ILogger<TournamentHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TournamentHistory>> GetAllTournamentsAsync()
    {
        try
        {
            return await _context.TournamentHistories
                .Include(t => t.Players)
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.PlayerResults)
                .OrderByDescending(t => t.StartedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tournaments");
            return new List<TournamentHistory>();
        }
    }

    public async Task<TournamentHistory?> GetTournamentByIdAsync(int id)
    {
        try
        {
            return await _context.TournamentHistories
                .Include(t => t.Players)
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.PlayerResults)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tournament {TournamentId}", id);
            return null;
        }
    }

    public async Task<int> SaveTournamentAsync(Tournament tournament)
    {
        try
        {
            var tournamentHistory = new TournamentHistory
            {
                StartedAt = tournament.StartedAt ?? DateTime.UtcNow,
                EndedAt = tournament.EndedAt,
                Status = tournament.Status,
                TotalPlayers = tournament.Players.Count,
                TotalRounds = tournament.Rounds.Count
            };

            // Find winner
            if (tournament.Status == TournamentStatus.Completed && tournament.Players.Any())
            {
                var winner = tournament.Players.OrderByDescending(p => p.TotalScore).First();
                tournamentHistory.WinnerName = winner.Name;
                tournamentHistory.WinnerScore = winner.TotalScore;
            }

            // Save players
            foreach (var player in tournament.Players)
            {
                tournamentHistory.Players.Add(new PlayerHistory
                {
                    PlayerId = player.Id,
                    Name = player.Name,
                    TotalScore = player.TotalScore,
                    RegisteredAt = player.RegisteredAt
                });
            }

            // Save rounds
            foreach (var round in tournament.Rounds)
            {
                var roundHistory = new RoundHistory
                {
                    RoundNumber = round.RoundNumber,
                    Question = round.Question,
                    CorrectAnswer = round.CorrectAnswer,
                    ServerMove = round.ServerMove,
                    StartedAt = round.StartedAt,
                    EndedAt = round.EndedAt
                };

                // Save player results for this round
                foreach (var result in round.PlayerResults)
                {
                    roundHistory.PlayerResults.Add(new PlayerRoundHistory
                    {
                        PlayerId = result.PlayerId,
                        Answer = result.Answer,
                        Move = result.Move,
                        AnswerCorrect = result.AnswerCorrect,
                        Score = result.Score,
                        SubmittedAt = result.SubmittedAt
                    });
                }

                tournamentHistory.Rounds.Add(roundHistory);
            }

            _context.TournamentHistories.Add(tournamentHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully saved tournament {TournamentId} to history", tournamentHistory.Id);
            return tournamentHistory.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving tournament to history");
            return -1;
        }
    }

    public async Task<bool> DeleteTournamentAsync(int id)
    {
        try
        {
            var tournament = await _context.TournamentHistories.FindAsync(id);
            if (tournament != null)
            {
                _context.TournamentHistories.Remove(tournament);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted tournament {TournamentId} from history", id);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tournament {TournamentId}", id);
            return false;
        }
    }

    public async Task<List<TournamentHistory>> GetRecentTournamentsAsync(int count = 10)
    {
        try
        {
            return await _context.TournamentHistories
                .Include(t => t.Players)
                .OrderByDescending(t => t.StartedAt)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent tournaments");
            return new List<TournamentHistory>();
        }
    }
}