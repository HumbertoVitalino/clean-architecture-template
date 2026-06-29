using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.Services;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;
using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.Login;

internal sealed class LoginUseCase(IUserRepository repository, IJwtService jwtService) : ILoginUseCase
{
    public async Task<Output> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        Output output = new();

        var email = Email.Create(input.Email);
        var user = await repository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            output.AddErrorMessage("Invalid credentials.");
            return output;
        }

        var token = jwtService.GenerateToken(user.Id.ToString(), user.Email.Value);
        output.AddResult(new LoginResponse(token));
        return output;
    }
}
