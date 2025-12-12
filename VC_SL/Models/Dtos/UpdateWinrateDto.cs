using System.ComponentModel.DataAnnotations;

namespace VC_SL.Models.Dtos;

public class UpdateWinrateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
    public int UserId { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
    public int Year { get; set; }

    [Range(0, 100, ErrorMessage = "Winrate must be between 0 and 100")]
    public float? BaseAttackWinrate { get; set; }

    [Range(0, 100, ErrorMessage = "Winrate must be between 0 and 100")]
    public float? BaseDefenceWinrate { get; set; }

    [Range(0, 100, ErrorMessage = "Winrate must be between 0 and 100")]
    public float? FleetWinrate { get; set; }
}