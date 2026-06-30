using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Repositories.Models;

namespace CompanyName.ProjectName.Infrastructure.Repositories.Mappers;

internal static class ModelMappers
{
    internal static UserModel MapToModel(this User user) =>
        new(user.Id, user.Email.Value, user.Name, user.Role.ToString());
}
