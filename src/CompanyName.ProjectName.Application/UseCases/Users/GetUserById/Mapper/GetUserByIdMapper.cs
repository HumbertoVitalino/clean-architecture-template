using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Mapper;

internal static class GetUserByIdMapper
{
    internal static UserResponse MapToOutput(this User user) =>
        new(user.Id, user.Name, user.Email.Value, user.Role.ToString());
}
