namespace VC_SL.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public List<string> UsernameHistory { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}