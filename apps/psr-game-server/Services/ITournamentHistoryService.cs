using PsrGameServer.Models;

namespace PsrGameServer.Services;

public interface ITournamentHistoryService
{
    Task<List<TournamentHistory>> GetAllTournamentsAsync();
    Task<TournamentHistory?> GetTournamentByIdAsync(int id);
    Task<int> SaveTournamentAsync(Tournament tournament);
    Task<bool> DeleteTournamentAsync(int id);
    Task<List<TournamentHistory>> GetRecentTournamentsAsync(int count = 10);
}