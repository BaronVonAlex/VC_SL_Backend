using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC_SL.Data;

namespace VC_SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WinrateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WinrateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetWinrates()
        {
            var winrates = _context.Winrates
                .Select(u => new
                {
                    u.Id,
                    u.UserId,
                    u.Month,
                    u.Year,
                    u.BaseAttackWinrate,
                    u.BaseDefenceWinrate,
                    u.FleetWinrate
                })
                .ToList();
            return Ok(winrates);
        }
    }
}
