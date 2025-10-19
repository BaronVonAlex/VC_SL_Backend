namespace VC_SL.Models.Dtos;

public class LeaderboardDto
{
    public int Rank { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public float Winrate { get; set; }
    public int MonthsPlayed { get; set; }
}

public class LeaderboardRequestDto
{
    public LeaderboardPeriod Period { get; set; } = LeaderboardPeriod.Monthly;
    public LeaderboardCategory Category { get; set; } = LeaderboardCategory.Combined;
    public int? Month { get; set; }
    public int? Year { get; set; }
    public int Limit { get; set; } = 100;
    public int MinimumMonths { get; set; } = 1;
}

public enum LeaderboardPeriod
{
    Monthly,
    Yearly,
    AllTime
}

public enum LeaderboardCategory
{
    Combined,
    BaseAttack,
    BaseDefence,
    Fleet
}