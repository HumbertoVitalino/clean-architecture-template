using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.Interfaces.Services;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;
using CompanyName.ProjectName.Domain.Users;
using Microsoft.Extensions.Logging;

namespace CompanyName.ProjectName.Application.UseCases.Users.Login;

public sealed class LoginUseCase(
    IUserRepository userRepository,
    IJwtService jwtService,
    ILogger<LoginUseCase> logger
) : ILoginUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<LoginUseCase> _logger = logger;

    public async Task<Output> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        Output output = new();

        var email = Email.Create(input.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning(
                "Login failed: user not found. Email: {Email} | CorrelationId: {CorrelationId}",
                input.Email, input.CorrelationId);
            output.AddErrorMessage("Invalid credentials.");
            return output;
        }

        var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email.Value);
        output.AddResult(new LoginResponse(token));
        return output;
    }
}
