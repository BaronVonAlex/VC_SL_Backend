using Microsoft.EntityFrameworkCore;
using VC_SL.Data;
using VC_SL.Models.Dtos;
using VC_SL.Exceptions;

namespace VC_SL.Services;

public class LeaderboardService(ApplicationDbContext context) : ILeaderboardService
{
    public async Task<List<LeaderboardDto>> GetLeaderboardAsync(LeaderboardRequestDto request)
    {
        ValidateRequest(request);

        var query = context.Winrates.AsQueryable();

        query = request.Period switch
        {
            LeaderboardPeriod.Monthly => query.Where(w => w.Month == request.Month && w.Year == request.Year),
            LeaderboardPeriod.Yearly => query.Where(w => w.Year == request.Year),
            LeaderboardPeriod.AllTime => query,
            _ => throw new LeaderboardValidationException(new Dictionary<string, List<string>>
            {
                { "Period", ["Invalid period specified"] }
            })
        };

        var userStats = await query
            .GroupBy(w => w.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                MonthsPlayed = g.Count(),
                AvgBaseAttack = g.Average(w => w.BaseAttackWinrate ?? 0),
                AvgBaseDefence = g.Average(w => w.BaseDefenceWinrate ?? 0),
                AvgFleet = g.Average(w => w.FleetWinrate ?? 0),
                CombinedWinrate = (g.Average(w => w.BaseAttackWinrate ?? 0) +
                                  g.Average(w => w.BaseDefenceWinrate ?? 0) +
                                  g.Average(w => w.FleetWinrate ?? 0)) / 3
            })
            .ToListAsync();

        if (request.Period != LeaderboardPeriod.Monthly)
        {
            userStats = userStats.Where(s => s.MonthsPlayed >= request.MinimumMonths).ToList();
        }

        var userIds = userStats.Select(s => s.UserId).ToList();
        var users = await context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u);

        var leaderboard = userStats.Select(s =>
        {
            var winrate = request.Category switch
            {
                LeaderboardCategory.BaseAttack => s.AvgBaseAttack,
                LeaderboardCategory.BaseDefence => s.AvgBaseDefence,
                LeaderboardCategory.Fleet => s.AvgFleet,
                _ => s.CombinedWinrate
            };

            return new LeaderboardDto
            {
                UserId = s.UserId,
                Username = GetLatestUsername(users.GetValueOrDefault(s.UserId)),
                Winrate = (float)Math.Round(winrate, 2),
                MonthsPlayed = s.MonthsPlayed
            };
        })
        .OrderByDescending(l => l.Winrate)
        .Take(request.Limit)
        .ToList();

        for (var i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }

        return leaderboard;
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
        {
            errors.Add("Year", ["Year is required for yearly leaderboard"]);
        }

        if (request.Limit is < 1 or > 1000)
        {
            errors.Add("Limit", ["Limit must be between 1 and 1000"]);
        }

        if (errors.Count != 0)
        {
            throw new LeaderboardValidationException(errors);
        }
    }

    private static string GetLatestUsername(Models.Entities.User? user)
    {
        if (user == null) return "Unknown";

        var history = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory);
        return history.LastOrDefault() ?? "Unknown";
    }
}