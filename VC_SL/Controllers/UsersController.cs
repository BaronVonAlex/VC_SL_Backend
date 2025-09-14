using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC_SL.Data;
using VC_SL.Models;
using VC_SL.Services;

namespace VC_SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UsernameHistory,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("GetUser")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null) {return NotFound();}

            return Ok(user);
        }
        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrEmpty(updateUserDto.UsernameHistory))
                return BadRequest("Username history is empty");

            var user = _context.Users.FirstOrDefault(x => x.Id == updateUserDto.Id);

            if (user == null) {return NotFound($"User with ID {updateUserDto.Id} not found.");}

            user.UsernameHistory = UsernameHistoryService.AddUsername(user.UsernameHistory, updateUserDto.UsernameHistory);

            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            var historyList = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory);

            return Ok(new
            {
                user.Id,
                historyList,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(CreateUserDto createUserDto)
        {
            createUserDto.Id = createUserDto.Id;
            createUserDto.UsernameHistory = createUserDto.UsernameHistory;
            createUserDto.CreatedAt = createUserDto.CreatedAt;
            createUserDto.UpdatedAt = createUserDto.UpdatedAt;

            _context.SaveChanges();

            return Ok(createUserDto);
        }
    }

}
