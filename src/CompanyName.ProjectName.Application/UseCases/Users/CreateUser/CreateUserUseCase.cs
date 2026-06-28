using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Mapper;
using CompanyName.ProjectName.Domain.Abstractions;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser;

public sealed class CreateUserUseCase(
    IUserRepository repository,
    CreateUserInputValidator validator
) : ICreateUserUseCase
{
    public async Task<Output> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = validator.Validate(input);
        if (!validationResult.IsValid)
            return new Output(validationResult);

        var email = Email.Create(input.Email);
        if (await repository.ExistsWithEmailAsync(email, cancellationToken))
        {
            var output = new Output();
            output.AddErrorMessage(UserErrors.EmailAlreadyInUse);
            return output;
        }

        try
        {
            var user = User.Create(input.Email, input.Name);
            await repository.AddAsync(user, cancellationToken);

            var committed = await repository.UnitOfWork.CommitAsync(cancellationToken);
            if (!committed)
            {
                var output = new Output();
                output.AddErrorMessage("Failed to persist user.");
                return output;
            }

            return new Output(user.MapToOutput());
        }
        catch (DomainException ex)
        {
            var output = new Output();
            output.AddErrorMessage(ex.Message);
            return output;
        }
    }
}
