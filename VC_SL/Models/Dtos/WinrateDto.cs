namespace VC_SL.Models.Dtos;

public class WinrateDto
{
    public int UserId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public float? BaseAttackWinrate { get; set; }
    public float? BaseDefenceWinrate { get; set; }
    public float? FleetWinrate { get; set; }
}