using VC_SL.Data;
using VC_SL.Models.Entities;
using VC_SL.Exceptions;
using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public class WinrateService(JsonDataStore store) : IWinrateService
{
    public Task<List<UpdateWinrateDto>> GetWinrateForUserAsync(int userId, int? year = null)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;

        if (store.GetUser(userId) == null)
            throw new NotFoundException($"User with ID {userId} not found");

        var winrates = store.GetWinrates()
            .Where(w => w.UserId == userId && w.Year == targetYear)
            .OrderBy(w => w.Month)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult(winrates);
    }

    public Task<UpdateWinrateDto> UpsertWinrateAsync(UpdateWinrateDto dto)
    {
        if (store.GetUser(dto.UserId) == null)
            throw new NotFoundException($"User with ID {dto.UserId} not found. Please create the user first.");

        var existingWinrate = store.GetWinrate(dto.UserId, dto.Year, dto.Month);

        if (existingWinrate != null)
        {
            existingWinrate.BaseAttackWinrate = dto.BaseAttackWinrate;
            existingWinrate.BaseDefenceWinrate = dto.BaseDefenceWinrate;
            existingWinrate.FleetWinrate = dto.FleetWinrate;
            existingWinrate.BaseAttackWin = dto.BaseAttackWin;
            existingWinrate.BaseAttackLoss = dto.BaseAttackLoss;
            existingWinrate.BaseAttackDraw = dto.BaseAttackDraw;
            existingWinrate.BaseDefenceWin = dto.BaseDefenceWin;
            existingWinrate.BaseDefenceLoss = dto.BaseDefenceLoss;
            existingWinrate.BaseDefenceDraw = dto.BaseDefenceDraw;
            existingWinrate.FleetWin = dto.FleetWin;
            existingWinrate.FleetLoss = dto.FleetLoss;
            existingWinrate.FleetDraw = dto.FleetDraw;

            store.UpdateWinrate(existingWinrate);

            return Task.FromResult(MapToDto(existingWinrate));
        }

        var winrate = new Winrate
        {
            UserId = dto.UserId,
            Month = dto.Month,
            Year = dto.Year,
            BaseAttackWinrate = dto.BaseAttackWinrate,
            BaseDefenceWinrate = dto.BaseDefenceWinrate,
            FleetWinrate = dto.FleetWinrate,
            BaseAttackWin = dto.BaseAttackWin,
            BaseAttackLoss = dto.BaseAttackLoss,
            BaseAttackDraw = dto.BaseAttackDraw,
            BaseDefenceWin = dto.BaseDefenceWin,
            BaseDefenceLoss = dto.BaseDefenceLoss,
            BaseDefenceDraw = dto.BaseDefenceDraw,
            FleetWin = dto.FleetWin,
            FleetLoss = dto.FleetLoss,
            FleetDraw = dto.FleetDraw
        };

        var added = store.AddWinrate(winrate);

        return Task.FromResult(MapToDto(added));
    }

    private static UpdateWinrateDto MapToDto(Winrate w) => new()
    {
        UserId = w.UserId,
        Month = w.Month,
        Year = w.Year,
        BaseAttackWinrate = w.BaseAttackWinrate,
        BaseDefenceWinrate = w.BaseDefenceWinrate,
        FleetWinrate = w.FleetWinrate,
        BaseAttackWin = w.BaseAttackWin,
        BaseAttackLoss = w.BaseAttackLoss,
        BaseAttackDraw = w.BaseAttackDraw,
        BaseDefenceWin = w.BaseDefenceWin,
        BaseDefenceLoss = w.BaseDefenceLoss,
        BaseDefenceDraw = w.BaseDefenceDraw,
        FleetWin = w.FleetWin,
        FleetLoss = w.FleetLoss,
        FleetDraw = w.FleetDraw
    };
}
