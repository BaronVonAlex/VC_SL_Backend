using Microsoft.AspNetCore.Mvc;
using VC_SL.Models.Dtos;
using VC_SL.Services;
using VC_SL.Exceptions;

namespace VC_SL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WinrateController(IWinrateService winrateService) : ControllerBase
{
    [HttpGet("GetWinrateForUser")]
    [ProducesResponseType(typeof(List<WinrateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWinrateForUser(int userId, int? year = null)
    {
        try
        {
            var winrates = await winrateService.GetWinrateForUserAsync(userId, year);
            return Ok(winrates);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("UpdateWinrate")]
    [ProducesResponseType(typeof(WinrateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWinrate([FromBody] UpdateWinrateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await winrateService.UpsertWinrateAsync(dto);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }
}