using Microsoft.EntityFrameworkCore;
using VC_SL.Data;
using VC_SL.Models.Entities;
using VC_SL.Exceptions;
using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public class WinrateService(ApplicationDbContext context) : IWinrateService
{
    public async Task<List<UpdateWinrateDto>> GetWinrateForUserAsync(int userId, int? year = null)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;

        var userExists = await context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new NotFoundException($"User with ID {userId} not found");

        var winrates = await context.Winrates
            .Where(w => w.UserId == userId && w.Year == targetYear)
            .OrderBy(w => w.Month)
            .Select(w => new UpdateWinrateDto
            {
                UserId = w.UserId,
                Month = w.Month,
                Year = w.Year,
                BaseAttackWinrate = w.BaseAttackWinrate ?? 0,
                BaseDefenceWinrate = w.BaseDefenceWinrate ?? 0,
                FleetWinrate = w.FleetWinrate ?? 0,
                FleetWin = w.FleetWin ?? 0,
                FleetLoss = w.FleetLoss ?? 0,
                FleetDraw = w.FleetDraw ?? 0,
                BaseAttackWin = w.BaseAttackWin ?? 0,
                BaseAttackLoss = w.BaseAttackLoss ?? 0,
                BaseAttackDraw = w.BaseAttackDraw ?? 0,
                BaseDefenceWin = w.BaseDefenceWin ?? 0,
                BaseDefenceLoss = w.BaseDefenceLoss ?? 0,
                BaseDefenceDraw = w.BaseDefenceDraw ?? 0
            })
            .ToListAsync();

        return winrates;
    }

    public async Task<UpdateWinrateDto> UpsertWinrateAsync(UpdateWinrateDto dto)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            throw new NotFoundException($"User with ID {dto.UserId} not found. Please create the user first.");

        var existingWinrate = await context.Winrates
            .FirstOrDefaultAsync(w =>
                w.UserId == dto.UserId &&
                w.Year == dto.Year &&
                w.Month == dto.Month);

        if (existingWinrate != null)
        {
            existingWinrate.BaseAttackWinrate = dto.BaseAttackWinrate;
            existingWinrate.BaseDefenceWinrate = dto.BaseDefenceWinrate;
            existingWinrate.FleetWinrate = dto.FleetWinrate;
            existingWinrate.FleetWin = dto.FleetWin ?? 0;
            existingWinrate.FleetLoss = dto.FleetLoss ?? 0;
            existingWinrate.FleetDraw = dto.FleetDraw ?? 0;
            existingWinrate.BaseAttackWin = dto.BaseAttackWin ?? 0;
            existingWinrate.BaseAttackLoss = dto.BaseAttackLoss ?? 0;
            existingWinrate.BaseAttackDraw = dto.BaseAttackDraw ?? 0;
            existingWinrate.BaseDefenceWin = dto.BaseDefenceWin ?? 0;
            existingWinrate.BaseDefenceLoss = dto.BaseDefenceLoss ?? 0;
            existingWinrate.BaseDefenceDraw = dto.BaseDefenceDraw ?? 0;
        }
        else
        {
            existingWinrate = new Winrate
            {
                UserId = dto.UserId,
                Month = dto.Month,
                Year = dto.Year,
                BaseAttackWinrate = dto.BaseAttackWinrate ?? 0,
                BaseDefenceWinrate = dto.BaseDefenceWinrate ?? 0,
                FleetWinrate = dto.FleetWinrate ?? 0,
                FleetWin = dto.FleetWin ?? 0,
                FleetLoss = dto.FleetLoss ?? 0,
                FleetDraw = dto.FleetDraw ?? 0,
                BaseAttackWin = dto.BaseAttackWin ?? 0,
                BaseAttackLoss = dto.BaseAttackLoss ?? 0,
                BaseAttackDraw = dto.BaseAttackDraw ?? 0,
                BaseDefenceWin = dto.BaseDefenceWin ?? 0,
                BaseDefenceLoss = dto.BaseDefenceLoss ?? 0,
                BaseDefenceDraw = dto.BaseDefenceDraw ?? 0
            };
            context.Winrates.Add(existingWinrate);
        }

        await context.SaveChangesAsync();

        return new UpdateWinrateDto
        {
            UserId = existingWinrate.UserId,
            Month = existingWinrate.Month,
            Year = existingWinrate.Year,
            BaseAttackWinrate = existingWinrate.BaseAttackWinrate,
            BaseDefenceWinrate = existingWinrate.BaseDefenceWinrate,
            FleetWinrate = existingWinrate.FleetWinrate,
            FleetWin = existingWinrate.FleetWin,
            FleetLoss = existingWinrate.FleetLoss,
            FleetDraw = existingWinrate.FleetDraw,
            BaseAttackWin = existingWinrate.BaseAttackWin,
            BaseAttackLoss = existingWinrate.BaseAttackLoss,
            BaseAttackDraw = existingWinrate.BaseAttackDraw,
            BaseDefenceWin = existingWinrate.BaseDefenceWin,
            BaseDefenceLoss = existingWinrate.BaseDefenceLoss,
            BaseDefenceDraw = existingWinrate.BaseDefenceDraw
        };
    }
}