using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Mapper;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser;

public sealed class CreateUserUseCase(IUserRepository repository) : ICreateUserUseCase
{
    public async Task<Output> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        Output output = new();

        var email = Email.Create(input.Email);
        if (await repository.ExistsWithEmailAsync(email, cancellationToken))
        {
            output.AddErrorMessage(UserErrors.EmailAlreadyInUse);
            return output;
        }

        var user = User.Create(input.Email, input.Name);
        await repository.AddAsync(user, cancellationToken);

        var committed = await repository.UnitOfWork.CommitAsync(cancellationToken);
        if (!committed)
        {
            output.AddErrorMessage("Failed to persist user.");
            return output;
        }

        output.AddResult(user.MapToOutput());
        return output;
    }
}
