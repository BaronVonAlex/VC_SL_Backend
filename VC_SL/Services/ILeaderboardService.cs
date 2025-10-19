using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public interface ILeaderboardService
{
    Task<List<LeaderboardDto>> GetLeaderboardAsync(LeaderboardRequestDto request);
}