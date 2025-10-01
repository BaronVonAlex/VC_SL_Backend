using System.ComponentModel.DataAnnotations;

namespace VC_SL.Models.Dtos;

public class CreateUserDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }

    public string? UsernameHistory { get; set; }
}