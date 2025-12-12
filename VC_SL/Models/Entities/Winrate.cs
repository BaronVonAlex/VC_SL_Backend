using System.ComponentModel.DataAnnotations.Schema;

namespace VC_SL.Models.Entities;

public class Winrate
{
    public int Id { get; set; }

    [Column("userId")] public int UserId { get; set; }

    [Column("month")] public int Month { get; set; }

    [Column("year")] public int Year { get; set; }

    [Column("baseAttackWinrate")] public float? BaseAttackWinrate { get; set; }

    [Column("baseDefenceWinrate")] public float? BaseDefenceWinrate { get; set; }

    [Column("fleetWinrate")] public float? FleetWinrate { get; set; }

    [Column("baseAttackWin")] public int? BaseAttackWin { get; set; }
    [Column("baseAttackLoss")] public int? BaseAttackLoss { get; set; }
    [Column("baseAttackDraw")] public int? BaseAttackDraw { get; set; }

    [Column("baseDefenceWin")] public int? BaseDefenceWin { get; set; }
    [Column("baseDefenceLoss")] public int? BaseDefenceLoss { get; set; }
    [Column("baseDefenceDraw")] public int? BaseDefenceDraw { get; set; }

    [Column("fleetWin")] public int? FleetWin { get; set; }
    [Column("fleetLoss")] public int? FleetLoss { get; set; }
    [Column("fleetDraw")] public int? FleetDraw { get; set; }
}