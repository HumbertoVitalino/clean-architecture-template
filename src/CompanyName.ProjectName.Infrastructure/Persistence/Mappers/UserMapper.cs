using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Persistence.Models;

namespace CompanyName.ProjectName.Infrastructure.Persistence.Mappers;

internal static class UserMapper
{
    internal static User ToDomain(this UserModel model) =>
        User.Reconstitute(model.Id, Email.Create(model.Email), model.Name);

    internal static UserModel ToModel(this User user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email.Value,
            Name = user.Name
        };
}
