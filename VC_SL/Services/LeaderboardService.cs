using VC_SL.Data;
using VC_SL.Models.Dtos;
using VC_SL.Exceptions;

namespace VC_SL.Services;

public class LeaderboardService(JsonDataStore store, IConfiguration config) : ILeaderboardService
{
    private float BayesianC => config.GetValue<float>("Leaderboard:BayesianC", 3f);

    public Task<List<LeaderboardDto>> GetLeaderboardAsync(LeaderboardRequestDto request)
    {
        ValidateRequest(request);

        IEnumerable<Models.Entities.Winrate> winrates = store.GetWinrates();

        winrates = request.Period switch
        {
            LeaderboardPeriod.Monthly => winrates.Where(w => w.Month == request.Month && w.Year == request.Year),
            LeaderboardPeriod.Yearly  => winrates.Where(w => w.Year == request.Year),
            LeaderboardPeriod.AllTime => winrates,
            _ => throw new LeaderboardValidationException(new Dictionary<string, List<string>>
            {
                { "Period", ["Invalid period specified"] }
            })
        };

        var userStats = winrates
            .GroupBy(w => w.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                MonthsPlayed = g.Count(),
                AvgBaseAttack   = g.Average(w => w.BaseAttackWinrate   ?? 0f),
                AvgBaseDefence  = g.Average(w => w.BaseDefenceWinrate  ?? 0f),
                AvgFleet        = g.Average(w => w.FleetWinrate        ?? 0f),
                CombinedWinrate = (g.Average(w => w.BaseAttackWinrate  ?? 0f) +
                                   g.Average(w => w.BaseDefenceWinrate ?? 0f) +
                                   g.Average(w => w.FleetWinrate       ?? 0f)) / 3f
            })
            .ToList();

        if (request.Period != LeaderboardPeriod.Monthly)
        {
            userStats = userStats.Where(s => s.MonthsPlayed >= request.MinimumMonths).ToList();
        }

        if (userStats.Count == 0)
            return Task.FromResult(new List<LeaderboardDto>());

        var globalAvg = (float)(request.Category switch
        {
            LeaderboardCategory.BaseAttack  => userStats.Average(s => s.AvgBaseAttack),
            LeaderboardCategory.BaseDefence => userStats.Average(s => s.AvgBaseDefence),
            LeaderboardCategory.Fleet       => userStats.Average(s => s.AvgFleet),
            _                               => userStats.Average(s => s.CombinedWinrate)
        });

        var c = BayesianC;

        var userIds = userStats.Select(s => s.UserId).ToHashSet();
        var users = store.GetUsers()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionary(u => u.Id, u => u);

        var leaderboard = userStats.Select(s =>
        {
            var rawWinrate = request.Category switch
            {
                LeaderboardCategory.BaseAttack  => s.AvgBaseAttack,
                LeaderboardCategory.BaseDefence => s.AvgBaseDefence,
                LeaderboardCategory.Fleet       => s.AvgFleet,
                _                               => s.CombinedWinrate
            };

            var adjustedWinrate = (c * globalAvg + s.MonthsPlayed * rawWinrate) / (c + s.MonthsPlayed);

            return new LeaderboardDto
            {
                UserId        = s.UserId,
                Username      = GetLatestUsername(users.GetValueOrDefault(s.UserId)),
                Winrate       = (float)Math.Round(adjustedWinrate, 2),
                RawWinrate    = (float)Math.Round(rawWinrate, 2),
                MonthsPlayed  = s.MonthsPlayed
            };
        })
        .OrderByDescending(l => l.Winrate)
        .Take(request.Limit)
        .ToList();

        for (var i = 0; i < leaderboard.Count; i++)
            leaderboard[i].Rank = i + 1;

        return Task.FromResult(leaderboard);
    }

    private static void ValidateRequest(LeaderboardRequestDto request)
    {
        var errors = new Dictionary<string, List<string>>();

        if (request.Period == LeaderboardPeriod.Monthly)
        {
            switch (request.Month)
            {
                case null:
                    errors.Add("Month", ["Month is required for monthly leaderboard"]);
                    break;
                case < 1:
                case > 12:
                    errors.Add("Month", ["Month must be between 1 and 12"]);
                    break;
            }

            if (!request.Year.HasValue)
                errors.Add("Year", ["Year is required for monthly leaderboard"]);
        }

        if (request is { Period: LeaderboardPeriod.Yearly, Year: null })
            errors.Add("Year", ["Year is required for yearly leaderboard"]);

        if (request.Limit is < 1 or > 1000)
            errors.Add("Limit", ["Limit must be between 1 and 1000"]);

        if (errors.Count != 0)
            throw new LeaderboardValidationException(errors);
    }

    private static string GetLatestUsername(Models.Entities.User? user)
    {
        if (user == null) return "Unknown";
        var history = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory);
        return history.LastOrDefault() ?? "Unknown";
    }
}
