using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;

namespace CompanyName.ProjectName.Application.Interfaces.UseCases;

public interface ICreateUserUseCase
{
    Task<Output> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default);
}
