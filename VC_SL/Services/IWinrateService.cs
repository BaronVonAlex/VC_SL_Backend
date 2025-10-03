using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public interface IWinrateService
{
    Task<List<UpdateWinrateDto>> GetWinrateForUserAsync(int userId, int? year = null);
    Task<UpdateWinrateDto> UpsertWinrateAsync(UpdateWinrateDto dto);
}