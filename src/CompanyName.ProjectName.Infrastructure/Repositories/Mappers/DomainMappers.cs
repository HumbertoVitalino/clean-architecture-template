using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Repositories.Models;

namespace CompanyName.ProjectName.Infrastructure.Repositories.Mappers;

internal static class DomainMappers
{
    internal static User MapToDomain(this UserModel model) =>
        new(model.Id, new Email(model.Email), model.Name);
}
