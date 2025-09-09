using System.ComponentModel.DataAnnotations.Schema;

namespace VC_SL.Models.Entities
{
    public class Winrate
    {
        public int Id { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("baseAttackWinrate")]
        public float? BaseAttackWinrate { get; set; }

        [Column("baseDefenceWinrate")]
        public float? BaseDefenceWinrate { get; set; }

        [Column("fleetWinrate")]
        public float? FleetWinrate { get; set; }
        public User User { get; set; } = null!;
    }
}
