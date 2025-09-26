using Microsoft.AspNetCore.Mvc;
using VC_SL.Data;
using VC_SL.Models;
using VC_SL.Models.Entities;

namespace VC_SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WinrateController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("GetWinrateForUser")]
        public IActionResult GetWinrateForUser(int userId, int? year = null)
        {
            var targetYear = year ?? DateTime.Now.Year;

            var userExists = context.Users.Any(u => u.Id == userId);
            if (!userExists)
                return NotFound($"user {userId} not found");

            var winratesQuery = context.Winrates
                .Where(w => w.UserId == userId && w.Year == targetYear);

            var winrates = winratesQuery
                .OrderBy(w => w.Month)
                .Select(w => new
                {
                    w.Month,
                    w.Year,
                    w.BaseAttackWinrate,
                    w.BaseDefenceWinrate,
                    w.FleetWinrate
                })
                .ToList();

            return Ok(winrates);
        }

        [HttpPost("UpdateWinrate")]
        public IActionResult UpdateWinrate([FromBody] UpdateWinrateDto updateWinrateDto)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == updateWinrateDto.UserId);

            if (user == null)
            {
                user = new User
                {
                    Id = updateWinrateDto.UserId,
                    UsernameHistory = "[]"
                };
                context.Users.Add(user);
                context.SaveChanges();
            }

            var existingWinrate = context.Winrates
                .FirstOrDefault(w =>
                    w.UserId == updateWinrateDto.UserId &&
                    w.Year == updateWinrateDto.Year &&
                    w.Month == updateWinrateDto.Month);

            if (existingWinrate != null)
            {
                existingWinrate.BaseAttackWinrate = updateWinrateDto.BaseAttackWinrate;
                existingWinrate.BaseDefenceWinrate = updateWinrateDto.BaseDefenceWinrate;
                existingWinrate.FleetWinrate = updateWinrateDto.FleetWinrate;
            }
            else
            {
                var newWinrate = new Winrate
                {
                    UserId = updateWinrateDto.UserId,
                    Month = updateWinrateDto.Month,
                    Year = updateWinrateDto.Year,
                    BaseAttackWinrate = updateWinrateDto.BaseAttackWinrate,
                    BaseDefenceWinrate = updateWinrateDto.BaseDefenceWinrate,
                    FleetWinrate = updateWinrateDto.FleetWinrate
                };

                context.Winrates.Add(newWinrate);
            }

            context.SaveChanges();

            return Ok(updateWinrateDto);
        }
    }
}
