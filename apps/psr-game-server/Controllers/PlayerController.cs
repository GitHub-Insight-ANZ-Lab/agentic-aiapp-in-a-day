using Microsoft.AspNetCore.Mvc;
using PsrGameServer.Models;
using PsrGameServer.Services;

namespace PsrGameServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public PlayerController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpPost("register")]
    public ActionResult<RegisterPlayerResponse> RegisterPlayer([FromBody] RegisterPlayerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new RegisterPlayerResponse
            {
                PlayerId = 0,
                Message = "Player name is required"
            });
        }

        var response = _tournamentService.RegisterPlayer(request.Name);
        
        if (response.PlayerId == 0)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{playerId}/status")]
    public ActionResult<TournamentStatusResponse> GetStatus(int playerId)
    {
        if (!_tournamentService.PlayerExists(playerId))
        {
            return NotFound(new { message = "Player not found" });
        }

        var status = _tournamentService.GetTournamentStatus(playerId);
        return Ok(status);
    }

    [HttpPost("submit-answer")]
    public ActionResult<SubmitAnswerResponse> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        if (request.PlayerId <= 0)
        {
            return BadRequest(new SubmitAnswerResponse
            {
                Success = false,
                Message = "Invalid player ID"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            return BadRequest(new SubmitAnswerResponse
            {
                Success = false,
                Message = "Answer is required"
            });
        }

        var response = _tournamentService.SubmitAnswer(request);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{playerId}/results")]
    public ActionResult<List<PlayerRoundResult>> GetPlayerResults(int playerId)
    {
        if (!_tournamentService.PlayerExists(playerId))
        {
            return NotFound(new { message = "Player not found" });
        }

        var results = _tournamentService.GetPlayerResults(playerId);
        return Ok(results);
    }
}