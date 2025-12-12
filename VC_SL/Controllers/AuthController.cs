using Microsoft.AspNetCore.Mvc;
using VC_SL.Models.Dtos;
using VC_SL.Services;

namespace VC_SL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await authService.RegisterAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }
}