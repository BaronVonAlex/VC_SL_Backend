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
            existingWinrate.BaseAttackWin = dto.BaseAttackWin;
            existingWinrate.BaseAttackLoss = dto.BaseAttackLoss;
            existingWinrate.BaseAttackDraw = dto.BaseAttackDraw;
            existingWinrate.BaseDefenceWin = dto.BaseDefenceWin;
            existingWinrate.BaseDefenceLoss = dto.BaseDefenceLoss;
            existingWinrate.BaseDefenceDraw = dto.BaseDefenceDraw;
            existingWinrate.FleetWin = dto.FleetWin;
            existingWinrate.FleetLoss = dto.FleetLoss;
            existingWinrate.FleetDraw = dto.FleetDraw;
        }
        else
        {
            existingWinrate = new Winrate
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
            BaseAttackWin = existingWinrate.BaseAttackWin,
            BaseAttackLoss = existingWinrate.BaseAttackLoss,
            BaseAttackDraw = existingWinrate.BaseAttackDraw,
            BaseDefenceWin = existingWinrate.BaseDefenceWin,
            BaseDefenceLoss = existingWinrate.BaseDefenceLoss,
            BaseDefenceDraw = existingWinrate.BaseDefenceDraw,
            FleetWin = existingWinrate.FleetWin,
            FleetLoss = existingWinrate.FleetLoss,
            FleetDraw = existingWinrate.FleetDraw
        };
    }
}
