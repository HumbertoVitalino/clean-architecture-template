using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Repositories.Models;

namespace CompanyName.ProjectName.Infrastructure.Repositories.Mappers;

internal static class UserDomainMapper
{
    internal static User ToDomain(this UserModel model) =>
        User.Reconstitute(model.Id, Email.Create(model.Email), model.Name);
}
