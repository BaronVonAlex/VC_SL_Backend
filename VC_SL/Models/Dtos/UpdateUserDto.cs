using System.ComponentModel.DataAnnotations;

namespace VC_SL.Models.Dtos;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 100 characters")]
    public string UsernameHistory { get; set; } = null!;
}