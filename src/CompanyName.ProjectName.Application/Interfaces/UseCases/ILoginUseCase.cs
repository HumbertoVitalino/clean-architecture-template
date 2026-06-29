using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

namespace CompanyName.ProjectName.Application.Interfaces.UseCases;

public interface ILoginUseCase
{
    Task<Output> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default);
}
