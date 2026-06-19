using System.Text.Json;
using VC_SL.Data;
using VC_SL.Models.Entities;
using VC_SL.Exceptions;
using VC_SL.Models.Dtos;

namespace VC_SL.Services;

public class UserService(JsonDataStore store) : IUserService
{
    public Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = store.GetUsers().Select(MapToDto).ToList();
        return Task.FromResult(users);
    }

    public Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = store.GetUser(id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        return Task.FromResult(MapToDto(user));
    }

    public Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        if (store.GetUser(dto.Id) != null)
            throw new ConflictException($"User with ID {dto.Id} already exists");

        var usernameHistoryJson = string.IsNullOrEmpty(dto.UsernameHistory)
            ? "[]"
            : JsonSerializer.Serialize(new List<string> { dto.UsernameHistory });

        var user = new User
        {
            Id = dto.Id,
            UsernameHistory = usernameHistoryJson,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        store.AddUser(user);

        return Task.FromResult(MapToDto(user));
    }

    public Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = store.GetUser(id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        user.UsernameHistory = UsernameHistoryService.AddUsername(
            user.UsernameHistory,
            dto.UsernameHistory
        );
        user.UpdatedAt = DateTime.UtcNow;

        store.UpdateUser(user);

        return Task.FromResult(MapToDto(user));
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        UsernameHistory = UsernameHistoryService.DeserializeHistoryToList(user.UsernameHistory),
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}
