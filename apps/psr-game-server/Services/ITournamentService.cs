using PsrGameServer.Models;

namespace PsrGameServer.Services;

public interface ITournamentService
{
    // Player registration
    RegisterPlayerResponse RegisterPlayer(string playerName);
    bool UnregisterPlayer(int playerId);
    bool PlayerExists(int playerId);
    
    // Tournament management
    Tournament GetTournament();
    bool StartTournament();
    bool EndTournament();
    bool ResetTournament();
    
    // Round management
    bool StartRound(int roundNumber, string? question = null, string? correctAnswer = null);
    bool EndRound(int roundNumber);
    bool ResetCurrentRound();
    Round? GetCurrentRound();
    
    // Player actions
    TournamentStatusResponse GetTournamentStatus(int playerId);
    SubmitAnswerResponse SubmitAnswer(SubmitAnswerRequest request);
    
    // Results and leaderboard
    List<LeaderboardEntry> GetLeaderboard(bool hideForLastTwoRounds = false);
    List<RoundResultEntry> GetRoundResults(int roundNumber);
    List<PlayerRoundResult> GetPlayerResults(int playerId);
}