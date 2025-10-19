using Microsoft.AspNetCore.Mvc;
using VC_SL.Models.Dtos;
using VC_SL.Services;
using VC_SL.Exceptions;

namespace VC_SL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase
{
    /// <summary>
    /// Get leaderboard with flexible filtering
    /// </summary>
    /// <param name="period">Monthly, Yearly, or AllTime</param>
    /// <param name="category">Combined, BaseAttack, BaseDefence, or Fleet</param>
    /// <param name="month">Month (1-12) - Required for Monthly period</param>
    /// <param name="year">Year - Required for Monthly and Yearly periods</param>
    /// <param name="limit">Number of top players to return (1-1000, default 100)</param>
    /// <param name="minimumMonths">Minimum months played to qualify (default 1)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<LeaderboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] LeaderboardPeriod period = LeaderboardPeriod.Monthly,
        [FromQuery] LeaderboardCategory category = LeaderboardCategory.Combined,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        [FromQuery] int limit = 100,
        [FromQuery] int minimumMonths = 1)
    {
        try
        {
            var request = new LeaderboardRequestDto
            {
                Period = period,
                Category = category,
                Month = month,
                Year = year,
                Limit = limit,
                MinimumMonths = minimumMonths
            };

            var leaderboard = await leaderboardService.GetLeaderboardAsync(request);
            return Ok(leaderboard);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }
}