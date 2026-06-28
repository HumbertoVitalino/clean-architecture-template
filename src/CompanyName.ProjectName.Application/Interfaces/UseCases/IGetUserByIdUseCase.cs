using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Boundaries;

namespace CompanyName.ProjectName.Application.Interfaces.UseCases;

public interface IGetUserByIdUseCase
{
    Task<Output> ExecuteAsync(GetUserByIdInput input, CancellationToken cancellationToken = default);
}
