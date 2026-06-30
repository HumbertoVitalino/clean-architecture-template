using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Repositories.Models;

namespace CompanyName.ProjectName.Infrastructure.Repositories.Mappers;

internal static class UserModelMapper
{
    internal static UserModel ToModel(this User user) =>
        new(user.Id, user.Email.Value, user.Name);
}
