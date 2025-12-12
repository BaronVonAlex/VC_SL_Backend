namespace VC_SL.Models.Dtos;

public class WinrateDto
{
    public int UserId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public float? BaseAttackWinrate { get; set; }
    public float? BaseDefenceWinrate { get; set; }
    public float? FleetWinrate { get; set; }
    public int? BaseAttackWin { get; set; }
    public int? BaseAttackLoss { get; set; }
    public int? BaseAttackDraw { get; set; }
    public int? BaseDefenceWin { get; set; }
    public int? BaseDefenceLoss { get; set; }
    public int? BaseDefenceDraw { get; set; }
    public int? FleetWin { get; set; }
    public int? FleetLoss { get; set; }
    public int? FleetDraw { get; set; }
}