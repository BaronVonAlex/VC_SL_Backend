namespace VC_SL.Models;

public class CreateUserDto
{
    public int Id { get; set; }
    public string UsernameHistory { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}