using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Mapper;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdUseCase(IUserRepository repository) : IGetUserByIdUseCase
{
    public async Task<Output> ExecuteAsync(GetUserByIdInput input, CancellationToken cancellationToken = default)
    {
        Output output = new();

        var user = await repository.GetByIdAsync(input.Id, cancellationToken);
        if (user is null)
        {
            output.AddErrorMessage(UserErrors.NotFound);
            return output;
        }

        output.AddResult(user.MapToOutput());
        return output;
    }
}
