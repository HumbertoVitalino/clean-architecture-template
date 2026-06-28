using CompanyName.ProjectName.Application.Abstractions;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(Email email, CancellationToken cancellationToken = default);
}
