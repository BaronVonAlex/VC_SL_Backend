using System.ComponentModel.DataAnnotations;

namespace VC_SL.Models.Dtos;

public class RegisterDto
{
    [Required] [EmailAddress] public string? Email { get; set; }

    [Required] [MinLength(6)] public string? Password { get; set; }

    [Required] public string? Username { get; set; }
}

public class LoginDto
{
    [Required] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
}