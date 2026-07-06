using BackendWinder.DTOs;
using BackendWinder.Models.Outside;

namespace BackendWinder.Mappers;

/// <summary>
/// Методы расширения для маппинга User → UserDto
/// </summary>
public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        if (user == null) return null!;

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public static List<UserDto> ToDtoList(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToDto()).ToList();
    }
}