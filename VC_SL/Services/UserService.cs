using Microsoft.EntityFrameworkCore;
using VC_SL.Data;
using VC_SL.Models.Entities;
using VC_SL.Exceptions;
using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UsernameHistory = UsernameHistoryService.DeserializeHistoryToList(u.UsernameHistory),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        return new UserDto
        {
            Id = user.Id,
            UsernameHistory = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == dto.Id);
        if (existingUser != null)
            throw new ConflictException($"User with ID {dto.Id} already exists");

        var user = new User
        {
            Id = dto.Id,
            UsernameHistory = dto.UsernameHistory ?? "[]",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            UsernameHistory = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        user.UsernameHistory = UsernameHistoryService.AddUsername(
            user.UsernameHistory,
            dto.UsernameHistory
        );
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            UsernameHistory = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}