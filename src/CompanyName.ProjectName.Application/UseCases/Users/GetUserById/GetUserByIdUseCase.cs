using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Mapper;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdUseCase(
    IUserRepository repository
) : IGetUserByIdUseCase
{
    private readonly IUserRepository _repository = repository;

    public async Task<Output> ExecuteAsync(GetUserByIdInput input, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(input.Id, cancellationToken);

        if (user is null)
        {
            var output = new Output();
            output.AddErrorMessage(UserErrors.NotFound);
            return output;
        }

        return new Output(user.MapToOutput());
    }
}
