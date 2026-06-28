using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Mapper;
using CompanyName.ProjectName.Domain.Abstractions;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser;

public sealed class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly CreateUserInputValidator _validator;

    public CreateUserUseCase(IUserRepository repository, CreateUserInputValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = _validator.Validate(input);
        if (!validationResult.IsValid)
            return new Output(validationResult);

        var email = Email.Create(input.Email);
        if (await _repository.ExistsWithEmailAsync(email, cancellationToken))
        {
            var output = new Output();
            output.AddErrorMessage(UserErrors.EmailAlreadyInUse);
            return output;
        }

        try
        {
            var user = User.Create(input.Email, input.Name);
            await _repository.AddAsync(user, cancellationToken);
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
