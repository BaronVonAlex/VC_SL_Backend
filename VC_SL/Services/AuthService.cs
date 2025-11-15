using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using VC_SL.Models.Dtos;
using VC_SL.Models.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace VC_SL.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration
) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new Exception("User with this email already exists");

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Username,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed with errors: {errors}");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Username = user.UserName!
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Invalid username or password");

        var isValidPassword = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValidPassword)
            throw new Exception("Invalid username or password");

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Username = user.UserName!
        };
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secret = jwtSettings["Secret"];
        if (string.IsNullOrEmpty(secret))
            throw new Exception("JWT Secret is not configured in appsettings");

        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var expiryMinutes = 60;
        var expiryValue = jwtSettings["ExpiryMinutes"];
        if (!string.IsNullOrEmpty(expiryValue) && int.TryParse(expiryValue, out var parsed)) expiryMinutes = parsed;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}