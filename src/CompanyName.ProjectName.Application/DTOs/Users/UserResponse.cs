using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.DTOs.Users;

public sealed record UserResponse(Guid Id, string Name, string Email, string Role)
{
    public static UserResponse FromUser(User user) =>
        new(user.Id, user.Name, user.Email.Value, user.Role.ToString());
}
