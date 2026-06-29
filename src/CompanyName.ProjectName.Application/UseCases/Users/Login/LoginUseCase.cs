using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.Services;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.Login;

internal sealed class LoginUseCase(
    IUserRepository repository,
    IJwtService jwtService,
    LoginInputValidator validator
) : ILoginUseCase
{
    public async Task<Output> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = validator.Validate(input);
        if (!validationResult.IsValid)
        {
            var invalid = new Output();
            invalid.AddErrorMessage("Invalid credentials.");
            return invalid;
        }

        var email = Email.Create(input.Email);
        var user = await repository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            var invalid = new Output();
            invalid.AddErrorMessage("Invalid credentials.");
            return invalid;
        }

        var token = jwtService.GenerateToken(user.Id.ToString(), user.Email.Value);
        return new Output(new LoginResponse(token));
    }
}
