using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Mapper;

internal static class CreateUserMapper
{
    internal static UserResponse MapToOutput(this User user) =>
        new(user.Id, user.Name, user.Email.Value, user.Role.ToString());
}
